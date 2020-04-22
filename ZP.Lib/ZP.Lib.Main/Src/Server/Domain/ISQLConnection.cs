using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Server.Domain
{
    public interface ISQLConnection : IDisposable
    {        

        void CreateTable<TData>(string TableName);

        void CreateTable<TData>(string TableName, TData sample);

        void CreateDB(string dbName);

        bool HasTable(string dbName, string tableName);

        bool HasTable(string tableName);

        void Connect();
        void Disconnect();

        List<T> Query<T>(string sql);
        List<T> QueryObjects<T>(string stable);

        List<T> QueryObjects<T>(string stable, uint linkedIndex);

        T QueryOne<T>(string stable, object value);

        int QueryCount(string sql);

        uint Insert<T>(string table, T obj);

        uint Insert<T>(string table, uint linkIndex, T obj);

        bool Update<T>(string table, T obj);

        bool Update<T>(string table, uint linkedIndex, T obj);

        bool Update<T>(string table, string indexName, uint Index, T obj);

        bool Update<T>(string table, T obj, string propID);

        bool Update<T>(string table, uint index, ZListUpdater listUpdater);

        bool Update<T>(string table, uint index, List<UpdateItem> updateItems);

        bool Delete(string table, object obj);
        bool Delete<T>(string table, uint index);
        bool Delete<T>(string table, uint index, T obj);
        int Execute(string sql);

    }
}
