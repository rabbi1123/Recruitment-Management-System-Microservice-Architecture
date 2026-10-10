using Notification.Application;
using Notification.Infrastructure;
using Notification.WebAPI;
using Notification.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddApplication()
	.AddInfrastructure(builder.Configuration)
	.AddPresentation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification API");
	c.RoutePrefix = "swagger";
});

app.UseCustomExceptionHandler();
app.MapControllers();
app.Run();
