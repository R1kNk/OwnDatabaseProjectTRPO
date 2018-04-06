using System;
using System.Collections.Generic;
using System.Text;

namespace DataModels.App.Shared.Events
{
    public class CascadeDeleteEventArgs:EventArgs
    {
        private readonly string columnName;
        private readonly int deletedRowPrimaryKey;
        public CascadeDeleteEventArgs(string columnname, int deletedrowprimarykey)
        {
            columnName = columnname;
            deletedRowPrimaryKey = deletedrowprimarykey;
        }
        public string ColumnName => columnName;
        public int DeletedRowPrimaryKey => deletedRowPrimaryKey;
        //

    }
}
