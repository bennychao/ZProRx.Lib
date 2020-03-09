using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using System.Reflection;
using ZP.Lib.Common;

#if ZP_SERVER
using MySql.Data.MySqlClient;
using UnityEngine;
namespace ZP.Lib.Server.SQL
{
    public  sealed partial class ZPropertyMysql
    {

        static public bool IsPrimaryIndex(IZProperty p)
        {
            return p.AttributeNode.GetAttribute<DBIndexAttribute>()?.IsPrimary ?? false;
        }

        static public IZProperty GetIndexProperty(object obj)
        {
            return ZPropertyMesh.GetPropertyWithAttribute<DBIndexAttribute>(obj);
        }

        static public IZProperty GetPrimaryIndexProperty(object obj)
        {
            var p = ZPropertyMesh.GetPropertyWithAttribute<DBIndexAttribute>(obj);
            if (p != null && IsPrimaryIndex(p))
                return p;

            return null;
        }

        string GetColumnName(IZProperty p)
        {
            var ret = p.PropertyID;
            if (p.AttributeNode.IsDefined<DBColumnNameAttribute>())
            {
                return p.AttributeNode.GetAttribute<DBColumnNameAttribute>().Name;
            }
            if (p.AttributeNode.GetAttribute<DBIndexAttribute>()?.IsPrimary == true)
            {
                var name = p.PropertyID.TrimStart('Z');
                return name[0].ToString().ToLower() + "pid";
            }

            return ret;
        }

        public List<string> GetMultiColumnName(IZProperty p)
        {
            List<string> ret = new List<string>();
            if (p.GetDefineType() == typeof(ZIntBar))
            {
                ret.Add(GetColumnName(p) + ".Cur");
            }

            else if (p.GetDefineType() == typeof(ZDataBar))
            {
                ret.Add(GetColumnName(p) + ".Cur");
            }
            else if (p.GetDefineType() == typeof(ZExp))
            {
                ret.Add(GetColumnName(p) + ".CurRank");
                ret.Add(GetColumnName(p) + ".CurExp");
            }
            else if (p.GetDefineType() == typeof(Vector2))
            {
                ret.Add(GetColumnName(p) + ".x");
                ret.Add(GetColumnName(p) + ".y");
            }
            else if (p.GetDefineType() == typeof(Vector3))
            {
                ret.Add(GetColumnName(p) + ".x");
                ret.Add(GetColumnName(p) + ".y");
                ret.Add(GetColumnName(p) + ".z");
            }
            else if (p.GetDefineType() == typeof(ZTransform))
            {
                ret.Add(GetColumnName(p) + ".Position.x");
                ret.Add(GetColumnName(p) + ".Position.y");
                ret.Add(GetColumnName(p) + ".Direction.x");
                ret.Add(GetColumnName(p) + ".Direction.y");
            }
            else if (ZPropertyMesh.IsRef(p))
            {
                ret.Add(GetColumnName(p) + ".refID");
            }
            else
            {
                ret.Add(GetColumnName(p));
            }

            return ret;
        }


        static public string GetIndexPropertyId(Type t)
        {
            return t.GetCustomAttribute<DBIndexAttribute>()?.IndexName ??

             ZPropertyMesh.GetPropertyIdWithAttribute<DBIndexAttribute>(t);
        }

        static public string GetLinkedIndexId(Type t)
        {
            return t.GetCustomAttribute<DBIndexAttribute>()?.IndexName;
        }

        string GetColumnProperties(object obj)
        {
            List<IZProperty> props = ZPropertyMesh.GetProperties(obj);

            if (props.Count <= 0)
                return "";

            string rows = "";

            foreach (var p in props)
            {
                //if (ZPropertyMesh.IsRef(p))
                //continue;

                if (ZPropertyMesh.IsPropertyListLike(p))
                    continue;

                if (ZPropertyMesh.IsDefineInterface(p))
                    continue;

                if (p.AttributeNode.IsDefined<DBExcludeAttribute>())
                    continue;

                var cols = GetMultiColumnName(p);
                foreach (var c in cols)
                    rows += "`" + c + "`" + ",";
            }

            rows = rows.Remove(rows.Length - 1, 1);

            return rows;
        }

