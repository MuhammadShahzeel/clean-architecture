using Clean.Application.DTOs;
using Clean.Application.Enums;
using Clean.Application.Exceptions;
using Clean.Application.Interfaces;
using Clean.Application.Settings;
using Clean.Application.Wrappers;
using Clean.Persistence.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Clean.Persistence.SharedServices
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AccountService(UserManager<ApplicationUser> userManager, ITokenService tokenService, IOptions<JwtSettings> jwtOptions, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            // jwtOptions kept for backward compatibility if needed elsewhere; not used here
            _ = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
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
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(userModel, registerRequest.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(userModel, Roles.Basic.ToString());
                string emailTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Welcome Email</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }
        .container {
            max-width: 600px;
            background: #ffffff;
            margin: 20px auto;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            text-align: center;
        }
        .header {
            font-size: 24px;
            font-weight: bold;
            color: #333;
        }
        .content {
            font-size: 16px;
            color: #666;
            margin-top: 10px;
        }
        .button {
            display: inline-block;
            background-color: #FF0000;
            color: #ffffff;
            padding: 12px 20px;
            margin-top: 20px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
        }
        .footer {
            margin-top: 20px;
            font-size: 12px;
            color: #999;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">Welcome [UserName]</div>
        <div class=""content"">
            Thank you for joining us. We are excited to have you on board. Click the button below to subscribe to my channel!
        </div>
        <a href="" target=""_blank"" class=""button"">Subscribe Now</a>
        <div class=""footer"">
            If you have any questions, feel free to contact us at <a href="" target=""_blank"">LinkedIn</a>
        </div>
    </div>
</body>
</html>
";

                var emailRequest = new EmailRequest()
                {
                    To = userModel.Email,
                    Body = emailTemplate.Replace("[UserName]", userModel.UserName), // aspper needed
                    Subject = $"Welcome {userModel.Email} to CleanArchitecture",
                    IsHtmlBody = true,
                };


               //Body = "User Register successfuly";
               // isHtmlBody=false  if for simple text

                await _emailService.SendAsync(emailRequest);
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

        public async Task<ApiResponse<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException("Email or password is incorrect");
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

        // Token generation is delegated to ITokenService to keep concerns separated and improve testability.
    }
}