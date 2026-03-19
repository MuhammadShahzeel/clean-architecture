using Clean.Application.DTOs;
using Clean.Application.Enums;
using Clean.Application.Exceptions;
using Clean.Application.Interfaces;
using Clean.Application.Settings;
using Clean.Application.Wrappers;
using Clean.Persistence.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;

namespace Clean.Persistence.SharedServices
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly AppSettings _appSettings;
        public AccountService(UserManager<ApplicationUser> userManager, ITokenService tokenService, IOptions<AppSettings> appSettings, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _appSettings = appSettings.Value;
            _emailService = emailService;
        }

        public async Task<ApiResponse<Guid>> RegisterUser(RegisterRequest registerRequest)
        {
            var user = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (user != null)
            {
                throw new ApiException($"User already taken {registerRequest.Email}");
            }

            var userModel = new ApplicationUser
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Gender = registerRequest.Gender,
                //EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(userModel, registerRequest.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(userModel, Roles.Basic.ToString());
              await SendConfirmationEmailAsync(userModel);

                return new ApiResponse<Guid>(userModel.Id, "User Registered successfully");
            }
            else
            {
                var errors = result.Errors
                    .Select(e => e.Description)
                    .ToList();

                throw new ValidationErrorException(errors);
            }
        }

        private async Task SendConfirmationEmailAsync(ApplicationUser userModel)
        {
            //  Confirmation token generate 
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);

            //  Token URL encode 
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            //  Confirmation link banao
            var confirmationLink =
  $"{_appSettings.ClientUrl}/api/account/confirm-email?userId={userModel.Id}&token={encodedToken}";
            string emailTemplate = $@"<!DOCTYPE html>
<html>
<body style='font-family:Arial; background:#f4f4f4; margin:0; padding:0;'>
    <div style='max-width:600px; background:#fff; margin:20px auto; padding:20px; border-radius:8px;'>
        <h2 style='color:#333;'>Hey {userModel.UserName}, confirm your email!</h2>
        <p style='color:#666;'>Click the button below to verify your account.</p>
        <a href='{confirmationLink}' 
           style='display:inline-block; background:#4F46E5; color:#fff; 
                  padding:12px 24px; border-radius:5px; text-decoration:none; 
                  font-weight:bold; margin-top:10px;'>
            Confirm Email
        </a>
        <p style='color:#999; font-size:12px; margin-top:20px;'>
            Link 24 hours mein expire ho jayega.
        </p>
    </div>
</body>
</html>";


            var emailRequest = new EmailRequest
            {
                To = userModel.Email,
                Subject = "Confirm Your Email",
                Body = emailTemplate,
                IsHtmlBody = true,
            };

            await _emailService.SendAsync(emailRequest);
        }

        public async Task<ApiResponse<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException("Email or password is incorrect");
            }

            if(!user.EmailConfirmed)
            {
                throw new ApiException("Email not confirmed. Please check your inbox.");
            }   

            var succeeded = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!succeeded)
            {
                throw new ApiException($"Email or password is incorrect");
            }


            // Single DB call — roles & claims yahan fetch karo, GenerateToken ko pass karo
            var roles = await _userManager.GetRolesAsync(user);
            var dbClaims = await _userManager.GetClaimsAsync(user);

            var token = _tokenService.GenerateToken(
                user.Id.ToString(),
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty,
                roles.ToList(),
                dbClaims);

            var authenticationResponse = new AuthenticationResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                IsVerified = user.EmailConfirmed,
                Roles = roles.ToList(),
                JWToken = token
            };

            return new ApiResponse<AuthenticationResponse>(authenticationResponse, "Authenticated User");
        }


        //this will call when click on verification button through controller


        public async Task<ApiResponse<string>> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new ApiException("User not found.");

            //  Token decode 
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                var errorMessage = string.Join(", ", errors);

                throw new ApiException(errorMessage);
            }

            return new ApiResponse<string>(user.Email, "Email confirmed successfully.");
        }





    }
}