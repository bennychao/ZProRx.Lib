using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using ZP.Lib.Server.Domain;


#if ZP_SERVER
using Microsoft.Data.Sqlite;
using UnityEngine;
using ZP.Lib;

namespace ZP.Lib.Server.SQL
{
     public sealed partial class ZPropertySQLite : IDisposable, ISQLConnection
    {
        public string ConnectionString { get; set; }


        SqliteConnection  sqLiteConnection;

        public ZPropertySQLite(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }

        public void Connect()
        {
            sqLiteConnection = GetConnection();

            sqLiteConnection.Open();
        }

        public void Disconnect()
        {
            sqLiteConnection?.Close();
            sqLiteConnection = null;
        }

        public List<T> Query<T>(string sql)// where T : IZProperty 
        {
            List<T> list = new List<T>();

            //查找数据库里面的表
            SqliteCommand mscommand = new SqliteCommand(sql, sqLiteConnection);
            using (SqliteDataReader reader = mscommand.ExecuteReader())
            {
                //读取数据
                while (reader.Read())
                {
                    var obj = ZPropertyMesh.CreateObject<T>();
                    ConvertObject(obj, reader);

                    list.Add(obj);

                    ZPropertyMesh.InvokeLoadMethod(obj);
                }
            }
                
            return list;
        }

        public List<T> QueryObjects<T>(string stable)
        {
            List<T> rets = new List<T>();
            var obj = ZPropertyMesh.CreateObject<T>();

            IZProperty pIndex = GetIndexProperty(obj);

            if (pIndex == null)
                return rets;

            StringBuilder sCommand = new StringBuilder();

            sCommand.Append("SELECT ");

            sCommand.Append(GetColumnProperties(obj));

            sCommand.Append(" ");
            sCommand.Append("\nFROM " + stable);

            SqliteCommand mscommand = new SqliteCommand(sCommand.ToString(), sqLiteConnection);
            using (SqliteDataReader reader = mscommand.ExecuteReader())
            {
                //读取数据
                while (reader.Read())
                {
                    ConvertObject(obj, reader);
                    ZPropertyMesh.InvokeLoadMethod(obj);
                    rets.Add(obj);
                }
            }

            return rets;
        }

        public List<T> QueryObjects<T>(string stable, uint linkedIndex)
        {
            List<T> rets = new List<T>();

            var obj = ZPropertyMesh.CreateObject<T>();

            //IZProperty pIndex = GetIndexProperty(obj);

            //if (pIndex == null)
            //    return rets;

            StringBuilder sCommand = new StringBuilder();

            sCommand.Append("SELECT ");

            sCommand.Append(GetColumnProperties(obj));

            var linkId = GetLinkedIndexId(obj.GetType());

            sCommand.Append(" ");
            sCommand.Append("\nFROM " + stable);
            sCommand.Append($"\nWHERE {linkId}={linkedIndex}");

            SqliteCommand mscommand = new SqliteCommand(sCommand.ToString(), sqLiteConnection);
            using (SqliteDataReader reader = mscommand.ExecuteReader())
            {
                //读取数据
                while (reader.Read())
                {
                    ConvertObject(obj, reader);
                    ZPropertyMesh.InvokeLoadMethod(obj);
                    //return obj;
                    rets.Add(obj);
                }
            }

            return rets;
        }

        public T QueryOne<T>(string stable, object value) {
            var obj = ZPropertyMesh.CreateObject<T>();

            IZProperty pIndex = GetIndexProperty(obj);

            if (pIndex == null)
                return default(T);

            StringBuilder sCommand = new StringBuilder();

            sCommand.Append("SELECT ");

            sCommand.Append(GetColumnProperties(obj));

            sCommand.Append(" ");
            sCommand.Append("\nFROM " + stable);

            sCommand.Append("\nWHERE ");

            sCommand.Append("`" + GetColumnName( pIndex) + "`" + "=" + value.ToString() + ";");

            SqliteCommand mscommand = new SqliteCommand(sCommand.ToString(), sqLiteConnection);
            using (SqliteDataReader reader = mscommand.ExecuteReader())
            {
                //读取数据
                if (reader.Read())
                {
                    ConvertObject(obj, reader);
                    ZPropertyMesh.InvokeLoadMethod(obj);
                    return obj;
                }
            }

            return default(T);
        }

        public int QueryCount(string sql)
        {
            using (SqliteDataReader reader = QueryRaw(sql))
            {
                if (reader.Read())
                {
                   return reader.GetInt32(reader.GetOrdinal("count"));
                }
            }

            return 0;
        }


