using AutoMapper;
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
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler  (IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ApiResponse<int>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Find the existing product
            var product = await _dbContext.Products.FindAsync(request.Id);
            if (product == null) { throw new ApiException("Product not found."); }

            ////   2. Update the product's properties 
            //product.Name = request.Name;
            //product.Description = request.Description;
            //product.Rate = request.Rate;

            _mapper.Map(request, product);


            // 3. Save changes

            await _dbContext.SaveChangesAsync(cancellationToken);

            // 4. Return the updated product's ID
            return new ApiResponse<int>(product.Id, "Product updated successfully.");
        }




        }
    }

