using Organization.Application;
using Organization.Infrastructure;
using Organization.WebAPI;
using Organization.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
	.AddApplication()
	.AddInfrastructure(builder.Configuration)
	.AddPresentation(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Organization API");
	c.RoutePrefix = "swagger";
});

app.UseCustomExceptionHandler();

app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();