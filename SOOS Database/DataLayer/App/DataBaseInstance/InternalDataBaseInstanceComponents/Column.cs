using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DataLayer.Shared.DataModels;
using DataLayer.Shared.ExtentionMethods;

namespace DataModels.App.InternalDataBaseInstanceComponents
{
    /// <summary>
    /// Class contains all logic and methods connected with column
    /// </summary>
    [Serializable]
    public class Column
    {
        /// <summary>
        /// General constructor
        /// </summary>
        /// <param name="name">Name of the column</param>
        /// <param name="DataType">Type of the data at this column</param>
        /// <param name="allowsnull">Does this column allows null data objects?</param>
        /// <param name="isFkey">Is this column a foreign key</param>
        /// <param name="isPkey">Is this column a primary key</param>
        /// <param name="def"> Default object for this column</param>
        public Column(string name, Type DataType, bool allowsnull, object def, Table parentTable)
        {
            _name = name;
            _systemName += parentTable.Name + "." + name;
            dataType = DataType;
            allowsNull = allowsnull;
            _Default = DataType.GetDefaultValue();
            isFkey = false;
            isPkey = false;
            thisTable = parentTable;
            if (def != null)
            {
                Type buf = def.GetType();

                if (def.GetType() == dataType) _Default = def;
            }
            _dataList = new List<DataObject>();

        }
        //
        public Column(Column copyColumn, Table parentTable)
        {
            _name = copyColumn.Name;
            _systemName += parentTable.Name + "." + copyColumn.Name;
            dataType = copyColumn.DataType;
            allowsNull = copyColumn.allowsNull;
            _Default = DataType.GetDefaultValue();
            isFkey = false;
            isPkey = false;
            thisTable = parentTable;
            if (copyColumn.Default != null)
            {
                Type buf = copyColumn.Default.GetType();

                if (copyColumn.Default.GetType() == dataType) _Default = copyColumn.Default;
            }
            _dataList = new List<DataObject>();
            for (int i = 0; i < copyColumn.DataList.Count; i++)
            {
                _dataList.Add(new DataObject(GetHashCode(), copyColumn.DataList[i].Data));
            }
        }
        public Column(Column copyColumn)
        {
            _name = copyColumn.Name;
            _systemName += copyColumn.thisTable.Name + "." + copyColumn.Name;
            dataType = copyColumn.DataType;
            allowsNull = copyColumn.allowsNull;
            _Default = DataType.GetDefaultValue();
            isFkey = false;
            isPkey = false;
            thisTable = copyColumn.thisTable;
            if (copyColumn.Default != null)
            {
                Type buf = copyColumn.Default.GetType();

                if (copyColumn.Default.GetType() == dataType) _Default = copyColumn.Default;
            }
            _dataList = new List<DataObject>();
            for (int i = 0; i < copyColumn.DataList.Count; i++)
            {
                _dataList.Add(new DataObject(GetHashCode(), copyColumn.DataList[i].Data));
            }
        }
        //fields
        private Table thisTable;
        //
        protected string _name;

        private string _systemName;
        //
        protected Type dataType;
        //
        protected bool allowsNull;
        //
        protected object _Default;
        //
        protected bool isFkey;
        //
        protected bool isPkey;
        //
        List<DataObject> _dataList;
        //
        bool isCascadeDeleteOn;

        //properties
        public string Name { get => _name; set => _name = value; }
        public Type DataType { get => dataType; protected set => dataType = value; }
        public bool AllowsNull { get => allowsNull; protected set => allowsNull = value; }
        public object Default { get => _Default; protected set => _Default = value; }
        public bool IsCascadeDeleteOn { get { if (IsFkey) return isCascadeDeleteOn; else throw new ArgumentException("You can't get cascade property of ordinary column"); } set {if (IsFkey)  isCascadeDeleteOn=value; else throw new ArgumentException("You can't set cascade property of ordinary column"); } }
        public List<DataObject> DataList { get => _dataList;  set => _dataList = value; }
        public string TypeToString { get => dataType.ToString();}
        public bool IsFkey { get => isFkey; protected set => isFkey = value; }
        public bool IsPkey { get => isPkey; protected set => isPkey = value; }
        public Table ThisTable { get => thisTable; }
        public string SystemName { get => _systemName; private set => _systemName = value; }

