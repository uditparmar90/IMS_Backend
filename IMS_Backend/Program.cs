using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//connectionstring
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<IMS_Backend.DBCommection.MyApplicationDB>(options =>
    options.UseSqlServer(connectionString));
// Add services to the container.
//1.Define the policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
       policy => policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());// Your Angular URL
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
var app = builder.Build();

// 2. Use the policy
app.UseCors("AllowAngularApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Explicitly point to the JSON file Swashbuckle generates
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "IMS API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
