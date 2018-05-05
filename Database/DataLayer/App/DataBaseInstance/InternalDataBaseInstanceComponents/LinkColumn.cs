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
    [Serializable]
    public class LinkColumn : Column
    {
        Column linkedColumn;
        public LinkColumn(string name, Type DataType, bool allowsnull, object def, Table thisTable, Column linkedcolumn) : base(name, DataType, allowsnull, def, thisTable)
        {
            try
            {

                if (linkedcolumn.IsPkey)
                {
                    linkedColumn = linkedcolumn;
                    SetFkeyProperty(true);
                    DataType = linkedColumn.DataType;
                    Default = DataType.GetDefaultValue();
                    for (int i = 0; i < DataList.Count; i++)
                    {
                        DataList.Add(new DataObject(GetHashCode(), Default));
                    }
                }
                else throw new ArgumentException("You can connect this column only with PrimaryKeyColumn");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public override void EditColumnElementByPrimaryKey(int key, object[] arguments)
        {
            try
            {
                if (ThisTable.isTableContainsData())
                {
                    if (arguments[0] == null) throw new ArgumentException("You can't change value of FK column to null");
                    if (DataType == arguments[0].GetType())
                    {
                        string dataforException = default(string);
                        if (arguments[0] == null) dataforException = "null";
                        else dataforException = arguments[0].ToString();
                        if (isLinkedColumnContainsSuchValue(arguments[0]))
                        {
                            if (ThisTable.returnIndexOfPrimaryKey(key) == -1) throw new NullReferenceException("There is no such Primary Key in this table");
                            DataList[ThisTable.returnIndexOfPrimaryKey(key)].Data = arguments[0]; 
                        }
                        else throw new ArgumentException("There is no such argument (" + dataforException + ") in linkedColumn (" + linkedColumn.Name + ")!");
                    }
                    else throw new ArgumentException("Type of argument is not similar to Column type");
                }
                else throw new NullReferenceException("Table doesn't contain any data");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        public override void  EditColumnElementByIndex(int index, object[] arguments)
        {
            try
            {
                if (ThisTable.isTableContainsData())
                {
                    if (arguments[0] == null) throw new ArgumentException("You can't change value of FK column to null");
                    if (DataType == arguments[0].GetType())
                    {
                        if (isLinkedColumnContainsSuchValue(arguments[0]))
                        {
                            if (index == -1) throw new NullReferenceException("There is no such Primary Key in this table");
                            DataList[index].Data = arguments[0];
                        }
                        else throw new ArgumentException("There is no such argument (" + arguments[0].ToString() + ") in linkedColumn (" + linkedColumn.Name + ")!");
                    }
                    else throw new ArgumentException("Type of argument is not similar to Column type");
                }
                else throw new NullReferenceException("Table doesn't contain any data");
            }
            catch (NullReferenceException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
            }
        } //UI 
        //
        public override void AddDataElement(object argument)
        {
            try
            {

                if (isLinkedColumnContainsSuchValue(argument))
                {
                    DataList.Add(new DataObject(GetHashCode(), argument));
                }
                else throw new ArgumentException("There is no such argument (" + argument.ToString() + ") in linkedColumn (" + linkedColumn.Name + ")!");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);

            }
        }
     public override bool isLinkedColumnContainsSuchValue(object value)
        {

            for (int i = 0; i < linkedColumn.DataList.Count; i++)
            {
                if ((int)linkedColumn.DataList[i].Data == (int)value) return true;
            }
            return false;
        }
    }
}
