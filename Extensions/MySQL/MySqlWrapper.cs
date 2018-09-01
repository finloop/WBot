using Bot.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Bot.Extensions.Debug;

namespace Bot.Extensions.MySql
{
    public class MySqlWrapper
    {

        public static Boolean checkForSQLInjection(string userInput)
        {
            bool isSQLInjection = false;
            string[] sqlCheckList = { "--",
                                       ";--",
                                       ";",
                                       "/*",
                                       "*/",
                                        "@@",
                                        "@",
                                        "char",
                                       "nchar",
                                       "varchar",
                                       "nvarchar",
                                       "alter",
                                       "begin",
                                       "cast",
                                       "create",
                                       "cursor",
                                       "declare",
                                       "delete",
                                       "drop",
                                       "end",
                                       "exec",
                                       "execute",
                                       "fetch",
                                            "insert",
                                          "kill",
                                             "select",
                                           "sys",
                                            "sysobjects",
                                            "syscolumns",
                                           "table",
                                           "update"

                                       };

            string CheckString = userInput.Replace("'", "''");
            for (int i = 0; i <= sqlCheckList.Length - 1; i++)
            {
                if ((CheckString.IndexOf(sqlCheckList[i], StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    isSQLInjection = true;
                }
            }
            return isSQLInjection;
        }
    
    public static List<string> MakeQuery(string query, string column)
    {
        var dbCon = DBConnection.Instance();
        MySqlConfig config = FileIO.ReadConfigJson(new MySqlConfig());

        dbCon.databaseName = config.databaseName;
        dbCon.login = config.login;
        dbCon.password = config.password;
        List<string> result = new List<string>();
        if (dbCon.IsConnect())
        {
            try
            {
                //suppose col0 and col1 are defined as VARCHAR in the DB
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(column));

                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
        dbCon.Close();
        return result;
    }

    public static void MakeQuery(string query)
    {
        var dbCon = DBConnection.Instance();
        MySqlConfig config = FileIO.ReadConfigJson(new MySqlConfig());

        dbCon.databaseName = config.databaseName;
        dbCon.login = config.login;
        dbCon.password = config.password;
        List<string> result = new List<string>();
        if (dbCon.IsConnect())
        {
            try
            {
                //suppose col0 and col1 are defined as VARCHAR in the DB
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("already exists"))
                {

                }
                else
                    Log.Exception(e);
            }
        }
        dbCon.Close();
    }
}
}