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
            "FROM",
            "INNNER_JOIN",
            "ON",
            "WHERE",
            "ORDERBY",
            "DESC",
            "ASC",
            "WHERE" 
        };
        static string _status;
        
        public static void Execute(string query)
        {
            _status = "OK";
            if (!(Interpreter.ConnectionString != null)) throw new Exception("\nERROR: There is no connection to database\n");
            if (!(query.Contains("SELECT") && query.Contains("FROM"))) throw new Exception("\nERROR: Invalid command syntax\n");

            char[] _separator = new char[] { ' ' };
            string[] _queryList = query.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

            if (!(_queryList.Length >= 4)) throw new Exception("\nERROR: Invalid command syntax\n");
            if (_queryList[0] == "SELECT" && _queryList[2] == "FROM")
            {    
                var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                var _table = GetTable(_queryList, _inst);
                
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

                    char[] _separator = new char[] { '=' };
                    string[] colNames = queryList[7].Split(_separator);
                    if (!(colNames.Length == 2)) throw new Exception("\nERROR: Invalid number of variable in INNER_JOIN params\n");

                    var colNameList = colNames.ToList();
                    inst.QueryInnerJoinSelection(_generalTable, _innerTable, colNameList, ref _status).OutTable();
                    return null;
                }
                else return null;
            }
            return null;

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
    }
}
