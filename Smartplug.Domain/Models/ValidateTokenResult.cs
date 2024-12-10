using System.Security.Claims;

namespace Smartplug.Domain.Models
{
    public class ValidateTokenResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public List<Claim> UserClaims { get; set; }
        public string UserId { get; set; }
        public ValidateTokenResult(bool isValid, string errorMessage, string userId = "")
        {
            this.IsValid = isValid;
            this.ErrorMessage = errorMessage;
            this.UserId = userId;
        }
    }
}
