using DataLayer;
using DataModels.App.InternalDataBaseInstanceComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UILayer.InterpreterMethods
{
    class UpdateMethods
    {
        //UPDATE Perosns VALUES (Age=19) WHERE (Name=Kolya)

        static List<string> _keywords = new List<string>()
        {
            "NULLPROPERTY",
            "DEFAULT",
            "TYPE",
            "VALUES",
            "IN",
            "NOT_IN",
            "BETWEEN",
            "NOT_BETWEEN",
            "!=",
            "<=",
            ">=",
            "=",
             "<",
            ">",
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

        
        
        static void UpdateValues(string tableName, string query)
        {
            try {
                char[] _separator = new char[] { ' ' };
                string[] _queryList = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                if (_queryList.Length == 2)
                {
                    if (IsValidSyntax(_queryList[1]))
                    {
                        int _elementId = Convert.ToInt32(_queryList[0]);
                        char[] _sep = new char[] { ',', '(', ')' };
                        string[] _params = _queryList[1].Split(_sep, StringSplitOptions.RemoveEmptyEntries);
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(tableName))
                        {
                            var _table = _inst.GetTableByName(tableName);

                            if (IsValidSyntax(_params, _table))// проверка
                            {
                                foreach (var param in _params)
                                {
                                    char[] sep = new char[] { '=' };
                                    string[] temp = param.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                                    string colName = temp[0];
                                    string value = temp[1];
                                    string status;
                                    var _column = _table.GetColumnByName(colName);
                                    object data = GetData(value, _column, out status);
                                    _column.EditColumnElementByPrimaryKey(_elementId, new object[] { data });
                                }                   
                                Console.WriteLine("\nAll data successfully updated\n");
                            }
                            else throw new Exception("\nERROR: Invalid command syntax\n");
                        }
                        else throw new NullReferenceException($"There is no table '{tableName}' in database '{_inst.Name}'!\n");

                    }
                    else throw new Exception("\nERROR: Invalid command syntax\n");
                }// with ID
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
                            if (IsValidSyntax(_params, _table))
                            {
                                foreach (var param in _params)
                                {
                                    char[] sep = new char[] { '=' };
                                    string[] temp = param.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                                    string status;
                                    var _column = _table.GetColumnByName(temp[0]);
                                    object data = GetData(temp[1], _column, out status);
                                    for (int i = 0; i < _column.DataList.Count; i++)
                                        _column.EditColumnElementByIndex(i, new object[] { data });
                                }
                                Console.WriteLine("\nAll data successfully updatet\n");
                            }
                            else throw new Exception("\nERROR: Invalid command syntax\n");
                        }
                        else throw new Exception($"ERROR: Table with name '{tableName}' doesn't exist\n");
                    }
                    else throw new Exception("\nERROR: Invalid command syntax\n");
                }//All columns
                else if(_queryList.Length==3)
                {
                    if (_queryList[1] == "WHERE")
                    {
                        if (IsValidSyntax(_queryList[2]) && IsValidSyntax(_queryList[0]))
                        {
                            char[] _separ = new char[] { '(', ',', ')' };
                            string[] _paramsForReplace = _queryList[0].Split(_separ, StringSplitOptions.RemoveEmptyEntries);
                            string[] _conditionParams = _queryList[2].Split(_separ, StringSplitOptions.RemoveEmptyEntries);

                            if (_conditionParams.Length != 1) throw new Exception("\nERROR: Invalid number of variables\n");

                            var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                            if (_inst.isTableExists(tableName))
                            {
                                var _table = _inst.GetTableByName(tableName);
                                if (IsValidSyntax(_paramsForReplace, _table) && IsValidSyntax(_conditionParams, _table))
                                {
                                    foreach (var param in _conditionParams)
                                    {
                                        string[] sep = new string[1];
                                        sep[0] = GetSeparator(param);
                                        
                                        string colName = param.Split(sep,StringSplitOptions.RemoveEmptyEntries)[0];
                                        string value = param.Split(sep,StringSplitOptions.RemoveEmptyEntries)[1];
                                        string status;
                                        var _columnCondition = _table.GetColumnByName(colName);

                                        object dataCondition = GetData(value, _columnCondition, out status);
                                        for (int i = 0; i < _columnCondition.DataList.Count; i++)
                                        {
                                            object data = _columnCondition.DataList[i].Data;
                                            if (dataCondition.Equals(data))
                                            {
                                                int Id = _table.returnPrimaryKeyOfIndex(i);
                                                foreach (var colParam in _paramsForReplace)
                                                {
                                                    string _colName = colParam.Split(sep, StringSplitOptions.RemoveEmptyEntries)[0];
                                                    string _value = colParam.Split(sep, StringSplitOptions.RemoveEmptyEntries)[1];
                                                    string stat;
                                                    var columnReplace = _table.GetColumnByName(_colName);
                                                    object _dataReplace = GetData(_value, columnReplace, out stat);

                                                    columnReplace.EditColumnElementByPrimaryKey(Id, new object[] { _dataReplace});
                                                }
                                            }
                                           
                                        }
                                    }
                                    Console.WriteLine("\nAll data successfully updatet\n");
                                }
                                else throw new Exception("\nERROR: Invalid command syntax\n");
                            }
                            else throw new Exception($"ERROR: Table with name '{tableName}' doesn't exist\n");
                        }
                        else throw new Exception("\nERROR: Invalid command syntax\n");
                    }
                    else throw new Exception("\nERROR: Invalid command syntax\n");
                }//With ColName and value
                else if(_queryList.Length==5)
                {
                    if (_queryList[1] == "WHERE" && IsValidSyntax(_queryList[0]) && IsValidSyntax(_queryList[4]) && IsKeyword(_queryList[3]))
                    {
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(tableName))
                        {
                            char[] separator = new char[] { '(', ',', ')' };
                            var _table = _inst.GetTableByName(tableName);
                            string[] replaceParams = _queryList[0].Split(separator,StringSplitOptions.RemoveEmptyEntries);
                            string[] condtionParams = _queryList[4].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            string keyword = _queryList[3];
                            if (keyword == "BETWEEN")
                            {
                                if (condtionParams.Length != 2) throw new Exception("\nERROR: Invalid command syntax in BETWEEN params\n");

                                int lowBoard = int.Parse(condtionParams[0]);
                                int upBoard = int.Parse(condtionParams[1]);

                                if (lowBoard >= upBoard) throw new Exception("\nERROR: Low board can't be more than up board\n");
                                if (!IsValidSyntax(replaceParams, _table)) throw new Exception("\nERROR: Invalid command syntax in params for replace\n");

                                foreach(var param in replaceParams)
                                {
                                    char[] temp = new char[] { '=' };
                                    string colName = param.Split(temp, StringSplitOptions.RemoveEmptyEntries)[0];
                                    string value= param.Split(temp, StringSplitOptions.RemoveEmptyEntries)[1];
                                    string status;
                                    var _column = _table.GetColumnByName(colName);
                                    object data = GetData(value, _column, out status);

                                    for(int i=0;i<_column.DataList.Count;i++)
                                    {
                                        int Id = _table.returnPrimaryKeyOfIndex(i);
                                        if(Id>=lowBoard&&Id<=upBoard)
                                        {
                                            _column.EditColumnElementByPrimaryKey(Id, new object[] { data });
                                        }
                                    }
                                }
                                Console.WriteLine("\nAll data successfully updatet\n");

                            }
                            else if (keyword == "NOT_BETWEEN")
                            {
                                if (condtionParams.Length != 2) throw new Exception("\nERROR: Invalid command syntax in BETWEEN params\n");

                                int lowBoard = int.Parse(condtionParams[0]);
                                int upBoard = int.Parse(condtionParams[1]);

                                if (lowBoard >= upBoard) throw new Exception("\nERROR: Low board can't be more than up board\n");
                                if (!IsValidSyntax(replaceParams, _table)) throw new Exception("\nERROR: Invalid command syntax in params for replace\n");


                                foreach (var param in replaceParams)
                                {
                                    char[] temp = new char[] { '=' };
                                    string colName = param.Split(temp, StringSplitOptions.RemoveEmptyEntries)[0];
                                    string value = param.Split(temp, StringSplitOptions.RemoveEmptyEntries)[1];
                                    string status;
                                    var _column = _table.GetColumnByName(colName);
                                    object data = GetData(value, _column, out status);

                                    for (int i = 0; i < _column.DataList.Count; i++)
                                    {
                                        int Id = _table.returnPrimaryKeyOfIndex(i);
                                        if (Id < lowBoard || Id > upBoard)
                                        {
                                            _column.EditColumnElementByPrimaryKey(Id, new object[] { data });
                                        }
                                    }
                                }
                                Console.WriteLine("\nAll data successfully updatet\n");
                            }
                            else if(keyword=="IN")
                            {
                                if (!_table.isColumnExists(_queryList[2])) throw new Exception();

                                var _column = _table.GetColumnByName(_queryList[2]);
                                string status;
                                object[] condData = new object[condtionParams.Length];
                                for(int i=0;i<condtionParams.Length;i++)
                                {
                                    object data = GetData(condtionParams[i], _column, out status);
                                    if (status == "OK")
                                        condData[i] = data;
                                    else throw new Exception("\nERROR:Type of condition params doesn't equals type of column\n");
                                }

                                foreach(var param in replaceParams)
                                {
                                    char[] temp = new char[] { '=' };
                                    string stat;
                                    string colName = param.Split(temp, StringSplitOptions.RemoveEmptyEntries)[0];
                                    string value= param.Split(temp, StringSplitOptions.RemoveEmptyEntries)[1];
                                    var column = _table.GetColumnByName(colName);
                                    object data = GetData(value, column, out stat);

                                    for(int i=0;i<_column.DataList.Count;i++)
                                    {
                                        object buff = _column.DataList[i].Data;
                                        for(int j=0;j<condData.Length;j++)
                                        {
                                            if(condData[j].Equals(buff))
                                            {
                                                column.EditColumnElementByIndex(i, new object[] { data });
                                            }
                                        }
                                    }
                                }
                                Console.WriteLine("\nAll data successfully updatet\n");
                            }
                            else
                            {
                                if (!_table.isColumnExists(_queryList[2])) throw new Exception();

                                var _column = _table.GetColumnByName(_queryList[2]);
                                string status;
                                object[] condData = new object[condtionParams.Length];
                                for (int i = 0; i < condtionParams.Length; i++)
                                {
                                    object data = GetData(condtionParams[i], _column, out status);
                                    if (status == "OK")
                                        condData[i] = data;
                                    else throw new Exception("\nERROR:Type of condition params doesn't equals type of column\n");
                                }

                                foreach (var param in replaceParams)
                                {
                                    char[] temp = new char[] { '=' };
                                    string stat;
                                    string colName = param.Split(temp, StringSplitOptions.RemoveEmptyEntries)[0];
                                    string value = param.Split(temp, StringSplitOptions.RemoveEmptyEntries)[1];
                                    var column = _table.GetColumnByName(colName);
                                    object data = GetData(value, column, out stat);

                                    for (int i = 0; i < _column.DataList.Count; i++)
                                    {
                                        object buff = _column.DataList[i].Data;
                                        for (int j = 0; j < condData.Length; j++)
                                        {
                                            if (!condData[j].Equals(buff))
                                            {
                                                column.EditColumnElementByPrimaryKey(i, new object[] { data });
                                            }
                                        }
                                    }
                                }
                                Console.WriteLine("\nAll data successfully updatet\n");
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
                        throw new Exception($"\nERROR: Column with name '{temp[0]}' doesn't exist");
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
        
        static string GetSeparator(string param)
        {
            foreach (var key in _keywords)
                if (param.Contains(key))
                    return key;
            throw new Exception("\nERROR: Invalid charecter in condition params\n");
        }
        
    }
}
