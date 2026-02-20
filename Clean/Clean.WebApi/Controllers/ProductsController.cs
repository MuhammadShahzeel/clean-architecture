using Clean.Application.Features.Products.Commands;
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
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProductById(int id, CancellationToken cancellationToken)
        {
            var product = await _mediator.Send(new GetProductByIdQuery { Id = id }, cancellationToken);
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteProductCommand { Id = id }, cancellationToken);
            return Ok(result);
        }
    }
}
