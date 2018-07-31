using Bot.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Bot.Extensions.MySql
{
    public class MySqlWrapper
    {
        public static List<string> MakeQuery(string query)
        {
            var dbCon = DBConnection.Instance();
            MySqlConfig config = FileIO.ReadConfigJson(new MySqlConfig());
            
            dbCon.databaseName = config.databaseName;
            dbCon.login = config.login;
            dbCon.password = config.password;
            List<string> result = new List<string>();
            if (dbCon.IsConnect())
            {
                //suppose col0 and col1 are defined as VARCHAR in the DB
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString("Name"));

                }
                dbCon.Close();
            }

            return result;
        }
    }
}