using DataModels.App.InternalDataBaseInstanceComponents;
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
        } //UI
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
        } //UI
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
                Table tableToDelete = GetTableByName(name);
                List<int> keys = new List<int>();
                for (int i = 0; i < tableToDelete.Columns[0].DataList.Count; i++)
                {
                    keys.Add((int)tableToDelete.Columns[0].DataList[i].Data);
                }
                foreach (int key in keys) tableToDelete.DeleteTableElementByPrimaryKey(key);
                for (int i = 0; i < tableToDelete.Columns.Count; i++)
                {
                    if (tableToDelete.Columns[i].IsFkey)
                    {
                        string LinkedTableName = default(string);
                        LinkedTableName = tableToDelete.Columns[i].Name.Substring(5);
                        Console.WriteLine(LinkedTableName);
                        UnLinkTables(tableToDelete, GetTableByName(LinkedTableName));
                    }
                }
                for (int i = 0; i < TablesDB.Count; i++)
                {
                    if (TablesDB[i].isColumnExists("FK_" + tableToDelete.Columns[0].Name))
                    {
                        TablesDB[i].GetColumnByName("FK_" + tableToDelete.Columns[0].Name).SetFkeyProperty(false);
                        TablesDB[i].DeleteColumn("FK_" + tableToDelete.Columns[0].Name);
                    }
                }
                TablesDB.RemoveAt(indexOfTable(name));
            }
            else throw new NullReferenceException();
        } //UI
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
        } //UI
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
        /// <summary>
        /// Add's link many-to-one (second parameter table will be general)
        /// </summary>
        /// <param name="tableToLink"></param>
        /// <param name="tableToLinkWith"></param>
        public void LinkTables(Table tableToLink, Table tableToLinkWith, bool isCascadeDelete)
        {
            LinkColumn newLink = new LinkColumn("FK_"+tableToLinkWith.Columns[0].Name, typeof(int), false, 0, tableToLink, tableToLinkWith.Columns[0]);
            newLink.IsCascadeDeleteOn = isCascadeDelete;
                for (int i = 0; i < tableToLink.Columns[0].DataList.Count; i++)
                {
                    newLink.DataList.Add(new Shared.DataModels.DataObject(newLink.GetHashCode(), newLink.Default));
                }
            tableToLink.Columns.Add(newLink);
            if(newLink.IsCascadeDeleteOn)
            tableToLinkWith.cascadeDelete += tableToLink.ExecuteCascadeDelete;
        } //UI (second table will be general)
        //
        public void EditCascadeDeleteOption(Table tableToEditLink, Table tableToEditLinkWith, bool isCascadeDelete)
        {
            if (tableToEditLink.isColumnExists("FK_" + tableToEditLinkWith.Columns[0].Name))
            {
                if (tableToEditLink.GetColumnByName("FK_" + tableToEditLinkWith.Columns[0].Name).IsCascadeDeleteOn)
                {
                    if(!isCascadeDelete) tableToEditLinkWith.cascadeDelete -= tableToEditLink.ExecuteCascadeDelete;
                }
                else if(!tableToEditLink.GetColumnByName("FK_" + tableToEditLinkWith.Columns[0].Name).IsCascadeDeleteOn)
                {
                    if(isCascadeDelete) tableToEditLinkWith.cascadeDelete += tableToEditLink.ExecuteCascadeDelete;
                }
            }
            else if (tableToEditLinkWith.isColumnExists("FK_" + tableToEditLink.Columns[0].Name))
            {
                if (tableToEditLinkWith.GetColumnByName("FK_" + tableToEditLink.Columns[0].Name).IsCascadeDeleteOn)
                {
                    if (!isCascadeDelete) tableToEditLink.cascadeDelete -= tableToEditLinkWith.ExecuteCascadeDelete;
                }
                else if (!tableToEditLinkWith.GetColumnByName("FK_" + tableToEditLink.Columns[0].Name).IsCascadeDeleteOn)
                {
                    if (isCascadeDelete) tableToEditLink.cascadeDelete += tableToEditLinkWith.ExecuteCascadeDelete;
                }
            }
            else throw new NullReferenceException("There's no link between this tables");
        } //UI (do not care about what table is on the first place and what table on the second)
        /// <summary>
        /// Unlinks two tables
        /// </summary>
        /// <param name="TableToUnlink"></param>
        /// <param name="TableToUnlinkWith"></param>
        public void UnLinkTables(Table TableToUnlink, Table TableToUnlinkWith)
        {
            if (TableToUnlink.isColumnExists("FK_" + TableToUnlinkWith.Columns[0].Name))
            {
                TableToUnlink.GetColumnByName("FK_" + TableToUnlinkWith.Columns[0].Name).SetFkeyProperty(false);
                TableToUnlink.DeleteColumn("FK_" + TableToUnlinkWith.Columns[0].Name);
                TableToUnlinkWith.cascadeDelete -= TableToUnlink.ExecuteCascadeDelete;
            }
            else if (TableToUnlinkWith.isColumnExists("FK_" + TableToUnlink.Columns[0].Name))
            {
                TableToUnlinkWith.GetColumnByName("FK_" + TableToUnlink.Columns[0].Name).SetFkeyProperty(false);
                TableToUnlinkWith.DeleteColumn("FK_" + TableToUnlink.Columns[0].Name);
                TableToUnlink.cascadeDelete -= TableToUnlinkWith.ExecuteCascadeDelete;
            }
            else throw new NullReferenceException("There's no link between this tables");
        } //UI (do not care about places of table's in parametres)
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
