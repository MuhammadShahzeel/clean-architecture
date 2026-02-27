using Clean.Application.Exceptions;
using Clean.Application.Interfaces;
using Clean.Application.Wrappers;
using Clean.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clean.Application.Features.Products.Queries
{
    public class GetProductByIdQueryHandler
       : IRequestHandler<GetProductByIdQuery, ApiResponse<Product>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetProductByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<Product>> Handle(
                GetProductByIdQuery request,
                CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

            if (product == null) throw new ApiException("Product not found");
            return new ApiResponse<Product>(product, "Product retrieved successfully.");
        }
    }
}

