using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Deneme.Common
{
    public static class Tools
    {
        private static SqlConnection _Connection;
        //ConfigurationManager ilk başta çıkmıyor referans eklemek gerekir .System.Configuration altında bulunur.
        public static SqlConnection Connection
        {
            get
            {
                if (_Connection == null)
                {
                    _Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCS"].ConnectionString);
                }
                return _Connection;

            }
            set { _Connection = value; }
        }

        //Buradaki this datatable list<ET>(); oluşturarak eklenir. Datatable entegresyonu eklenir.
        public static Result<List<ET>> ToList<ET>(this SqlDataAdapter adp) where ET : class, new()
        {

            try
            {
                DataTable dt = new DataTable();
                adp.Fill(dt);
                Type type = typeof(ET);
                List<ET> list = new List<ET>();
                PropertyInfo[] propertyInfos = type.GetProperties();
                foreach (DataRow dr in dt.Rows)
                {
                    ET tip = new ET();

                    foreach (PropertyInfo pi in propertyInfos)
                    {
                        try
                        {
                            object value = dr[pi.Name];
                            if (value != null)
                            {
                                pi.SetValue(tip, value);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    list.Add(tip);
                }
                return new Result<List<ET>>()
                {
                    IsSuccess = true,
                    Data = list
                };
            }
            catch (Exception ex)
            {
                return new Result<List<ET>>()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }

        }

        public static Result<bool> Exec(this SqlCommand sqlCommand)
        {
            try
            {
                if (sqlCommand.Connection.State != ConnectionState.Open)
                {
                    sqlCommand.Connection.Open();
                }
                int result = sqlCommand.ExecuteNonQuery();
                return new Result<bool>()
                {
                    IsSuccess = true,
                    Message = "İşlem başarılı",
                    Data = result > 0

                };
            }
            catch (Exception ex)
            {
                return new Result<bool>()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
            finally
            {
                if (sqlCommand.Connection.State != ConnectionState.Closed)
                {
                    sqlCommand.Connection.Close();
                }
            }
        }
    }
}
