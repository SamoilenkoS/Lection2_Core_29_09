using Lection2_Core_BL.Profiles;
using Lection2_Core_BL.Services;
using Lection2_Core_DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<EfDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<GoodsService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddControllers();
var assemblies = new[]
{
    typeof(GoodProfile).Assembly,
    typeof(UserProfile).Assembly
};
builder.Services.AddAutoMapper(assemblies);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
