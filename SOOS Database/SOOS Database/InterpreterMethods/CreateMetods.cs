using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using DataModels.App.InternalDataBaseInstanceComponents;

namespace UILayer.InterpreterMethods
{
    class CreateMethods
    {
        static List<string> _keywords = new List<string>() { "DATABASE", "TABLE" };

        public static void Execute(string query)
        {
            try
            {
                char[] separators = new char[] { ' ' };
                string[] queryList = query.Split(separators, 2, StringSplitOptions.RemoveEmptyEntries);

                if (queryList.Length == 2)
                {
                    string _createParam = default(string);
                    _createParam = queryList[0];
                    if (IsKeyword(_createParam))
                    {
                        var _inst = new CreateMethods();
                        string _methodName = "Create" + _createParam;
                        var _method = _inst.GetType().GetMethod(_methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
                        object[] _params = new object[] { queryList[1] };
                        _method?.Invoke(_inst, _params);
                    }
                    else throw new Exception($"\nERROR: Invalid command syntax\n");
                }
                else throw new Exception($"\nERROR: Invalid number of variables\n");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        static void CreateDatabase(string dbName)
        {
            try {
                if (!Kernel.isDatabaseExists(dbName))
                {
                    Kernel.AddDBInstance(dbName);
                    Interpreter.ConnectionString = dbName;
                    Console.WriteLine($"\nDatabase created with name '{dbName}'\n");
                }
                else throw new Exception("\nERROR: Invalid database name. Some database have same name!\n");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void CreateTable(string _param)
        {
            try {
                if (Interpreter.ConnectionString != null)
                {
                    string _tableName = default(string);
                    string _tableParams = default(string);

                    if (IsCreateWithColums(_param))
                    {
                        if (IsValidSyntax(_param))
                        {
                            char[] separetor = new char[] { ' ' };
                            string[] queryList = _param.Split(separetor, 2, StringSplitOptions.RemoveEmptyEntries);
                            _tableName = queryList[0];
                            _tableParams = queryList[1];

                            char[] _temp = new char[] { ')', ';', '(' };
                            var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                            if (!_inst.isTableExists(_tableName))
                            {
                                _inst.AddTable(_tableName);
                                string[] _colParams = queryList[1].Split(_temp, StringSplitOptions.RemoveEmptyEntries);

                                foreach (var _column in _colParams)
                                {
                                    char[] _tempery = new char[] { ',', '\'' };
                                    string[] _colParam = _column.Split(_tempery, StringSplitOptions.RemoveEmptyEntries);
                                    if (_colParam.Length == 4)
                                    {
                                        if (_inst.isTableExists(_tableName))
                                        {
                                            _inst.GetTableByName(_tableName).AddColumn(GetColumn(_colParam, _inst.GetTableByName(_tableName)));
                                        }
                                        else throw new NullReferenceException($"There is no table '{_tableName}' in database '{_inst.Name}'!");
                                    }
                                    else throw new Exception("\nERROR: Invalid number of variables\n");
                                }
                                Console.WriteLine($"\nTable created with name '{_tableName}'\n");
                            }
                            else throw new Exception("\nERROR: Invalid table name. Some table in this database have same name!\n");
                        }
                        else throw new Exception("\nERROR: Invalid command syntax\n");
                    }
                    else
                    {
                        string[] temp = _param.Split(' ');
                        if (temp.Length == 1)
                        {
                            var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                            if (!_inst.isTableExists(_param))
                            {
                                _inst.AddTable(_param);
                                Console.WriteLine($"\nTable created with name '{_param}'\n");
                            }
                            else throw new Exception("\nERROR: Invalid table name. Some table in this database have same name!\n");
                        }
                        else
                            throw new Exception("\nERROR: Invalid number of variables\n");
                    }
                }
                else
                    throw new Exception("\nERROR: There is no connection to database\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    
        static bool IsCreateWithColums(string query)
        {
            if (query.Contains('(') && query.Contains(')') &&
                query.Where(x => x == '(').Count() == 1 &&
                query.Where(x => x == ')').Count() == 1)
                return true;
            return false;
        }

        static bool IsValidSyntax(string query)
        {
            if (query[query.Length - 1] == ')' && query[query.IndexOf('(') - 1] == ' ')
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

        static Column GetColumn(string[] _variables, Table thisTable)
        {
            string _colName = _variables[0];
            Type _colType = GetType(_variables[1]);
            bool _isAllowNull = Convert.ToBoolean(_variables[2]);
            object _defValue = GetDefaultValue(_variables[3], _colType);

            if (_colType != _defValue.GetType())
                Console.WriteLine("\nType of default value doesn't equals column type. Default value will be set by default\n");
            return new Column(_colName, _colType, _isAllowNull, _defValue, thisTable);
        }

        static Type GetType(string _typeName)
        {
            string _name = _typeName.ToLower();
            switch (_name)
            {
                case "int": return typeof(int);
                case "string": return typeof(string);
                case "double": return typeof(double);
                case "bool": return typeof(bool);
                default: throw new Exception($"\nERROR: Type '{_typeName}' doesn't exist");
            }


        }

        static object GetDefaultValue(string value, Type _colType)
        {
            if (_colType == typeof(int))
                return Convert.ToInt32(value);
            else if (_colType == typeof(string))
                return value;
            else if (_colType == typeof(double))
            {
                string val = value.Replace('.', ',');
                return Convert.ToDouble(val);
            }
            else if (_colType == typeof(bool))
                return Convert.ToBoolean(value);
            else throw new Exception();
        }
    }
}
