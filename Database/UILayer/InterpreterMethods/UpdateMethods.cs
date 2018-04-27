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
        };

        static List<string> _operators = new List<string>()
        {
            "IN",
            "NOT_IN",
            "BETWEEN",
            "NOT_BETWEEN",
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
                                        _column.EditColumnElementByPrimaryKey(_table.returnPrimaryKeyOfIndex(i), new object[] { data });
                                }
                                Console.WriteLine("\nAll data successfully updated\n");
                            }
                            else throw new Exception("\nERROR: Invalid command syntax\n");
                        }
                        else throw new Exception($"ERROR: Table with name '{tableName}' doesn't exist\n");
                    }
                    else throw new Exception("\nERROR: Invalid command syntax\n");
                }//All columns
                else if(_queryList.Length==3)
                {
                    //UPDATE Persons VALUES (colname=value) WHERE (Colname!=value)
                    if (_queryList[1] == "WHERE"&& IsValidSyntax(_queryList[2]) && IsValidSyntax(_queryList[0]))
                    {
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(tableName))
                        {
                            var _table = _inst.GetTableByName(tableName);

                            char[] _separ = new char[] { '(', ',', ')' };
                            string[] _paramsForReplace = _queryList[0].Split(_separ, StringSplitOptions.RemoveEmptyEntries);
                            string[] _conditionParams = _queryList[2].Split(_separ, StringSplitOptions.RemoveEmptyEntries);
                            if (!(_conditionParams.Length == 1)) throw new Exception("\nERROR: Invalid number of variables in condition params\n");
                            if (!(IsValidSyntax(_paramsForReplace,_table)&&IsValidSyntax(_conditionParams,_table))) throw new Exception("\nERROR: Invalid command syntax\n");


                            foreach (var param in _conditionParams)
                            {
                                string[] sep = new string[1];
                                sep[0] = GetSeparator(param);
                                        
                                string condColName = param.Split(sep,StringSplitOptions.RemoveEmptyEntries)[0];
                                string condValue = param.Split(sep,StringSplitOptions.RemoveEmptyEntries)[1];
                                string status;

                                var _columnCondition = _table.GetColumnByName(condColName);
                                object dataCondition = GetData(condValue, _columnCondition, out status);

                                string[] colNames = new string[_paramsForReplace.Length];
                                object[] colValues = new object[_paramsForReplace.Length];

                                for(int i=0;i<_paramsForReplace.Length;i++)
                                {
                                    char[] separ = new char[] { '=' };
                                    string[] temp = _paramsForReplace[i].Split(separ, StringSplitOptions.RemoveEmptyEntries);
                                    colNames[i] = temp[0];
                                    string stat;
                                    colValues[i] = GetData(temp[1], _table.GetColumnByName(temp[0]), out stat);      
                                }
                                string buff = "OK";
                                _inst.QueryWhereConditionUpdate(_table, condColName, sep[0], dataCondition, colNames, colValues, ref buff);
                                if (!(buff == "OK")) throw new Exception(buff);     
                            }
                            Console.WriteLine("\nAll data successfully updated\n");
                        }
                        else throw new Exception($"ERROR: Table with name '{tableName}' doesn't exist\n"); 
                    }
                    else throw new Exception("\nERROR: Invalid command syntax\n");
                }//With ColName and value
                else if(_queryList.Length==5)
                {
                    //    0                 1       2      3      4
                    //(colName=value,...) WHERE colName BETWEEN (1,3)
                    if (_queryList[1] == "WHERE" && IsValidSyntax(_queryList[0]) && IsValidSyntax(_queryList[4]) && IsOperator(_queryList[3]))
                    {
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(tableName))
                        {
                            var _table = _inst.GetTableByName(tableName);

                            char[] separator = new char[] { '(', ',', ')' };
                            string[] replaceParams = _queryList[0].Split(separator,StringSplitOptions.RemoveEmptyEntries);
                            string[] condtionParams = _queryList[4].Split(separator, StringSplitOptions.RemoveEmptyEntries);

                            string colName = _queryList[2];
                            if (!_table.isColumnExists(colName)) throw new Exception($"\nERROR: Column with name '{colName}' doesn't exist in table '{tableName}'\n");
                            var condColumn = _table.GetColumnByName(colName);
                            object[] conditionValues = new object[condtionParams.Length];

                            string keyword = _queryList[3];

                            if (!IsValidSyntax(replaceParams, _table)) throw new Exception("\nERROR: Invalid command syntax\n");
                            string[] colNames = new string[replaceParams.Length];
                            object[] values = new object[replaceParams.Length];


                            for (int i = 0; i < replaceParams.Length;i++)
                            {
                                char[] sep = new char[] { '=' };
                                string[] temp = replaceParams[i].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                                colNames[i] = temp[0];
                                values[i] = GetData(temp[1], _table.GetColumnByName(temp[0]), out string status);
                            }

                            string buff;
                            for (int i = 0; i < condtionParams.Length; i++)
                                conditionValues[i] = GetData(condtionParams[i],condColumn,out buff);

                            string stat = "OK";
                            _inst.QueryWhereConditionUpdate(_table, colName, _queryList[3], conditionValues, colNames, values,ref stat);
                            if (stat != "OK") throw new Exception(stat);
                        }
                    }
                    Console.WriteLine("\nAll data successfully updated\n");
                }// With (BETWENN/NOT_BETWEEN\IN\NOT_IN) 
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

        static bool IsKeyword(string param)
        {
            foreach (var key in _keywords)
                if (key == param)
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
            foreach (var sep in _separtors)
                if (param.Contains(sep))
                    return sep;
            throw new Exception("\nERROR: Invalid charecter in condition params\n");
        }
        
    }
}
