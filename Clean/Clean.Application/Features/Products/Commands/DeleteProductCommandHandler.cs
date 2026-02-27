using Clean.Application.Exceptions;
using Clean.Application.Interfaces;
using Clean.Application.Wrappers;
using Clean.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Application.Features.Products.Commands
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeleteProductCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<ApiResponse<int>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Find the existing product
            var product = await _dbContext.Products.FindAsync(request.Id);
            if (product == null) { throw new ApiException("Product not found."); }

            // 2. Remove the product
            _dbContext.Products.Remove(product);

            // 3. Save changes

            await _dbContext.SaveChangesAsync(cancellationToken);

            // 4. Return the updated product's ID
            return new ApiResponse<int>(product.Id, "Product deleted successfully.");
        }   




        }
    }

