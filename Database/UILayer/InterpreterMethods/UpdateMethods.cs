using DataLayer;
using DataModels.App.InternalDataBaseInstanceComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UILayer.InterpreterMethods
{
    class UpdateMethods
    {
        //(ColName=Param,ColName=)

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
                            var _inst = new UpdateMethods();
                            string _methodName = "Update" + _params[1];
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

        
        
        static void UpdateElement(string tableName, string query)
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
                                        string status;
                                        var _column = _table.GetColumnByName(colName);
                                        object data = GetData(value, _column, out status);
                                        if (status != "OK")
                                            throw new Exception("\nERROR: Ivalid type of data\n");
                                    }
                                    else throw new Exception($"\nERROR: Column with name '{temp[0]}' doesn't exist\n");
                                }
                                else throw new Exception("\nERROR: Invalid number of variables\n");
                            }// проверка

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
                                        string status;
                                        var _column = _table.GetColumnByName(colName);
                                        object data = GetData(value, _column,out status);
                                        _column.EditColumnElementByPrimaryKey(_elementId, data);
                                    }
                                    else throw new Exception($"\nERROR: Column with name '{temp[0]}' doesn't exist\n");
                                }
                                else throw new Exception("\nERROR: Invalid number of variables\n");
                            }// добавление                   
                             Console.WriteLine("\nAll data successfully edited\n");                      
                        }
                        else throw new NullReferenceException($"There is no table '{tableName}' in database '{_inst.Name}'!\n");

                    }
                    else throw new Exception("\nERROR: Invalid command syntax\n");
                }
                else if (_queryList.Length==1)
                {
                    if(IsValidSyntax(_queryList[0]))
                    {
                        char[] _sep = new char[] { ',', '(', ')' };
                        string[] _params = _queryList[0].Split(_sep, StringSplitOptions.RemoveEmptyEntries);
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(tableName))
                        {
                            var _table = _inst.GetTableByName(tableName);
                            for(int i=0;i<_params.Length;i++)
                            {
                                char[] sep = new char[] { '=' };
                                string[] temp = _params[i].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                                if (temp.Length == 2)
                                {
                                    if (!_table.isColumnExists(temp[0]))
                                    {
                                        throw new Exception($"\nERROR: Column with name '{temp[0]}' doesn't exist");
                                    }
                                    else
                                    {
                                        var _column = _table.GetColumnByName(temp[0]);
                                        string status;
                                        object data = GetData(temp[1], _column, out status);
                                        if (status != "OK")
                                            throw new Exception("\nERROR: Ivalid type of data\n");
                                    }
                                }
                                else throw new Exception("\nERROR: Invalid number of variables\n");
                            }

                            foreach (var param in _params)
                            {
                                char[] sep = new char[] { '=' };
                                string[] temp = param.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                                if (temp.Length == 2)
                                {
                                    string status;
                                    var _column = _table.GetColumnByName(temp[0]);
                                    object data = GetData(temp[1], _column, out status);
                                    for (int i = 0; i < _column.DataList.Count; i++)
                                        _column.EditColumnElementByPrimaryKey(_table.returnPrimaryKeyOfIndex(i), data);
                                }
                                else throw new Exception("\nERROR: Invalid number of variables\n");
                            }
                        }
                        else throw new Exception($"ERROR: Table with name '{tableName}' doesn't exist\n");
                    }
                    else throw new Exception("\nERROR: Invalid command syntax\n");
                }
                else throw new Exception("\nERROR: Invalid number of variables\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void UpdateNullProperty(string tableName, string query)
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

        static void UpdateDefault(string tableName, string query)
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
                                string status;
                                var _column = _table.GetColumnByName(_params[1]);
                                _column.SetDefaultObject(GetData(_params[2], _column,out status));
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

        static void UpdateType(string tableName, string query)
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
                command[command.Length - 1] == ')')
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
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                status = "ERROR";
                return null;
            }
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
