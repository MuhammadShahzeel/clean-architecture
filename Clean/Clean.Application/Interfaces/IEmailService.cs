using Clean.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
        }
    }

