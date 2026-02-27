using AutoMapper;
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
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IApplicationDbContext dbContext,IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<ApiResponse<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Create a Product entity
            //var product = new Product
            //{
            //    Name = request.Name,
               
            //    Description = request.Description,
            //    Rate = request.Rate

            //};

            // automapping
            // <source, destination>
            var product = _mapper.Map<Product>(request);

            // 2. Add it to DbContext
            await  _dbContext.Products.AddAsync(product);

            // 3. Save changes
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 4. Return the new product's ID
            return new ApiResponse<int>(product.Id, "Product created successfully.");



        }
    }
}
