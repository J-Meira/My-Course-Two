using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((builderContext, loggerConfiguration) =>
{
  loggerConfiguration
    .WriteTo.MSSqlServer(
    builderContext.Configuration["ConnectionStrings:Default"],
      sinkOptions: new MSSqlServerSinkOptions()
      {
        AutoCreateSqlDatabase = true,
        SchemaName = "MySchema",
        TableName = "LogEvents",
      }
    )
    .WriteTo.Console();
});

builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddExceptionHandler<DefaultExceptionHandler>();

builder.Services.AddSqlServer<AppDbContext>(
  builder.Configuration["ConnectionStrings:Default"],
  builder => builder.EnableRetryOnFailure()
);

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
  configure.AddPolicy("EmployeePolicy", configurePolicy=>
    configurePolicy
      .RequireAuthenticatedUser()
      .RequireClaim("employeeRegistration")
  );
  configure.AddPolicy("ClientPolicy", configurePolicy=>
    configurePolicy
      .RequireAuthenticatedUser()
      .RequireClaim("nationalRegistrationNumber")
  );
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
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.UseExceptionHandler(opt => { });

app.MapGet("/Exception", () =>
{
  throw new NotImplementedException();
}).ExcludeFromDescription(); ;

app.MapGet("/Timeout", () => {
  throw new TimeoutException();
}).ExcludeFromDescription(); ;


app.Run();
