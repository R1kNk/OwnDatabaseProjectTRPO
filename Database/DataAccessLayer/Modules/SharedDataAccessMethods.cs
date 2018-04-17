using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataAccessLayer.Modules
{
    static class SharedDataAccessMethods
    {
        #region Directly call methods
        static internal void CreateDatabasesDirectory()
        {
            System.IO.Directory.CreateDirectory("./DataBases");
        }
        /// <summary>
        /// Check if dir ./Databases exists
        /// </summary>
        /// <returns></returns>
        static internal bool isDirectoryExists()
        {
            if (Directory.Exists("./Databases")) return true;
            return false;
        }
        static internal int HowManyDBFilesInFolder()
        {
            if (!isDirectoryExists()) CreateDatabasesDirectory();
            return Directory.GetFiles("./DataBases", "*.soos").Length;
        }
        #endregion
        #region Extention methods
        static internal bool isDatabaseExistsInList(this List<DataLayer.DataBaseInstance> list,string Name)
        {
            foreach (DataLayer.DataBaseInstance db in list)
                if (db.Name == Name) return true;
            return false;
        }
        static internal int IndexOfDatabase(this List<DataLayer.DataBaseInstance> list, string Name)
        {
            try
            {
                if (list.isDatabaseExistsInList(Name))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Name == Name) return i;
                    }
                }
                else throw new ArgumentException("There is no such Database in list!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return -1;
            
        }
        #endregion
    }
}
