using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelModuleAPI.Model
{
    public class User
    {
        public class RegFormDtotest
        {
            public string? userName { get; set; }
            public string? password { get; set; }
        }
        public class RegFormDto
        {
            [Required]
            public required string CompanyName { get; set; }
            [Required]
            public required string WebsiteUrl { get; set; }
            [Required]
            public required string RegistrationNumber { get; set; }
            [Required]
            public required string LegalEntityName { get; set; }
            public int Country { get; set; }
            public int City { get; set; }
            [Required]
            public required string Address { get; set; }
            [Required]
            public required string PhoneCode { get; set; }
            [Required]
            public required string PhoneNumber { get; set; }
            [Required]
            public required string AdminFirstName { get; set; }
            [Required]
            public required string AdminLastName { get; set; }
            [Required]
            public required string Designation { get; set; }
            [Required]
            public required string Email { get; set; }
            [Required]
            public required string ConfirmEmail { get; set; }
            [Required]
            public required string AdminPhoneCode { get; set; }
            [Required]
            public required string AdminPhoneNumber { get; set; }
            [Required]
            public required string Currency { get; set; }
            [Required]
            public required string Username { get; set; }
            [Required]
            public required string Password { get; set; }
            [Required]
            public required string ConfirmPassword { get; set; }
            [DefaultValue(false)]
            public bool AcceptTerms { get; set; }
            //public RegistrationRequest? FileUpload { get; set; }
        }

        public class RegistrationRequest

        {
            [Required]
            public required string Data { get; set; }  // JSON string
            public IFormFile? File { get; set; }

        }
        public class AgentResponseData
        {
            public int AgentId { get; set; }
            public string AgentCode { get; set; }
            public string? Message { get; set; }
            public string? FileUpload { get; set; }
            public bool Status { get; set; }
        }

        public class UploadResponse
        {
            public string agentCode { get; set; }
            public bool status { get; set; }
            public string FileName { get; set; }
            public string filePath { get; set; }
            public string message { get; set; }

        }
    }
}
