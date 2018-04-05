using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataLayer.Shared.ExtentionMethods
{

    public static class StringExtentionMethods
    {
        static string undefSymbols = "#^&()-=+[]~'//\\.,;|? ";
        


       static public bool isThereNoUndefinedSymbols(this string str)
        {
            foreach(char stringSymbol in str)
            {
                if (undefSymbols.Contains(stringSymbol)) return false;
            }
            return true;    
        }
    }
}
