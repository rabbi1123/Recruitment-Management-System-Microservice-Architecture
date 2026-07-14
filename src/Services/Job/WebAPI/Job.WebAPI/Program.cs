using Job.Application;
using Job.Infrastructure;
using Job.WebAPI;
using Job.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

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
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Job API");
	c.RoutePrefix = "swagger";
});

app.UseCustomExceptionHandler();

app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();
