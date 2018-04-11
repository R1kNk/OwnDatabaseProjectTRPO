using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UILayer.InterpreterMethods
{
    class DeleteMethods
    {
        static List<string> _keywords = new List<string>()
        {
            "DATABASE",
            "TABLE",
            "COLUMN",
            "ELEMENT"
        };



        public static void Execute(string query)
        {
            char[] separator = new char[] { ' ' };
            string[] _queryList = query.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
            if (_queryList.Length == 2)
            {
                if (IsKeyword(_queryList[0]))
                {
                    string _methodName = "Delete" + _queryList[0];
                    var _inst = new DeleteMethods();
                    var _method = _inst.GetType().GetMethod(_methodName,System.Reflection.BindingFlags.IgnoreCase|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);
                    object[] _paramas = new object[] { _queryList[1] };
                    _method.Invoke(_inst, _paramas);
                }
                else throw new Exception();
            }
            else throw new Exception();
        }
        /// <summary>
        /// delete database |dbname|
        /// </summary>
        /// <param name="query"></param>
        static void DeleteDatabase(string query)
        {
            char[] _separator = new char[] { ' ' };
            string[] _params = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
            if (_params.Length == 1)
            {
                if (Kernel.isDatabaseExists(_params[0]))
                { } //Here must be delete of database
                else throw new Exception();
            }
            else throw new Exception();
        }
        /// <summary>
        /// delete table |tableName|
        /// </summary>
        /// <param name="query"></param>
        static void DeleteTable(string query)
        {
            if (Interpreter.ConnectionString != "")
            {
                char[] _separator = new char[] { ' ' };
                string[] _params = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                if (_params.Length == 1)
                {
                    var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                    if (_inst.isTableExists(_params[0]))
                    {
                        _inst.DeleteTable(_params[0]);
                    }
                    else throw new Exception();
                }
                else throw new Exception();
            }
            else throw new Exception();
        }
        static void DeleteColumn(string query)
        {
            if (Interpreter.ConnectionString != "")
            {
                Console.WriteLine("Succes");
            }
            else throw new Exception();
        }
        static void DeleteElement(string query)
        {
            if (Interpreter.ConnectionString != "")
            {
                Console.WriteLine("Succes");
            }
            else throw new Exception();
        }

        static bool IsKeyword(string word)
        {
            string _key = word.ToUpper();
            foreach (var key in _keywords)
                if (_key == key)
                    return true;
            return false;
        }

    }
    
    
    //delete column tbname colname
    //delete value tbname primary
}
