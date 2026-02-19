using Clean.Domain.Entities;
using MediatR;
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
            public async Task<IEnumerable<Product>> Handle(
                GetAllProductsQuery request,
                CancellationToken cancellationToken)
            {

            // dummy data for now, in real scenario we will get data from database
            var list = new List<Product>();

                for (int i = 0; i < 100; i++)
                {
                    list.Add(new Product
                    {
                        Name = "Mobile",
                        Description = "Test Mobile",
                        Rate = 100 + i
                    });
                }

                return list;
            }
        }
    
}
