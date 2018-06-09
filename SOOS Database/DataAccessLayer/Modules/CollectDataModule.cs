using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityLayer;
using System.IO;
using DataAccessLayer.Exceptions;
using DataLayer;

namespace DataAccessLayer.Modules
{
    static class CollectDataModule
    {
        /// <summary>
        /// Search for such db file in folder and load it
        /// </summary>
        /// <param name="DBName">Name of database</param>
        /// <returns></returns>
        /// <exception cref="DatabasesNotFoundInFolderException">Throws ehen there is no databases folder</exception>
        static internal DataBaseInstance LoadDataBase(string DBName)
        {
            try
            {
                if (SharedDataAccessMethods.HowManyDBFilesInFolder() == 0) throw new DatabasesNotFoundInFolderException("There is no Databases in folder!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            string[] _filePaths = Directory.GetFiles("./DataBases", "*.soos");
            if (_filePaths.Contains<string>("./DataBases\\"+DBName+".soos"))
            {
                // pa ongleske, pidar
                string _filePath = ("./DataBases\\" + DBName+".soos");

                return DecryptDataBaseFromPath(_filePath);
            }
            return new DataBaseInstance("nullDB"); ;
        }

        /// <summary>
        /// Delete all db instances from list and adds all db files that contains folder
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabasesNotFoundInFolderException">Throws ehen there is no databases folder</exception>

        static internal List<DataBaseInstance> LoadAllDataBases()
        {

            if (SharedDataAccessMethods.HowManyDBFilesInFolder() == 0) throw new DatabasesNotFoundInFolderException("There is no Databases in folder!");
            List<DataBaseInstance> bufList = new List<DataBaseInstance>();
            if (SharedDataAccessMethods.isDirectoryExists())
            {
                string[] _filePaths = System.IO.Directory.GetFiles("./DataBases", "*.soos");
                for (int i = 0; i < _filePaths.Length; i++)
                {
                    bufList.Add(DecryptDataBaseFromPath(_filePaths[i]));
                }
            }
            else SharedDataAccessMethods.CreateDatabasesDirectory();
            return bufList;
        }
        /// <summary>
        /// Taking all db's from folder and adds them to the list, if list doesn't contains db with such name - adds it. Otherwise saves old db object in list (P.S. NO INFO DELETE, ONLY ADDING)
        /// </summary>
        /// <param name="list">List with databases</param>
        /// <returns></returns>
        static internal List<DataBaseInstance> UpdatativeDatabasesLoad(List<DataBaseInstance> list)
        {
            if (SharedDataAccessMethods.HowManyDBFilesInFolder() == 0) return list;
            List<DataBaseInstance> bufList = list;
            DataBaseInstance bufInst;
            if (SharedDataAccessMethods.isDirectoryExists())
            {
                string[] _filePaths = System.IO.Directory.GetFiles("./DataBases", "*.soos");
                
                for (int i = 0; i < _filePaths.Length; i++)
                {
                    char[] name = new char[_filePaths[i].Length - 17];
                    _filePaths[i].CopyTo(12, name, 0, _filePaths[i].Length - 17);
                    string DBName = new string(name);
                    bufInst = DecryptDataBaseFromPath(_filePaths[i]);
                    if(bufInst==null) Console.WriteLine("Error: Database {0} is corrupted and can't be loaded!", DBName);
                    else
                    if (!bufList.isDatabaseExistsInList((bufInst.Name))) bufList.Add(bufInst);
                }
            }
            else SharedDataAccessMethods.CreateDatabasesDirectory();
            return bufList;
        }

        /// <summary>
        /// Takes a path to file and perform it to database instance
        /// </summary>
        /// <param name="filePathToDecrypt"></param>
        /// <returns></returns>
        static private DataBaseInstance DecryptDataBaseFromPath(string filePathToDecrypt)
        {
            byte[] _array = File.ReadAllBytes(filePathToDecrypt);
            //
            DataBaseInstance inst = SecurityLayer.Modules.DecryptionModule.DecryptDataBase(_array);
            return inst;
        }
    }
}
