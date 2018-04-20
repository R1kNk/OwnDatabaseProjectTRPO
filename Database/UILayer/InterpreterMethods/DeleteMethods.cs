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
            try {
                char[] separator = new char[] { ' ' };
                string[] _queryList = query.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
                if (_queryList.Length == 2)
                {
                    if (IsKeyword(_queryList[0]))
                    {
                        string _methodName = "Delete" + _queryList[0];
                        var _inst = new DeleteMethods();
                        var _method = _inst.GetType().GetMethod(_methodName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        object[] _paramas = new object[] { _queryList[1] };
                        _method?.Invoke(_inst, _paramas);
                    }
                    else throw new Exception($"\nERROR: Word '{_queryList[0]}' doesn't a keyword\n");
                }
                else throw new Exception($"\nERROR: Invalid command syntax\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// delete database |dbname|
        /// </summary>
        /// <param name="query"></param>
        static void DeleteDatabase(string query)
        {
            try
            {
                char[] _separator = new char[] { ' ' };
                string[] _params = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                if (_params.Length == 1)
                {
                    Kernel.DeleteDatabase(_params[0]);
                    if (Interpreter.ConnectionString == _params[0])
                        Interpreter.ConnectionString = null;
                }
                else throw new Exception("\nERROR: Invalid number of variables");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// delete table |tableName|
        /// </summary>
        /// <param name="query"></param>
        static void DeleteTable(string query)
        {
            try
            {
                if (Interpreter.ConnectionString != null)
                {
                    char[] _separator = new char[] { ' ' };
                    string[] _params = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (_params.Length == 1)
                    {
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(_params[0]))
                        {
                            _inst.DeleteTable(_params[0]);
                            Console.WriteLine("\nTable successfully deleted\n");
                        }
                        else throw new Exception($"There is no table '{_params[0]}' in database '{_inst.Name}'!\n");
                    }
                    else throw new Exception("\nERROR: Invalid number of variables\n");
                }
                else throw new Exception("\nThere is no connection to database\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// delete column |tableName| |colName|
        /// </summary>
        /// <param name="query"></param>
        static void DeleteColumn(string query)
        {
            try
            {
                if (Interpreter.ConnectionString != null)
                {
                    char[] _separator = new char[] { ' ' };
                    string[] _params = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (_params.Length == 2)
                    {
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(_params[0]))
                        {
                            var _table = _inst.GetTableByName(_params[0]);
                            if (_table.isColumnExists(_params[1]))
                            {
                                _table.DeleteColumn(_params[1]);
                                Console.WriteLine("\nColumn Successfully deleted\n");
                            }
                            else throw new Exception($"There is no  column '{_params[1]}' in table '{_params[0]}'!\n");
                        }
                        else throw new Exception($"There is no table '{_params[0]}' in database '{_inst.Name}'!\n");
                    }
                    else throw new Exception("\nERROR: Invalid number of variables\n");
                }
                else throw new Exception("\nThere is no connection to database\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //
        /// <summary>
        /// delete element |tableName| |primaryKeyId|
        /// </summary>
        /// <param name="query"></param>
        static void DeleteElement(string query)
        {
            try {
                if (Interpreter.ConnectionString != null)
                {
                    char[] _separator = new char[] { ' ' };
                    string[] _params = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (_params.Length == 2)
                    {
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(_params[0]))
                        {
                            var _table = _inst.GetTableByName(_params[0]);
                            int _pk = Convert.ToInt32(_params[1]);
                            _table.DeleteTableElementByPrimaryKey(_pk);
                            Console.WriteLine("\nData successfully deleted\n");
                        }
                         else throw new NullReferenceException($"There is no table '{_params[0]}' in database '{_inst.Name}'!");
                    }   
                    else throw new Exception("\nERROR: Invalid number of variables\n");
                }
                else throw new Exception("\nThere is no connection to database\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static bool IsKeyword(string word)
        {           
            foreach (var key in _keywords)
                if (word == key)
                    return true;
            return false;
        }

    }
}
