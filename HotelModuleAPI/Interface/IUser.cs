using HotelModuleAPI.Model;
using static HotelModuleAPI.Model.User;

namespace HotelModuleAPI.Interface
{
    public interface IUser
    {
        Task<AgentResponseData> CreateAgentRegistration(RegistrationRequest user);
    }
}
