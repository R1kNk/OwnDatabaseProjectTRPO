using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using DataAccessLayer.Modules;
using DataLayer.Shared.ExtentionMethods;

[assembly: InternalsVisibleTo("FilesLayer")]

namespace DataLayer
{
    public static class Kernel
    {
        private static List<DataBaseInstance> instance;
        private static object lockObject = new Object();

        internal static List<DataBaseInstance> GetInstance()
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {

                        //!!! here check enviroment for something
                        instance = new List<DataBaseInstance>();
                        LoadAllDatabases(true);

                    }
                }
            }
            return instance;
        }

        public static DataBaseInstance GetInstance(string name)
        {
            var _instance = Kernel.GetInstance();
            var element =  _instance.FindLast(x => x.Name == name);
            if (element != null)
                return element;
            throw new IndexOutOfRangeException("Коля, лови!");
        }
        #region Done
        public static void OutDatabaseInfo()
        {
            if (GetInstance().Count == 0) throw new NullReferenceException("There is no DB's in list!");
            for (int i = 0; i < GetInstance().Count; i++)
            {
                Console.WriteLine(GetInstance()[i].ToString());
            }
        }
        public static void OutDatabaseInfo(string name)
        {
            int index = SharedDataAccessMethods.IndexOfDatabase(GetInstance(), name);
            Console.WriteLine(GetInstance()[index].ToString());
        }
        #endregion

        #region NotDone
        public static void OutNamesOfExistingDBs()
        {
            if (GetInstance().Count == 0) throw new NullReferenceException("There is no DB's in list!");
            else
            {
                string info = "DB's list:";
                for (int i = 0; i < GetInstance().Count; i++)
                {
                    info += " " + GetInstance()[i].Name;
                }
                Console.WriteLine(info);
            }
        }
        internal static void LoadDatabase(string name)
        {
            DataBaseInstance bufInst = CollectDataModule.LoadDataBase(name);
            if (bufInst.Name == "nullDB") throw new ArgumentException("THere is no DB with such name in folder");
            if (GetInstance().isDatabaseExistsInList(bufInst.Name))
            {
                GetInstance()[GetInstance().IndexOfDatabase(bufInst.Name)] = bufInst;
            }
        }
        #endregion

      
        internal static bool isDatabaseExistsInList(string name)
        {
            return SharedDataAccessMethods.isDatabaseExistsInList(GetInstance(), name);
        }

        internal static void RenameDatabase(string currentName, string futureName)
        {
            if (isDatabaseExists(currentName))
            {
                if (futureName.isThereNoUndefinedSymbols())
                {
                    GetInstance(currentName).Name = futureName;
                }
                else throw new ArgumentException("your name contains undefined symbols!");
            }
            else throw new NullReferenceException("There's no such database in list");
        }
        internal static void AddDBInstance(string name)
        {
            DataBaseInstance bufInst = new DataBaseInstance(name);
            AddDBInstance(bufInst);
        }
        //
        internal static void AddDBInstance(DataBaseInstance inst)
        {
            var _instance = Kernel.GetInstance();
            if (_instance.FindAll(x => x.Name == inst.Name).Count != 0 || !inst.Name.isThereNoUndefinedSymbols())
                throw new ArgumentException("Invalid name of database");
            _instance.Add(inst);
        }
        internal static void SaveDataBaseInstanceToFolder(this DataBaseInstance inst)
        {
            CacheModule.SaveDataBaseToFolder(inst);
        }
        public static void SaveAllDatabases()
        {
            CacheModule.SaveAllDatabases(GetInstance());
        }
        internal static void LoadAllDatabases(bool isUpdatativeLoad)
        {
            if (!isUpdatativeLoad) instance = CollectDataModule.LoadAllDataBases();
            instance = CollectDataModule.UpdatativeDatabasesLoad(instance);

        }

        internal static bool isDatabaseExists(string name)
        {
            if (SharedDataAccessMethods.isDatabaseExistsInList(GetInstance(), name)) return true;
            return false;
        }
    }
}
