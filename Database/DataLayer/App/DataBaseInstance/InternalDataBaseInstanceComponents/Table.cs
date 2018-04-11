using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DataLayer.Shared.ExtentionMethods;
using DataLayer.Shared.DataModels;
using DataModels.App.Shared.Events;

namespace DataModels.App.InternalDataBaseInstanceComponents
{
    [Serializable]
    public class Table
    {
        //fields
        string _name;
        //
        bool _isPKNeed;
        int currentPrimaryKey;
        List<Column> _columns;
        //
        public event EventHandler<CascadeDeleteEventArgs> cascadeDelete;
        //
        //properties
        public string Name { get => _name;  set => _name = value; }
        //
        public List<Column> Columns { get => _columns; private set => _columns = value; }
        public int CurrentPrimaryKey { get => currentPrimaryKey; private set => currentPrimaryKey = value; }
        public int newPrimaryKey()
        {
            CurrentPrimaryKey += 1;
            return CurrentPrimaryKey;
        }
        //
        /// <summary>
        /// Table constructor
        /// </summary>
        /// <param name="name"></param>
        public Table(string name)
        {
            Name = name;
            _columns = new List<Column>();
            currentPrimaryKey = 0;
            _isPKNeed = true;
                Column PrimaryKey = new Column("ID" + Name, typeof(int), false, 0, this);
                PrimaryKey.SetPkeyProperty(true);
                Columns.Add(PrimaryKey);
        }
        //
        public Table(string name, bool isPKNeed)
        {
            Name = name;
            _isPKNeed = isPKNeed;
            _columns = new List<Column>();
            currentPrimaryKey = 0;
            if (isPKNeed)
            {
                Column PrimaryKey = new Column("ID" + Name, typeof(int), false, 0, this);
                PrimaryKey.SetPkeyProperty(true);
                Columns.Add(PrimaryKey);
            }
        }
        public Table(Table copyTable)
        {
            Name = copyTable.Name;
            _isPKNeed = copyTable._isPKNeed;
            _columns = new List<Column>();
            currentPrimaryKey = 0;
            for (int i = 0; i < copyTable.Columns.Count; i++)
            {
                Columns.Add( new Column(copyTable.Columns[i], this));
            }
        }
        //
        /// <summary>
        /// Adds new column to current table!
        /// </summary>
        /// <param name="newTable"></param>
        public void AddColumn(Column newColumn)
        {
            if (newColumn.Name.isThereNoUndefinedSymbols())
            {
                foreach (Column tblProp in Columns)
                {
                    if (tblProp.Name == newColumn.Name) throw new FormatException("Invalid column name. Some column in this table have same name!");
                }
                if (isTableContainsData())
                {
                    int countDataRows = Columns[1].DataList.Count;
                    for (int i = 0; i < countDataRows; i++)
                    {
                        newColumn.DataList.Add(new DataObject(newColumn.GetHashCode(), newColumn.Default));
                    }
                }
                Columns.Add(newColumn);
            }
            else throw new FormatException("There is invalid symbols in column's name!");

        }  

