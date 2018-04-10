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
            var inst = Kernel.GetInstance("inst2");
        //    Kernel.OutDatabaseInfo();
           // Console.WriteLine(inst.GetTableByName("Cars").OutTable());
            // Interpreter.Run();
           Table kk = inst.ColumnSelection(new List<string> { "Cars.IDCars", "Cars.Price" }, inst.GetTableByName("Cars"));
            //Column asad = new Column("Car", typeof(string), false, "ke", inst.GetTableByName("Cars"));
           Console.WriteLine(kk.OutTable());

            Kernel.OutDatabaseInfo();

        }
    }
}
