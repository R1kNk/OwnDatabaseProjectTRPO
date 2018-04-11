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
            Kernel.OutDatabaseInfo();
            inst.GetTableByName("Cars").AddTableElement(new object[] { "ain", 304 });
            Table query = inst.QueryColumnSelection(new List<string> { "Cars.IDCars", "Cars.CarMark" }, inst.GetTableByName("Cars"));
            query = inst.QuerySortTable("Cars.CarMark", query, false);
            Console.WriteLine(query.OutTable());




        }
    }
}
