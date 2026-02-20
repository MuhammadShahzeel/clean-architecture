using Clean.Application.Interfaces;
using Clean.Domain.Entities;
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
        private readonly IApplicationDbContext _dbContext;

        public CreateProductCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Create a Product entity
            var product = new Product
            {
                Name = request.Name,
               
                Description = request.Description,
                Rate = request.Rate

            };

            // 2. Add it to DbContext
           await  _dbContext.Products.AddAsync(product);

            // 3. Save changes
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 4. Return the new product's ID
            return product.Id;



        }
    }
}
