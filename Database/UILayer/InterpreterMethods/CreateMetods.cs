using DataLayer;
//using DataLayer.InternalDataBaseInstanceComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Shared.ExtentionMethods;

namespace UILayer.InterpreterMethods
{
    static class CreateMethods
    {
        static List<string> _keywords = new List<string>() { "DATABASE", "TABLE" };

        public static void Execute(string query)
        {
            char[] separators = new char[] { ' ' };
            string[] queryList = query.Split(separators, 2, StringSplitOptions.RemoveEmptyEntries);

            if (queryList.Length == 2)
            {
                string _createParam = default(string);
                _createParam = queryList[0];
                if (IsKeyword(_createParam))
                {
                    if (_createParam.ToUpper() == _keywords[0])
                        CreateDatabase(queryList[1]);
                    else
                        CreateTable(queryList[1]);
                }
                else
                    Console.WriteLine($"\nERROR: Invalid command syntax\n");
            }
            else
                Console.WriteLine($"\nERROR: Invalid number of variables\n");
        }

        static void CreateDatabase(string dbName)
        {
            Kernel.AddDBInstance(dbName);
            Interpreter.ConnectionString = dbName;
            Console.WriteLine($"\nDatabase created with name '{dbName}'\n");
        }

        static void CreateTable(string _param)
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

                    if (Interpreter.ConnectionString != "")
                    {
                        char[] _temp = new char[] { ')', ';', '(' };
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        _inst.AddTable(_tableName);
                        string[] _colParams = queryList[1].Split(_temp, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var _column in _colParams)
                        {
                            char[] _tempery = new char[] { ',', '\'' };
                            string[] _colParam = _column.Split(_tempery, StringSplitOptions.RemoveEmptyEntries);
                            if (_colParam.Length == 4)
                            {
                                _inst.GetTableByName(_tableName).AddColumn(GetColumn(_colParam));
                            }
                        }
                        Console.WriteLine($"\nTable created with name '{_tableName}'\n");
                    }
                    else
                        throw new Exception("\nERROR: There is no connection to database\n");
                }
                else
                    throw new Exception("\nERROR: Invalid command syntax\n");
            }
            else
            {
                string[] temp = _param.Split(' ');
                if (temp.Length == 1)
                {
                    if (Interpreter.ConnectionString != null)
                    {
                        var inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        inst.AddTable(_param);
                        Console.WriteLine($"\nTable created with name '{_param}'\n");
                    }
                    else
                        throw new Exception("\nERROR: There is no connection to database\n");
                }
                else
                    throw new Exception("\nERROR: Invalid comand syntax\n");
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
            string temp = param.ToUpper();
            foreach (var key in _keywords)
                if (key == temp)
                    return true;
            return false;
        }

        static Column GetColumn(string[] _variables)
        {
            string _colName = _variables[0];
            Type _colType = GetType(_variables[1]);
            bool _isAllowNull = Convert.ToBoolean(_variables[2]);
            object _defValue = GetDefaultValue(_variables[3], _colType);

            if (_colType != _defValue.GetType())
                Console.WriteLine(_defValue.GetType().Name);
            return new Column(_colName, _colType, _isAllowNull, _defValue);
        }

        static Type GetType(string _typeName)
        {
            string _name = _typeName.ToLower();
            switch (_name)
            {
                case "int": return typeof(int);
                case "string": return typeof(string);
                case "double": return typeof(double);
                default: throw new Exception();
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
                return Convert.ToDouble(value);
            }

            else throw new Exception();
        }
    }
}
