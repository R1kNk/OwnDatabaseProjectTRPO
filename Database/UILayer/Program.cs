using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.InternalDataBaseInstanceComponents;

namespace UILayer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Kernel.AddDBInstance("inst2");
            var inst = Kernel.GetInstance("inst2");
            //inst.AddTable("table");
            //inst.TablesDB[0].AddColumn(new Column("testColumn", typeof(string), true, "testDef"));
            inst.GetTableByName("table").GetColumnByName("testColumn").SetNullableProperty(true);
            inst.TablesDB[0].AddTableElement(new object[] { null });

            // Kernel.SaveAllDatabases();
            Kernel.OutDatabaseInfo();
            // Interpreter.Run();


        }
    }
}
