using Clean.Application.DTOs;

using Clean.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clean.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
       
        private readonly IAccountService _accountService;

 
        public AccountController( IAccountService accountService)
        {
         
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            var result = await _accountService.RegisterUser(registerRequest);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest registerModel, CancellationToken cancellationToken)
        {
            var result = await _accountService.Authenticate(registerModel);
            return Ok(result);
        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest confirmEmailRequest, CancellationToken cancellationToken)
        {
            var result = await _accountService.ConfirmEmail(confirmEmailRequest);
            return Ok(result);
        }




    }
}