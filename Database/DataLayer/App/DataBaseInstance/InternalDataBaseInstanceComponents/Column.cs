using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DataLayer.Shared.DataModels;
using DataLayer.Shared.ExtentionMethods;

namespace DataLayer.InternalDataBaseInstanceComponents
{
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
        public Column(string name, Type DataType, bool allowsnull, object def)
        {
            _name = name;
            dataType = DataType;
            allowsNull = allowsnull;
            _Default = DataType.GetDefaultValue();
            isFkey = false;
            if (def != null)
            {
                Type buf = def.GetType();

                if (def.GetType() == dataType) _Default = def;
            }
            _dataList = new List<DataObject>();

        }
        //fields

        string _name;
        //
        Type dataType;
        //
        bool allowsNull;
        //
        object _Default;
        //
        bool isFkey;
        //
        List<DataObject> _dataList;

        //properties
        public string Name { get => _name;   set => _name = value; }
        public Type DataType { get => dataType; private set => dataType = value; }
        public bool AllowsNull { get => allowsNull; private set => allowsNull = value; }
        public object Default { get => _Default; private set => _Default = value; }
       
        public List<DataObject> DataList { get => _dataList; private set => _dataList = value; }
        public string TypeToString { get => dataType.ToString();}
        public bool IsFkey { get => isFkey; private set => isFkey = value; }

        /// <summary>
        /// Edit type of the column's data
        /// </summary>
        /// <param name="newColumnType"></param>
        public void EditColumnType(Type newColumnType )
        {
            if (!IsFkey && !Name.Contains("Id"))
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
        /// <summary>
        /// Change Nullable property
        /// </summary>
        /// <param name="isNullable"></param>
        public void SetNullableProperty(bool isNullable)
        {
            if (!IsFkey && !Name.Contains("Id"))
            {
                AllowsNull = isNullable;
                UpdateDataHashCode();
            }
            else throw new ArgumentException("You can't change nullable property of this column!");
        }
        /// <summary>
        /// set another default object
        /// </summary>
        /// <param name="defaultObject"></param>
        public void SetDefaultObject(object defaultObject)
        {
            if (defaultObject.GetType() == DataType) Default = defaultObject;
            else throw new ArgumentException("Type of your defaultObject isn't similar to type of column");
        }
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
        public override bool Equals(object obj)
        {
            return (this.GetHashCode() + DataList.Count == obj.GetHashCode() + DataList.Count);
        }

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
            return NameHashCode * TypeHashCode * AllowsNullHashCode;
        }

        public override string ToString()
        {
            string columnInfo = "[COLUMN] " + Name + " contains data of " + DataType.Name + " variables,";
            if (AllowsNull) columnInfo += " allows null data,";
            else columnInfo += " doesn't allows null data,";
            columnInfo += " default object = " + Default.ToString()+", hash = "+ GetHashCode()+"\n[COLUMN]"+Name +" INFO END";
            return columnInfo;
        }
    }

}
