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
    public class LinkColumn : Column
    {
        Column linkedColumn;
        public LinkColumn(string name, Type DataType, bool allowsnull, object def, Column linkedcolumn) : base(name, DataType, allowsnull, def)
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

        public override void EditColumnElementByPrimaryKey(Table thisTable, int key, object argument)
        {
            if (thisTable.isTableContainsData())
            {
                if (DataType == argument.GetType())
                {
                    if (isLinkedColumnContainsSuchValue(argument))
                        DataList[thisTable.returnIndexOfPrimaryKey(key)].Data = argument;
                    else throw new ArgumentException("There is no such argument ("+argument.ToString()+") in linkedColumn (" + linkedColumn.Name + ")!");
                }
                else throw new ArgumentException("Type of argument is not similar to Column type");
            }
            else throw new NullReferenceException("Table doesn't contain any data");
        }
        bool isLinkedColumnContainsSuchValue(object value)
        {
            for (int i = 0; i < linkedColumn.DataList.Count; i++)
            {
                if ((int)linkedColumn.DataList[i].Data == (int)value) return true;
            }
            return false;
        }
    }
}
