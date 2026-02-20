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
   
        public class GetAllProductsQueryHandler
       : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
        {
        private readonly IApplicationDbContext _dbContext;

        public GetAllProductsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> Handle(
                GetAllProductsQuery request,
                CancellationToken cancellationToken)
            {
                return await _dbContext.Products.ToListAsync(cancellationToken);
            }
        }
    
}
