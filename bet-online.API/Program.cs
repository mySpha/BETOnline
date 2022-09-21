using bet_online.API.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("Authentication:SecreteKey").Value);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }); ;
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();

    options.OperationFilter<AuthorizeOperationFilter>();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Auhtorization header is Bearer",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"

    });

    var info = new OpenApiInfo()
    {
        Version = "1",
        Title = "BET Online",
        Description = "Online Shopping Dev Test API",
        Contact = new OpenApiContact() { Name = "Siphamandla", Email = "sphah@gmail.com" },
        License = new OpenApiLicense() { Name = "No license required" }
    };
    options.SwaggerDoc("v1", info);
});

//allow calls from any front end if port number is not known
builder.Services.AddCors(options =>
{
    options.AddPolicy(
      "CorsPolicy",
      builder => builder
                .AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true) // allow any origin
               .AllowCredentials()
               .SetPreflightMaxAge(TimeSpan.FromSeconds(60))
      );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
