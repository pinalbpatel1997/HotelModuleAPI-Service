using HotelModuleAPI.Interface;
using System.Data;
using System.Data.SqlClient;

namespace HotelModuleAPI.Repository
{
    public class DBConnectionRepository : IDBConnection
    {
        private readonly string connectionStr;
        public DBConnectionRepository(IConfiguration configuration)
        {
            connectionStr = configuration.GetConnectionString("DefaultConnection").ToString();
        }
        public async Task<object> GetExecuteScalarSP(String SPName, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.CommandText = SPName;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                con.Open();
                return await Task.Run(() => cmd.ExecuteScalar());
            }
        }
        public DataTable GetDataTableSync(String SPName, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SPName;
                cmd.CommandTimeout = 1000;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }
        public async Task<object> GetExecuteScalarQry(String Qry, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = Qry;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                con.Open();
                return await Task.Run(() => cmd.ExecuteScalar());
            }
        }
        public async Task<DataTable> GetDataTable(String SPName, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SPName;
                cmd.CommandTimeout = 1000;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                await Task.Run(() => da.Fill(dt));
                return dt;
            }
        }
        public async Task<DataSet> GetDataSet(String SPName, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SPName;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                cmd.CommandTimeout = 1000;
                con.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                await Task.Run(() => da.Fill(ds));
                return ds;
            }
        }
        public async Task<int> ExecuteNonQuery(string spName, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                cmd.CommandTimeout = 100000;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                con.Open();
                return await Task.Run(() => cmd.ExecuteNonQuery());
            }
        }
        public async Task<int> LogException(Exception exdetails)
        {
            SqlParameter[] para = new SqlParameter[]
            {
                new SqlParameter("@SourceName","ActivityLinker"),
                new SqlParameter("@Title", "Error in "+ exdetails.Message),
                new SqlParameter("@Details", exdetails.ToString()),
                new SqlParameter("@Source", ""),
                new SqlParameter("@WebsiteName", "ActivityLinker")
            };
            return await ExecuteNonQuery("LogException_B2B", para);
        }
    }
}