        private List<(string columnName, string valueStr)> GetColumnIValues(IZProperty p)
        {
            List<(string columnName, string valueStr)> ret = new List<(string columnName, string valueStr)>();

            if (ZPropertyMesh.IsPropertyListLike(p))
                return ret ;

            if (ZPropertyMesh.IsDefineInterface(p) 
                && !ZPropertyMesh.IsRawDataRef(p)
                 && !ZPropertyMesh.IsRef(p)
                )
                return ret;

            if (p.AttributeNode.IsDefined<DBExcludeAttribute>())
                return ret;

            if (p.Value == null && !ZPropertyMesh.IsRef(p))
            {
                return ret;
            }

            var type = p.GetDefineType();


            if (type == typeof(IRawDataPref))
            {
                var v = ZPropertyPrefs.ConvertToStr(p.Value);
                ret.Add((GetColumnName(p), v));
                //return "varchar(1024)";
                //return "json";
            }
            else if (type == typeof(ZDateTime))
            {
                ret.Add((GetColumnName(p), "\"" + (p.Value as ZDateTime).ToString() + "\""));
            }
            else if (type == typeof(ZIntBar))
            {
                ret.Add((GetColumnName(p) + ".Cur", (p.Value as ZIntBar).Cur.ToString()));
            }
            else if (type == typeof(ZDataBar))
            {
                ret.Add((GetColumnName(p) + ".Cur", (p.Value as ZDataBar).Cur.ToString()));
            }
            else if (type == typeof(ZExp))
            {
                ret.Add((GetColumnName(p) + ".CurRank", (p.Value as ZExp).CurRank.ToString()));
                ret.Add((GetColumnName(p) + ".CurExp", (p.Value as ZExp).CurExp.ToString()));
            }
            else if (type == typeof(Vector2))
            {
                ret.Add((GetColumnName(p) + ".x", ((Vector2)(p.Value)).x.ToString()));
                ret.Add((GetColumnName(p) + ".y", ((Vector2)(p.Value)).y.ToString()));

            }
            else if (type == typeof(Vector3))
            {
                ret.Add((GetColumnName(p) + ".x", ((Vector3)(p.Value)).x.ToString()));
                ret.Add((GetColumnName(p) + ".y", ((Vector3)(p.Value)).y.ToString()));
                ret.Add((GetColumnName(p) + ".z", ((Vector3)(p.Value)).z.ToString()));
            }
            else if (type == typeof(ZTransform))
            {
                ret.Add((GetColumnName(p) + ".Position.x", (p.Value as ZTransform).Position.Value.x.ToString()));
                ret.Add((GetColumnName(p) + ".Position.y", (p.Value as ZTransform).Position.Value.y.ToString()));
                ret.Add((GetColumnName(p) + ".Direction.x", (p.Value as ZTransform).Direction.Value.x.ToString()));
                ret.Add((GetColumnName(p) + ".Direction.y", (p.Value as ZTransform).Direction.Value.y.ToString()));
            }
            else if (ZPropertyMesh.IsEnum(p))
            {
                ret.Add((GetColumnName(p), "\"" + p.Value.ToString() + "\""));
            }
            else if (ZPropertyMesh.IsMultiEnum(p))
            {
                ret.Add((GetColumnName(p), "\"" + p.Value.ToString() + "\""));
            }
            else if (ZPropertyMesh.IsRef(p))
            {
                //ret.Add(GetColumnName(p) + ".refID");
                ret.Add((GetColumnName(p) + ".refID", (p as IRefable).RefID.ToString()));
            }
            else if (type == typeof(string))
            {
                ret.Add((GetColumnName(p), "\"" + p.Value.ToString() + "\""));
            }
            else if (p.Value != null)
            {
                ret.Add((GetColumnName(p), p.Value.ToString()));
            }

            return ret;
        }
        private List<(string typeStr, string columnName)> GetColumnItem(IZProperty p)
        {
            List<(string typeStr, string columnName)> ret = new List<(string typeStr, string columnName)>();

            var type = p.GetDefineType();

            if (type == typeof(int))
            {
                ret.Add(("int(11)", GetColumnName(p)));
            }
            else if (type == typeof(uint))
            {
                ret.Add(("int(20)", GetColumnName(p)));
            }
            else if (type == typeof(float))
            {
                ret.Add(("float", GetColumnName(p)));
            }
            else if (type == typeof(ZDateTime))
            {
                ret.Add(("datetime", GetColumnName(p)));
                //return "datetime";
            }
            else if (type == typeof(UInt32))
            {
                ret.Add(("int(20)", GetColumnName(p)));
            }
            else if (type.IsEnum)
            {
                ret.Add(("varchar(100)", GetColumnName(p)));
                // return "varchar(100)";
            }
            else if (ZPropertyMesh.IsMultiEnum(type))
            {
                ret.Add(("varchar(100)", GetColumnName(p)));
            }
            else if (type == typeof(System.String))
            {
                ret.Add(("varchar(255)", GetColumnName(p)));
                //return "varchar(255)";
            }
            else if (type == typeof(string))
            {
                ret.Add(("varchar(255)", GetColumnName(p)));
                //return "varchar(255)";
            }
            //for json string
            else if (type == typeof(IRawDataPref))
            {
                ret.Add(("json", GetColumnName(p)));
                //return "varchar(1024)";
                //return "json";
            }
            else if (type == typeof(ZIntBar))
            {
                ret.Add(("int(11)", GetColumnName(p) + ".Cur"));
            }
            else if (type == typeof(ZDataBar))
            {
                ret.Add(("float", GetColumnName(p) + ".Cur"));
            }
            else if (type == typeof(ZExp))
            {
                ret.Add(("int(11)", GetColumnName(p) + ".CurRank"));
                ret.Add(("float", GetColumnName(p) + ".CurExp"));
            }
            else if (type == typeof(Vector2))
            {
                ret.Add(("float", GetColumnName(p) + ".x"));
                ret.Add(("float", GetColumnName(p) + ".y"));

            }
            else if (type == typeof(Vector3))
            {
                ret.Add(("float", GetColumnName(p) + ".x"));
                ret.Add(("float", GetColumnName(p) + ".y"));
                ret.Add(("float", GetColumnName(p) + ".z"));
            }
            else if (type == typeof(ZTransform))
            {
                ret.Add(("float", GetColumnName(p) + ".Position.x"));
                ret.Add(("float", GetColumnName(p) + ".Position.y"));
                ret.Add(("float", GetColumnName(p) + ".Direction.x"));
                ret.Add(("float", GetColumnName(p) + ".Direction.y"));
            }
            else if (ZPropertyMesh.IsRef(p))
            {
                //ret.Add(GetColumnName(p) + ".refID");
                ret.Add(("int(20)", GetColumnName(p) + ".refID"));
            }


            return ret;
        }

