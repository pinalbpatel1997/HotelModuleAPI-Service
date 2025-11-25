using HotelModuleAPI.Model;

namespace HotelModuleAPI.Interface
{
    public interface ICommon
    {
        void errorLogs(Exception ex);
        bool IsValidEmail(string email);
        bool IsValidPhone(string phoneNumber);
        void saveRequestHistory(string Title, Object Request, object Response = null , string token = "" ,int AgentID = 0, string Session = "");
    }
}