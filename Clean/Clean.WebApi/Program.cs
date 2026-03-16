using Clean.Application.Extensions;
using Clean.Application.Interfaces;
using Clean.Infrastructure.Extensions;
using Clean.Persistence.Extensions;
using Clean.Persistence.Seeds;
using Clean.WebApi.Extensions;
using Clean.WebApi.Middlewares;
using Clean.WebApi.Services.SharedServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerWithJwt(); // Extension method to config jwt authroize btn
builder.Services.AddJwtAuthentication(builder.Configuration); // extension method to configure jwt authentication 
builder.Services.AddApplication(); // extension method to register application layer services
builder.Services.AddInfrastructure(builder.Configuration); // extension method to register infrastructure layer services
builder.Services.AddPersistance(builder.Configuration); // extension method to register persistence layer services
builder.Services.AddScoped<IAuthenticatedUser, AuthenticatedUser>(); // registering the implementation of IAuthenticatedUser to be used in the application, it will be used to get the current authenticated user's information in the application
builder.Services.AddHttpContextAccessor(); // registering the HttpContextAccessor to be used in the AuthenticatedUser class to get the current authenticated user's information from the HttpContext
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// seed roles and users

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await DefaultRoles.SeedRolesAsync(serviceProvider);
    await DefaultUsers.SeedUsersAsync(serviceProvider);
}

app.UseHttpsRedirection();
app.UseAuthentication(); // authentication middleware should be before authorization middleware
app.UseAuthorization();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();
app.Run();