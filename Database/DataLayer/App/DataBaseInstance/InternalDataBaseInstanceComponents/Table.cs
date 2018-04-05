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
        List<Column> _columns;
        //
        //properties
        public string Name { get => _name; private set => _name = value; }
        //
        public List<Column> Columns { get => _columns; private set => _columns = value; }
        //
        /// <summary>
        /// Table constructor
        /// </summary>
        /// <param name="name"></param>
        public Table(string name)
        {

            Name = name;
            _columns = new List<Column>();
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
        /// Add element to Table!
        /// </summary>
        /// <param name="arguments"></param>
        public void AddTableElement(object[] arguments)
        {
            int TablesCount = Columns.Count;
            if (arguments.Length == TablesCount)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (Columns[i].AllowsNull && arguments[i] == null) Columns[i].DataList.Add(new DataObject(Columns[i].GetHashCode(), Columns[i].Default));
                    else
                    if (arguments[i].GetType() == Columns[i].DataType)
                    {
                        Columns[i].DataList.Add(new DataObject(Columns[i].GetHashCode(),arguments[i]));
                    }
                    else throw new FormatException("You can't add null element to this column because it doesn't allows null");
                }
            }
            else throw new IndexOutOfRangeException("Arguments array isn't similar to count of columns in table");

        }
        /// <summary>
        /// removes row of data by index
        /// </summary>
        /// <param name="index"></param>
        public void DeleteTableElement(int index)
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
        /// <summary>
        /// edit row of table by index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="arguments"></param>
        public void EditTableElement(int index, object[] arguments)
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
        /// <summary>
        /// delete column by name
        /// </summary>
        /// <param name="name"></param>
        public void DeleteColumn(string name)
        {
            if (Columns.Count == 0) throw new NullReferenceException();
            if (indexOfColumn(name) != -1)
            {
                Columns.RemoveAt(indexOfColumn(name));
            }
            else throw new NullReferenceException();
        }
        /// <summary>
        /// get index of column by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int indexOfColumn(string name)
        {
            if (Columns.Count == 0) throw new NullReferenceException();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Name == name) return i;
            }
            return -1;
        }
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
        /// <summary>
        /// checks if table contains some data
        /// </summary>
        /// <returns></returns>
        public bool isTableContainsData()
        {
            if (Columns.Count != 0)
                if (Columns[0].DataList.Count != 0) return true;
            return false;
        } 

        public string ColumnType()
        {
            string result = default(string);
            if (Columns.Count != 0)
            {
                foreach(Column column in Columns)
                {
                    result += column.Name +" - "+column.TypeToString+"\n";
                }
                return result;
            }
            return "There is no columns in this table";
        }

        public override string ToString()
        {
            string tableInfo = "\n<TABLE> " + Name + " contains " + Columns.Count+" columns.";
            for (int i = 0; i < Columns.Count; i++)
            {
                tableInfo += "\n" + Columns[i].ToString();
            }
            if (isTableContainsData())
            {
                tableInfo += "\nTable contains " + Columns[0].DataList.Count + " rows of data:";
                for (int i = 0; i < Columns[0].DataList.Count; i++)
                {
                    object[] buf = GetDataByIndex(i);
                    tableInfo += "\n" + i + ". ";
                    foreach (object obj in buf) tableInfo += "("+obj.GetType().Name+")"+obj.ToString() + "  ";
                }
            }
            else tableInfo += "\nable doesn't contains any data";

            tableInfo += "\n<TABLE>" + Name+ " INFO END\n";
            return tableInfo;
        }
    }

}
