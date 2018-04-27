using DataModels.App.InternalDataBaseInstanceComponents;
using DataLayer.Shared.ExtentionMethods;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using DataLayer.Shared.DataModels;
using DataModels.App.Shared.ExtentionMethods;
using System.Linq;

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
        } //UI done
        //
        /// <summary>
        /// adds table to db
        /// </summary>
        /// <param name="bufTable"></param>
        public void AddTable(Table bufTable)
        {
            try
            {
                if (bufTable.Name.isThereNoUndefinedSymbols())
                {
                    if (isTableExists(bufTable.Name)) throw new FormatException("Invalid table name. Some table in this database have same name!");
                    TablesDB.Add(bufTable);
                }
                else throw new FormatException("There is invalid symbols in table's name!");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI done
        //
        /// <summary>
        /// Delete table by name
        /// </summary>
        /// <param name="name"></param>
        public void DeleteTable(string name)
        {
            try
            {
                if (TablesDB.Count == 0) throw new NullReferenceException("There is no tables in this database!");
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
                else throw new NullReferenceException("There is no such table in this database");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI done
        //
        /// <summary>
        /// Rename table
        /// </summary>
        /// <param name="currentName"></param>
        /// <param name="futureName"></param>
        public void RenameTable(string currentName, string futureName)
        {
            try
            {
                if (isTableExists(currentName))
                {
                    if (futureName.isThereNoUndefinedSymbols()) GetTableByName(currentName).Name = futureName;
                    else throw new ArgumentException("Your name contains undefined symbols!");
                }
                throw new ArgumentNullException("there is no such table in this database!");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI done
        //
        /// <summary>
        /// check if this database already contains table with such name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool isTableExists(string name)
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
            try
            {

                if (tableToLink.isColumnExists("FK_" + tableToLinkWith.Columns[0].Name) || tableToLinkWith.isColumnExists("FK_" + tableToLink.Columns[0].Name)) throw new Exception("Those tables already linked!");
                LinkColumn newLink = new LinkColumn("FK_" + tableToLinkWith.Columns[0].Name, typeof(int), false, 0, tableToLink, tableToLinkWith.Columns[0]);
                newLink.IsCascadeDeleteOn = isCascadeDelete;
                for (int i = 0; i < tableToLink.Columns[0].DataList.Count; i++)
                {
                    newLink.DataList.Add(new DataObject(newLink.GetHashCode(), newLink.Default));
                }
                tableToLink.Columns.Add(newLink);
                if (newLink.IsCascadeDeleteOn)
                    tableToLinkWith.cascadeDelete += tableToLink.ExecuteCascadeDelete;
            }
            catch(Exception e)
            {
                Console.WriteLine("Critical Error: " + e.Message+"\n");
            }
        } //UI (second table will be general)
        //
        public void EditCascadeDeleteOption(Table tableToEditLink, Table tableToEditLinkWith, bool isCascadeDelete)
        {
            try
            {
                if (tableToEditLink.isColumnExists("FK_" + tableToEditLinkWith.Columns[0].Name))
                {
                    if (tableToEditLink.GetColumnByName("FK_" + tableToEditLinkWith.Columns[0].Name).IsCascadeDeleteOn)
                    {
                        if (!isCascadeDelete) tableToEditLinkWith.cascadeDelete -= tableToEditLink.ExecuteCascadeDelete;
                    }
                    else if (!tableToEditLink.GetColumnByName("FK_" + tableToEditLinkWith.Columns[0].Name).IsCascadeDeleteOn)
                    {
                        if (isCascadeDelete) tableToEditLinkWith.cascadeDelete += tableToEditLink.ExecuteCascadeDelete;
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
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } //UI (do not care about what table is on the first place and what table on the second)
        /// <summary>
        /// Unlinks two tables
        /// </summary>
        /// <param name="TableToUnlink"></param>
        /// <param name="TableToUnlinkWith"></param>
        public void UnLinkTables(Table TableToUnlink, Table TableToUnlinkWith)
        {
            try
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
        /// <summary>
        /// returns table by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Table GetTableByName(string name)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        //
        public void setNewTable(Table oldTable, Table newTable)
        {
            oldTable = newTable;
        }
        //
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

        //query methods


        //for selection

        /// <summary>
        /// Condition selection
        /// </summary>
        /// <param name="tableForSelection"></param>
        /// <param name="columnName"></param>
        /// <param name="selectOperator"></param>
        /// <param name="selectObject"></param>
        /// <returns></returns>
        public Table QueryWhereConditionSelection(Table tableForSelection, string columnName, string selectOperator, object selectObject, ref string outResult)
        {
            if (!outResult.isStatusCodeOk()) return null;
            try
            {
                Table queryResult = new Table(tableForSelection.Name, false);
                for (int i = 0; i < tableForSelection.Columns.Count; i++)
                    queryResult.Columns.Add(new Column(tableForSelection.Columns[i], queryResult));

                if (tableForSelection.isTableContainsData())
                {
                    if (queryResult.isColumnExists(columnName))
                    {
                        Column queryColumn = queryResult.GetColumnByName(columnName);
                        if (selectOperator.isSelectOperator())
                        {
                            if (queryColumn.DataType == selectObject.GetType())
                                switch (selectOperator)
                                {
                                    case "=":
                                        {
                                            if (selectObject.GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                    if ((string)queryColumn.DataList[i].Data != (string)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; }
                                            }
                                            else if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data != (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data != (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObject.GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((bool)queryColumn.DataList[i].Data != (bool)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return queryResult;
                                        }
                                    case "!=":
                                        {
                                            if (selectObject.GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                    if ((string)queryColumn.DataList[i].Data == (string)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                            }
                                            else if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data == (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data == (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            }
                                            else if (selectObject.GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((bool)queryColumn.DataList[i].Data == (bool)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            }
                                            return queryResult;
                                        }
                                    case ">":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data < (int)selectObject || (int)queryColumn.DataList[i].Data == (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data < (double)selectObject || (double)queryColumn.DataList[i].Data == (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            }
                                            return queryResult;
                                        }
                                    case "<":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data > (int)selectObject || (int)queryColumn.DataList[i].Data == (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data > (double)selectObject || (double)queryColumn.DataList[i].Data == (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            }
                                            return queryResult;
                                        }
                                    case "<=":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data > (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data > (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            }
                                            return queryResult;
                                        }
                                    case ">=":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data < (int)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data < (double)selectObject) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            }
                                            return queryResult;
                                        }
                                }
                            else throw new ArgumentException("Invalid object for select!");
                        }
                        else { if (selectOperator.isSelectComplexOperator()) throw new ArgumentException(selectOperator + " is complex operator, you need more selectObjects to use it!"); else throw new ArgumentException("Invalid operator"); }
                    }
                    else throw new ArgumentException("There is no " + columnName + " column in " + tableForSelection.Name + " table");
                }
                else return tableForSelection;
                return queryResult;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                Console.WriteLine(outResult);
                return null;
            }
        }

        /// <summary>
        /// condition selection for complex operators
        /// </summary>
        /// <param name="tableForSelection"></param>
        /// <param name="columnName"></param>
        /// <param name="selectOperator"></param>
        /// <param name="selectObjects"></param>
        /// <param name="outResult"></param>
        /// <returns></returns>
        public Table QueryWhereConditionSelection(Table tableForSelection, string columnName, string selectOperator, object[] selectObjects, ref string outResult)
        {
            if (!outResult.isStatusCodeOk()) return null;
            try
            {
                Table queryResult = new Table(tableForSelection.Name, false);
                for (int i = 0; i < tableForSelection.Columns.Count; i++)
                    queryResult.Columns.Add(new Column(tableForSelection.Columns[i], queryResult));

                if (tableForSelection.isTableContainsData())
                {
                    if (queryResult.isColumnExists(columnName))
                    {
                        Column queryColumn = queryResult.GetColumnByName(columnName);
                        if (selectOperator.isSelectComplexOperator())
                        {
                            Type typeColumn = queryColumn.DataType;
                            if (selectObjects.Length == 0) throw new ArgumentException("There is no objects to select!");
                            if (selectObjects.IsArrayContainOnlyTValues(typeColumn))
                            {
                                switch (selectOperator)
                                {
                                    case "BETWEEN":
                                        {
                                            if (typeColumn == typeof(string)) throw new ArgumentException("String value is not compatible with BETWEEN operator!");
                                            if (typeColumn == typeof(bool)) throw new ArgumentException("Bool value is not compatible with BETWEEN operator!");

                                            Array.Sort(selectObjects);
                                            if (selectObjects.Length != 2) throw new ArgumentException("Complex select operator " + selectOperator + " works only with 2 objects for selection!");

                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data <= (int)selectObjects[0] || (int)queryColumn.DataList[i].Data >= (int)selectObjects[1]) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data <= (double)selectObjects[0] || (double)queryColumn.DataList[i].Data >= (double)selectObjects[1]) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            }
                                            return queryResult;
                                        }
                                    case "IN":
                                        {
                                            Array.Sort(selectObjects);
                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((int)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((double)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((string)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((bool)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return queryResult;
                                        }
                                    case "NOT_BETWEEN":
                                        {
                                            if (typeColumn == typeof(string)) throw new ArgumentException("String value is not compatible with NOT_BETWEEN operator!");
                                            if (typeColumn == typeof(bool)) throw new ArgumentException("Bool value is not compatible with NOT_BETWEEN operator!");

                                            Array.Sort(selectObjects);
                                            if (selectObjects.Length != 2) throw new ArgumentException("Complex select operator " + selectOperator + "works only with 2 objects for selection!");
                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data > (int)selectObjects[0] && (int)queryColumn.DataList[i].Data < (int)selectObjects[1]) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data > (double)selectObjects[0] && (double)queryColumn.DataList[i].Data < (double)selectObjects[1]) { queryResult.DeleteTableElementByIndex(i); i--; ; }
                                                }
                                            }
                                            return queryResult;
                                        }
                                    case "NOT_IN":
                                        {
                                            Array.Sort(selectObjects);
                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((int)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((double)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((string)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((bool)queryColumn.DataList[i].Data)) { queryResult.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return queryResult;
                                        }
                                }

                            }
                            else throw new ArgumentException("Invalid objects for select!");

                        }
                        else { if (selectOperator.isSelectComplexOperator()) throw new ArgumentException(selectOperator + " is complex operator, you need more selectObjects to use it!"); else throw new ArgumentException("Invalid operator"); }
                    }
                    else throw new ArgumentException("There is no " + columnName + " column in " + tableForSelection.Name + " table");
                }
                else return tableForSelection;
                return queryResult;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                Console.WriteLine(outResult);
                return null;
            }
        }

        /// <summary>
        /// selects columns from a table and return result
        /// </summary>
        /// <param name="ColumnNames"></param>
        /// <param name="tableForQuery"></param>
        /// <returns></returns>
        public Table QueryColumnSelection(List<string> ColumnNames, Table tableForQuery, ref string outResult)
        {
            try
            {
                if (!outResult.isStatusCodeOk()) return null;
                foreach (string name in ColumnNames) if (!name.Contains(".") || !tableForQuery.isColumnExists(name)) throw new ArgumentException("Invalid column name!");
                Table queryresult = new Table(tableForQuery.Name, false);
                foreach (string name in ColumnNames)
                {
                    Column oldColumn = tableForQuery.GetColumnByName(name);
                    Column toAdd = new Column(oldColumn.Name, oldColumn.DataType, oldColumn.AllowsNull, oldColumn.Default, queryresult);
                    toAdd.DataList = oldColumn.CloneData();
                    queryresult.Columns.Add(toAdd);
                }
                outResult = "OK";
                return queryresult;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                Console.WriteLine(outResult);
                return null;
            }

        }

        /// <summary>
        /// Returns table with one column which contains number of records in table
        /// </summary>
        /// <param name="tableforQuery"></param>
        /// <param name="outResult"></param>
        /// <returns></returns>
        public Table QueryCountSelection(Table tableforQuery, ref string outResult)
        {
            try
            {
                if (!outResult.isStatusCodeOk()) return null;
                if (tableforQuery.Columns.Count <= 1) throw new ArgumentException("Table " + tableforQuery.Name + " doesn't contains any columns");
                Table queryresult = new Table(tableforQuery.Name, false);
                Column toAdd = new Column($"RECORDS_COUNT({tableforQuery.Name})", typeof(int), false, 0, queryresult);
                toAdd.DataList.Add(new DataObject(toAdd.GetHashCode(), tableforQuery.Columns[0].DataList.Count));
                queryresult.Columns.Add(toAdd);
                outResult = "OK";
                return queryresult;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                Console.WriteLine(outResult);
                return null;
            }
        }
        
        /// <summary>
        /// Returns first values from table according to percents or values 
        /// </summary>
        /// <param name="tableforQuery"></param>
        /// <param name="Value"></param>
        /// <param name="isValueCount"></param>
        /// <param name="outResult"></param>
        /// <returns></returns>
        public Table QueryTopSelection(Table tableforQuery, int Value, bool isValueCount, ref string outResult)
        {
            try
            {
                if (!outResult.isStatusCodeOk()) return null;
                Table queryresult = new Table(tableforQuery);

                if (isValueCount)
                {
                    if (Value < 1) throw new ArgumentException("Value must be more than zero to select!");
                    if (Value > queryresult.Columns[0].DataList.Count) Value = queryresult.Columns[0].DataList.Count;

                }
                else
                {
                    if (Value < 1 || Value > 100) throw new ArgumentException("Percents must be more than 0 and less than 100 to select!");
                    Value = (int)Math.Truncate(Convert.ToDouble(queryresult.Columns[0].DataList.Count) * Convert.ToDouble(Convert.ToDouble(Value) / 100));


                    for (int i = Value; i < queryresult.Columns[0].DataList.Count; i++)
                    {
                        queryresult.DeleteTableElementByIndex(i); i--;
                    }
                }
                    outResult = "OK";
                    return queryresult;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                Console.WriteLine(outResult);
                return null;
            }
        }

        /// <summary>
        /// Sorts table by values in column
        /// </summary>
        /// <param name="columnNameSortBy"></param>
        /// <param name="tableForQuery"></param>
        /// <param name="isAscending"></param>
        /// <returns></returns>
        public Table QuerySortTable(string columnNameSortBy, Table tableForQuery, bool isAscending, ref string outResult)
        {
            if (!outResult.isStatusCodeOk()) return null;
            try
            {
                Table queryResult = new Table(tableForQuery);
                if (queryResult.isColumnExists(columnNameSortBy))
                {
                    List<DataObject[]> data = new List<DataObject[]>();
                    for (int i = 0; i < queryResult.Columns[0].DataList.Count; i++)
                        data.Add(queryResult.GetDataByIndex(i));

                    int columnSortIndex = queryResult.getIndexOfColumn(columnNameSortBy);
                    Column columnToSort = queryResult.Columns[columnSortIndex];
                    List<object> columnData = new List<object>();

                    for (int i = 0; i < columnToSort.DataList.Count; i++)
                    {
                        if (columnToSort.DataList[i].Data != null)
                        columnData.Add(columnToSort.DataList[i].Data);
                    }
                    columnData.Sort();
                    if (!isAscending) columnData.Reverse();
                    for (int i = 0; i < columnData.Count; i++)
                    {
                        for (int j = 0; j < data.Count; j++)
                        {
                            object[] dataArray = new object[queryResult.Columns.Count];
                            bool finded = false;
                            if (data[j][columnSortIndex].Data == columnData[i])
                            {
                                if (data[j][columnSortIndex].Data != null)
                                {
                                    queryResult.EditTableElementByIndex(i, data[j]);
                                    data.RemoveAt(j);
                                    finded = true;
                                }
                            }
                            if (finded)
                                break;
                        }
                    }
                    int nullIndex = queryResult.Columns[0].DataList.Count - data.Count;
                    if(nullIndex!= queryResult.Columns[0].DataList.Count)
                    for (int i = nullIndex, j = 0; i < queryResult.Columns[0].DataList.Count || j < data.Count; i++, j++)
                    {
                        queryResult.EditTableElementByIndex(i, data[j]);

                    }

                }
                else throw new ArgumentException("There is no " + columnNameSortBy + " column in " + tableForQuery.Name + "!");
                return queryResult;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                Console.WriteLine(outResult);
                return null;
            }
        }

        /// <summary>
        /// Average value from column, available only for int32 and double
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="tableForQuery"></param>
        /// <param name="outResult"></param>
        /// <returns></returns>
        public Table QueryAvgSelection(string ColumnName, Table tableForQuery, ref string outResult)
        {
            try
            {
                if (!outResult.isStatusCodeOk()) return null;
                if (!ColumnName.Contains(".") || !tableForQuery.isColumnExists(ColumnName)) throw new ArgumentException("Invalid column name!");
                Table queryresult = new Table(tableForQuery.Name, false);
               
                    Column oldColumn = tableForQuery.GetColumnByName(ColumnName);

                if (oldColumn.DataType == typeof(string)) throw new ArgumentException("Average selections don't work with string variables!");
                if (oldColumn.DataType == typeof(bool)) throw new ArgumentException("Average selections don't work with bool variables!");
                string averageData = default(string);
                if (!tableForQuery.isTableContainsData()) throw new ArgumentException($"There is no data in {tableForQuery.Name}!");
                if (oldColumn.DataList.Where(x => x.Data != null).Count() != 0)
                {
                    if (oldColumn.DataType == typeof(int))
                        averageData = oldColumn.DataList.Where(x => x.Data != null).Average(x => (int)x.Data).ToString();
                    else if (oldColumn.DataType == typeof(double))
                        averageData = oldColumn.DataList.Where(x => x.Data != null).Average(x => (double)x.Data).ToString();
                    else throw new ArgumentException("Invalid data type!");
                }
                else averageData = "null";
                //
                Column AvgInfo = new Column($"AVG_VALUE({ColumnName})", typeof(string), false, 0.0, queryresult);
                AvgInfo.DataList.Add(new DataObject(AvgInfo.GetHashCode(), averageData));
                queryresult.Columns.Add(AvgInfo);
                outResult = "OK";
                return queryresult;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                Console.WriteLine(outResult);
                return null;
            }

        }

        public Table QueryMINMAXSUMSelection(string ColumnName, Table tableForQuery, string Action, ref string outResult)
        {
            try
            {
                if (!outResult.isStatusCodeOk()) return null;
                if (!ColumnName.Contains(".") || !tableForQuery.isColumnExists(ColumnName)) throw new ArgumentException("Invalid column name!");
                Table queryresult = new Table(tableForQuery.Name, false);

                Column oldColumn = tableForQuery.GetColumnByName(ColumnName);
                if (Action != "MIN" && Action != "MAX" && Action != "SUM") throw new ArgumentException("Invalid action for selection!");
                if (oldColumn.DataType == typeof(string)) throw new ArgumentException("MIN/MAX selections don't work with string variables!");
                if (oldColumn.DataType == typeof(bool)) throw new ArgumentException("MIN/MAX selections don't work with bool variables!");
                string averageData = default(string);
                if (!tableForQuery.isTableContainsData()) throw new ArgumentException($"There is no data in {tableForQuery.Name}!");
                if (oldColumn.DataList.Where(x => x.Data != null).Count() != 0)
                {
                    if (oldColumn.DataType == typeof(int))
                    {
                        if (Action == "MIN")
                            averageData = oldColumn.DataList.Where(x => x.Data != null).Min(x => (int)x.Data).ToString();
                        else if (Action == "MAX") averageData = oldColumn.DataList.Where(x => x.Data != null).Max(x => (int)x.Data).ToString();
                        else if (Action == "SUM") averageData = oldColumn.DataList.Where(x => x.Data != null).Sum(x => (int)x.Data).ToString();
                    }
                    else if (oldColumn.DataType == typeof(double))
                    {
                        if (Action == "MIN")
                            averageData = oldColumn.DataList.Where(x => x.Data != null).Min(x => (double)x.Data).ToString();
                        else if(Action=="MAX")averageData = oldColumn.DataList.Where(x => x.Data != null).Max(x => (double)x.Data).ToString();
                        else if(Action=="SUM") averageData = oldColumn.DataList.Where(x => x.Data != null).Sum(x => (double)x.Data).ToString();
                    }
                    else throw new ArgumentException("Invalid data type!");
                }
                else averageData = "null";
                //
                string name = default(string);
                if (Action=="MIN")
                    name = $"MIN({ColumnName})";
                else if(Action=="MAX") name = $"MAX({ColumnName})";
                else name = $"SUM({ColumnName})";
                Column Info = new Column(name, typeof(string), false, 0.0, queryresult);
                Info.DataList.Add(new DataObject(Info.GetHashCode(), averageData));
                queryresult.Columns.Add(Info);
                outResult = "OK";
                return queryresult;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                Console.WriteLine(outResult);
                return null;
            }

        }

        //for update
        /// <summary>
        /// Condition update
        /// </summary>
        /// <param name="tableForSelection"></param>
        /// <param name="columnName"></param>
        /// <param name="selectOperator"></param>
        /// <param name="selectObject"></param>
        /// <returns></returns>
        public void QueryWhereConditionUpdate(Table tableForSelection, string columnName, string selectOperator, object selectObject, string[] columnsName, object[] values, ref string outResult)
        {
            if (!outResult.isStatusCodeOk()) return;
            try
            {
                
                if (tableForSelection.isTableContainsData())
                {
                    if (tableForSelection.isColumnExists(columnName))
                    {
                        Column queryColumn = tableForSelection.GetColumnByName(columnName);
                        List<Column> columns = new List<Column>();
                        if (columnsName.Length != values.Length) throw new ArgumentException("Names of columns to update isn't similar to count of values");
                        for (int i = 0; i < columnsName.Length; i++)
                        {
                            columns.Add(tableForSelection.GetColumnByName(columnsName[i]));
                        }
                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (columns[i].DataType != values[i].GetType()) throw new ArgumentException("One of the arguments type is not similar with type of column");
                        }
                        if (selectOperator.isSelectOperator())
                        {
                            if (queryColumn.DataType == selectObject.GetType())
                                switch (selectOperator)
                                {
                                    case "=":
                                        {
                                            if (selectObject.GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                    if ((string)queryColumn.DataList[i].Data == (string)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                            }
                                            else if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data == (int)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                        columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data == (double)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                        columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            else if (selectObject.GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((bool)queryColumn.DataList[i].Data == (bool)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                        columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            return ;
                                        }
                                    case "!=":
                                        {
                                            if (selectObject.GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                    if ((string)queryColumn.DataList[i].Data != (string)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                        columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                            }
                                            else if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data != (int)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                        columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data != (double)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                        columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            else if (selectObject.GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((bool)queryColumn.DataList[i].Data != (bool)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            return ;
                                        }
                                    case ">":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data > (int)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data > (double)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            return ;
                                        }
                                    case "<":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data < (int)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data < (double)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            return;
                                        }
                                    case "<=":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data < (int)selectObject || (int)queryColumn.DataList[i].Data == (int)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data < (double)selectObject || (double)queryColumn.DataList[i].Data == (double)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            return ;
                                        }
                                    case ">=":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data > (int)selectObject || (int)queryColumn.DataList[i].Data == (int)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data > (double)selectObject || (double)queryColumn.DataList[i].Data == (double)selectObject)
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            return;
                                        }
                                }
                            else throw new ArgumentException("Invalid object for select!");
                        }
                        else { if (selectOperator.isSelectComplexOperator()) throw new ArgumentException(selectOperator + " is complex operator, you need more selectObjects to use it!"); else throw new ArgumentException("Invalid operator"); }
                    }
                    else throw new ArgumentException("There is no " + columnName + " column in " + tableForSelection.Name + " table");
                }
                else return;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                return;
            }
        }

        /// <summary>
        /// condition update for complex operators
        /// </summary>
        /// <param name="tableForSelection"></param>
        /// <param name="columnName"></param>
        /// <param name="selectOperator"></param>
        /// <param name="selectObjects"></param>
        /// <param name="outResult"></param>
        /// <returns></returns>
        public void QueryWhereConditionUpdate(Table tableForSelection, string columnName, string selectOperator, object[] selectObjects, string[] columnsName, object[] values, ref string outResult)
        {
            if (!outResult.isStatusCodeOk()) return;
            try
            {
               
                if (tableForSelection.isTableContainsData())
                {
                    if (tableForSelection.isColumnExists(columnName))
                    {
                        Column queryColumn = tableForSelection.GetColumnByName(columnName);
                        List<Column> columns = new List<Column>();
                        if (columnsName.Length != values.Length) throw new ArgumentException("Names of columns to update isn't similar to count of values");
                        for (int i = 0; i < columnsName.Length; i++)
                        {
                            columns.Add(tableForSelection.GetColumnByName(columnsName[i]));
                        }
                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (columns[i].DataType != values[i].GetType()) throw new ArgumentException("One of the arguments type is not similar with type of column");
                        }
                        if (selectOperator.isSelectComplexOperator())
                        {
                            Type typeColumn = queryColumn.DataType;
                            if (selectObjects.Length == 0) throw new ArgumentException("There is no objects to select!");
                            if (selectObjects.IsArrayContainOnlyTValues(typeColumn))
                            {
                                switch (selectOperator)
                                {
                                    case "BETWEEN":
                                        {
                                            if (typeColumn == typeof(string)) throw new ArgumentException("String value is not compatible with BETWEEN operator!");
                                            if (typeColumn == typeof(bool)) throw new ArgumentException("Bool value is not compatible with BETWEEN operator!");

                                            Array.Sort(selectObjects);
                                            if (selectObjects.Length != 2) throw new ArgumentException("Complex select operator " + selectOperator + " works only with 2 objects for selection!");

                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data > (int)selectObjects[0] && (int)queryColumn.DataList[i].Data < (int)selectObjects[1])
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data > (double)selectObjects[0] && (double)queryColumn.DataList[i].Data < (double)selectObjects[1])
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            return;
                                        }
                                    case "IN":
                                        {
                                            Array.Sort(selectObjects);
                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((int)queryColumn.DataList[i].Data))
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((double)queryColumn.DataList[i].Data))
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((string)queryColumn.DataList[i].Data))
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((bool)queryColumn.DataList[i].Data))
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            return;
                                        }
                                    case "NOT_BETWEEN":
                                        {
                                            if (typeColumn == typeof(string)) throw new ArgumentException("String value is not compatible with NOT_BETWEEN operator!");
                                            if (typeColumn == typeof(bool)) throw new ArgumentException("Bool value is not compatible with NOT_BETWEEN operator!");

                                            Array.Sort(selectObjects);
                                            if (selectObjects.Length != 2) throw new ArgumentException("Complex select operator " + selectOperator + "works only with 2 objects for selection!");
                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data <= (int)selectObjects[0] || (int)queryColumn.DataList[i].Data >= (int)selectObjects[1])
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data <= (double)selectObjects[0] && (double)queryColumn.DataList[i].Data >= (double)selectObjects[1])
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            return;
                                        }
                                    case "NOT_IN":
                                        {
                                            Array.Sort(selectObjects);
                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((int)queryColumn.DataList[i].Data))
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((double)queryColumn.DataList[i].Data))
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((string)queryColumn.DataList[i].Data))
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((bool)queryColumn.DataList[i].Data))
                                                    {
                                                        for (int j = 0; j < columns.Count; j++)
                                                            columns[j].EditColumnElementByIndex(i,values[j].toObjectArray());
                                                    }
                                                }
                                            }
                                            return;
                                        }
                                }

                            }
                            else throw new ArgumentException("Invalid objects for select!");

                        }
                        else { if (selectOperator.isSelectComplexOperator()) throw new ArgumentException(selectOperator + " is complex operator, you need more selectObjects to use it!"); else throw new ArgumentException("Invalid operator"); }
                    }
                    else throw new ArgumentException("There is no " + columnName + " column in " + tableForSelection.Name + " table");
                }
                else return;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                return;
            }
        }

        //for delete
        /// <summary>
        /// Condition update
        /// </summary>
        /// <param name="tableForSelection"></param>
        /// <param name="columnName"></param>
        /// <param name="selectOperator"></param>
        /// <param name="selectObject"></param>
        /// <returns></returns>
        public void QueryWhereConditionDelete(Table tableForSelection, string columnName, string selectOperator, object selectObject, ref string outResult)
        {
            if (!outResult.isStatusCodeOk()) return;
            try
            {

                if (tableForSelection.isTableContainsData())
                {
                    if (tableForSelection.isColumnExists(columnName))
                    {
                        Column queryColumn = tableForSelection.GetColumnByName(columnName);
                        if (selectOperator.isSelectOperator())
                        {
                            if (queryColumn.DataType == selectObject.GetType())
                                switch (selectOperator)
                                {
                                    case "=":
                                        {
                                            if (selectObject.GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                    if ((string)queryColumn.DataList[i].Data == (string)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                            }
                                            else if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data == (int)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data == (double)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObject.GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((bool)queryColumn.DataList[i].Data == (bool)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return;
                                        }
                                    case "!=":
                                        {
                                            if (selectObject.GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                    if ((string)queryColumn.DataList[i].Data != (string)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                            }
                                            else if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data != (int)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data != (double)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObject.GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((bool)queryColumn.DataList[i].Data != (bool)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return;
                                        }
                                    case ">":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data > (int)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data > (double)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return;
                                        }
                                    case "<":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data < (int)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data < (double)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return;
                                        }
                                    case "<=":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data < (int)selectObject || (int)queryColumn.DataList[i].Data == (int)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data < (double)selectObject || (double)queryColumn.DataList[i].Data == (double)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return;
                                        }
                                    case ">=":
                                        {
                                            if (selectObject.GetType() == typeof(string)) throw new ArgumentException("String type is not compatible with " + selectOperator + " operator!");
                                            if (selectObject.GetType() == typeof(bool)) throw new ArgumentException("Bool type is not compatible with " + selectOperator + " operator!");

                                            Type type = selectObject.GetType();
                                            if (selectObject.GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data > (int)selectObject || (int)queryColumn.DataList[i].Data == (int)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObject.GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data > (double)selectObject || (double)queryColumn.DataList[i].Data == (double)selectObject)
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return;
                                        }
                                }
                            else throw new ArgumentException("Invalid object for select!");
                        }
                        else { if (selectOperator.isSelectComplexOperator()) throw new ArgumentException(selectOperator + " is complex operator, you need more selectObjects to use it!"); else throw new ArgumentException("Invalid operator"); }
                    }
                    else throw new ArgumentException("There is no " + columnName + " column in " + tableForSelection.Name + " table");
                }
                else return;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                return;
            }
        }

        /// <summary>
        /// condition update for complex operators
        /// </summary>
        /// <param name="tableForSelection"></param>
        /// <param name="columnName"></param>
        /// <param name="selectOperator"></param>
        /// <param name="selectObjects"></param>
        /// <param name="outResult"></param>
        /// <returns></returns>
        public void QueryWhereConditionDelete(Table tableForSelection, string columnName, string selectOperator, object[] selectObjects, ref string outResult)
        {
            if (!outResult.isStatusCodeOk()) return;
            try
            {

                if (tableForSelection.isTableContainsData())
                {
                    if (tableForSelection.isColumnExists(columnName))
                    {
                        Column queryColumn = tableForSelection.GetColumnByName(columnName);
                        if (selectOperator.isSelectComplexOperator())
                        {
                            Type typeColumn = queryColumn.DataType;
                            if (selectObjects.Length == 0) throw new ArgumentException("There is no objects to select!");
                            if (selectObjects.IsArrayContainOnlyTValues(typeColumn))
                            {
                                switch (selectOperator)
                                {
                                    case "BETWEEN":
                                        {
                                            if (typeColumn == typeof(string)) throw new ArgumentException("String value is not compatible with BETWEEN operator!");
                                            if (typeColumn == typeof(bool)) throw new ArgumentException("Bool value is not compatible with BETWEEN operator!");

                                            Array.Sort(selectObjects);
                                            if (selectObjects.Length != 2) throw new ArgumentException("Complex select operator " + selectOperator + " works only with 2 objects for selection!");

                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((int)queryColumn.DataList[i].Data > (int)selectObjects[0] && (int)queryColumn.DataList[i].Data < (int)selectObjects[1])
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if ((double)queryColumn.DataList[i].Data > (double)selectObjects[0] && (double)queryColumn.DataList[i].Data < (double)selectObjects[1])
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return;
                                        }
                                    case "IN":
                                        {
                                            Array.Sort(selectObjects);
                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((int)queryColumn.DataList[i].Data))
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((double)queryColumn.DataList[i].Data))
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((string)queryColumn.DataList[i].Data))
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (selectObjects.IsArrayContainThisValue((bool)queryColumn.DataList[i].Data))
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return;
                                        }
                                    case "NOT_BETWEEN":
                                        {
                                            if (typeColumn == typeof(string)) throw new ArgumentException("String value is not compatible with NOT_BETWEEN operator!");
                                            if (typeColumn == typeof(bool)) throw new ArgumentException("Bool value is not compatible with NOT_BETWEEN operator!");

                                            Array.Sort(selectObjects);
                                            if (selectObjects.Length != 2) throw new ArgumentException("Complex select operator " + selectOperator + "works only with 2 objects for selection!");
                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (((int)queryColumn.DataList[i].Data <= (int)selectObjects[0] || (int)queryColumn.DataList[i].Data >= (int)selectObjects[1]))
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (((double)queryColumn.DataList[i].Data <= (double)selectObjects[0] || (double)queryColumn.DataList[i].Data >= (double)selectObjects[1]))
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return;
                                        }
                                    case "NOT_IN":
                                        {
                                            Array.Sort(selectObjects);
                                            if (selectObjects[0].GetType() == typeof(int))
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((int)queryColumn.DataList[i].Data))
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            else if (selectObjects[0].GetType() == typeof(double))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((double)queryColumn.DataList[i].Data))
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(string))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((string)queryColumn.DataList[i].Data))
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            else if (selectObjects[0].GetType() == typeof(bool))
                                            {
                                                for (int i = 0; i < queryColumn.DataList.Count; i++)
                                                {
                                                    if (!selectObjects.IsArrayContainThisValue((bool)queryColumn.DataList[i].Data))
                                                    { tableForSelection.DeleteTableElementByIndex(i); i--; }
                                                }
                                            }
                                            return;
                                        }
                                }

                            }
                            else throw new ArgumentException("Invalid objects for select!");

                        }
                        else { if (selectOperator.isSelectComplexOperator()) throw new ArgumentException(selectOperator + " is complex operator, you need more selectObjects to use it!"); else throw new ArgumentException("Invalid operator"); }
                    }
                    else throw new ArgumentException("There is no " + columnName + " column in " + tableForSelection.Name + " table");
                }
                else return ;
            }
            catch (Exception e)
            {
                outResult = "Critical Error: " + e.Message;
                return;
            }
        }

        /// <summary>
        /// Inner joins of tables by two columns
        /// </summary>
        /// <param name="firstTableToJoin"></param>
        /// <param name="secondTableToJoin"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public Table QueryInnerJoinSelection(Table firstTableToJoin, Table secondTableToJoin, List<string> ColumnNames, ref string outResult)
        {
            if (!outResult.isStatusCodeOk()) return null;

                Table queryFirstTable = new Table(firstTableToJoin);
                Table querySecondTable = new Table(secondTableToJoin);
                if (ColumnNames.Count != 2) throw new ArgumentException("The number of columns to join must be 2!");
                Column queryColumnFirst=null;
                Column queryColumnSecond=null;
                for (int i = 0; i < ColumnNames.Count; i++)
                {
                    if (queryFirstTable.isColumnExists(ColumnNames[i])) { if (queryColumnFirst == null) queryColumnFirst = new Column(queryFirstTable.GetColumnByName(ColumnNames[i]), queryFirstTable); }
                    else if (querySecondTable.isColumnExists(ColumnNames[i])) { if(queryColumnSecond==null) queryColumnSecond = new Column(querySecondTable.GetColumnByName(ColumnNames[i]), querySecondTable); }
                    else throw new ArgumentException("There is no column " + ColumnNames[i] + " in both tables!");
                }
                if (queryColumnFirst.DataType != queryColumnSecond.DataType) throw new ArgumentException("DataType of columns is not similar!");
                //takes all data from first table
                List<List<object>> dataFirstTable = new List<List<object>>();
                for (int i = 0; i < queryFirstTable.Columns[0].DataList.Count; i++)
                {
                    DataObject[] buf = queryFirstTable.GetDataByIndex(i);
                    List<object> bufData = new List<object>();
                    for (int j = 0; j < buf.Length; j++)
                        bufData.Add(buf[j].Data);
                    dataFirstTable.Add(bufData);
                }
                //takes all data from second table
                List<List<object>> dataSecondTable = new List<List<object>>();
                for (int i = 0; i < querySecondTable.Columns[0].DataList.Count; i++)
                {
                    DataObject[] buf = querySecondTable.GetDataByIndex(i);
                    List<object> bufData = new List<object>();
                    for (int j = 0; j < buf.Length; j++)
                        bufData.Add(buf[j].Data);
                    dataSecondTable.Add(bufData);
                }
                List<List<object>> finalData = new List<List<object>>();
                int indexOfFirstTableColumn = queryFirstTable.getIndexOfColumn(queryColumnFirst.Name);
                int indexOfSecondTableColumn = querySecondTable.getIndexOfColumn(queryColumnSecond.Name);
           
                for (int i = 0; i < dataFirstTable.Count; i++)
                {
                    object bufSimilar = dataFirstTable[i][indexOfFirstTableColumn];
                    for (int j = 0; j < dataSecondTable.Count; j++)
                    {
                    if (dataSecondTable[j][indexOfSecondTableColumn].Equals(bufSimilar))
                    {
                        List<object> newRow = new List<object>();
                        for (int l = 0; l < dataFirstTable[i].Count; l++)
                        {
                            newRow.Add(dataFirstTable[i][l]);
                        }
                        for (int p = 0; p < dataSecondTable[j].Count; p++)
                            newRow.Add(dataSecondTable[j][p]);
                        finalData.Add(newRow);
                    }
                }
                }
            queryFirstTable.DeleteAllData();
            querySecondTable.DeleteAllData();
            for (int i = 0; i < querySecondTable.Columns.Count; i++)
                    queryFirstTable.Columns.Add(new Column(querySecondTable.Columns[i], queryFirstTable));
            for (int i = 0; i < finalData.Count; i++)
                {
                    List<object> currentRow = finalData[i];
                    for (int j = 0; j < queryFirstTable.Columns.Count; j++)
                    {
                    DataObject k = new DataObject(queryFirstTable.Columns[j].GetHashCode(), currentRow[j]);
                        queryFirstTable.Columns[j].DataList.Add(k);
                    }
                }
                return queryFirstTable;
           

        }
    }
    
}
