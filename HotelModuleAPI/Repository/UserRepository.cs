using HotelModuleAPI.Interface;
using HotelModuleAPI.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.Xml;
using static HotelModuleAPI.Model.User;

namespace HotelModuleAPI.Repository
{
    public class UserRepository : IUser
    {
        private readonly IDBConnection _db;
        private readonly ICommon _common;

        public UserRepository(IDBConnection db, ICommon common)
        {
            _db = db;
            _common = common;
        }
        public async Task<AgentResponseData> CreateAgentRegistration(RegistrationRequest UserData)
        {
            AgentResponseData Response = new AgentResponseData();
            DataTable dt = new DataTable();
            try
            {
                RegFormDto User = JsonConvert.DeserializeObject<RegFormDto>(UserData.Data);
                dt = await CreateAgent(User);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int agentId = Convert.ToInt32(dt.Rows[0]["agentId"]);
                    string agentCode = Convert.ToString(dt.Rows[0]["agentcode"]);
                    string Message = string.Empty;
                    Response.Status = false;
                    if (Convert.ToString(dt.Rows[0]["status"]) == "success")
                    {
                        Message = "Agent Created Successfully";
                        UploadResponse fileUpload = await UploadFile(UserData.File, agentCode);
                        if (fileUpload != null)
                        {
                            Response.FileUpload = fileUpload.message;
                            UpdateAgentDetails(agentId, agentCode, fileUpload.FileName, fileUpload.filePath);
                        }
                        Response.AgentId = agentId;
                        Response.AgentCode = agentCode;
                        Response.Message = Message;
                        Response.Status = true;
                    }
                    else
                    {
                        Message = Convert.ToString(dt.Rows[0]["status"]);
                        Response.AgentId = agentId;
                        Response.AgentCode = agentCode;
                        Response.Message = Message;
                    }
                }
            }
            catch (Exception ex)
            {
                _common.errorLogs(ex);
            }
            return Response;
        }

        private void UpdateAgentDetails(int agentId, string agentCode, string fileName, string filePath)
        {
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@agentId", agentId),
                    new SqlParameter("@agentCode", agentCode),
                    new SqlParameter("@fileName", fileName),
                    new SqlParameter("@filePath", filePath),
                };
                DataTable dt = _db.GetDataTableSync("UpdateAgentDetails", para);
            }
            catch (Exception ex)
            {
                _common.errorLogs(ex);
            }
        }

        private async Task<UploadResponse> UploadFile(IFormFile? file, string agentCode)
        {
            try
            {
                UploadResponse Response = new UploadResponse();
                Response.status = false;
                if (file != null && file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);

                    var folder = Path.Combine("wwwroot", "uploads", agentCode);

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string fileName = agentCode + "-Profile" + fileExtension;
                    var filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    Response.agentCode = agentCode;
                    Response.filePath = filePath;
                    Response.FileName = fileName;
                    Response.status = true;
                    Response.message = "File uploaded successfully";
                }
                else
                {
                    Response.message = "No file selected";
                }
                return Response;
            }
            catch (Exception ex)
            {
                _common.errorLogs(ex);
                return null;
            }
        }

        private async Task<DataTable> CreateAgent(RegFormDto user)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                new SqlParameter("@CompanyName", user.CompanyName),
                new SqlParameter("@WebsiteUrl", user.WebsiteUrl),
                new SqlParameter("@RegistrationNumber", user.RegistrationNumber),
                new SqlParameter("@LegalEntityName", user.LegalEntityName),
                new SqlParameter("@Country", user.Country),
                new SqlParameter("@City", user.City),
                new SqlParameter("@Address", user.Address),

                new SqlParameter("@PhoneCode", user.PhoneCode),
                new SqlParameter("@PhoneNumber", user.PhoneNumber),

                new SqlParameter("@AdminFirstName", user.AdminFirstName),
                new SqlParameter("@AdminLastName", user.AdminLastName),
                new SqlParameter("@Designation", user.Designation),

                new SqlParameter("@Email", user.Email),
                new SqlParameter("@AdminPhoneCode", user.AdminPhoneCode),
                new SqlParameter("@AdminPhoneNumber", user.AdminPhoneNumber),

                new SqlParameter("@Currency", user.Currency),

                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Password", user.Password),
                };

                // DB Call
                dt = await _db.GetDataTable("AgentSignUp", para);

                if (dt.Rows.Count == 0)
                    return null;
            }
            catch (Exception ex)
            {
                _common.errorLogs(ex);
                return null;
            }
            return dt;
        }
    }
}


/*

{
  "companyName": "ABC Travels",
  "websiteUrl": "https://abctravels.com",
  "registrationNumber": "REG123",
  "legalEntityName": "ABC Pvt Ltd",
  "country": 91,
  "city": 120,
  "address": "Some Address",
  "phoneCode": "+91",
  "phoneNumber": "9876543210",
  "adminFirstName": "John",
  "adminLastName": "Doe",
  "designation": "Owner",
  "email": "john@gmail.com",
  "confirmEmail": "john@gmail.com",
  "adminPhoneCode": "+91",
  "adminPhoneNumber": "9876543210",
  "currency": "INR",
  "username": "john123",
  "password": "Test1234",
  "confirmPassword": "Test1234",
  "acceptTerms": true
}

{"companyName":"ABC Travels","websiteUrl":"https://abctravels.com","registrationNumber":"REG123","legalEntityName":"ABC Pvt Ltd","country":91,"city":120,"address":"Some Address","phoneCode":"+91","phoneNumber":"9876543210","adminFirstName":"John","adminLastName":"Doe","designation":"Owner","email":"john@gmail.com","confirmEmail":"john@gmail.com","adminPhoneCode":"+91","adminPhoneNumber":"9876543210","currency":"INR","username":"john123","password":"Test1234","confirmPassword":"Test1234","acceptTerms":true}

*/