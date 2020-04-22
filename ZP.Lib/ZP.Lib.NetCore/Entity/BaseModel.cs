using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ZP.Lib;
using ZP.Lib.Server.SQL;
using ZP.Lib.NetCore.Domain;
using ZP.Lib.Server.Domain;
using ZP.Lib.NetCore.Entity;

namespace ZP.Lib.Web
{
    public class BaseModel : IDisposable
    {
        protected string connectStr;

        protected ISQLConnection sqlCtl;

        protected Type engintType;
        public BaseModel(IConfiguration configuration)
        {
            var engine = (this.GetType().GetCustomAttributes(typeof(SQLEngineAttribute), true)?.FirstOrDefault() as SQLEngineAttribute)
                ?.EngineType ?? typeof(ZPropertyMysql);

            if (engine == typeof(ZPropertyMysql))
                connectStr = configuration.GetConnectionString("mysql");
            else if (engine == typeof(ZPropertySQLite))
                connectStr = configuration.GetConnectionString("sqlite3");

            engintType = engine;
        }

        public void Connect()
        {
            if (sqlCtl == null)
            {
                sqlCtl = CreateCtrl();

                sqlCtl.Connect();
            }
        }

        protected ISQLConnection CreateCtrl()
        {
            ISQLConnection ret = null;
            if (engintType == typeof(ZPropertyMysql))
                ret = new ZPropertyMysql(connectStr);
            else if (engintType == typeof(ZPropertySQLite))
                ret = new ZPropertySQLite(connectStr);

            return ret;
        }

        public void Disconnect()
        {
            sqlCtl?.Disconnect();
            sqlCtl = null;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }

    public class BaseModel<TModeDatal> : BaseModel , ICreatableModel
        where TModeDatal : class
    {
        readonly protected string DbName = "";
        readonly protected string TableName = "";

        readonly protected string fullTableName = "";
        public string FullTableName => fullTableName;// $"{DbName}.{TableName}";

        public string MainIndexName => $"{TableName[0].ToString().ToLower()}pid";

        public BaseModel(IConfiguration configuration, string dbName) : base(configuration)
        {
            this.DbName = dbName;

            TableName = this.GetType().Name.Replace("Model", "");

            fullTableName = string.IsNullOrEmpty(dbName) || engintType == typeof(ZPropertySQLite) ? TableName : $"{DbName}.{TableName}";
        }

        public void CheckOrCreateDB()
        {
            using (ISQLConnection sql = CreateCtrl())
            {
                sql.Connect();

                if (!sql.HasTable(DbName, TableName))
                {
                    sql.CreateDB(DbName);

                    sql.CreateTable<TModeDatal>($"{TableName}");//{DbName}.
                }
            }
        }

        public bool Add(TModeDatal model)
        {
            var index = sqlCtl.Insert<TModeDatal>(fullTableName, model);

            var primaryIndex = ZPropertyMysql.GetPrimaryIndexProperty(model);
            if (primaryIndex != null)
            {
                primaryIndex.Value = index;
                return index >= 0;
            }

            return index >= 0;
        }

        public TModeDatal Get(uint index)
        {
            var model = sqlCtl.QueryOne<TModeDatal>(fullTableName, index);
            //user.Id.Value = index;

            return model;
        }

        public TModeDatal Get(string propertyId, string value)
        {
            string strsql =
                $"SELECT * FROM {FullTableName} WHERE `{propertyId}`={value} Limit 1";

            return sqlCtl.Query<TModeDatal>(strsql)?.ToList()?.FirstOrDefault();

            //return model;
        }

        public bool Update(uint index, ZListUpdater updateInfo)
        {
            return sqlCtl.Update<TModeDatal>(FullTableName, index, updateInfo);
        }

        public bool Update(uint index, List<UpdateItem> updateItems)
        {
            return sqlCtl.Update<TModeDatal>(FullTableName, index, updateItems);
        }

        public bool Update(TModeDatal data)
        {
            return sqlCtl.Update<TModeDatal>(FullTableName, data);
        }

        public bool Update(uint index, TModeDatal data)
        {
            return sqlCtl.Update<TModeDatal>(FullTableName, MainIndexName, index, data);
        }

        public bool Update(TModeDatal data, string propID)
        {
            return sqlCtl.Update<TModeDatal>(FullTableName, data, propID);
        }

        public bool Update(TModeDatal data, uint linkedIndex)
        {
            return sqlCtl.Update<TModeDatal>(FullTableName, linkedIndex, data);
        }

        public bool Delete(TModeDatal data)
        {
            return sqlCtl.Delete(FullTableName, data);
        }

        public bool Delete(uint mainIndex)
        {
            string strsql =
                $"DELETE FROM {FullTableName} WHERE `{MainIndexName}`={mainIndex}";

           return sqlCtl.Execute(strsql) > 0;
        }

        public void Delete(string propertyId, string value)
        {
            string strsql =
                $"DELETE FROM {FullTableName} WHERE `{propertyId}`={value}";

            sqlCtl.Execute(strsql);
        }
    }
}
