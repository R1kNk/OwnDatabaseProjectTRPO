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
        //Interpreter.Run();


            // Interpreter.Run();
            //Kernel.AddDBInstance("inst2");
            //Kernel.AddDBInstance("inst2");
            var inst = Kernel.GetInstance("inst2");

            //Kernel.SaveAllDatabases();
            //    Kernel.OutDatabaseInfo();
            // Console.WriteLine(inst.GetTableByName("Cars").OutTable());
            // Interpreter.Run();
           

            Kernel.OutDatabaseInfo();
           
            Table query = inst.QueryWhereConditionSelection(inst.GetTableByName("Cars"), "Cars.DoubleValue", "NOT_BETWEEN",  new object[] { 0.0 , 120.5} );
           // query = inst.QuerySortTable("Cars.CarMark", query, false);
            Console.WriteLine(query.OutTable());

        }

    }

   
}