        public void ConvertObject(object obj, MySqlDataReader reader)
        {
            //FieldInfo[] typeInfo = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            List<IZProperty> props = ZPropertyMesh.GetProperties(obj);

            //host = reader.GetString("host")

            foreach (var p in props)
            {
                try
                {


                    if (p.GetDefineType() == typeof(int))
                    {
                        p.Value = reader.GetInt32(GetColumnName(p));
                    }
                    else if (p.GetDefineType() == typeof(uint))
                    {
                        p.Value = reader.GetUInt32(GetColumnName(p));
                    }
                    else if (p.GetDefineType() == typeof(ZDateTime))    //before string column
                    {
                        //p.Value = reader.GetString(p.PropertyID);
                        ((ZDateTime)p.Value).ConvertFromString(reader.GetString(GetColumnName(p)));
                    }
                    else if (p.GetDefineType() == typeof(string))
                    {
                        p.Value = reader.GetString(GetColumnName(p));
                    }
                    else if (p.GetDefineType() == typeof(float))
                    {
                        p.Value = reader.GetFloat(GetColumnName(p));
                    }
                    else if (p.GetDefineType() == typeof(bool))
                    {
                        p.Value = reader.GetBoolean(GetColumnName(p));
                    }


                    else if (p.GetDefineType() == typeof(ZIntBar))
                    {
                        (p.Value as ZIntBar).Cur.Value = reader.GetInt16(GetColumnName(p) + ".Cur");
                        //(p.Value as ZIntBar).Cur.Value = reader.GetInt16(GetColumnName(p) + ".Max");
                    }

                    else if (p.GetDefineType() == typeof(ZDataBar))
                    {
                        (p.Value as ZDataBar).Cur.Value = reader.GetFloat(GetColumnName(p) + ".Cur");
                        //(p.Value as ZDataBar).Cur.Value = reader.GetFloat(GetColumnName(p) + ".Max");
                    }

                    else if (p.GetDefineType() == typeof(ZExp))
                    {
                        (p.Value as ZExp).CurRank.Value = reader.GetInt16(GetColumnName(p) + ".CurRank");
                        (p.Value as ZExp).CurExp.Value = reader.GetFloat(GetColumnName(p) + ".CurExp");
                    }

                    else if (p.GetDefineType().IsEnum)
                    {
                        p.Value = Enum.Parse(p.GetDefineType(), reader.GetString(GetColumnName(p)), true);
                    }
                    else if (ZPropertyMesh.IsMultiEnum(p))
                    {
                       p.Value =  (p.Value as IMultiEnumerable)?.Parse(reader.GetString(GetColumnName(p)));
                    }
                    else if (p.GetDefineType() == typeof(Vector2))
                    {
                        var v = (Vector2)p.Value;

                        v.x = reader.GetFloat(GetColumnName(p) + ".x");
                        v.y = reader.GetFloat(GetColumnName(p) + ".y");
                    }
                    else if (p.GetDefineType() == typeof(Vector3))
                    {
                        var v = (Vector3)p.Value;

                        v.x = reader.GetFloat(GetColumnName(p) + ".x");
                        v.y = reader.GetFloat(GetColumnName(p) + ".y");
                        v.z = reader.GetFloat(GetColumnName(p) + ".z");
                    }
                    else if (p.GetDefineType() == typeof(ZTransform))
                    {
                        var v = (ZTransform)p.Value;

                        v.Position.Value = new Vector2(reader.GetFloat(GetColumnName(p) + ".Position.x"), reader.GetFloat(GetColumnName(p) + ".Position.y"));
                        v.Direction.Value = new Vector2(reader.GetFloat(GetColumnName(p) + ".Direction.x"), reader.GetFloat(GetColumnName(p) + ".Direction.y"));

                    }
                    else if (ZPropertyMesh.IsRef(p))
                    {
                        (p as IRefable).RefID = (int)reader.GetUInt32(GetColumnName(p) + ".refID");
                    }
                    else
                    {
                        if (p.AttributeNode.IsDefined<DBJsonStringAttribute>())
                        {
                            //if (ZPropertyMesh.IsPropertyListLike())
                            ZPropertyPrefs.LoadValueFromStr(p, reader.GetString(GetColumnName(p)));
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("no column " + e.ToString());
                }
            }

        }

    }
}
#endif