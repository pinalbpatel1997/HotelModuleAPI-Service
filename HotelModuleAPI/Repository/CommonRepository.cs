using HotelModuleAPI.Interface;
using System.Text.RegularExpressions;

namespace HotelModuleAPI.Repository
{
    public class CommonRepository : ICommon
    {
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
    }
}
