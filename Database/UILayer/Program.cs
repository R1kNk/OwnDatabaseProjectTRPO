﻿using System;
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
            //Only for testing purposes!!
            var some = Kernel.GetInstance("first");
            some.AddTable(new Table("same"));
            string xx = default(string);
            xx = Console.ReadLine();
           
        }
    }
}
