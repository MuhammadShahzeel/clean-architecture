using Clean.Application.DTOs;
using Clean.Application.Wrappers;

namespace Clean.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<Guid>> RegisterUser(RegisterRequest registerRequest);
        Task<ApiResponse<AuthenticationResponse>> Authenticate(AuthenticationRequest request);
        Task<ApiResponse<string>> ConfirmEmail(ConfirmEmailRequest request);
        Task<ApiResponse<string>> ResendConfirmEmail(string email);
        Task<ApiResponse<bool>> ForgotPasswordAsync(string userEmail);
        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest resetPassword);
    }
}
