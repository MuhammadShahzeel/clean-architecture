using Clean.Application.Interfaces;
using System.Security.Claims;

namespace Clean.WebApi.Services.SharedServices
{
    public class AuthenticatedUser : IAuthenticatedUser
    {
        public AuthenticatedUser(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier); // because we store id in sub instead of  adding extra claim uid
            //UserId = httpContextAccessor.HttpContext.User.FindFirstValue("uid"); 


        }

        public string UserId { get; set; }
    }
}
