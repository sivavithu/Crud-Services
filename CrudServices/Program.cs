using CrudService.Data;
using CrudService.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PdfSharp.Fonts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<BookDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookDatabase")));
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway",
        policy =>
        {
            policy.AllowAnyOrigin()  // Or specific origins like "http://localhost:5000"
                  .AllowAnyMethod()  // Includes OPTIONS, POST
                  .AllowAnyHeader();
        });
});
GlobalFontSettings.FontResolver = new CustomFontResolver();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Key"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Recommended for no clock skew tolerance
        };
    });

// Corrected: Only one call to AddAuthorization is needed
builder.Services.AddAuthorization();
GlobalFontSettings.FontResolver = new CustomFontResolver();
var app = builder.Build();

// Configure the HTTP request pipeline.

// Use static files from the 'wwwroot' folder. This is necessary for IWebHostEnvironment.WebRootPath to be populated.
app.UseStaticFiles();

//app.UseHttpsRedirection(); // Uncomment if you have HTTPS set up
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
