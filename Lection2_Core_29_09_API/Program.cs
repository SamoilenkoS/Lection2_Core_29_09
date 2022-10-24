using Lection2_Core_API.Middlewares;
using Lection2_Core_BL.Options;
using Lection2_Core_BL.Profiles;
using Lection2_Core_BL.Services;
using Lection2_Core_BL.Services.GeneratorService;
using Lection2_Core_BL.Services.HashService;
using Lection2_Core_BL.Services.QueryService;
using Lection2_Core_BL.Services.SmtpService;
using Lection2_Core_BL.Services.TokenService;
using Lection2_Core_DAL;
using Lection2_Core_DAL.Interfaces;
using Lection2_Core_DAL.RolesHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, _, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddScoped<ISmtpService, MockSmtpService>();
builder.Services.AddSingleton<IGeneratorService, GeneratorService>();
builder.Services.AddSingleton<IQueryService, QueryService>();
builder.Services.AddSingleton<IHashService, HashService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IRolesHelper, RolesHelper>();
builder.Services.AddDbContext<EfDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<GoodsService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(typeof(IBasicGenericRepository<>), typeof(BasicGenericRepository<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection(nameof(AuthOptions)));
builder.Services.Configure<HashingOptions>(
    builder.Configuration.GetSection(nameof(HashingOptions)));
builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection(nameof(SmtpOptions)));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            builder.Configuration.GetSection(nameof(AuthOptions))["Key"]))
    };
});
builder.Services.AddControllers();
var assemblies = new[]
{
    typeof(GoodProfile).Assembly,
    typeof(UserProfile).Assembly
};
builder.Services.AddAutoMapper(assemblies);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                    Enter 'Bearer' [space] and then your token in the text input below.
                    \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

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
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CustomErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