        public void AddColumn(string name, Type DataType, bool allowsnull, object def)
        {
            AddColumn(new Column(name, DataType, allowsnull, def, this));
        } //UI done
        //
        /// <summary>
        /// Edit's column name
        /// </summary>
        public void RenameColumn(string currentName, string futureName)
        {
            if (indexOfColumn(currentName) != -1)
            {
                if (Columns[indexOfColumn(currentName)].IsFkey || Columns[indexOfColumn(currentName)].IsPkey) throw new ArgumentException("You can't rename PrimaryKey or ForeignKey column");
                if (futureName.isThereNoUndefinedSymbols())
                {
                    foreach (Column tblProp in Columns)
                    {
                        if (tblProp.Name == futureName) throw new FormatException("Invalid column name. Some column in this table have same name!");
                    }
                    Columns[indexOfColumn(currentName)].Name = futureName;
                }
                else throw new FormatException("There is invalid symbols in column's name!");
            }
            else throw new NullReferenceException("there's no such column");
        } //UI done
        //
        /// <summary>
        /// Add element to Table!
        /// </summary>
        /// <param name="arguments"></param> 
        public void AddTableElement(object[] arguments)
        {
            int ColumnsCount = default(int);//Columns.Count-1;
            if (_isPKNeed) ColumnsCount = Columns.Count - 1;
            else ColumnsCount = Columns.Count;
            if (arguments.Length == ColumnsCount)
            {
                if (_isPKNeed)
                {
                    Columns[0].DataList.Add(new DataObject(Columns[0].GetHashCode(), newPrimaryKey()));
                    //Columns+1 because of column "Primary Key"
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        if (Columns[i + 1].AllowsNull && arguments[i] == null) Columns[i + 1].DataList.Add(new DataObject(Columns[i + 1].GetHashCode(), arguments[i]));
                        else if (!Columns[i + 1].AllowsNull && arguments[i] == null) Columns[i + 1].DataList.Add(new DataObject(Columns[i + 1].GetHashCode(), Columns[i + 1].Default));
                        else
                        if (arguments[i].GetType() == Columns[i + 1].DataType)
                        {
                            Columns[i + 1].DataList.Add(new DataObject(Columns[i + 1].GetHashCode(), arguments[i]));
                        }
                        else throw new FormatException("Incorrect data type for " + Columns[i + 1].Name + " column!");
                    }
                }
                else
                {
                    //Columns+1 because of column "Primary Key"
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        if (Columns[i].AllowsNull && arguments[i] == null) Columns[i].DataList.Add(new DataObject(Columns[i].GetHashCode(), arguments[i]));
                        else if (!Columns[i].AllowsNull && arguments[i] == null) Columns[i].DataList.Add(new DataObject(Columns[i].GetHashCode(), Columns[i].Default));
                        else
                        if (arguments[i].GetType() == Columns[i].DataType)
                        {
                            Columns[i].DataList.Add(new DataObject(Columns[i].GetHashCode(), arguments[i]));
                        }
                        else throw new FormatException("Incorrect data type for " + Columns[i].Name + " column!");
                    }
                }
            }
            else throw new IndexOutOfRangeException("Arguments array isn't similar to count of columns in table");

        } //UI done
        //
        /// <summary>
        /// removes row of data according to primary key
        /// </summary>
        /// <param name="key"></param>
        public void DeleteTableElementByPrimaryKey(int key)
        {
            DeleteTableElementByIndex(returnIndexOfPrimaryKey(key));
            CascadeDeleteEventArgs e = new CascadeDeleteEventArgs("FK_"+Columns[0].Name, key);
            if (cascadeDelete != null)
            cascadeDelete(this, e);
        } //UI
        //
        /// <summary>
        /// removes row of data by index
        /// </summary>
        /// <param name="index"></param>
        void DeleteTableElementByIndex(int index)
        {
            if (isTableContainsData())
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    Columns[i].DataList.RemoveAt(index);
                    //
                }
            }
            else throw new NullReferenceException("There is no data in table!");
        }
        //
       public void DeleteAllData()
        {
            for (int i = 0; i < Columns[0].DataList.Count; i++)
                DeleteTableElementByIndex(i);
        }
        //
        /// <summary>
        /// edit table row by primary key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        public void EditTableElementByPrimaryKey(int key, object[] args)
        {
            if (_isPKNeed)
            {
                if (args.Length != Columns.Count - 1) throw new ArgumentException("Count of arguments is not similar to count of tables");
                for (int i = 0; i < Columns.Count - 1; i++)
                {
                    if (Columns[i + 1].DataList[0].Data.GetType() != args[i].GetType()) throw new ArgumentException("type of argument isn't the same as type of column!");
                }
                for (int i = 0; i < Columns.Count - 1; i++)
                {

                    Columns[i + 1].EditColumnElementByPrimaryKey(key, args[i]);
                }
            }
            else throw new ArgumentException("This table doesn't containt Primary Key!");
        } //UI
        //
        public void EditTableElementByIndex(int index, object[] args) // ignores pk 
        {

            if (args.Length != Columns.Count) throw new ArgumentException("Count of arguments is not similar to count of tables");
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].DataList[0].Data.GetType() != args[i].GetType()) throw new ArgumentException("type of argument isn't the same as type of column!");
            }
            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].DataList[index].Data = args[i];
            }
        }
        //
        public void EditTableElementByIndex(int index, DataObject[] args) // ignores pk 
        {

            if (args.Length != Columns.Count) throw new ArgumentException("Count of arguments is not similar to count of tables");
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].DataList[0].Data.GetType() != args[i].Data.GetType()) throw new ArgumentException("type of argument isn't the same as type of column!");
            }
            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].DataList[index] = args[i];
            }
        }
        //
        /// <summary>
        /// delete column by name
        /// </summary>
        /// <param name="name"></param>
        public void DeleteColumn(string name)
        {
          if (!isTableContainsColumns()) throw new NullReferenceException();
          if (indexOfColumn(name) != -1)
          {
               if(Columns[indexOfColumn(name)].IsFkey || Columns[indexOfColumn(name)].IsPkey) throw new ArgumentException("You can't delete PrimaryKey or ForeignKey column");
               Columns.RemoveAt(indexOfColumn(name));
          }
          else throw new NullReferenceException();

        } //UI
        //
        /// <summary>
        /// get index of column by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int indexOfColumn(string name)
        {
            if (!isTableContainsColumns()) throw new NullReferenceException();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Name == name) return i;
            }
            throw new NullReferenceException("There is no such column in table, or you can't get access to it");
        }
        //
        /// <summary>
        /// return's technical index of row
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int returnIndexOfPrimaryKey(int key)
        {
            if (key < 1) throw new ArgumentException("Invalid key");
            for (int i = 0; i < Columns[0].DataList.Count; i++)
            {
                if ((int)Columns[0].DataList[i].Data == key) return i;
            }
            throw new ArgumentException("There is no such key");
        }
        //
        /// <summary>
        /// return's primary key of index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int returnPrimaryKeyOfIndex(int index)
        {
            if (index < 0||index>=Columns[0].DataList.Count) throw new ArgumentException("Invalid key");
            return (int)Columns[0].DataList[index].Data;
        }
        //
        /// <summary>
        /// get data by Primary key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DataObject[] GetDataByPrimaryKey(int key)
        {
            return GetDataByIndex(returnIndexOfPrimaryKey(key));
        } //UI ??? (under question)
        //
        public Column GetColumnByName(string name)
        {
            if (name=="ID"+Name) throw new ArgumentException("you can't get PrimaryKey column");
            if (isTableContainsColumns())
            {
                if (getIndexOfColumn(name) != -1)
                    return Columns[getIndexOfColumn(name)];
                else throw new NullReferenceException("there's no such column!");
            } else throw new NullReferenceException("there is no Columns in this table!");
        }
        //
        /// <summary>
        /// returns data by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
         public DataObject[] GetDataByIndex(int index)
        {
                if(isTableContainsData())
                {
                DataObject[] dataArray = new DataObject[Columns.Count];
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        dataArray[i] = Columns[i].DataList[index];
                    }
                    return dataArray;
                 } else throw new NullReferenceException("There is no data in table!");
        
        }
        //
        /// <summary>
        /// method for cascade delete event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExecuteCascadeDelete(Object sender, CascadeDeleteEventArgs e)
        {
            Column linkedcolumn = GetColumnByName(e.ColumnName);
            List<int> keys = new List<int>();
            for (int i = 0; i < linkedcolumn.DataList.Count; i++)
            {
                if ((int)linkedcolumn.DataList[i].Data == e.DeletedRowPrimaryKey) keys.Add(i);
            }
            for (int i = 0; i < keys.Count; i++)
                keys[i] = returnPrimaryKeyOfIndex(keys[i]);
            foreach (int key in keys) DeleteTableElementByPrimaryKey(key);
        
        }
        //
        /// <summary>
        /// checks if table contains some data
        /// </summary>
        /// <returns></returns>
        public bool isTableContainsData()
        {
            if (isTableContainsColumns())
                if (Columns[1].DataList.Count != 0) return true;
            return false;
        } 
        //
        bool isTableContainsColumns()
        {
            if (Columns.Count == 1) return false;
            return true;
        }
        //
        //
        public string OutTable()
        {
            int[] maxCharSizesArray = new int[Columns.Count];
            for (int i = 0; i < maxCharSizesArray.Length; i++)
            {
                int biggestcharData = default(int);
                if (isTableContainsData())
                    biggestcharData = Columns[i].DataList.Select(x => x.Data.ToString().Length).Max();
                else biggestcharData = 0;
                int ColumnNameLength = Columns[i].Name.Length;
                if (biggestcharData > ColumnNameLength) maxCharSizesArray[i] = biggestcharData;
                else maxCharSizesArray[i] = ColumnNameLength;
            }
            string Space = " ";
            string VertDash = "|";
            string Plus = "+";
            string Dash = "-";
            string MainFrame = Plus;
            for (int i = 0; i < maxCharSizesArray.Length; i++)
            {
                for (int j = 0; j < maxCharSizesArray[i] + 2; j++) MainFrame += Dash;
                MainFrame += Plus;
            }
            string ColumnNamesFrame = VertDash;
            for (int i = 0; i < maxCharSizesArray.Length; i++)
            {
                int SpacesAfter = maxCharSizesArray[i] - Columns[i].Name.Length + 1;
                ColumnNamesFrame += Space; ColumnNamesFrame += Columns[i].Name;
                for (int j = 0; j < SpacesAfter; j++)
                    ColumnNamesFrame += Space;
                ColumnNamesFrame += VertDash;
            }
            string Data = default(string);
            for (int j = 0; j < Columns[0].DataList.Count; j++)
            {
                Data += "\n" + VertDash;
                for (int i = 0; i < maxCharSizesArray.Length; i++)
                {

                    int SpacesAfter = maxCharSizesArray[i] - Columns[i].DataList[j].Data.ToString().Length + 1;
                    Data += Space; Data += Columns[i].DataList[j].Data.ToString();
                    for (int k = 0; k < SpacesAfter; k++)
                        Data += Space;
                    Data += VertDash;
                }
            }
            return MainFrame + "\n" + ColumnNamesFrame + "\n" + MainFrame + Data + "\n" + MainFrame;
        }
        //
        public bool isColumnExists(string name)
        {
            foreach (Column column in Columns) if (column.Name == name || column.SystemName == name) return true;
            return false;
        }
        //
        public int getIndexOfColumn(string name)
        {
            for(int i =0; i< Columns.Count; i++)
            {
                if (Columns[i].Name == name||Columns[i].SystemName == name) return i;
            }
            return -1;
        }
        public string ColumnType()
        {
            string result = default(string);
            if (isTableContainsColumns())
            {
                foreach(Column column in Columns)
                {
                    if(!column.IsPkey)
                    result += column.Name +" - "+column.TypeToString+"\n";
                }
                return result;
            }
            return "There is no columns in this table";
        }
        //
      //  public Column GetColumn(string name)
        public override string ToString()
        {
            string tableInfo = "\n<TABLE> " + Name + " contains " + Columns.Count+" columns.";
            for (int i = 0; i < Columns.Count; i++)
            {
                tableInfo += "\n" + Columns[i].ToString();
            }
            if (isTableContainsData())
            {
                tableInfo += "\n" + OutTable();
            }
            else tableInfo += "\nable doesn't contains any data";

            tableInfo += "\n<TABLE>" + Name+ " INFO END\n";
            return tableInfo;
        }
    }

}
