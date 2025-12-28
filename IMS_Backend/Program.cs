using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
//connectionstring
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var Key = builder.Configuration["Jwt:Key"];
var Issuer = builder.Configuration["Jwt:Issuer"];
var Audience = builder.Configuration["Jwt:Audience"];
var ValidDurationInMinitues = builder.Configuration["Jwt:ValidDurationInMinutes"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = "https://securetoken.google.com/ims-backend-7f6f4";
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = Issuer,
        ValidAudience = Audience,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Key)),
    };
});


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
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "IMS API", Version = "v1" });

    //// Define the Security Scheme (The "Authorize" Button)
    //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //{
    //    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    //    Name = "Authorization",
    //    In = ParameterLocation.Header,
    //    Type = SecuritySchemeType.ApiKey,
    //    Scheme = "Bearer"
    //});
    // Apply the Security Scheme to endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
