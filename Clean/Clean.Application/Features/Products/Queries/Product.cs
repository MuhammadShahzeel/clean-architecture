using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Application.Features.Products.Queries
{
    public class Product
    {
        // this is temporary class to hold the data, in real scenario we will use the entity class from domain layer
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
    }
}
