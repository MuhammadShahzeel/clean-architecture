using Clean.Application.Interfaces;
using Clean.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Application.Features.Products.Queries
{
    public class GetProductByIdQueryHandler
       : IRequestHandler<GetProductByIdQuery, Product>
        {
        private readonly IApplicationDbContext _dbContext;

        public GetProductByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> Handle(
                GetProductByIdQuery request,
                CancellationToken cancellationToken)
            {
           var product = await _dbContext.Products
           .Where(x => x.Id == request.Id)
           .FirstOrDefaultAsync(cancellationToken);

           if (product == null) return default;
           return product;
        }
    }
}

