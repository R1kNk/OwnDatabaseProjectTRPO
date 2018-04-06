using DataLayer.InternalDataBaseInstanceComponents;
using DataLayer.Shared.ExtentionMethods;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DataLayer
{
    
     [Serializable]
     public class DataBaseInstance
     {
         //fields
         List<Table> _tablesDB = new List<Table>();
         string _name;
         //properties
         public string Name { get => _name; set => _name = value; }
         public List<Table> TablesDB { get => _tablesDB; set => _tablesDB = value; }
         
         /// <summary>
         /// DB constructor
         /// </summary>
         /// <param name="name"></param>
         public DataBaseInstance(string name)
         {
             _name = name;
         }
        //
        /// <summary>
        /// Add table to this Database
        /// </summary>
        /// <param name="bufTable"></param>
        public void AddTable(string name)
        {
            Table bufTable = new Table(name);
            AddTable(bufTable);
        }
        //
        /// <summary>
        /// adds table to db
        /// </summary>
        /// <param name="bufTable"></param>
        public void AddTable(Table bufTable)
        {
            if (bufTable.Name.isThereNoUndefinedSymbols())
            {
                if (isTableExists(bufTable.Name)) throw new FormatException("Invalid table name. Some table in this database have same name!");
                TablesDB.Add(bufTable);
            }
            else throw new FormatException("There is invalid symbols in table's name!");
        }
        //
        /// <summary>
        /// Delete table by name
        /// </summary>
        /// <param name="name"></param>
        public void DeleteTable(string name)
        {
            if(TablesDB.Count==0) throw new NullReferenceException();
            if (indexOfTable(name) != -1)
            {
                TablesDB.RemoveAt(indexOfTable(name));
            }
            else throw new NullReferenceException();
        }
        //
        /// <summary>
        /// Rename table
        /// </summary>
        /// <param name="currentName"></param>
        /// <param name="futureName"></param>
        public void RenameTable(string currentName, string futureName)
        {
            if (isTableExists(currentName))
            {
                if (futureName.isThereNoUndefinedSymbols()) GetTableByName(currentName).Name = futureName;
                else throw new ArgumentException("Your name contains undefined symbols!");
            }
            throw new ArgumentNullException("there is no such table in this database!");
        }
        //
        /// <summary>
        /// check if this database already contains table with such name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool isTableExists(string name)
        {
            if (TablesDB.Count == 0) return false;
            else
            {
                foreach (Table tbl in TablesDB)
                if (tbl.Name == name) return true;
            }
            return false;
        }
        //
        int indexOfTable(string name)
        {
            if (TablesDB.Count == 0) throw new NullReferenceException();
            for (int i = 0; i < TablesDB.Count; i++)
            {
                if (TablesDB[i].Name == name) return i;
            }
            return -1;
        }
        public Table GetTableByName(string name)
        {
            if (TablesDB.Count != 0)
            {
                if (isTableExists(name))
                {
                    return TablesDB[indexOfTable(name)];
                }
                throw new NullReferenceException("There's no such table in Database");
            }
            throw new NullReferenceException("There's no tables in Database");
        }

        public override string ToString()
        {
            string info = "|DATABASE| " + Name + " contains " + TablesDB.Count + " tables ";
            for (int i = 0; i < TablesDB.Count; i++)
            {
                info += "\n"+ TablesDB[i].ToString();
            }
            info += "\n|DATABASE| "+Name +" END\n";
            return info; 
        }
    }
    
}
