using HotelModuleAPI.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using static HotelModuleAPI.Model.User;

namespace HotelModuleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ICommon _common;
        public UserController(IUser User, ICommon Common)
        {
            _user = User;
            _common = Common;
        }

        [HttpGet("Get")]
        public string test()
        {
            return "test";
        }
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> AgentSignUp([FromForm] RegistrationRequest UserData)
        {
            if (!ModelState.IsValid)
            {
                return (BadRequest(ModelState));
            }
            else
            {
                if (UserData.Data != null)
                {

                    var isvalid = ValidateRegistration(UserData);
                    if (isvalid != null)
                        return isvalid;
                    var response = await AgentRegistration(UserData);
                    if (response == null)
                        return BadRequest("Registration failed");
                    return Ok(response);
                }
                else
                {
                    return (BadRequest("Required Data Missing!!!!!"));
                }
            }
        }

        private IActionResult? ValidateRegistration(RegistrationRequest UserData)
        {
            if (string.IsNullOrWhiteSpace(UserData.Data))
                return BadRequest("Data is required.");

            if (UserData.Data != null)
            {
                RegFormDto User;
                try
                {
                    User = JsonConvert.DeserializeObject<RegFormDto>(UserData.Data);
                    if (User == null)
                        return BadRequest("Invalid data format.");
                }
                catch (Exception ex)
                {
                    return BadRequest("Invalid JSON format.");
                }
                // Validate Required Fields of DTO
                var validationContext = new ValidationContext(User);
                var validationResults = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(User, validationContext, validationResults, true);

                if (!isValid)
                {
                    string errors = string.Join(" | ", validationResults.Select(v => v.ErrorMessage));
                    return BadRequest("Missing or invalid fields: " + errors);
                }

                if (User != null)
                {
                    if (!_common.IsValidEmail(User.Email))
                        return BadRequest("Invalid email format.");
                    if (!_common.IsValidPhone(User.PhoneNumber))
                        return BadRequest("Invalid Phone Number.");
                    if (User.Email != User.ConfirmEmail)
                        return BadRequest("Email & Confirm Email do not match.");
                    if (User.Password != User.ConfirmPassword)
                        return BadRequest("Password & Confirm Password do not match.");
                    if (User.AcceptTerms == false)
                        return BadRequest("You must accept terms & conditions.");
                }
            }
            if (UserData.File == null || UserData.File.Length == 0)
                return BadRequest("No file selected");
            if (UserData.File != null)
            {

                // File Extension
                var fileExtension = Path.GetExtension(UserData.File.FileName); // e.g., ".jpg", ".png", ".txt"

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".txt" };
                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    return BadRequest("Unsupported file type");


                // File size restriction (2 MB)
                var maxFileSize = 2 * 1024 * 1024; // 2 MB in bytes
                if (UserData.File.Length > maxFileSize)
                    return BadRequest("File size exceeds 2 MB");

            }
            return null;

        }
        private async Task<object?> AgentRegistration(RegistrationRequest User)
        {
            try
            {
                var Response = await _user.CreateAgentRegistration(User);
            }
            catch (Exception ex)
            {
                return null;
            }
            return Response;
        }

        #region File Upload
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string data)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected");

            var folder = Path.Combine("wwwroot", "uploads");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("File uploaded successfully");
        }

        #endregion
    }
}
