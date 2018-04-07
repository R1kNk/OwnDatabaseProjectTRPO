using DataLayer;
using DataModels.App.InternalDataBaseInstanceComponents;
using System;
using System.Collections.Generic;

namespace UILayer.InterpreterMethods
{
    class InsertMethods
    {
        static List<string> _keywords = new List<string>()
        {
            "VALUES",
            "COLUMN"
        };

        //insert values tableName

        public static void Execute(string query)
        {
            if (Interpreter.ConnectionString != "")
            {
                char[] separator = new char[] { ' ' };
                string[] queryList = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (queryList.Length == 2)
                {
                    if (IsKeyword(queryList[0]))
                    {
                        if (queryList[0].ToUpper() == _keywords[0])
                            InsertValues(queryList[1]);
                        else
                            InsertColumn(queryList[1]);
                    }
                }
            }
        }

        public static void InsertColumn(string tableName)
        {
            var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
            var _table = _inst.GetTableByName(tableName);
            string param = Console.ReadLine();


            char[] _separator = new char[] { ';' };
            string[] _colParams = param.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var _column in _colParams)
            {
                char[] _tempery = new char[] { ',', ' ' };
                string[] _colParam = _column.Split(_tempery, StringSplitOptions.RemoveEmptyEntries);
                if (_colParam.Length == 4)
                {
                    _table.AddColumn(GetColumn(_colParam, _table));
                }
                else
                    throw new Exception("\nERROR: Ivalid numbers of variables\n");

            }
            Console.WriteLine("\nColumns successfilly inserted\n");

        }
        public static void InsertValues(string tabelName)
        {
            Console.WriteLine("Here must be information about column name and type");
        }



        static Column GetColumn(string[] _variables, Table thisTable)
        {
            string _colName = _variables[0];
            Type _colType = GetType(_variables[1]);
            bool _isAllowNull = Convert.ToBoolean(_variables[2]);
            object _defValue = GetDefaultValue(_variables[3], _colType);

            if (_colType != _defValue.GetType())
                Console.WriteLine(_defValue.GetType().Name);
            return new Column(_colName, _colType, _isAllowNull, _defValue, thisTable);
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

        static bool IsKeyword(string word)
        {
            string _key = word.ToUpper();
            foreach (var k in _keywords)
                if (_key == k)
                    return true;
            return false;
        }

    }
}
