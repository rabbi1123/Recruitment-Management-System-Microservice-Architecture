using Candidate.WebAPI.Extensions;
using Candidate.Application;
using Candidate.Infrastructure;
using Candidate.WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabaseConnectionString();

builder.Services
	.AddApplication()
	.AddInfrastructure(builder.Configuration)
	.AddPresentation(builder.Configuration);

// ✅ Authorization Policy
//builder.Services.AddAuthorization(options =>
//{
//	options.AddPolicy("HasUserId", policy =>
//		policy.RequireClaim("UserId"));
//});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Candidate Recruitment API");
	c.RoutePrefix = "swagger";
});

app.UseCustomExceptionHandler();

app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();
