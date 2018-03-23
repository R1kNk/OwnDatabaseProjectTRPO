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
        public void DeleteTable(string name)
        {
            if(TablesDB.Count==0) throw new NullReferenceException();
            if (indexOfTable(name) != -1)
            {
                TablesDB.RemoveAt(indexOfTable(name));
            }
            else throw new NullReferenceException();
        }
        public void AddTable(Table bufTable)
         {
             if (bufTable.Name.isThereNoUndefinedSymbols())
             {
                 foreach (Table tbl in TablesDB)
                 {
                     if (tbl.Name == bufTable.Name) throw new FormatException("Invalid table name. Some table in this database have same name!");
                 }
                 TablesDB.Add(bufTable);
             }
             else throw new FormatException("There is invalid symbols in table's name!");
         }

        public int indexOfTable(string name)
        {
            if (TablesDB.Count == 0) throw new NullReferenceException();
            for (int i = 0; i < TablesDB.Count; i++)
            {
                if (TablesDB[i].Name == name) return i;
            }
            return -1;
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
