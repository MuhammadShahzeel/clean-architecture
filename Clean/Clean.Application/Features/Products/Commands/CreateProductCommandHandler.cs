using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Application.Features.Products.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Logic for db insertion will be here, for now we are just returning 1 as a dummy value        
            return 1;
        }
    }
}
