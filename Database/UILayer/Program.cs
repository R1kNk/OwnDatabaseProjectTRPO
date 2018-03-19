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
            //Kernel.AddDBInstance("lul");
            //var inst = Kernel.GetInstance("lul");
            //inst.AddTable("sus");
            //inst.TablesDB[0].AddColumn(new Column("testColumn", typeof(string), true, false, false, "testDef"));
            //inst.TablesDB[0].AddTableElement(new object[] { null });
            //inst.SaveDataBaseInstanceToFolder();
            Kernel.OutDatabaseInfo();
            ////Interpreter.Run();
        }
    }
}
