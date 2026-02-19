using Clean.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clean.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        //inject the mediator 
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
        {
            var products = await _mediator.Send(new GetAllProductsQuery(), cancellationToken);
            return Ok(products);
        }


    }
}
