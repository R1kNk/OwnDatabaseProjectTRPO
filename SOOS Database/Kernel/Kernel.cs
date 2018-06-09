using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using DataLayer.Shared;
using DataLayer.Shared.ExtentionMethods;
using DataAccessLayer.Modules;
using System.IO;

[assembly: InternalsVisibleTo("FilesLayer")]

namespace DataLayer
{
    public static class Kernel
    {
        private static List<DataBaseInstance> instance;
        private static object lockObject = new Object();
        /// <summary>
        /// Gets singletone main instance
        /// </summary>
        /// <returns></returns>
        internal static List<DataBaseInstance> GetInstance()
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new List<DataBaseInstance>();
                        Console.WriteLine("<Databases load process from folder was started!>", instance.Count);

                        LoadAllDatabases(true);
                        if(instance.Count==1) Console.WriteLine("One database was loaded from folder!");
                        else
                        Console.WriteLine("{0} databases were loaded from folder!", instance.Count);

                    }
                }
            }
            return instance;
        }
        /// <summary>
        /// Returns database instance by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DataBaseInstance GetInstance(string name)
        {

                var _instance = Kernel.GetInstance();
                var element = _instance.FindLast(x => x.Name == name);
                try
                {
                    if (element != null)
                        return element;
                    throw new IndexOutOfRangeException("Null Instance");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return element;
           

        }
        
        /// <summary>
        /// Outs info about all databases to console
        /// </summary>
        public static void OutDatabaseInfo()
        {
            try
            {
                if (GetInstance().Count == 0) throw new NullReferenceException("There is no DB's in list!");
                for (int i = 0; i < GetInstance().Count; i++)
                {
                    Console.WriteLine(GetInstance()[i].ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        ///  Outs info about  database to console
        /// </summary>
        /// <param name="name">Name of database</param>
        public static void OutDatabaseInfo(string name)
        {
            int index = SharedDataAccessMethods.IndexOfDatabase(GetInstance(), name);
            Console.WriteLine(GetInstance()[index].ToString());
        }
        /// <summary>
        /// Outs names of eisting database to  console
        /// </summary>
        public static void OutNamesOfExistingDBs()
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } //UI done
        /// <summary>
        /// Load database to instance by name
        /// </summary>
        /// <param name="name">Database Name</param>
        internal static void LoadDatabase(string name)
        {
            try
            {
                DataBaseInstance bufInst = CollectDataModule.LoadDataBase(name);
                if (bufInst.Name == "nullDB") throw new ArgumentException("THere is no DB with such name in folder");
                if (GetInstance().isDatabaseExistsInList(bufInst.Name))
                {
                    GetInstance()[GetInstance().IndexOfDatabase(bufInst.Name)] = bufInst;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        /// <summary>
        /// Checks s database exists
        /// </summary>
        /// <param name="name">Database Name</param>
        /// <returns></returns>
        internal static bool isDatabaseExists(string name)
        {
            return SharedDataAccessMethods.isDatabaseExistsInList(GetInstance(), name);
        }
        /// <summary>
        /// Renames database
        /// </summary>
        /// <param name="currentName">Current DB name</param>
        /// <param name="futureName">Future DB name</param>
        internal static void RenameDatabase(string currentName, string futureName)
        {
            try
            {
                if (isDatabaseExists(currentName))
                {
                    if (futureName.isThereNoUndefinedSymbols())
                    {
                        GetInstance(currentName).Name = futureName;
                    }
                    else throw new ArgumentException("Your name contains undefined symbols!");
                }
                else throw new NullReferenceException("There's no such database in list");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } //UI done
        /// <summary>
        /// Adds new database
        /// </summary>
        /// <param name="name">DB name</param>
        internal static void AddDBInstance(string name)
        {
            DataBaseInstance bufInst = new DataBaseInstance(name);
            AddDBInstance(bufInst);
        } //UI done
        //
        /// <summary>
        /// Adds instance of database
        /// </summary>
        /// <param name="inst">instance object</param>
        internal static void AddDBInstance(DataBaseInstance inst)
        {
            try
            {
                var _instance = Kernel.GetInstance();
                if (_instance.FindAll(x => x.Name == inst.Name).Count != 0 || !inst.Name.isThereNoUndefinedSymbols())
                    throw new ArgumentException("Invalid name of database");
                _instance.Add(inst);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// Save databases to folder
        /// </summary>
        /// <param name="inst">instance to save</param>
        internal static void SaveDataBaseInstanceToFolder(this DataBaseInstance inst)
        {
            CacheModule.SaveDataBaseToFolder(inst);
        } //UI done
        /// <summary>
        /// Saves all databases to folder
        /// </summary>
        public static void SaveAllDatabases()
        {
            CacheModule.SaveAllDatabases(GetInstance());
        } //UI done
        /// <summary>
        /// Deletes database by name
        /// </summary>
        /// <param name="name">DB Name</param>
        internal static void DeleteDatabase(string name)
        {
            try
            {
                if (isDatabaseExists(name))
                {
                    GetInstance().Remove(GetInstance(name));
                    DirectoryInfo dirInfo = new DirectoryInfo("./DataBases/");
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        if (file.Name == name)
                        { file.Delete(); return; }
                    }

                }
                else throw new NullReferenceException($"\nERROR: Database '{name}' doesn't exist\n");
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// Loads all databases from folder using regular or updatative load
        /// </summary>
        /// <param name="isUpdatativeLoad">Is we must use updatative load?</param>
        internal static void LoadAllDatabases(bool isUpdatativeLoad)
        {
            if (!isUpdatativeLoad) instance = CollectDataModule.LoadAllDataBases();
            instance = CollectDataModule.UpdatativeDatabasesLoad(instance);

        }

    }
}
