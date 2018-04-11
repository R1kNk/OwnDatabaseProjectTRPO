using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
//using DataLayer.InternalDataBaseInstanceComponents;
using System.Reflection;
using UILayer.InterpreterMethods;

namespace UILayer
{
    class Interpreter
    {
        static object _lockObj = new object();
        static Interpreter _instance;
        public static List<string> _keywords = new List<string>()
        {
            "CREATE",
            "CLEAR",
            "CONNECT",
            "INFO",
            "LOADDB",
            "SAVEDB",
            "INSERT",
            "RENAME",
            "DELETE"
        };
        public static string ConnectionString { get; set; }


        static Interpreter GetInstance()
        {
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new Interpreter();
                        return _instance;
                    }
                }
            }
            return _instance;
        }

        public static void Run()
        {
            ConnectionString = default(string);
            while (true)
            {
                string _query = default(string);
                _query = Console.ReadLine();
                if (_query.Any(x => char.IsLetterOrDigit(x)))
                {
                    char[] _separator = new char[] { ' ' };
                    string _keyword = _query.Split(_separator, StringSplitOptions.RemoveEmptyEntries)[0];
                    if (GetInstance().IsKeyword(_keyword))
                    {
                        var _method = GetInstance().GetType().GetMethod(_keyword, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                        object[] param = new object[] { _query };
                        _method?.Invoke(GetInstance(), param);
                    }
                    else
                    {
                        Console.WriteLine($"\nERROR: Command '{_keyword}' doesn't exist\n");
                    }
                }
            }
        }


        #region LocalMethods
        bool IsKeyword(string command)
        {
            string _temp = command.ToUpper();
            foreach (var key in _keywords)
                if (key == _temp)
                    return true;
            return false;
        }
        #endregion

        #region MainMetods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        private static void Connect(string query)
        {
            char[] separator = new char[] { ' ' };
            string[] queryList = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (queryList[1].ToLower() == "to")
            {
                if (queryList.Length == 3)
                {
                    if (Kernel.isDatabaseExists(queryList[2]))
                    {
                        ConnectionString = queryList[2];
                        Console.WriteLine($"\nNow you connected to database '{queryList[2]}'\n");
                    }
                    else
                        Console.WriteLine($"\nERROR: Database with name '{queryList[2]}' doesn't exist\n");
                }
                else
                    Console.WriteLine("\nERROR: Invalid number of variables\n");
            }
            else
                Console.WriteLine($"\nERROR: This command doesn't exist\n");
        }

        private static void Info(string query)
        {

            char[] separator = new char[] { ' ' };
            string[] queryList = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (queryList.Length == 2)
            {
                if (Kernel.isDatabaseExists(queryList[1]))
                {
                    Kernel.OutDatabaseInfo(queryList[1]);
                }
                else
                    Console.WriteLine($"\nERROR: Database with name '{queryList[1]}' doesn't exist\n");
            }
            else if (queryList.Length == 1)
            {
                Console.WriteLine($"\nConnection string - {ConnectionString}");
                Kernel.OutDatabaseInfo();
            }
            else
            {
                Console.WriteLine($"\nERROR: Invalid numbers of variables\n");
            }

        }

        private static void SaveDb(string query)
        {
            char[] separator = new char[] { ' ' };
            string[] queryList = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (queryList.Length == 1)
            {
                Kernel.SaveAllDatabases();
                Console.WriteLine("\nAll databases saved\n");
            }
            else if (query.Length == 2)
            {
                if (Kernel.isDatabaseExists(queryList[1]))
                {
                    var _inst = Kernel.GetInstance(queryList[1]);
                    _inst.SaveDataBaseInstanceToFolder();
                    Console.WriteLine($"\nDatabase {queryList[1]} successfully saved\n");
                }
                else
                    throw new Exception($"\nERROR: Database {queryList[1]} doesn't exist\n");
            }
            else
                Console.WriteLine($"\nERROR: Invalid number of variables");
        }

        private static void LoadDb(string query)
        {
            char[] separator = new char[] { ' ' };
            string[] queryList = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (queryList.Length == 1)
            {
                Kernel.LoadAllDatabases(true);
                Console.WriteLine("\nDatabases loaded\n");
            }
            else
                Console.WriteLine($"\nERROR: Invalid number of variables");
        }

        private static void Create(string query)
        {
            string _command = query.Substring(6);
            try
            {
                CreateMethods.Execute(_command);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void Insert(string query)
        {
            string param = query.Substring(6);
            InsertMethods.Execute(param);
        }

        private static void Delete(string query)
        {
            string _command = query.Substring(6);
            DeleteMethods.Execute(_command);
        }

        private static void Clear(string query)
        {
            Console.Clear();
        }

        private static void Rename(string query)
        {
            string param = query.Substring(6);
            RenameMethods.Execute(param);
        }

        private static void Databases(string query)
        {
            char[] separator = new char[] { ' ' };
            string[] command = query.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (command.Length == 1)
            {
                Console.WriteLine();
                Kernel.OutNamesOfExistingDBs();
                Console.WriteLine();
            }
            else
                throw new Exception();
        }
        #endregion

    }
}
