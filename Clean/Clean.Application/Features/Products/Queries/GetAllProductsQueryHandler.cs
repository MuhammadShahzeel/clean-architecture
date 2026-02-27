using Clean.Application.Interfaces;
using Clean.Application.Wrappers;
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
       : IRequestHandler<GetAllProductsQuery, ApiResponse<IEnumerable<Product>>>
        {
        private readonly IApplicationDbContext _dbContext;

        public GetAllProductsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<IEnumerable<Product>>> Handle(
                GetAllProductsQuery request,
                CancellationToken cancellationToken)
            {
                var products = await _dbContext.Products.ToListAsync(cancellationToken);
                return new ApiResponse<IEnumerable<Product>>(products, "Products retrieved successfully.");
            }
        }
    
}
