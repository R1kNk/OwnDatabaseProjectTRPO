using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataModels.App.InternalDataBaseInstanceComponents;

namespace UILayer
{
    class Program
    {
        static void Main(string[] args)
        {

            
            //Kernel.AddDBInstance("inst2");
            //var inst = Kernel.GetInstance("inst2");
            //inst.AddTable("Persons");
            //inst.TablesDB[0].AddColumn(new Column("LastName", typeof(string), false, "#undefLastName", inst.TablesDB[0]));

            //inst.TablesDB[0].AddTableElement(new object[] { "Familiya1" });
            //inst.TablesDB[0].AddTableElement(new object[] { "Familiya2" });
            //inst.TablesDB[0].AddTableElement(new object[] { "Familiya3" });

            //inst.AddTable("Cars");
            //inst.TablesDB[1].AddColumn(new Column("CarMark", typeof(string), false, "#undefCarName", inst.TablesDB[1]));
            //inst.TablesDB[1].AddColumn(new Column("Price", typeof(int), false, 0, inst.TablesDB[1]));

            //inst.TablesDB[1].AddTableElement(new object[] { "mazda", 200 });
            //inst.TablesDB[1].AddTableElement(new object[] { "mers", 150 });
            //inst.TablesDB[1].AddTableElement(new object[] { "porche", 100 });
            ////Kernel.SaveAllDatabases();
            //Kernel.OutDatabaseInfo();
            //Console.WriteLine("\n\n\n\n\n\n");
            //inst.LinkTables(inst.GetTableByName("Persons"), inst.GetTableByName("Cars"), true);
            //Kernel.OutDatabaseInfo();
            //inst.TablesDB[0].EditTableElementByPrimaryKey(1, new object[] { "Familiya1", 1 });
            //inst.TablesDB[0].EditTableElementByPrimaryKey(2, new object[] { "Familiya2", 1 });
            //inst.TablesDB[0].EditTableElementByPrimaryKey(3, new object[] { "Familiya3", 2 });
            //Console.WriteLine("\n\n\n\n\n\n");
            //Kernel.OutDatabaseInfo();

            ////inst.DeleteTable("Cars");
            ////
            //inst.TablesDB[1].DeleteTableElementByPrimaryKey(1);
            ////
            //Console.WriteLine("\n\n\n\n\n\n");

            //Kernel.OutDatabaseInfo();
            //inst.EditCascadeDeleteOption(inst.TablesDB[0], inst.TablesDB[1], false);
            //inst.TablesDB[1].DeleteTableElementByPrimaryKey(2);
            //Console.WriteLine("\n\n\n\n\n\n");

            //Kernel.OutDatabaseInfo();
            Interpreter.Run();

        }
    }
}
