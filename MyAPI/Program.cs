using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDBContext>(builder.Configuration["ConnectionStrings:Default"]);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

List<string> products = new List<string>(
 ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"]
);

app.MapGet("/products", (ApplicationDBContext context) =>
{
  return Results.Ok(context.Products
    .Include(p => p.Category)
    .Include(p => p.Tags)
    .ToList());
});
// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.MapGet("/products/{id}", (int id, ApplicationDBContext context) =>
{
  Product? product = context.Products
    .Include(p => p.Category)
    .Include(p => p.Tags)
    .Where(p => p.Id == id)
    .FirstOrDefault();
  return product != null ? Results.Ok(product) : Results.BadRequest("Product not found");
});

app.MapPost("/products", (ProductDTO product, ApplicationDBContext context) =>
{
  Category? category = context.Categories.Where(c => c.Id == product.CategoryId).FirstOrDefault<Category>();
  if (category == null) return Results.BadRequest("Category invalid");
  Product productDb = new Product()
  {
    Code = product.Code,
    Name = product.Name,
    Description = product.Description,
    Category = category
  };
  if (product.Tags != null)
  {
    productDb.Tags = new List<Tag>();
    foreach (string item in product.Tags)
    {
      productDb.Tags.Add(new Tag { Name = item });
    }
  }
  context.Products.Add(productDb);
  context.SaveChanges();
  return Results.Created();
});

app.MapPut("/products/{id}", (ProductDTO product, int id, ApplicationDBContext context) =>
{
  Product? productDb = context.Products
    .Include(p => p.Tags)
    .Where(p => p.Id == id)
    .FirstOrDefault();
  if (productDb == null) return Results.BadRequest("Product not found");

  Category? category = context.Categories.Where(c => c.Id == product.CategoryId).FirstOrDefault<Category>();
  if (category == null) return Results.BadRequest("Category invalid");

  productDb.Code = product.Code;
  productDb.Name = product.Name;
  productDb.Description = product.Description;
  productDb.Category = category;
  productDb.Tags = new List<Tag>();
  if (product.Tags != null)
  {
    foreach (string item in product.Tags)
    {
      productDb.Tags.Add(new Tag { Name = item });
    }
  }
  context.SaveChanges();
  return Results.Ok();
});

app.MapDelete("/products/{id}", (int id, ApplicationDBContext context) =>
{
  Product? productDb = context.Products
    .Where(p => p.Id == id)
    .FirstOrDefault();
  if (productDb == null) return Results.BadRequest("Product not found");

  context.Products.Remove(productDb);  
  context.SaveChanges();
  return Results.Ok();
  
});

app.Run();
