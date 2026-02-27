using Clean.Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Application.Features.Products.Commands
{
    public class DeleteProductCommand :IRequest<ApiResponse<int>>
    {
            public int Id { get; set; }

        }
    }

    