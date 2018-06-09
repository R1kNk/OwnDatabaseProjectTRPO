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
        /// <summary>
        /// Event which handles cascade delete
        /// </summary>
        public event EventHandler<CascadeDeleteEventArgs> cascadeDelete;
        //
        //properties
        public string Name { get => _name;  set => _name = value; }
        //
        public List<Column> Columns { get => _columns; private set => _columns = value; }
        public int CurrentPrimaryKey { get => currentPrimaryKey; private set => currentPrimaryKey = value; }
        /// <summary>
        /// Generates new Primary Key
        /// </summary>
        /// <returns></returns>
        public int newPrimaryKey()
        {
            CurrentPrimaryKey += 1;
            return CurrentPrimaryKey;
        }
        //
        /// <summary>
        /// Table constructor
        /// </summary>
        /// <param name="name">Table name</param>
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
        /// <summary>
        /// Another constructos which gives you a choise if Pkey is Need
        /// </summary>
        /// <param name="name">Name of table</param>
        /// <param name="isPKNeed">Bool value</param>
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
        /// <summary>
        /// Returns new table which was copied from another table
        /// </summary>
        /// <param name="copyTable">table to copy</param>
        /// <param name="inner">Variable for Inner join query (True if you use method in InnerJoin)</param>
        public Table(Table copyTable, bool inner=false)
        {
            Name = copyTable.Name;
            _isPKNeed = copyTable._isPKNeed;
            _columns = new List<Column>();
            currentPrimaryKey = 0;
            if (inner == true)
            {
                for (int i = 0; i < copyTable.Columns.Count; i++)
                {
                    Columns.Add(new Column(copyTable.Columns[i]));
                }
            }
            else
            for (int i = 0; i < copyTable.Columns.Count; i++)
            {
                Columns.Add( new Column(copyTable.Columns[i], copyTable));
            }
        }
        //
        /// <summary>
        /// Adds new column to current table!
        /// </summary>
        /// <param name="newTable"></param>
        public void AddColumn(Column newColumn)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }  

        /// <summary>
        /// Constructor for column
        /// </summary>
        /// <param name="name">Name of column</param>
        /// <param name="DataType">Type of data in column</param>
        /// <param name="allowsnull">is this column will allow null</param>
        /// <param name="def">Default object for column</param>
        public void AddColumn(string name, Type DataType, bool allowsnull, object def)
        {
            AddColumn(new Column(name, DataType, allowsnull, def, this));
        } //UI done
          //
          /// <summary>
          /// Edit's column name
          /// </summary>
          /// <param name="currentName">Current name of column to find</param>
          /// <param name="futureName">Future name of finded column</param>
        public void RenameColumn(string currentName, string futureName)
        {
            try
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
                        Columns[indexOfColumn(futureName)].UpdateSystemName();
                    }
                    else throw new FormatException("There is invalid symbols in column's name!");
                }
                else throw new NullReferenceException("There's no such column");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI done
        //
        /// <summary>
        /// Add element to Table!
        /// </summary>
        /// <param name="arguments">Element array</param> 
        public void AddTableElement(object[] arguments)
        {
            try
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
                            if(!Columns[i + 1].isLinkedColumnContainsSuchValue(arguments[i])) throw new ArgumentException("There is no such argument (" + arguments[i].ToString() + ") in linkedColumn (" + Columns[i+1].Name + ")!"); ;
                        }
                            for (int i = 0; i < arguments.Length; i++)
                            {
                            if (Columns[i + 1].AllowsNull && arguments[i] == null) Columns[i + 1].DataList.Add(new DataObject(Columns[i + 1].GetHashCode(), arguments[i]));
                            else if (!Columns[i + 1].AllowsNull && arguments[i] == null) Columns[i + 1].DataList.Add(new DataObject(Columns[i + 1].GetHashCode(), Columns[i + 1].Default));
                            else
                            if (arguments[i].GetType() == Columns[i + 1].DataType)
                            {
                                
                                Columns[i + 1].DataList.Add(new DataObject(Columns[i + 1].GetHashCode(), new object[] {0}));
                                Columns[i + 1].EditColumnElementByIndex(Columns[i + 1].DataList.Count - 1, arguments[i].toObjectArray());
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        } //UI done
        //
        /// <summary>
        /// Eemoves row of data according to primary key
        /// </summary>
        /// <param name="key">Primary key of a row of data</param>
        public void DeleteTableElementByPrimaryKey(int key)
        {
            try
            {
                if (returnIndexOfPrimaryKey(key) == -1) throw new NullReferenceException("There is no such Primary Key in this table");
                DeleteTableElementByIndex(returnIndexOfPrimaryKey(key));
                CascadeDeleteEventArgs e = new CascadeDeleteEventArgs("FK_" + Columns[0].Name, key);
                if (cascadeDelete != null)
                    cascadeDelete(this, e);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI done
        //
        /// <summary>
        /// removes row of data by index
        /// </summary>
        /// <param name="index"> index of row</param>
      public  void DeleteTableElementByIndex(int index)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //

        /// <summary>
        /// Deletes all data from table
        /// </summary>
        public void DeleteAllData()
        {
           
            for (int i = 0; i < Columns[0].DataList.Count; i++)
            {
                DeleteTableElementByIndex(i);
                i--;
            }
        }
        //
        /// <summary>
        /// edit table row by primary key
        /// </summary>
        /// <param name="key">PK of row</param>
        /// <param name="args">Argumnets for editing</param>
        public void EditTableElementByPrimaryKey(int key, object[] args)
        {
            try
            {
                if (_isPKNeed)
                {
                    if (args.Length != Columns.Count - 1) throw new ArgumentException("Count of arguments is not similar to count of tables");
                    for (int i = 0; i < Columns.Count - 1; i++)
                    {
                        if (args[i] != null && Columns[i+1].DataList[0].Data != null)
                            if (Columns[i + 1].DataList[0].Data.GetType() != args[i].GetType()) throw new ArgumentException("type of argument isn't the same as type of column!");
                    }
                    for (int i = 0; i < Columns.Count - 1; i++)
                    {

                        Columns[i + 1].EditColumnElementByPrimaryKey(key, args[i].toObjectArray());
                    }
                }
                else throw new ArgumentException("This table doesn't containt Primary Key!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI
        //
        /// <summary>
        /// Edit table row by index
        /// </summary>
        /// <param name="index">Index of row</param>
        /// <param name="args">Arguments for editing</param>
        public void EditTableElementByIndex(int index, object[] args) // ignores pk 
        {
            try
            {
                if (args.Length != Columns.Count) throw new ArgumentException("Count of arguments is not similar to count of tables");
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (args[i] != null && Columns[i].DataList[0].Data != null)

                        if (Columns[i].DataList[0].Data.GetType() != args[i].GetType()) throw new ArgumentException("type of argument isn't the same as type of column!");
                }
                for (int i = 0; i < Columns.Count; i++)
                {
                    Columns[i].DataList[index].Data = args[i];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// Edit table row by index using Dataobject arguments
        /// </summary>
        /// <param name="index">index of row</param>
        /// <param name="args">Objects for edit</param>
        public void EditTableElementByIndex(int index, DataObject[] args) // ignores pk 
        {
            try
            {
                if (args.Length != Columns.Count) throw new ArgumentException("Count of arguments is not similar to count of tables");
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (args[i].Data != null && Columns[i].DataList[0].Data != null)
                        if (Columns[i].DataList[0].Data.GetType() != args[i].Data.GetType()) throw new ArgumentException("type of argument isn't the same as type of column!");
                }
                for (int i = 0; i < Columns.Count; i++)
                {
                    Columns[i].DataList[index] = args[i];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// Delete column by name
        /// </summary>
        /// <param name="name">Name of column</param>
        public void DeleteColumn(string name)
        {
            try
            {
                if (!isTableContainsColumns()) throw new NullReferenceException("there is no columns in this table!");
                if (indexOfColumn(name) != -1)
                {
                    if (Columns[indexOfColumn(name)].IsFkey || Columns[indexOfColumn(name)].IsPkey) throw new ArgumentException("You can't delete PrimaryKey or ForeignKey column");
                    Columns.RemoveAt(indexOfColumn(name));
                }
                else throw new NullReferenceException("There is no such column in table!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        } //UI done
        //
        /// <summary>
        /// get index of column by name
        /// </summary>
        /// <param name="name">Name of column</param>
        /// <returns></returns>
        int indexOfColumn(string name)
        {
            try
            {
                if (!isTableContainsColumns()) throw new NullReferenceException("There is no columns in this table");

                for (int i = 0; i < Columns.Count; i++)
                {
                    if (Columns[i].Name == name || Columns[i].SystemName == name) return i;
                }
                throw new NullReferenceException("There is no such column in table, or you can't get access to it");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return -1;

        }
        //
        /// <summary>
        /// return's technical index of row
        /// </summary>
        /// <param name="key">Primary key of row</param>
        /// <returns></returns>
        public int returnIndexOfPrimaryKey(int key)
        {
            try
            {
                if (key < 1) throw new ArgumentException("Invalid key");
                for (int i = 0; i < Columns[0].DataList.Count; i++)
                {
                    if ((int)Columns[0].DataList[i].Data == key) return i;
                }
                throw new ArgumentException("There is no such key");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }
        //
        /// <summary>
        /// return's primary key of index
        /// </summary>
        /// <param name="index">index of row</param>
        /// <returns></returns>
        public int returnPrimaryKeyOfIndex(int index)
        {
            try
            {
                if (index < 0 || index >= Columns[0].DataList.Count) throw new ArgumentException("Invalid key");
            return (int)Columns[0].DataList[index].Data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }
        //
        /// <summary>
        /// get data by Primary key
        /// </summary>
        /// <param name="key">pk</param>
        /// <returns></returns>
        public DataObject[] GetDataByPrimaryKey(int key)
        {
            try
            {
                if (returnIndexOfPrimaryKey(key) == -1) throw new NullReferenceException("There is no such Primary Key in this table");
                return GetDataByIndex(returnIndexOfPrimaryKey(key));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        } //UI ??? (under question)
        //
        /// <summary>
        /// Return column finded by it's name
        /// </summary>
        /// <param name="name">Name of column</param>
        /// <returns></returns>
        public Column GetColumnByName(string name)
        {
            try
            {           
            if (isTableContainsColumns())
                {
                    if (getIndexOfColumn(name) != -1)
                        return Columns[getIndexOfColumn(name)];
                    else throw new NullReferenceException("there's no such column!");
                }
                else throw new NullReferenceException("there is no Columns in this table!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }
        //
        /// <summary>
        /// returns data by index
        /// </summary>
        /// <param name="index">index of row</param>
        /// <returns></returns>
         public DataObject[] GetDataByIndex(int index)
        {
            try
            {
                if (isTableContainsData())
                {
                    if (index == -1) throw new NullReferenceException("There is no such Primary Key in table!");
                    DataObject[] dataArray = new DataObject[Columns.Count];
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        dataArray[i] = Columns[i].DataList[index];
                    }
                    return dataArray;
                }
                else throw new NullReferenceException("There is no data in table!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

}
        //
        /// <summary>
        /// method for cascade delete event
        /// </summary>
        /// <param name="sender"> object sender</param>
        /// <param name="e"> event arguments</param>
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
       
        /// <summary>
        /// Checks is table contains atleast one column
        /// </summary>
        /// <returns></returns>
        bool isTableContainsColumns()
        {
            if (Columns.Count == 1) return false;
            return true;
        }
        //

        /// <summary>
        ///Out's table's info to console 
        /// </summary>
        /// <returns></returns>
        public string OutTable()
        {
            int[] maxCharSizesArray = new int[Columns.Count];
            for (int i = 0; i < maxCharSizesArray.Length; i++)
            {
                int biggestcharData = default(int);
                if (isTableContainsData())
                {
                    if (Columns[i].DataList.Count != 0)
                        if(Columns[i].DataList.Where(x => x.Data != null).Count()!=0)
                    biggestcharData = Columns[i].DataList.Where(x => x.Data != null).Select(x => x.Data.ToString().Length).Max();
                }
                else biggestcharData = 0;
                if (biggestcharData < 4) biggestcharData = 4;
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
                    string bufData = default(string);
                    if (Columns[i].DataList[j].Data == null)
                        bufData = "null";
                    else bufData = Columns[i].DataList[j].Data.ToString();
                    int SpacesAfter = maxCharSizesArray[i] - bufData.Length + 1;
                    Data += Space; Data += bufData;
                    for (int k = 0; k < SpacesAfter; k++)
                        Data += Space;
                    Data += VertDash;
                }
            }
            return MainFrame + "\n" + ColumnNamesFrame + "\n" + MainFrame + Data + "\n" + MainFrame;
        }
        //
        /// <summary>
        /// Out's table's info to console in Querymethods
        /// </summary>
        /// <returns></returns>
        public string QueryOutTable()
        {
            int[] maxCharSizesArray = new int[Columns.Count];
            for (int i = 0; i < maxCharSizesArray.Length; i++)
            {
                int biggestcharData = default(int);
                if (isTableContainsData())
                {
                    if (Columns[i].DataList.Count != 0)
                        if (Columns[i].DataList.Where(x => x.Data != null).Count() != 0)
                            biggestcharData = Columns[i].DataList.Where(x => x.Data != null).Select(x => x.Data.ToString().Length).Max();
                }
                else biggestcharData = 0;
                if (biggestcharData < 4) biggestcharData = 4;
                int ColumnNameLength = Columns[i].SystemName.Length;
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
                int SpacesAfter = maxCharSizesArray[i] - Columns[i].SystemName.Length + 1;
                ColumnNamesFrame += Space; ColumnNamesFrame += Columns[i].SystemName;
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
                    string bufData = default(string);
                    if (Columns[i].DataList[j].Data == null)
                        bufData = "null";
                    else bufData = Columns[i].DataList[j].Data.ToString();
                    int SpacesAfter = maxCharSizesArray[i] - bufData.Length + 1;
                    Data += Space; Data += bufData;
                    for (int k = 0; k < SpacesAfter; k++)
                        Data += Space;
                    Data += VertDash;
                }
            }
            return MainFrame + "\n" + ColumnNamesFrame + "\n" + MainFrame + Data + "\n" + MainFrame;
        }
        //
        /// <summary>
        /// Checks is this column exists
        /// </summary>
        /// <param name="name">Column name</param>
        /// <returns></returns>
        public bool isColumnExists(string name)
        {
            foreach (Column column in Columns) if (column.SystemName == name || column.Name == name) return true;
            return false;
        }
        //
        /// <summary>
        /// Returns column index
        /// </summary>
        /// <param name="name">Column name</param>
        /// <returns></returns>
        public int getIndexOfColumn(string name)
        {
            for(int i =0; i< Columns.Count; i++)
            {
                if (Columns[i].Name == name||Columns[i].SystemName == name) return i;
            }
            return -1;
        }

        /// <summary>
        /// Sets currnet Primary key to another
        /// </summary>
        /// <param name="newKey"> new Pk</param>
        public void setCurrentPrimaryKey(int newKey)
        {
            CurrentPrimaryKey = newKey;
        }
        //
      //  public Column GetColumn(string name)
        /// <summary>
        /// returns string info about table
        /// </summary>
        /// <returns></returns>
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