        public SqliteDataReader QueryRaw(string sql)
        {
            SqliteCommand mscommand = new SqliteCommand(sql, sqLiteConnection);
            return mscommand.ExecuteReader();
        }

        

        //return id
        public uint Insert<T>(string table, T obj)
        {
            //uint id = 0;
            StringBuilder sCommand = new StringBuilder();
            sCommand.Append("INSERT INTO " + table);

            List<IZProperty> props = ZPropertyMesh.GetProperties(obj);

            if (props.Count <= 0)
                return 0;

            string rows = " (";
            string datas = "(";
            foreach (var p in props)
            {
                //auto ++ 
                if (IsPrimaryIndex(p))
                    continue;

                var values = GetColumnIValues(p);
                foreach (var v in values)
                {
                    rows += "`" +  v.columnName + "`,";
                    datas += v.valueStr + ",";
                }
            }

            rows = rows.Remove(rows.Length - 1);   //remove the last ","
            datas= datas.Remove(datas.Length - 1);

            rows += ") VALUES ";
            datas += ")";

            sCommand.Append(rows + datas);

            
            using (SqliteCommand myCmd = new SqliteCommand(sCommand.ToString(), sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();

                
            }
            //var newIndex = sqLiteConnection.LastInsertRowId;
            var newIndex = QueryNewIndexId(table);// sqLiteConnection.las

            return (uint)newIndex;// QueryNewIndexId(table);
        }

        public uint Insert<T>(string table, uint linkIndex, T obj)
        {
            //uint id = 0;
            StringBuilder sCommand = new StringBuilder();
            sCommand.Append("INSERT INTO " + table);

            List<IZProperty> props = ZPropertyMesh.GetProperties(obj);

            if (props.Count <= 0)
                return 0;

            string rows = " (";
            string datas = "(";
            foreach (var p in props)
            {
                //auto ++ 
                if (IsPrimaryIndex(p))
                    continue;

                var values = GetColumnIValues(p);
                foreach (var v in values)
                {
                    rows += "`" + v.columnName + "`,";
                    datas += v.valueStr + ",";
                }

                //if (ZPropertyMesh.IsRawDataRef(p))
                //{
                //    rows += GetColumnName(p) + ",";
                //    datas += ZPropertyPrefs.ConvertToStr(p.Value);
                //}
                //else
            }

            //set linKed index
            rows += GetLinkedIndexId(obj.GetType());
            datas += linkIndex;


            //rows.Remove(rows.Length - 1);   //remove the last ","
            //datas.Remove(rows.Length - 1);

            rows += ") VALUES ";
            datas += ")";

            sCommand.Append(rows + datas);
            using (SqliteCommand myCmd = new SqliteCommand(sCommand.ToString(), sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }
            

            var newIndex = QueryNewIndexId(table);// sqLiteConnection.LastInsertRowId;

            return (uint)newIndex;// QueryNewIndexId(table);
        }

        public void Insert<T>(string table, List<T> list)
        {
            //TODO
            //https://q.cnblogs.com/q/105283
        }

        public uint QueryNewIndexId(string table)
        {
            uint ret = 0;
            string strsql = $"SELECT last_insert_rowid() as newId;";
            using (SqliteDataReader reader = QueryRaw(strsql))
            {
                if (reader.Read())
                {
                    ret = (uint)reader.GetInt32(reader.GetOrdinal("newId"));
                }
            }

            return ret;
        }

        public bool Update<T>(string table, T obj)
        {
            //            UPDATE gegefly2.Box
            //SET Bid = 0, Uid = 0, `rank`= 0, OpenTime = '', Status = 0, NextValidTime = '', `Test.ID`= ''
            //WHERE UBid = 0;

            StringBuilder sCommand = new StringBuilder();
            sCommand.Append("UPDATE " + table + " SET ");

            List<IZProperty> props = ZPropertyMesh.GetProperties(obj);

            if (props.Count <= 0)
                return false;

            IZProperty pIndex = null;

            foreach (var p in props)
            {
                var values = GetColumnIValues(p);
                foreach (var v in values)
                {
                    sCommand.Append("`" + v.columnName + "`=");
                    sCommand.Append(v.valueStr);
                    sCommand.Append(",");
                }

                if (p.AttributeNode.IsDefined<DBIndexAttribute>())
                {
                    pIndex = p;
                }
            }

            //remove ","
            sCommand.Remove(sCommand.Length - 1, 1);

            sCommand.Remove(sCommand.Length - 1, 1);
            sCommand.Append(" WHERE `" + GetColumnName(pIndex) + "`=");
            sCommand.Append(pIndex.Value);

            bool bRet = false;

            using (SqliteCommand myCmd = new SqliteCommand(sCommand.ToString(), sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = sCommand.ToString();
                bRet = myCmd.ExecuteNonQuery() > 0;
            }
            return bRet;
        }

        public bool Update<T>(string table, string indexName, uint Index, T obj)
        {

            StringBuilder sCommand = new StringBuilder();
            sCommand.Append("UPDATE " + table + " SET ");

            List<IZProperty> props = ZPropertyMesh.GetProperties(obj);

            if (props.Count <= 0)
                return false;

            IZProperty pIndex = null;

            foreach (var p in props)
            {
                var values = GetColumnIValues(p);
                foreach (var v in values)
                {
                    sCommand.Append("`" + v.columnName + "`=");
                    sCommand.Append(v.valueStr);

                    sCommand.Append(",");
                }
                //sCommand.Append("`" + GetColumnName(p) + "`=");
                //sCommand.Append(p.Value);
                //sCommand.Append(",");

                if (p.AttributeNode.IsDefined<DBIndexAttribute>())
                {
                    pIndex = p;
                }
            }
            //remove ","
            sCommand.Remove(sCommand.Length - 1, 1);

            //sCommand.Remove(sCommand.Length - 1, 1);
            sCommand.Append(" WHERE `" + indexName + "`=");
            sCommand.Append(Index);
            if (pIndex != null)
            {
                sCommand.Append(" AND `" + GetColumnName(pIndex) + "`=");
                sCommand.Append(pIndex.Value);
            }

            bool bRet = false;
            using (SqliteCommand myCmd = new SqliteCommand(sCommand.ToString(), sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = sCommand.ToString();
                bRet = myCmd.ExecuteNonQuery() > 0;
            }
            return bRet;
        }

        public bool Update<T>(string table,uint linkedIndex, T obj)
        {
            //            UPDATE gegefly2.Box
            //SET Bid = 0, Uid = 0, `rank`= 0, OpenTime = '', Status = 0, NextValidTime = '', `Test.ID`= ''
            //WHERE UBid = 0;

            StringBuilder sCommand = new StringBuilder();
            sCommand.Append("UPDATE " + table + " SET ");

            List<IZProperty> props = ZPropertyMesh.GetProperties(obj);

            if (props.Count <= 0)
                return false;

            IZProperty pIndex = null;

            foreach (var p in props)
            {
                var values = GetColumnIValues(p);
                foreach ( var v in values)
                {
                    sCommand.Append("`" + v.columnName + "`=");
                    sCommand.Append(v.valueStr);
                    
                    sCommand.Append(",");
                }
                //sCommand.Append("`" + GetColumnName(p) + "`=");
                //sCommand.Append(p.Value);
                //sCommand.Append(",");

                if (p.AttributeNode.IsDefined<DBIndexAttribute>())
                {
                    pIndex = p;
                }
            }
            //remove ","
            sCommand.Remove(sCommand.Length - 1, 1);

            //sCommand.Remove(sCommand.Length - 1, 1);
            sCommand.Append(" WHERE `" + GetLinkedIndexId(obj.GetType()) + "`=");
            sCommand.Append(linkedIndex);
            if (pIndex != null)
            {
                sCommand.Append(" AND `" + GetColumnName(pIndex) + "`=");
                sCommand.Append(pIndex.Value);
            }

            bool bRet = false;
            using (SqliteCommand myCmd = new SqliteCommand(sCommand.ToString(), sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = sCommand.ToString();
                bRet =  myCmd.ExecuteNonQuery() > 0;
            }
            return bRet;
        }

        public bool Update<T>(string table, T obj, string propID)
        {
            StringBuilder sCommand = new StringBuilder();
            sCommand.Append("UPDATE " + table + " SET ");

            var p = ZPropertyMesh.GetProperty(obj, propID);

            var values = GetColumnIValues(p);

            if (values.Count <= 0)
                return false;

            foreach (var v in values)
            {
                sCommand.Append("`" + v.columnName + "`=");
                sCommand.Append(v.valueStr);
                sCommand.Append(",");
            }

            //remove ","
            sCommand.Remove(sCommand.Length - 1, 1);

            //sCommand.Append("`" + propID + "`=");
            //sCommand.Append(p.Value);

            IZProperty pIndex = GetIndexProperty(obj);

            sCommand.Append(" WHERE `" + GetColumnName(pIndex) + "`=");
            sCommand.Append(pIndex.Value);

            bool bRet = false;
            using (SqliteCommand myCmd = new SqliteCommand(sCommand.ToString(), sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = sCommand.ToString();
                bRet = myCmd.ExecuteNonQuery() > 0;
            }
            return bRet;
        }

        public bool Update<T>(string table, uint index, ZListUpdater listUpdater)
        {
            var updateItems = listUpdater.Items.
                ToList().
                Where(u => u.UpdateType.Value == UpdateType.Update).ToList();

            return Update<T>(table, index, updateItems);
        }

        public bool Update<T>(string table, uint index, List<UpdateItem> updateItems)
        {
            StringBuilder sCommand = new StringBuilder();
            sCommand.Append("UPDATE " + table + " SET ");

            foreach (var u in updateItems)
            {
                sCommand.Append("`" + u.SubPropID.Value + "`=");
                sCommand.Append(u.RawData.Value.RawData);
                sCommand.Append(",");
            }

            //remove ","
            sCommand.Remove(sCommand.Length - 1, 1);

            var indexId = GetIndexPropertyId(typeof(T));

            sCommand.Append(" WHERE `" + indexId + "`=");
            sCommand.Append(index);

            bool bRet = false;
            using (SqliteCommand myCmd = new SqliteCommand(sCommand.ToString(), sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = sCommand.ToString();
                bRet = myCmd.ExecuteNonQuery() > 0;
            }

            return bRet;
        }




        public bool Delete(string table, object obj)
        {
            //DELETE FROM  gegefly2.Box
            //WHERE Ubid = 0;

            StringBuilder sCommand = new StringBuilder();
            sCommand.Append("DELETE FROM " + table + " WHERE ");

            var indexP = ZPropertyMesh.GetPropertyWithAttribute<DBIndexAttribute>(obj);
            if (indexP != null)
            {
                sCommand.Append("`" + GetColumnName(indexP) + "`=");
                sCommand.Append(indexP.Value);
            }

            bool bRet = false;
            using (SqliteCommand myCmd = new SqliteCommand(sCommand.ToString(), sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = sCommand.ToString();
                bRet = myCmd.ExecuteNonQuery() > 0;
            }

            return bRet;
        }

        public bool Delete<T>(string table, uint index)
        {
            //DELETE FROM  gegefly2.Box
            //WHERE Ubid = 0;

            StringBuilder sCommand = new StringBuilder();
            sCommand.Append("DELETE FROM " + table + " WHERE ");

            var indexId = GetIndexPropertyId(typeof(T));

            sCommand.Append("`" + indexId + "`=");
            sCommand.Append(index);

            //var indexP = ZPropertyMesh.GetPropertyWithAttribute<DBIndexAttribute>(typeof(T));
            //if (indexP != null)
            //{
            //    sCommand.Append(" AND " + indexP.PropertyID + "=");
            //    sCommand.Append(indexP.Value);
            //}

            bool bRet = false;
            using (SqliteCommand myCmd = new SqliteCommand(sCommand.ToString(), sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = sCommand.ToString();
                bRet = myCmd.ExecuteNonQuery() > 0;
            }

            return bRet;
        }

        public bool Delete<T>(string table, uint index, T obj)
        {
            //DELETE FROM  gegefly2.Box
            //WHERE Ubid = 0;

            StringBuilder sCommand = new StringBuilder();
            sCommand.Append("DELETE FROM " + table + " WHERE ");



            var indexId = GetIndexPropertyId(typeof(T));

            sCommand.Append("`" + indexId + "`=");
            sCommand.Append(index);

            var indexP = ZPropertyMesh.GetPropertyWithAttribute<DBIndexAttribute>(obj);
            if (indexP != null)
            {
                sCommand.Append(" AND `" + GetColumnName(indexP) + "`=");
                sCommand.Append(indexP.Value);
            }

            bool bRet = false;

            using (SqliteCommand myCmd = new SqliteCommand(sCommand.ToString(), sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = sCommand.ToString();
                bRet = myCmd.ExecuteNonQuery() > 0;
            }

            return bRet;
        }

        public int Execute(string sql)
        {
            int ret = 0;
            using (SqliteCommand myCmd = new SqliteCommand(sql, sqLiteConnection))
            {
                myCmd.CommandType = CommandType.Text;
               ret = myCmd.ExecuteNonQuery();
            }
            return ret;
        }


        public void Dispose()
        {
            //   throw new NotImplementedException();
            Disconnect();
        }
    }
}
#endif