        /// <summary>
        /// Edit type of the column's data
        /// </summary>
        /// <param name="newColumnType">Future Type of the column</param>
        public void EditColumnType(Type newColumnType )
        {
            try
            {
                if (!IsFkey && !IsPkey)
                {
                    if (newColumnType != DataType)
                    {
                        DataType = newColumnType;
                        SetDefaultObject(DataType.GetDefaultValue());
                        UpdateDataHashCode();
                    }
                }
                else throw new ArgumentException("You can't change type of this column!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        } //UI
        /// <summary>
        /// Change Nullable property
        /// </summary>
        /// <param name="isNullable">Value is Nullable?</param>
        public void SetNullableProperty(bool isNullable)
        {
            try
            {
                if (!IsFkey && !IsPkey)
                {
                    AllowsNull = isNullable;
                    UpdateDataHashCode();
                }
                else throw new ArgumentException("You can't change nullable property of this column!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI
        /// <summary>
        /// set another default object
        /// </summary>
        /// <param name="defaultObject">Future default object for this column</param>
        public void SetDefaultObject(object defaultObject)
        {
            try
            {
                if (!IsFkey && !IsPkey)
                {
                    if (defaultObject.GetType() == DataType) Default = defaultObject;
                    else throw new ArgumentException("Type of your defaultObject isn't similar to type of column");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI
        /// <summary>
        /// Updates hash code inside DataList
        /// </summary>
        public void UpdateDataHashCode()
       {
            if (DataList.Count != 0)
            {
                if (DataList[0].Data.GetType() != DataType)
                {
                    for (int i = 0; i < DataList.Count; i++)
                    {
                        DataObject buf = new DataObject(GetHashCode(), Default);
                        DataList[i] = buf;
                    }
                }
                else
                {
                    for (int i = 0; i < DataList.Count; i++)
                    {
                        DataObject buf = new DataObject(GetHashCode(), DataList[i].Data);
                        DataList[i] = buf;
                    }
                }
            }
        }
        //
        /// <summary>
        /// Set's Fkey property
        /// </summary>
        /// <param name="Fkey">Is this column FKey?</param>
        public void SetFkeyProperty(bool Fkey)
        {
            IsFkey = Fkey;
        }
        //
        /// <summary>
        /// Set's Pkey property
        /// </summary>
        /// <param name="Fkey">Is this column Pkey?</param>
        public void SetPkeyProperty(bool Pkey)
        {
            IsPkey = Pkey;
        }
        //
        /// <summary>
        /// Updates System name of column
        /// </summary>
        public void UpdateSystemName()
        {
            SystemName = ThisTable.Name + "." + Name;
        }
        //
        /// <summary>
        /// Adds object to dataList
        /// </summary>
        /// <param name="argument">object to Add</param>
        public virtual void AddDataElement(object argument)
        {
            DataList.Add(new DataObject(GetHashCode(), argument));

        }
        //
        /// <summary>
        ///  Edits data of single column element by primary key
        /// </summary>
        /// <param name="key">Primary key of elements row</param>
        /// <param name="arguments">Object array which must contain 1 value in it</param>
        /// <exception cref="ArgumentException">Throws when there is more than 1 value</exception>
        /// <exception cref="ArgumentException">Throws when you trying to change column data</exception>
        /// <exception cref="ArgumentException">Throws when there is no such Primary Key in this table</exception>
        /// <exception cref="ArgumentException">Throws when type of argument is not similar to Column type</exception>
        /// <exception cref="NullReferenceException">Throws when table doesn't contain any data</exception>

        virtual public void EditColumnElementByPrimaryKey(int key, object[] arguments)
        {
            try
            {
                if (thisTable.isTableContainsData())
                {
                    if(arguments.Length>1) throw new ArgumentException("There is more than 1 value");
                    if (IsPkey) throw new ArgumentException("You can't change the PrimaryKey Column Data");
                    if (arguments[0] == null && AllowsNull)
                        DataList[thisTable.returnIndexOfPrimaryKey(key)].Data = null;
                    else if(arguments[0]==null) DataList[thisTable.returnIndexOfPrimaryKey(key)].Data = _Default;

                    if (DataType == arguments[0].GetType())
                    {
                        if (thisTable.returnIndexOfPrimaryKey(key) == -1) throw new NullReferenceException("There is no such Primary Key in this table");
                       
                        else DataList[thisTable.returnIndexOfPrimaryKey(key)].Data = arguments[0];
                    }
                    else throw new ArgumentException("Type of argument is not similar to Column type");
                }
                else throw new NullReferenceException("Table doesn't contain any data");
            }
            catch(NullReferenceException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI 
        //
        /// <summary>
        /// Edits column element by its index
        /// </summary>
        /// <param name="index">Index of element</param>
        /// <param name="arguments">Object array which must contain 1 value</param>
        /// <exception cref="ArgumentException">Throws when you can't change the PrimaryKey Column Data</exception>
        /// <exception cref="NullReferenceException">Throws when invalid index is entered</exception>
        /// <exception cref="ArgumentException">Throws when Type of argument is not similar to Column type</exception>
        /// <exception cref="NullReferenceException">Throws when Table doesn't contain any data</exception>
        virtual public void EditColumnElementByIndex(int index, object[] arguments)
        {
            try
            {
                if (thisTable.isTableContainsData())
                {
                    if (IsPkey) throw new ArgumentException("You can't change the PrimaryKey Column Data");
                    if (arguments[0] == null && AllowsNull)
                        DataList[index].Data = null;
                    else if (arguments[0] == null) DataList[index].Data = _Default;
                    else if (DataType == arguments[0].GetType())
                    {
                        if (index<0 || index >= DataList.Count) throw new NullReferenceException("Invalid Index");
                        else DataList[index].Data = arguments[0];
                    }
                    else throw new ArgumentException("Type of argument is not similar to Column type");
                }
                else throw new NullReferenceException("Table doesn't contain any data");
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI 
        //
        /// <summary>
        /// Creates new list of data according to data in this column
        /// </summary>
        /// <returns></returns>
        public List<DataObject> CloneData()
        {
            List<DataObject> list = new List<DataObject>();
            for (int i = 0; i < DataList.Count; i++)
                list.Add(new DataObject(DataList[i].DataHashcode,DataList[i].Data));
            return list;
        }
        //
        /// <summary>
        /// Checks if Linked column contains such
        /// </summary>
        /// <param name="value">Object to check</param>
        /// <returns></returns>
        public virtual bool isLinkedColumnContainsSuchValue(object value)
        {
            return true;
        }
        //
        /// <summary>
        /// Overriden equals methos
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.GetHashCode() + DataList.Count == obj.GetHashCode() + DataList.Count);
        }
        /// <summary>
        /// Gets column HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int NameHashCode = 1;
            for (int i = 0; i < Name.Length; i++)
            {
                NameHashCode += Math.Abs((int)Name[i]);
            }
            int TypeHashCode = 1;
            for (int i = 0; i < DataType.Name.Length; i++)
            {
                TypeHashCode += Math.Abs((int)DataType.Name[i]);
            }
            int AllowsNullHashCode = 1;
            if (!AllowsNull) AllowsNullHashCode =2;
            int parentTableHash = 1;
            for (int i = 0; i < ThisTable.Name.Length; i++)
            {
                parentTableHash += Math.Abs((int)ThisTable.Name[i]);
            }
            return NameHashCode * TypeHashCode * parentTableHash * AllowsNullHashCode;
        }

        /// <summary>
        /// Performs column object to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string columnInfo = "[COLUMN] " +"<"+Name + "> DataType = " + DataType.Name + ",";
            if (AllowsNull) columnInfo += " AllowsNull,";
            else columnInfo += " !AllowsNull,";
            if (IsPkey) columnInfo += " isPrimaryKey,";
            if (IsFkey) columnInfo += " isForeignKey,";
            columnInfo += " DefaultValue = " + Default.ToString()+", hash = "+ GetHashCode();
            return columnInfo;
        }
    }

}
