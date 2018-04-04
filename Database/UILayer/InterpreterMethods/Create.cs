using DataLayer;
using DataLayer.InternalDataBaseInstanceComponents;
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
        static List<string> _keywords = new List<string>(){ "DATABASE", "TABLE" };

        public static void Execuet(string query)
        {
            char[] separators = new char[] { ' ' };
            string[] queryList = query.Split(separators,2, StringSplitOptions.RemoveEmptyEntries);

            if (queryList.Length == 2)
            {
                string _createParam = default(string);
                _createParam = queryList[0];
                if (IsKeyword(_createParam))
                {
                    if (_createParam == _keywords[0]) 
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
            Kernel.GetInstance(dbName).SaveDataBaseInstanceToFolder();
            Interpreter.ConnectionString = dbName;
            Console.WriteLine($"\nDatabase created with name '{dbName}'\n");
        }

        static void CreateTable(string _param)
        {
            string _tableName = default(string);
            string _tableParams = default(string);
           
            if(IsCreateWithColums(_param))
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
                        /*
                         There must be check of exist table in list
                         */
                        _inst.AddTable(_tableName);
                        string[] _colParams = queryList[1].Split(_temp, StringSplitOptions.RemoveEmptyEntries);


                        foreach (var _col in _colParams)
                        {
                            char[] _s = new char[] { ',', '\\' };
                            string[] _colParam = _col.Split(_s, StringSplitOptions.RemoveEmptyEntries);
                            
                            //string[] _params=_colParam
                            //_inst.TablesDB[_inst.indexOfTable(_tableName)].AddColumn();
                        }

                        
                    }
                    else
                        throw new Exception("\nERROR: There is no connection to database\n");
                }
                else
                    throw new Exception("\nERROR: Invalid command syntax\n");
            }
            else 
            {
                if (Interpreter.ConnectionString != null)
                {
                    var inst = Kernel.GetInstance(Interpreter.ConnectionString);
                    inst.AddTable(_param);
                    Kernel.GetInstance(Interpreter.ConnectionString).SaveDataBaseInstanceToFolder();
                    Console.WriteLine($"\nTable created with name '{_param}'\n");
                }
                else
                    throw new Exception("\nERROR: There is no connection to database\n");
            }
        }


        static bool IsCreateWithColums(string query)
        {
            if (query.Contains('(') && query.Contains(')') &&
                query.Where(x => x == '(').Count() == 1 &&
                query.Where(x => x == ')').Count() == 1 &&
                query[query.Length - 1] == ')')
                return true;
            return false;
        }
        
        static bool IsValidSyntax(string query)
        {
            if (query[query.IndexOf('(') - 1] == ' ')
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

    }
}
