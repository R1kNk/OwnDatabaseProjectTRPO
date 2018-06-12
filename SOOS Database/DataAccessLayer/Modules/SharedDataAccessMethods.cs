using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataAccessLayer.Modules
{
    /// <summary>
    /// Class represents shared methods for saving and loading
    /// </summary>
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

        /// <summary>
        /// Checks Is such databases exists in list
        /// </summary>
        /// <param name="list">list with databases</param>
        /// <param name="Name">NAme of database to check</param>
        /// <returns></returns>
        static internal bool isDatabaseExistsInList(this List<DataLayer.DataBaseInstance> list,string Name)
        {
            foreach (DataLayer.DataBaseInstance db in list)
                if (db.Name == Name) return true;
            return false;
        }

        /// <summary>
        /// Returns index of database by name in provided list
        /// </summary>
        /// <param name="list">List with databases to search in</param>
        /// <param name="Name">Name of database</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">There is no such Database in list!</exception>

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
