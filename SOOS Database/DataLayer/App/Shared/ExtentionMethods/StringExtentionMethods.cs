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
        static string[] selectOperators = new string[] {"=","!=",">","<",">=","<="};
        static string[] selectComplexOperators = new string[] { "BETWEEN", "IN", "NOT_BETWEEN", "NOT_IN" };

       static public bool isThereNoUndefinedSymbols(this string str)
        {
           if (str.Contains("ID")|| str.Contains("FK")) return false;
            foreach(char stringSymbol in str)
            {
                if (undefSymbols.Contains(stringSymbol)) return false;
            }
            return true;    
        }
        static public bool isSelectOperator(this string str)
        {
            foreach (string op in selectOperators) if (str == op) return true;
            return false;
        }
        static public bool isSelectComplexOperator(this string str)
        {
            foreach (string op in selectComplexOperators) if (str == op) return true;
            return false;
        }
        static public bool isStatusCodeOk(this string str)
        {
            if (str == "OK") return true;
            return false;
        }
    }
}
