using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DataLayer.Shared.ExtentionMethods;
using DataLayer.Shared.DataModels;

namespace DataLayer.InternalDataBaseInstanceComponents
{
    [Serializable]
    public class Table
    {
        //fields
        string _name;
        //
        uint currentPrimaryKey;
        List<Column> _columns;
        //
        //properties
        public string Name { get => _name;  set => _name = value; }
        //
        public List<Column> Columns { get => _columns; private set => _columns = value; }
        public uint CurrentPrimaryKey { get => currentPrimaryKey; private set => currentPrimaryKey = value; }
        public uint newPrimaryKey()
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
            Column PrimaryKey = new Column("Id" + Name, typeof(uint), false, 0);
            Columns.Add(PrimaryKey);
        }
        //
        /// <summary>
        /// Adds new column to current table!
        /// </summary>
        /// <param name="newTable"></param>
        public void AddColumn(Column newTable)
        {
            if (newTable.Name.isThereNoUndefinedSymbols())
            {
                foreach (Column tblProp in Columns)
                {
                    if (tblProp.Name == newTable.Name) throw new FormatException("Invalid column name. Some column in this table have same name!");
                }
                Columns.Add(newTable);
            }
            else throw new FormatException("There is invalid symbols in column's name!");

        }

        public void AddColumn(string name, Type DataType, bool allowsnull, object def)
        {
            AddColumn(new Column(name, DataType, allowsnull, def));
        }
        //
        /// <summary>
        /// Edit's column name
        /// </summary>
        public void RenameColumn(string currentName, string futureName)
        {
            if (indexOfColumn(currentName) != -1)
            {
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
        }
        //
        /// <summary>
        /// Add element to Table!
        /// </summary>
        /// <param name="arguments"></param>
        public void AddTableElement(object[] arguments)
        {
            int ColumnsCount = Columns.Count-1;
            if (arguments.Length == ColumnsCount)
            {
                Columns[0].DataList.Add(new DataObject(Columns[0].GetHashCode(), newPrimaryKey()));
                //Columns+1 because of column "Primary Key"
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (Columns[i+1].AllowsNull && arguments[i] == null) Columns[i+1].DataList.Add(new DataObject(Columns[i+1].GetHashCode(), arguments[i]));
                    else
                    if (arguments[i].GetType() == Columns[i+1].DataType)
                    {
                        Columns[i+1].DataList.Add(new DataObject(Columns[i+1].GetHashCode(),arguments[i]));
                    }
                    else throw new FormatException("You can't add null element to this column because it doesn't allows null");
                }
            }
            else throw new IndexOutOfRangeException("Arguments array isn't similar to count of columns in table");

        }
        //
        /// <summary>
        /// removes row of data according to primary key
        /// </summary>
        /// <param name="key"></param>
        public void DeleteTableElementByPrimaryKey(int key)
        {
            DeleteTableElementByIndex(returnIndexOfPrimaryKey(key));
        }
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
                }
            }
            else throw new NullReferenceException("There is no data in table!");
        }
        //
        /// <summary>
        /// edit table row by primary key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        public void EditTableElementByPrimaryKey(int key, object[] args)
        {
            EditTableElementByIndex(returnIndexOfPrimaryKey(key), args);
        }
        //
        /// <summary>
        /// edit row of table by index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="arguments"></param>
        void EditTableElementByIndex(int index, object[] arguments)
        {
            if (isTableContainsData())
            {
                if (arguments.Length == Columns.Count)
                {
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        if (Columns[i].DataList[0].GetType() != arguments[i].GetType()) throw new ArgumentException("type of argument isn't the same as type of column!");
                    }
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        Columns[i].DataList[index].Data = arguments[i];
                    }
                }
            }
            else throw new NullReferenceException("There is no data in table!");
        }
        //
        /// <summary>
        /// delete column by name
        /// </summary>
        /// <param name="name"></param>
        public void DeleteColumn(string name)
        {
            if (name != "Id" + Name)
            {
                if (!isTableContainsColumns()) throw new NullReferenceException();
                if (indexOfColumn(name) != -1)
                {
                    Columns.RemoveAt(indexOfColumn(name));
                }
                else throw new NullReferenceException();
            }
            else throw new ArgumentException("You can't delete PrimaryKey column");
        }
        //
        /// <summary>
        /// get index of column by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int indexOfColumn(string name)
        {
            if (!isTableContainsColumns()) throw new NullReferenceException();
            for (int i = 1; i < Columns.Count; i++)
            {
                if (Columns[i].Name == name) return i;
            }
            return -1;
        }
        //
        /// <summary>
        /// return's technical index of row
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int returnIndexOfPrimaryKey(int key)
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
        /// get data by Primary key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DataObject[] GetDataByPrimaryKey(int key)
        {
            return GetDataByIndex(returnIndexOfPrimaryKey(key));
        }
        //
        public Column GetColumnByName(string name)
        {
            if (name == "Id" + Name) throw new ArgumentException("you can't get PrimaryKey column");
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
         DataObject[] GetDataByIndex(int index)
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
        int getIndexOfColumn(string name)
        {
            for(int i =0; i< Columns.Count; i++)
            {
                if (Columns[i].Name == name) return i;
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
                    if(!column.Name.Contains("Id"))
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
                tableInfo += "\nTable contains " + Columns[1].DataList.Count + " rows of data:";
                for (int i = 0; i < Columns[1].DataList.Count; i++)
                {
                    DataObject[] buf = GetDataByIndex(i);
                    tableInfo += "\n" + i + ". ";
                    foreach (DataObject obj in buf)
                    {
                        string bufNull = default(string);
                        if (obj.Data == null)
                        {
                            bufNull = "null";
                            tableInfo += "(" + bufNull + ")" + bufNull + "  ";
                        }
                        else
                            tableInfo += "(" + obj.Data.GetType().Name + ")" + obj.Data.ToString() + "  ";
                    }
                }
            }
            else tableInfo += "\nable doesn't contains any data";

            tableInfo += "\n<TABLE>" + Name+ " INFO END\n";
            return tableInfo;
        }
    }

}
