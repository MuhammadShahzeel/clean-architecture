using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Application.Features.Products.Commands
{
    public class CreateProductCommand :IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }

    }
}
