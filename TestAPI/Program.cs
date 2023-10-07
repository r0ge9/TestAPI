using Microsoft.EntityFrameworkCore;
using TestAPI.Model;
using TestAPI.Services;
using TestAPI.Services.Interfaces;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestAPI.Auth;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config=>
{
    var xmlFile=$"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    config.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddSingleton<IAPIUserRepo,APIUserRepo>();
builder.Services.AddSingleton<ITokenService>(new TestAPI.Auth.TokenService());
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.ConfigureLogging(logging =>
{
    logging.AddSerilog();
    logging.SetMinimumLevel(LogLevel.Information);
})
    .UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/auth", [AllowAnonymous] async (HttpContext context,
    ITokenService tokenService, IAPIUserRepo userRepo) =>
{
    APIUser user = new()
    {
        UserName = context.Request.Query["Username"],
        Password = context.Request.Query["Password"],

    };
    var APIUserDto = userRepo.GetAPIUser(user);
    if (APIUserDto == null) return Results.Unauthorized();
    var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"],
        builder.Configuration["Jwt:Issuer"], APIUserDto);
    return Results.Ok(token);
});

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
