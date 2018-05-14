﻿using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UILayer.InterpreterMethods
{
    class RenameMethods
    {
        static List<string> _keywords = new List<string>()
        {
            "DATABASE",
            "TABLE",
            "COLUMN"
        };

        public static void Execute(string query)
        {
            try
            {
                char[] separator = new char[] { ' ' };
                string[] queryList = query.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);

                if (queryList.Length == 2)
                {
                    if (IsKeyword(queryList[0]))
                    {
                        var _inst = new RenameMethods();
                        string _methodName = "Rename" + queryList[0];
                        var _method = _inst.GetType().GetMethod(_methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
                        _method?.Invoke(_inst, new object[] { queryList[1] });
                    }
                    else throw new Exception("\nERROR: Invalid command syntax\n");
                }
                else throw new Exception("\nERROR: Invalid number of variables\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void RenameColumn(string command)
        {
            try
            {
                if (Interpreter.ConnectionString != null)
                {
                    char[] _separator = new char[] { ' ' };
                    string[] _colNames = command.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (_colNames.Length == 3)
                    {
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        if (_inst.isTableExists(_colNames[0]))
                        {
                            var _table = _inst.GetTableByName(_colNames[0]);
                            _table.RenameColumn(_colNames[1], _colNames[2]);

                        }
                        else throw new NullReferenceException($"There is no table '{_colNames[0]}' in database '{_inst.Name}'!");
                    }
                    else throw new Exception($"\nERROR: Ivalid nuber of variables\n");
                }
                else throw new Exception("\nERROR: There is no connection to database\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void RenameTable(string command)
        {
            try
            {
                if (Interpreter.ConnectionString != null)
                {
                    char[] _separator = new char[] { ' ' };
                    string[] _tableNames = command.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    if (_tableNames.Length == 2)
                    {
                        var _inst = Kernel.GetInstance(Interpreter.ConnectionString);
                        _inst.RenameTable(_tableNames[0], _tableNames[1]);    
                    }
                    else
                        throw new Exception($"\nERROR: Ivalid nuber of variables\n");
                }
                else
                    throw new Exception("\nERROR: There is no connection to database\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void RenameDatabase(string command)
        {
            try {
                char[] _separator = new char[] { ' ' };
                string[] _dbNames = command.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                if (_dbNames.Length == 2)
                {
                    Kernel.RenameDatabase(_dbNames[0], _dbNames[1]);
                    Interpreter.ConnectionString = null;
                }
                else
                    throw new Exception($"\nERROR: Ivalid nuber of variables\n");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static bool IsKeyword(string word)
        {
            foreach (var key in _keywords)
                if (word == key)
                    return true;
            return false;
        }

    }
}
