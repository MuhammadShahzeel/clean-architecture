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

        public AccountService(UserManager<ApplicationUser> userManager, ITokenService tokenService, IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            // jwtOptions kept for backward compatibility if needed elsewhere; not used here
            _ = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
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