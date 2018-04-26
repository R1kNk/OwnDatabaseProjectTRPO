using DataLayer;
using DataModels.App.InternalDataBaseInstanceComponents;
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
        static List<string> _operators = new List<string>() {
            "IN",
            "NOT_IN",
            "BETWEEN",
            "NOT_BETWEEN"
        };
        static List<string> _separtors = new List<string>()
        {
            "!=",
            "<=",
            ">=",
            "=",
             "<",
            ">",
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
        /// DELETE ELEMENT |tableName| WHERE (colName=value,...)
        /// DELETE ELEMENT |tableName| WHERE |colName| BETWEEN (1,2) 
        /// DELETE ELEMENT |tableName| *
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
                    }   //With ID
                    else if(_params.Length==3)
                    {
                        if (!(_params[1] == "WHERE"&&IsValidSyntax(_params[2]))) throw new Exception("\nERROR: Invalid command syntax\n");//syntax check

                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        string tableName = _params[0];

                        if (!_inst.isTableExists(tableName)) throw new Exception($"There is no table '{_params[0]}' in database '{_inst.Name}'!");//Is table exist check

                        var _table = _inst.GetTableByName(tableName);

                        char[] _sep = new char[] { '(', ',', ')' };
                        string[] condParams = _params[2].Split(_sep, StringSplitOptions.RemoveEmptyEntries);

                        if (!(condParams.Length == 1)) throw new Exception("\nERROR: Invalid number of variables in condition params\n");//Is valid params check
                        if (!IsValidSyntax(condParams, _table)) throw new Exception("\nERROR: Invalid command syntax in condition params\n");//check types and values of condition

                        string condOperator = GetSeparator(condParams[0]);
                        string[] temp = new string[] { condOperator };
                        string status;
                        string buff = "OK";

                        string colName = condParams[0].Split(temp,StringSplitOptions.None)[0];
                        string value= condParams[0].Split(temp, StringSplitOptions.None)[1];
                        object data = GetData(value, _table.GetColumnByName(colName), out status);

                        _inst.QueryWhereConditionDelete(_table, colName, condOperator, data, ref buff);
                        Console.WriteLine("\nAll data successfully deleted\n");
                    } //With Column and value
                    else if(_params.Length==5)
                    {
                        if (!(_params[1] == "WHERE" && IsOperator(_params[3]))&&IsValidSyntax(_params[4])) throw new Exception("\nERROR: Invalid command syntax\n");

                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        string tableName = _params[0];

                        if (!_inst.isTableExists(tableName)) throw new Exception($"There is no table '{_params[0]}' in database '{_inst.Name}'!");

                        var _table = _inst.GetTableByName(tableName);
                        string colName = _params[2];

                        if (!_table.isColumnExists(colName)) throw new Exception($"There is no column '{colName}' in table '{tableName}'!");

                        var _column = _table.GetColumnByName(colName);

                        char[] _sep = new char[] { '(', ',', ')' };
                        string[] values = _params[4].Split(_sep, StringSplitOptions.RemoveEmptyEntries);
                        object[] data = new object[values.Length];

                        for (int i = 0; i < data.Length; i++)
                        {
                            string status;
                            data[i] = GetData(values[i], _column, out status);
                            if (!(status == "OK")) throw new Exception("\nERROR: Invalid variables in condition params\n");
                        }

                        string buff = "OK";
                        _inst.QueryWhereConditionDelete(_table, colName, _params[3], data, ref buff);
                        Console.WriteLine("\nAll data successfully deleted\n");
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

        static bool IsValidSyntax(string command)
        {
            if (command.Contains('(') && command.Contains(')') &&
                command.Where(x => x == '(').Count() == 1 &&
                command.Where(x => x == ')').Count() == 1 &&
                command[command.Length - 1] == ')')
                return true;
            return false;
        }

        static bool IsOperator(string operat)
        {
            foreach (var op in _operators)
                if (op == operat)
                    return true;
            return false;
        }

        static object GetData(string value, Column column, out string status)
        {
            try
            {
                if (value.ToLower() == "null")
                {
                    if (column.AllowsNull)
                    {
                        status = "OK";
                        return null;
                    }
                }

                if (column.DataType == typeof(string))
                {
                    status = "OK";
                    return value;
                }
                else if (column.DataType == typeof(int))
                {
                    status = "OK";
                    return Convert.ToInt32(value);
                }
                else if (column.DataType == typeof(double))
                {
                    status = "OK";
                    value = value.Replace('.', ',');
                    return Convert.ToDouble(value);
                }
                else if (column.DataType == typeof(bool))
                {
                    status = "OK";
                    return Convert.ToBoolean(value);
                }
                else throw new Exception("\nERROR\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                status = "ERROR";
                return null;
            }
        }

        static string GetSeparator(string param)
        {
            foreach (var sep in _separtors)
                if (param.Contains(sep))
                    return sep;
            throw new Exception("\nERROR: Invalid charecter in condition params\n");
        }

        static bool IsValidSyntax(string[] variables, Table table)
        {
            foreach (var param in variables)
            {
                string[] tem = new string[1];
                tem[0] = GetSeparator(param);
                string[] temp = param.Split(tem, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length == 2)
                {
                    if (!table.isColumnExists(temp[0]))
                    {
                        throw new Exception($"\nERROR: Column with name '{temp[0]}' doesn't exist\n");
                    }
                    else
                    {
                        var _column = table.GetColumnByName(temp[0]);
                        string status;
                        object data = GetData(temp[1], _column, out status);
                        if (status != "OK")
                            throw new Exception("\nERROR: Ivalid type of data\n");
                    }
                }
                else throw new Exception("\nERROR: Invalid number of variables\n");
            }
            return true;
        }
    }
}
