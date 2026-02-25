using FluentValidation;
using MediatR;

namespace Clean.Application.Behaviours
{
    public class ValidationBehaviors<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        //constructor
        public ValidationBehaviors(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;

        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            //Pre-Processing logic here
            //For Example, Logging, validation

            if (_validators.Any())
            {
                var validationContext = new ValidationContext<TRequest>(request);
                var result = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));
                var failers = result.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failers.Count > 0)
                {
                    throw new ValidationException(failers);
                }
            }

            //Next
            var response = await next();

            //Post-Processing logic here
            //For Example, Response modification...

            return response;
        }
    }
}