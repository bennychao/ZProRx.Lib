using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ZP.Lib;
using ZP.Lib.Server.SQL;
using ZP.Lib.NetCore.Domain;

namespace ZP.Lib.Web
{
    public class BaseModel : IDisposable
    {
        protected string connectStr;

        protected ZPropertyMysql sqlCtl;


        public BaseModel(IConfiguration configuration)
        {
            connectStr = configuration.GetConnectionString("mysql");
        }

        public void Connect()
        {
            if (sqlCtl == null)
            {
                sqlCtl = new ZPropertyMysql(connectStr);
                sqlCtl.Connect();
            }
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

        public string FullTableName => $"{DbName}.{TableName}";

        public BaseModel(IConfiguration configuration, string dbName) : base(configuration)
        {
            this.DbName = dbName;
            TableName = this.GetType().Name.Replace("Model", "");
        }

        public void CheckOrCreateDB()
        {
            using (ZPropertyMysql sql = new ZPropertyMysql(connectStr))
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
            var index = sqlCtl.Insert<TModeDatal>($"{DbName}.{TableName}", model);

            var primaryIndex = ZPropertyMysql.GetPrimaryIndexProperty(model);
            if (primaryIndex != null)
            {
                primaryIndex.Value = index;
                return index >= 0;
            }

            return false;
        }

        public TModeDatal Get(uint index)
        {
            var model = sqlCtl.QueryOne<TModeDatal>($"{DbName}.{TableName}", index);
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

        public void Delete(string propertyId, string value)
        {
            string strsql =
                $"DELETE FROM {FullTableName} WHERE `{propertyId}`={value}";

            sqlCtl.Execute(strsql);
        }
    }
}
