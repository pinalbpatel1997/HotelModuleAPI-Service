using HotelModuleAPI.Interface;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace HotelModuleAPI.Repository
{
    public class CommonRepository : ICommon
    {
        private readonly IDBConnection _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommonRepository(IDBConnection db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        public bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string pattern = @"^[6-9]\d{9}$";
            return Regex.IsMatch(phone, pattern);
        }

        public void saveRequestHistory(string Title, object Request, object Response = null, string token = "" , int AgentID = 0, string Session = "")
        {
            try
            {
                string jsonRequest = null;

                if (Request != null)
                {
                    jsonRequest = JsonConvert.SerializeObject(Request);
                }
                string jsonResponse = null;

                if (Response != null)
                {
                    jsonResponse = JsonConvert.SerializeObject(Response);
                }
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@Title",Title ?? (object)DBNull.Value),
                    new SqlParameter("@Url", Convert.ToString(_httpContextAccessor.HttpContext.Request.Path)),
                    new SqlParameter("@Request", jsonRequest ?? (object)DBNull.Value),
                    new SqlParameter("@Response", jsonResponse ?? (object)DBNull.Value),
                    new SqlParameter("@token", token ??(object) DBNull.Value),
                    new SqlParameter("@AgentID", AgentID),
                    new SqlParameter("@Session", Session ??(object) DBNull.Value)
                };
                var i = _db.ExecuteNonQuerysync("SaveRequestResponse", para);
            }
            catch (Exception ex)
            {
                errorLogs(ex);
            }
        }

        public async void errorLogs(Exception ex)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            System.Diagnostics.StackFrame stackFrame = trace.GetFrame(trace.FrameCount - 1);
            System.Reflection.MethodBase site = ex.TargetSite;
            string fileName = stackFrame.GetFileName();
            string methodName = stackFrame.GetMethod().Name;
            int lineNo = stackFrame.GetFileLineNumber();
            string ErrorPageName = "Page:" + fileName + " ###Method:" + methodName + " ###Line No:" + lineNo + "";


            SqlParameter[] para = new SqlParameter[]{
              new SqlParameter("@title",  ErrorPageName),
              new SqlParameter("@status",  SqlDbType.Int){Value=0},
              new SqlParameter("@details", ex.Message + ";"+ ex.StackTrace),
              new SqlParameter("@datetime",  DateTime.Now),
              new SqlParameter("@source", Convert.ToString(_httpContextAccessor.HttpContext.Request.Path)),
              new SqlParameter("@WebsiteName", "HotelModule"),
            };
            await _db.ExecuteNonQuery("ExceptionLog", para);
        }
    }
}
