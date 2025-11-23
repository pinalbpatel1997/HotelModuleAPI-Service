using System.Data;
using System.Data.SqlClient;

namespace HotelModuleAPI.Interface
{
    public interface IDBConnection
    {
        Task<object> GetExecuteScalarSP(String SPName, SqlParameter[] para);
        Task<object> GetExecuteScalarQry(String Qry, SqlParameter[] para);
        Task<DataTable> GetDataTable(String SPName, SqlParameter[] para);
        Task<DataSet> GetDataSet(String SPName, SqlParameter[] para);
        Task<int> ExecuteNonQuery(string spName, SqlParameter[] para);
        Task<int> LogException(Exception ex);
        DataTable GetDataTableSync(string v, SqlParameter[] para);
    }
}
