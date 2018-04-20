using DataLayer;
using DataModels.App.InternalDataBaseInstanceComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UILayer.InterpreterMethods
{
    class EditMethods
    {
        //(ColName=Param,...)

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
                    string[] _params = queru.Split(_separator,3, StringSplitOptions.RemoveEmptyEntries);
                    string _tableName = _params[0];
                    if (_params.Length == 3)
                    {
                        if (IsKeyword(_params[1]))
                        {
                            var _inst = new EditMethods();
                            string _methodName = "Edit" + _params[1];
                            var _method = _inst.GetType().GetMethod(_methodName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                            _method?.Invoke(_inst, new object[] { _tableName, _params[2] });
                        }
                        else throw new Exception($"ERROR: Invalid command syntax\n");
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
                string[] _queryList = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                if (_queryList.Length == 2)
                {
                    if (IsValidSyntax(query))
                    {
                        int _elementId = Convert.ToInt32(_queryList[0]);
                        char[] _sep = new char[] { ',', '(', ')' };
                        string[] _params = _queryList[1].Split(_sep, StringSplitOptions.RemoveEmptyEntries);
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(tableName))
                        {
                            var _table = _inst.GetTableByName(tableName);
                            foreach (var param in _params)
                            {
                                char[] sep = new char[] { '=' };
                                string[] temp = param.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                                if (temp.Length == 2)
                                {
                                    string colName = temp[0];
                                    string value = temp[1];
                                    if (_table.isColumnExists(colName))
                                    {
                                        var _column = _table.GetColumnByName(colName);
                                        object data = GetData(value, _column);
                                        _column.EditColumnElementByPrimaryKey(_elementId, data);
                                    }
                                    else throw new Exception();
                                }
                                else throw new Exception();
                            }                             
                                Console.WriteLine("\nAll data successfully edited\n");                      
                        }
                        else throw new NullReferenceException($"There is no table '{tableName}' in database '{_inst.Name}'!\n");

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
                if (_params.Length == 2)
                {
                    var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                    if (_inst.isTableExists(tableName))
                    {
                        var _table = _inst.GetTableByName(tableName);
                        if (_table.isColumnExists(_params[0]))
                        {
                            var _column = _table.GetColumnByName(_params[0]);
                            _column.SetNullableProperty(Convert.ToBoolean(_params[1]));
                            Console.WriteLine($"\nNull property of column {_params[0]} setted '{_params[1]}'\n");
                        } else throw new NullReferenceException("\nERROR: There is no column " + _params[0] + " in table "+tableName+"!\n");
                    }
                    else throw new NullReferenceException("\nERROR: There is no table " +tableName+" in this database!\n");
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
                if (_params.Length == 3)
                {
                    if (_params[0] == "VALUE")
                    {
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(tableName))
                        {
                            var _table = _inst.GetTableByName(tableName);
                            if (_table.isColumnExists(_params[1]))
                            {
                                var _column = _table.GetColumnByName(_params[1]);
                                _column.SetDefaultObject(GetData(_params[2], _column));
                                Console.WriteLine("\nDefault value succesfully setted\n");
                            }
                            else throw new NullReferenceException("\nERROR: There is no column " + _params[1] + " in table " + tableName + "!\n");
                        }
                        else throw new NullReferenceException("\nERROR: There is no table " + tableName + " in this database!\n");
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
                string[] _params = query.Split(_separator,StringSplitOptions.RemoveEmptyEntries);
                if (_params.Length == 2)
                {
                    var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                    if (_inst.isTableExists(tableName))
                    {
                        var _table = _inst.GetTableByName(tableName);
                        if (_table.isColumnExists(_params[0]))
                        {
                            var _column = _table.GetColumnByName(_params[0]);
                            _column.EditColumnType(GetType(_params[1]));
                            Console.WriteLine("\nType successfully changed\n");
                        }
                        else throw new NullReferenceException("\nERROR: There is no column " + _params[1] + " in table " + tableName + "!\n");
                    }
                    else throw new NullReferenceException("\nERROR: There is no table " + tableName + " in this database!\n");
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
            foreach (var key in _keywords)
                if (key == param)
                    return true;
            return false;
        }

        static object GetData(string value, Column column)
        {
            if (value.ToLower() == "null")
            {
                if (column.AllowsNull) return null;
            }

            if (column.DataType == typeof(string))
                return value;
            else if (column.DataType == typeof(int))
                return Convert.ToInt32(value);
            else if (column.DataType == typeof(double))
            {
                value = value.Replace('.', ',');
                return Convert.ToDouble(value);
            }
            else if (column.DataType == typeof(bool))
            {
                return Convert.ToBoolean(value);
            }
            else throw new Exception("\nERROR\n");
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
