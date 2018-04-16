using DataLayer;
using DataModels.App.InternalDataBaseInstanceComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UILayer.InterpreterMethods
{
    class EditMethods
    {
        static List<string> _keywords = new List<string>()
        {
            "NULLPROPERTY",
            "DEFAULT",
            "TYPE",
            "ELEMENT"
        };

        public static void Execute(string queru)
        {
            try {
                if (Interpreter.ConnectionString != null)
                {
                    char[] _separator = new char[] { ' ' };
                    string[] tableNames = queru.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (tableNames.Length == 1)
                    {
                        string _tableName = tableNames[0];
                        Console.WriteLine("\nAll commands:\n" +
                            "Element |ElementID| (<params>)\n" +
                            "NullProperty |colName| |true/false|\n" +
                            "Default value |colName| |value|\n" +
                            "Type |colName| |newType|\n");

                        string _command = Console.ReadLine();
                        string[] _commandList = _command.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                        if (IsKeyword(_commandList[0]))
                        {
                            var _inst = new EditMethods();
                            string _methodName = "Edit" + _commandList[0];
                            var _method = _inst.GetType().GetMethod(_methodName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                            _method?.Invoke(_inst, new object[] { _tableName, _command });
                        }
                        else throw new Exception($"\nERROR: Word {_commandList[0]} doesn't keyword\n");
                    }
                    else throw new Exception("\nERROR: Invalid number of variables\n");
                }
                else throw new Exception("\nERROR: There is no connection to database\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void EditElement(string tableName, string query)
        {
            try {
                char[] _separator = new char[] { ' ' };
                string[] _queryList = query.Split(_separator, 3, StringSplitOptions.RemoveEmptyEntries);
                if (_queryList.Length == 3)
                {
                    if (IsValidSyntax(query))
                    {
                        int _elementId = Convert.ToInt32(_queryList[1]);
                        char[] _sep = new char[] { ',', '(', ')' };
                        string[] _params = _queryList[2].Split(_sep, StringSplitOptions.RemoveEmptyEntries);
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        var _table = _inst.GetTableByName(tableName);
                        if (_table.Columns.Count - 1 == _params.Length)
                        {
                            object[] _data = new object[_params.Length];
                            for (int i = 0; i < _params.Length; i++)
                            {
                                _data[i] = GetData(_params[i], _table.Columns[i + 1]);
                            }
                            _table.EditTableElementByPrimaryKey(_elementId, _data);
                            Console.WriteLine("\nAll data successfully edited\n");
                        }
                        else throw new Exception("\nERROR: Count of params doesn't equals count of columns\n");
                    }
                    else throw new Exception("\nERROR: Invalid command syntax\n");
                }
                else throw new Exception("\nERROR: Invalid number of variables\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void EditNullProperty(string tableName, string query)
        {
            try {
                char[] _separator = new char[] { ' ' };
                string[] _params = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                if (_params.Length == 3)
                {
                    var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                    var _table = _inst.GetTableByName(tableName);
                    var _column = _table.GetColumnByName(_params[1]);
                    _column.SetNullableProperty(Convert.ToBoolean(_params[2]));
                    Console.WriteLine($"\nNull property of column {_params[1]} setted {_params[2]}\n");
                }
                else throw new Exception("\nERROR: Invalid number of variables\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void EditDefault(string tableName, string query)
        {
            try {
                char[] _separator = new char[] { ' ' };
                string[] _params = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                if (_params.Length == 4)
                {
                    if (_params[1].ToUpper() == "VALUE")
                    {
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        var _table = _inst.GetTableByName(tableName);
                        var _column = _table.GetColumnByName(_params[3]);
                        _column.SetDefaultObject(GetData(_params[3], _column));
                        Console.WriteLine("Default value succesfully setted\n");
                    }
                    else throw new Exception("\nERROR: Invalid command syntax\n");
                }
                else throw new Exception("\nERROR: Invalid number of variables\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void EditType(string tableName, string query)
        {
            try
            {
                char[] _separator = new char[] { ' ' };
                string[] _params = query.Split(_separator, 3, StringSplitOptions.RemoveEmptyEntries);
                if (_params.Length == 3)
                {
                    var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                    var _table = _inst.GetTableByName(tableName);
                    var _column = _table.GetColumnByName(_params[1]);
                    _column.EditColumnType(GetType(_params[2]));
                    Console.WriteLine("\nType successfully changed\n");
                }
                else throw new Exception("\nERROR: Invalid number of variales\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static bool IsValidSyntax(string command)
        {
            if (command.Contains('(') && command.Contains(')') &&
                command.Where(x => x == '(').Count() == 1 &&
                command.Where(x => x == ')').Count() == 1&&
                command[command.Length - 1] == ')' && command[command.IndexOf('(') - 1] == ' ')
                return true;
            return false;
        }

        static bool IsKeyword(string param)
        {
            string temp = param.ToUpper();
            foreach (var key in _keywords)
                if (key == temp)
                    return true;
            return false;
        }

        static object GetData(string value, Column column)
        {
            if (column.DataType == typeof(string))
                return value;
            else if (column.DataType == typeof(int))
                return Convert.ToInt32(value);
            else if (column.DataType == typeof(double))
                return Convert.ToDouble(value);
            else if (column.DataType == typeof(bool))
            {
                value = value.Replace('.', ',');
                return Convert.ToBoolean(value);
            }
            else if (value == "null")
                return null;
            else return null;
        }

        static Type GetType(string typeName)
        {
            string _name = typeName.ToLower();
            switch (_name)
            {
                case "int": return typeof(int);
                case "string": return typeof(string);
                case "double": return typeof(double);
                case "bool": return typeof(bool);
                default: throw new Exception($"\nERROR: Type {typeName} doesn't exist");
            }


        }
        
    }
}
