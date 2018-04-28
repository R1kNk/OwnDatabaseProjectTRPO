using DataLayer;
using DataModels.App.InternalDataBaseInstanceComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UILayer.InterpreterMethods
{
    class SelectMethods
    {
        static List<string> _keywords = new List<string>()
        {
            "*",
            "COUNT",
            "AVG",
            "SUM",
            "MAX",
            "MIN",
            "TOP"
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

        static List<string> _operators = new List<string>()
        {
            "IN",
            "NOT_IN",
            "BETWEEN",
            "NOT_BETWEEN",
        };

        static string _status;
        private static bool isValueCount;

        public static void Execute(string query)
        {
            _status = "OK";
            if (!(Interpreter.ConnectionString != null)) throw new Exception("\nERROR: There is no connection to database\n");
            if (!(query.Contains("SELECT ") && query.Contains(" FROM "))) throw new Exception("\nERROR: Invalid command syntax\n");

            char[] _separator = new char[] { ' ' };
            string[] _queryList = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

            if (!(_queryList.Length >= 4)) throw new Exception("\nERROR: Invalid command syntax\n");
            if (_queryList[0] == "SELECT" && _queryList[2] == "FROM")
            {    
                var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                var _table = GetTable(_queryList, _inst);
                if (_status != "OK") throw new Exception(_status);
  
                if (query.Contains(" WHERE "))
                {          
                    _table =QueryWhere(_inst,_table, _queryList,query);
                }
                if (_status != "OK") throw new Exception(_status);
                if (query.Contains(" ORDER_BY "))
                {
                    _table = QuerySort(_inst, _table, _queryList);
                }
                _table = QuerySelect(_inst, _table, _queryList);
                if (_status != "OK") throw new Exception(_status);
                Console.WriteLine("\n"+_table.OutTable());
            }
            else throw new Exception("\nERROR: Invalid command syntax\n");
            _status= null;
        }



        static Table GetTable(string[] queryList, DataBaseInstance inst)
        {
            string _generalTableName = queryList[3];
            
            if (!inst.isTableExists(_generalTableName)) throw new Exception($"There is no table '{_generalTableName}' in database '{inst.Name}'!\n");
            var _generalTable = inst.GetTableByName(_generalTableName);
            if (queryList.Length >= 8)
            {
                if (queryList[4] == "INNER_JOIN" && queryList[6] == "ON")
                {
                    string _innerTableName = queryList[5];
                    if (!inst.isTableExists(_innerTableName)) throw new Exception($"There is no table '{_innerTableName}' in database '{inst.Name}'!\n");

                    var _innerTable = inst.GetTableByName(_innerTableName);
                    if (!IsValidSyntax(queryList[7])) throw new Exception("\nERROR: Invalid command syntax\n");

                    char[] _separator = new char[] { '(', '=', ')' };
                    string[] colNames = queryList[7].Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (!(colNames.Length == 2)) throw new Exception("\nERROR: Invalid number of variable in INNER_JOIN params\n");

                    var colNameList = colNames.ToList();
                    return inst.QueryInnerJoinSelection(_generalTable, _innerTable, colNameList, ref _status);
                }
                else if (queryList[4] == "WHERE" || queryList[4] == "ORDER_BY")
                {
                    return _generalTable;
                }
                else throw new Exception("\nERROR: Invalid query syntax\n");
            }
            else if (queryList.Length >= 4)
            {
                return _generalTable;
            }
            else throw new Exception("\nERROR: Invalid query syntax\n");
        }

        static Table QueryWhere(DataBaseInstance inst, Table table,string[] queryList,string query)
        {
           
            int index = GetIndexOfKeyword(queryList, "WHERE");

            if (index == 4)
            {
                if (query.Contains(" IN ")|| query.Contains(" NOT_IN ")|| query.Contains(" BETWEEN ")||query.Contains(" NOT_BETWEEN "))
                {
                    if (queryList.Length < 8) throw new Exception();
                    
                    string _operator = GetComplexOperator(queryList);
                    if (queryList[6] != _operator) throw new Exception();
                    if (!IsValidSyntax(queryList[7])) throw new Exception();

                    string _colName = queryList[5];
                    if (!_colName.Contains('.')) throw new Exception();
                    if (!table.isColumnExists(_colName)) throw new Exception();

                   
                    var _column = table.GetColumnByName(_colName);
                    char[] _sep = new char[] { '(', ',', ')' };
                    string[] _val = queryList[7].Split(_sep, StringSplitOptions.RemoveEmptyEntries);
                    object[] _values = new object[_val.Length];
                    for (int i = 0; i < _val.Length; i++)
                    {
                        string stat;
                        _values[i] = GetData(_val[i], _column, out stat);
                        if (!(stat == "OK")) throw new Exception();      
                    }
                    return inst.QueryWhereConditionSelection(table, _colName, _operator, _values, ref _status);
                }
                else if(queryList.Length>=6)
                {
                    if (!IsValidSyntax(queryList[5])) throw new Exception();

                    string[] _separator = new string[] { "(", ")", GetSeparator(queryList[5]) };
                    string[] conditionParams = queryList[5].Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (!(conditionParams.Length == 2)) throw new Exception();

                    string _colName = conditionParams[0];
                    string _value = conditionParams[1];
                    if (!table.isColumnExists(_colName)) throw new Exception();

                    string buff;
                    var _column = table.GetColumnByName(_colName);
                    object _data = GetData(_value, _column, out buff);
                    if (!(buff == "OK")) throw new Exception();
                    return inst.QueryWhereConditionSelection(table, _colName, GetSeparator(queryList[5]), _data, ref _status);
                }
                else throw new Exception();
            }
            else if (index == 8)
            {
                if (query.Contains(" IN ") || query.Contains(" NOT_IN ") || query.Contains(" BETWEEN ") || query.Contains(" NOT_BETWEEN "))
                {
                    if (queryList.Length < 12) throw new Exception();

                    string _operator = GetComplexOperator(queryList);
                    if (queryList[10] != _operator) throw new Exception();
                    if (!IsValidSyntax(queryList[11])) throw new Exception();

                    string _colName = queryList[9];
                    if (!_colName.Contains('.')) throw new Exception();
                    if (!table.isColumnExists(_colName)) throw new Exception();

                    var _column = table.GetColumnByName(_colName);
                    char[] _sep = new char[] { '(', ',', ')' };
                    string[] _val = queryList[11].Split(_sep, StringSplitOptions.RemoveEmptyEntries);
                    object[] _values = new object[_val.Length];
                    for (int i = 0; i < _val.Length; i++)
                    {
                        string stat;
                        _values[i] = GetData(_val[i], _column, out stat);
                        if (!(stat == "OK")) throw new Exception();
                    }
                    return inst.QueryWhereConditionSelection(table, _colName, _operator, _values, ref _status);
                }
                else if (queryList.Length >= 10)
                {
                    if (!IsValidSyntax(queryList[9])) throw new Exception();

                    string[] _separator = new string[] { "(", ")", GetSeparator(queryList[9]) };
                    string[] conditionParams = queryList[9].Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (!(conditionParams.Length == 2)) throw new Exception();

                    string _colName = conditionParams[0];
                    if (!_colName.Contains('.')) throw new Exception();

                    string _value = conditionParams[1];
                    if (!table.isColumnExists(_colName)) throw new Exception();

                    string buff;
                    var _column = table.GetColumnByName(_colName);
                    object _data = GetData(_value, _column, out buff);
                    if (!(buff == "OK")) throw new Exception();

                    return inst.QueryWhereConditionSelection(table, _colName, GetSeparator(queryList[9]), _data, ref _status);
                }//with condition
                else throw new Exception();
            }
            else throw new Exception();
        }

        static Table QuerySort(DataBaseInstance inst,Table table,string[] queryList)
        {
            int index = GetIndexOfKeyword(queryList, "ORDER_BY");
            if (index == 4)
            {
                if (!(queryList.Length == 7)) throw new Exception();

                bool _isAscending;
                string _colName = queryList[5];
                if (!table.isColumnExists(_colName)) throw new Exception();

                if (queryList[6] == "ASC")
                    _isAscending = true;
                else if (queryList[6] == "DESC")
                    _isAscending = false;
                else throw new Exception();

                return inst.QuerySortTable(_colName, table, _isAscending, ref _status);
            }
            else if (index == 6)
            {
                if (!(queryList.Length == 9)) throw new Exception();

                bool _isAscending;
                string _colName = queryList[7];
                if (!table.isColumnExists(_colName)) throw new Exception();

                if (queryList[8] == "ASC")
                    _isAscending = true;
                else if (queryList[8] == "DESC")
                    _isAscending = false;
                else throw new Exception();

                return inst.QuerySortTable(_colName, table, _isAscending, ref _status);
            }
            else if (index == 8)
            {
                if (!(queryList.Length == 11)) throw new Exception();

                bool _isAscending;
                string _colName = queryList[9];
                if (!table.isColumnExists(_colName)) throw new Exception();

                if (queryList[10] == "ASC")
                    _isAscending = true;
                else if (queryList[10] == "DESC")
                    _isAscending = false;
                else throw new Exception();

                return inst.QuerySortTable(_colName, table, _isAscending, ref _status);
            }
            else if (index == 10)
            {
                if (!(queryList.Length == 13)) throw new Exception();

                bool _isAscending;
                string _colName = queryList[11];
                if (!table.isColumnExists(_colName)) throw new Exception();

                if (queryList[12] == "ASC")
                    _isAscending = true;
                else if (queryList[12] == "DESC")
                    _isAscending = false;
                else throw new Exception();

                return inst.QuerySortTable(_colName, table, _isAscending, ref _status);
            }
            else if (index == 12)
            {
                if (!(queryList.Length == 15)) throw new Exception();

                bool _isAscending;
                string _colName = queryList[13];
                if (!table.isColumnExists(_colName)) throw new Exception();

                if (queryList[14] == "ASC")
                    _isAscending = true;
                else if (queryList[14] == "DESC")
                    _isAscending = false;
                else throw new Exception();

                return inst.QuerySortTable(_colName, table, _isAscending, ref _status);
            }
            else throw new Exception();
        }

        static Table QuerySelect(DataBaseInstance inst,Table table,string[] queryList)
        {
            if (!IsValidSyntax(queryList[1])) throw new Exception();
            char[] _separator = new char[] { '(', ',', ')' };
            string[] _params = queryList[1].Split(_separator, StringSplitOptions.RemoveEmptyEntries);
            if (_params.Length == 0) throw new Exception();
            if(IsKeyword(_params[0]))
            {
                string _keyword = _params[0];
                if (_keyword == "*")
                {
                    if (_params.Length != 1) throw new Exception();
                    return table;
                }
                else if (_keyword == "COUNT")
                {
                    if (_params.Length == 1) return inst.QueryCountSelection(table, ref _status);
                    else throw new Exception();
                }
                else if (_keyword == "AVG")
                {

                    if (_params.Length != 2) throw new Exception();
                    string _colName = _params[1];

                    if (!table.isColumnExists(_colName)&&!_colName.Contains('.')) throw new Exception();
                    return inst.QueryAvgSelection(_colName, table, ref _status);
                }
                else if(_keyword=="SUM"||_keyword=="MIN"||_keyword=="MAX")
                { 
                    if (_params.Length != 2) throw new Exception();
                    string _colName = _params[1];
                    if (!table.isColumnExists(_colName) && !_colName.Contains('.')) throw new Exception();
                    return inst.QueryMINMAXSUMSelection(_colName,table,_keyword, ref _status);
                }
                else if(_keyword=="TOP")
                {
                    if (_params.Length != 2) throw new Exception();
                    char[] _sep = new char[] { '=' };
                    string[] _temp = _params[1].Split(_sep,StringSplitOptions.None);
                    if (_temp.Length != 2) throw new Exception();
                    if (_temp[0] == "VALUES")
                    {
                        bool isValueCount = true;
                        int count = Convert.ToInt32(_temp[1]);
                        return inst.QueryTopSelection(table, count,isValueCount, ref _status);
                    }
                    else if( _temp[0] == "PERC")
                    {
                        bool isValueCount = false;
                        int perc = Convert.ToInt32(_temp[1]);
                        return inst.QueryTopSelection(table, perc, isValueCount, ref _status);
                    }
                    else throw new Exception();
                }
                
            }
            else
            {
                List<string> colNames = new List<string>();
                foreach (var col in _params)
                {
                    if (table.isColumnExists(col) && col.Contains('.'))
                        colNames.Add(col);
                    else throw new Exception();
                }
                return inst.QueryColumnSelection(colNames, table, ref _status);
            }
            throw new Exception();
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

        static bool IsValidSyntax(string command)
        {
            if (command.Contains('(') && command.Contains(')') &&
                command.Where(x => x == '(').Count() == 1 &&
                command.Where(x => x == ')').Count() == 1 &&
                command[command.Length - 1] == ')')
                return true;
            return false;
        }


        static bool IsKeyword(string command)
        {
            foreach (var key in _keywords)
                if (key == command)
                    return true;
            return false;
        }

        static string GetComplexOperator(string[] queryList)
        {
            foreach (var word in queryList)
                foreach (var oper in _operators)
                    if (word == oper)
                        return oper;
            return null;
        }

        static int GetIndexOfKeyword(string[] words,string word)
        {
            for(int i=0;i<words.Length;i++)
                if (words[i] == word)
                    return i;
            return -1;
        }
    }
}
