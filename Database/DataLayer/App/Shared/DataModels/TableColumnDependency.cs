using DataModels.App.InternalDataBaseInstanceComponents;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataModels.App.Shared.DataModels
{
    class TableColumnDependency
    {
        string tableName;
        string columnName;
        string inColumnInfo;

        public string TableName { get => tableName; private set => tableName = value; }
        public string ColumnName { get => columnName; private set => columnName = value; }

        public TableColumnDependency(string inColumnInfo)
        {
            if (inColumnInfo.Contains("."))
            {
                string[] splittedInfo = inColumnInfo.Split('.');
                if (splittedInfo.Length != 2) throw new ArgumentException("Invalid inColumnInfo. It must contain only one '.' symbol!");
                else { TableName = splittedInfo[0]; ColumnName = splittedInfo[1]; }
                this.inColumnInfo = inColumnInfo;
            }
            else throw new ArgumentException("Invalid inColumnInfo. It must contain at least one '.' symbol!");
        }

       
    }
}
