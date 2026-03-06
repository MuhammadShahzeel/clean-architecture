using Clean.Application.DTOs;
using Clean.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<Guid>> RegisterUser(RegisterRequest registerRequest);
    }
}
