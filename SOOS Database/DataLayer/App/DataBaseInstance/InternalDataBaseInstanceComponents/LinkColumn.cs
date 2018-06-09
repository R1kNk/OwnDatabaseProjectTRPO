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
        /// <summary>
        /// General constructor for Fkey Column
        /// </summary>
        /// <param name="name">Columns Name</param>
        /// <param name="DataType">Type of column</param>
        /// <param name="allowsnull">Is it allows null</param>
        /// <param name="def">Default column object</param>
        /// <param name="thisTable">Table which will contain this </param>
        /// <param name="linkedcolumn"></param>
        /// <exception cref="ArgumentException">Throws when you trying connect not Pkey column </exception>
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
        /// <summary>
        /// Overriden edit clumn element by primary key method
        /// </summary>
        /// <param name="key">Pkey of element</param>
        /// <param name="arguments">Objet array with single argument</param>
        /// <exception cref="ArgumentException">Throws when you trying t ochange value of FK column to null</exception>
        /// <exception cref="ArgumentException">Throws where is no such pk in table</exception>
        /// <exception cref="ArgumentException">Throws when there is no such argument in linked column</exception>
        /// <exception cref="ArgumentException">Throws then the type of argument os not similar to clumn type</exception>
        /// <exception cref="NullReferenceException">Throws when table doesn't contain data</exception>
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
        /// <summary>
        /// Overriden method to edit column element by index
        /// </summary>
        /// <param name="index">Index of element</param>
        /// <param name="arguments">Index of element</param>
        /// <exception cref="ArgumentException">Throws when you can't change value of FK column to null</exception>
        /// <exception cref="ArgumentException">Throws where is no such pk in table</exception>
        /// <exception cref="ArgumentException">Throws when there is no such argument in linked column</exception>
        /// <exception cref="ArgumentException">Throws then the type of argument os not similar to column type</exception>
        /// <exception cref="NullReferenceException">Throws when there is no such Primary Key in this table</exception>
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
        /// <summary>
        /// Adds element to column
        /// </summary>
        /// <param name="argument">Obect to add</param>
        /// <exception cref="ArgumentException">Throws whem is no such argument in linked column</exception>
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
        /// <summary>
        /// Overridden method for Fkey column which checks is linked column contains such value
        /// </summary>
        /// <param name="value">Object to check</param>
        /// <returns></returns>
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
