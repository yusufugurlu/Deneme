using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Deneme.Common
{
    //Et entity yani o classa göre CRUD işlemleri yapılacak.
    public class ORMBase<ET, OT> : IORM<ET> where ET : class, new()
        where OT : class, new()

    {
        private static OT _Current;

        public static OT Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new OT();
                }
                return _Current;
            }
            set { _Current = value; }
        }

        private Type ETTtpe
        {
            get
            {
                return typeof(ET);
            }
        }

        private Table TableAt
        {
            get
            {
                Table table = null;
                var attribure = ETTtpe.GetCustomAttributes(typeof(Table), false);
                if (attribure != null && attribure.Any())
                {
                    table = (Table)attribure[0];
                }
                return table;
            }
        }
        public Result<bool> Delete(ET Entity)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Tools.Connection;
            string query = "delete from ";
            query += string.Format(" {0} ", TableAt.TableName);
            PropertyInfo[] propertyInfos = ETTtpe.GetProperties();

            foreach (PropertyInfo pi in propertyInfos)
            {
                if (pi.Name == TableAt.IdentityColumn)
                {
                    var obj = pi.GetValue(Entity);
                    if (obj == null)
                    {
                        continue;
                    }
                    query +=string.Format("where {0}= {1}",pi.Name,obj);
                    break;
                }
            }
            cmd.CommandText = query;
            return cmd.Exec();

        }

        public Result<bool> Insert(ET Entity)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Tools.Connection;
            string query = "insert into";
            query += string.Format(" {0}(", TableAt.TableName);
            PropertyInfo[] propertyInfos = ETTtpe.GetProperties();
            string values = "values(";
            foreach (PropertyInfo pi in propertyInfos)
            {
                if (pi.Name == TableAt.IdentityColumn)
                {
                    continue;
                }
                var obj = pi.GetValue(Entity);
                if (obj==null)
                {
                    continue;
                }
                query += string.Format(" {0},", pi.Name);
                values += string.Format(" @{0},", pi.Name);              
                cmd.Parameters.AddWithValue(string.Format("@{0}", pi.Name), obj);
            }
            query = query.Remove(query.Length - 1, 1);
            values = values.Remove(values.Length - 1, 1);
            query += string.Format(") {0})",values);
           
            cmd.CommandText = query;
            return cmd.Exec();
         
        }

        public Result<List<ET>> Select()
        {
            Type type = typeof(ET);
            string query = "select * from ";
            var attribure = type.GetCustomAttributes(typeof(Table), false);
            if (attribure != null && attribure.Any())
            {
                Table table = (Table)attribure[0];
                query += table.TableName;
            }
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            dataAdapter.SelectCommand = new SqlCommand();
            dataAdapter.SelectCommand.Connection = Tools.Connection;
            dataAdapter.SelectCommand.CommandText = query;
           // Tools.ToList<ET>(dt); yada 
            return dataAdapter.ToList<ET>();
        }

        public Result<bool> Update(ET Entity)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Tools.Connection;
            string query = "update";
            string Wherevalues = "";
            query += string.Format(" {0} set ", TableAt.TableName);
            PropertyInfo[] propertyInfos = ETTtpe.GetProperties();
            foreach (PropertyInfo pi in propertyInfos)
            {
                var obj = pi.GetValue(Entity);
                if (obj == null)
                {
                    continue;
                }
                if (pi.Name == TableAt.IdentityColumn)
                {
                    Wherevalues += string.Format(" where {0}= @{1}", pi.Name, pi.Name);
                    cmd.Parameters.AddWithValue(string.Format("@{0}", pi.Name), obj);
                }
                else
                {
                    query += string.Format(" {0} = @{1},", pi.Name,pi.Name);
                    cmd.Parameters.AddWithValue(string.Format("@{0}", pi.Name), obj);
                }
            }
            query = query.Remove(query.Length - 1, 1);
            query += Wherevalues;

            cmd.CommandText = query;
            return cmd.Exec();
        }
    }
}
