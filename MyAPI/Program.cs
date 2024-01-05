using Microsoft.AspNetCore.Identity;
using MyAPI.Entities.Categories;
using MyAPI.Infra.DataBase;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<AppDbContext>(builder.Configuration["ConnectionStrings:Default"]);
builder.Services.AddIdentity<IdentityUser, IdentityRole>(setupAction =>
{
  setupAction.Password.RequireNonAlphanumeric = false;
  setupAction.Password.RequireDigit = false;
  setupAction.Password.RequireLowercase = false;
  setupAction.Password.RequireUppercase = false;
  setupAction.Password.RequiredLength = 10;
})
  .AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
