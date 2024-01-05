using MyAPI.Entities.Categories;
using MyAPI.Infra.DataBase;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<AppDbContext>(builder.Configuration["ConnectionStrings:Default"]);

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
