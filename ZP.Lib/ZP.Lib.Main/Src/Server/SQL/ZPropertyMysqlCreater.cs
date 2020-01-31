using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


#if ZP_SERVER
using MySql.Data.MySqlClient;
using UnityEngine;
using ZP.Lib;

namespace ZP.Lib.Server.SQL
{
    public sealed partial class ZPropertyMysql
    {
        public void CreateTable<TData>(string TableName)
        {
            TData sample = ZPropertyMesh.CreateObject<TData>();

            CreateTable<TData>(TableName, sample);
        }

        public void CreateTable<TData>(string TableName, TData sample)
        {
            //TODO
            //uint id = 0;
            StringBuilder sCommand = new StringBuilder();
            sCommand.Append($"CREATE TABLE {TableName} (");

            List<IZProperty> props = ZPropertyMesh.GetProperties(sample);

            if (props.Count <= 0)
                return;

            var mainIndexStr = $"{TableName[0].ToString().ToLower()}pid";

            //cpid for card link player
            sCommand.Append($"`{mainIndexStr}` bigint(20) NOT NULL AUTO_INCREMENT,");

            foreach (var p in props)
            {
                if (p.AttributeNode.GetAttribute<DBIndexAttribute>()?.IsPrimary == true)
                {
                    //primary index is mainIndexStr
                    continue;
                }
                //`PlayerCardLink.Blood.Max` float DEFAULT NULL,
                var items = GetColumnItem(p);
                //if (string.IsNullOrEmpty(typ))
                //    continue;

                if (items.Count <= 0)
                    continue;

                foreach (var c in items)
                {
                    sCommand.Append($"`{c.columnName}` {c.typeStr} DEFAULT NULL,");
                }
            }

            var indexStr = GetLinkedIndexId(typeof(TData));
            if (!string.IsNullOrEmpty(indexStr) && string.Compare(indexStr, mainIndexStr, true) != 0)
            {
                sCommand.Append($"`{indexStr}` bigint(20) DEFAULT NULL,");
            }

            sCommand.Append($"PRIMARY KEY(`{TableName[0].ToString().ToLower()}pid`)");

            sCommand.Append(") ENGINE=InnoDB AUTO_INCREMENT = 0 DEFAULT CHARSET = utf8");
            using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mySqlConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }
        }

        public void CreateDB(string dbName)
        {
            string sql = $"CREATE DATABASE IF NOT EXISTS {dbName} DEFAULT CHARSET utf8 COLLATE utf8_general_ci;";
            Execute(sql);

            Execute("USE " + dbName);
        }

        public bool HasTable(string dbName, string tableName)
        {
            var list = ShowDBs();
            if (!list.Contains(dbName))
                return false;

            Execute("USE " + dbName);

            string sql = "SHOW TABLES;";

            using (MySqlDataReader reader = QueryRaw(sql))
            {
                //读取数据
                while (reader.Read())
                {
                    var name = reader.GetString("Tables_in_" + dbName) ;
                    if (string.Compare(tableName, name, true) == 0)
                        return true;
                }
            }
            return false;
        }

        public List<string> ShowDBs()
        {
            string sql = "SHOW DATABASES;";
            List<string> rets = new List<string>();
            using (MySqlDataReader reader = QueryRaw(sql))
            {
                //读取数据
                while (reader.Read())
                {
                    var dbname = reader.GetString("Database");
                    if (string.Compare(dbname, "performance_schema") == 0)
                        continue;

                    if (string.Compare(dbname, "information_schema") == 0)
                        continue;

                    if (string.Compare(dbname, "sys") == 0)
                        continue;
                    if (string.Compare(dbname, "innodb") == 0)
                        continue;
                    if (string.Compare(dbname, "mysql") == 0)
                        continue;

                    rets.Add(dbname);
                }
            }

            return rets;
        }



        //        CREATE TABLE `CardLink` (
        //  `cpid` bigint(20) NOT NULL AUTO_INCREMENT,
        //  `PlayerCardLink.CardRef.refID` int (11) DEFAULT NULL,
        //   `PlayerCardLink.Blood.Max` float DEFAULT NULL,
        //  `PlayerCardLink.Blood.Cur` float DEFAULT NULL,
        //  `PlayerCardLink.RepairCost` bigint(20) DEFAULT NULL,
        //  `PlayerCardLink.CreateTime` datetime DEFAULT NULL,
        //  `PlayerCardLink.Exp.CurRank` int (11) DEFAULT NULL,
        //   `PlayerCardLink.Exp.CurExp` float DEFAULT NULL,
        //  `Count.Cur` int (11) DEFAULT NULL,
        //   `PlayerID` bigint(20) DEFAULT NULL,
        //   PRIMARY KEY(`cpid`)
        //) ENGINE=InnoDB AUTO_INCREMENT = 3 DEFAULT CHARSET = utf8
    }
}

#endif
