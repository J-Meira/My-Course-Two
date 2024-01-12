using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyAPI.Entities.Categories;
using MyAPI.Entities.Employees;
using MyAPI.Infra.DataBase;
using System.Text;

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
builder.Services.AddSwaggerGen(setup =>
{
  // Include 'SecurityScheme' to use JWT Authentication
  var jwtSecurityScheme = new OpenApiSecurityScheme
  {
    BearerFormat = "JWT",
    Name = "JWT Authentication",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = JwtBearerDefaults.AuthenticationScheme,
    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

    Reference = new OpenApiReference
    {
      Id = JwtBearerDefaults.AuthenticationScheme,
      Type = ReferenceType.SecurityScheme
    }
  };

  setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

  setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});

string? tokenKeyString = builder.Configuration.GetSection("JwtBearerTokenSettings:SecretKey").Value;

builder.Services.AddAuthorization(configure =>
{

});
builder.Services.AddAuthentication(defaultScheme =>
{
  defaultScheme.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  defaultScheme.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(authenticationScheme =>
{
  authenticationScheme.TokenValidationParameters = new TokenValidationParameters()
  {
    ValidateActor = true,
    ValidateAudience = true,
    ValidateIssuer = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ClockSkew = TimeSpan.Zero,
    ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
    ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(tokenKeyString ?? ""))
  };
});


builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
