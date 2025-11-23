namespace HotelModuleAPI.Interface
{
    public interface ICommon
    {
        bool IsValidEmail(string email);
        bool IsValidPhone(string phoneNumber);
    }
}