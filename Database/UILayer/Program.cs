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

            // Interpreter.Run();
            //Kernel.AddDBInstance("inst2");
            //Kernel.AddDBInstance("inst2");
            var inst = Kernel.GetInstance("inst2");

            //Kernel.SaveAllDatabases();
            //    Kernel.OutDatabaseInfo();
            // Console.WriteLine(inst.GetTableByName("Cars").OutTable());
            // Interpreter.Run();
           


            inst.GetTableByName("Cars").AddTableElement(new object[] {"mazda", 304 });
            Kernel.OutDatabaseInfo();

            Table query = inst.QueryWhereConditionSelection(inst.GetTableByName("Cars"), "Cars.Price", "=",  100);
           // query = inst.QuerySortTable("Cars.CarMark", query, false);
            Console.WriteLine(query.OutTable());




        }
    }
}
