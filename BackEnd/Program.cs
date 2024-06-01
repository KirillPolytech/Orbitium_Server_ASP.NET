using Microsoft.EntityFrameworkCore;
using BackEndForGame.Configuration;
using BackEndForGame.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// jwt
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
     new OpenApiSecurityScheme
     {
       Reference = new OpenApiReference
       {
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer"
       }
      },
      new string[] { }
    }
  });
});
//

builder.Services.AddTransient<UserService, UserService>();
builder.Services.AddTransient<PlayerService, PlayerService>();
builder.Services.AddTransient<LeaderBoardService, LeaderBoardService>();
builder.Services.AddTransient<ItemService, ItemService>();
builder.Services.AddTransient<InventoryService, InventoryService>();
builder.Services.AddTransient<ItemStorageService, ItemStorageService>();
builder.Services.AddTransient<JwtService, JwtService>();

builder.Services.Configure<ConnectionStringsOptions>( builder.Configuration.GetSection("ConnectionStrings") );
builder.Services.Configure<JwtAuthOptions>( builder.Configuration.GetSection("JwtAuthentication") );

//
string con = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ConnectionStringsOptions>>().Value.DbConString;
builder.Services.AddDbContext<OrbitiumDataBaseContext>( options => { options.UseMySQL(con); } ) ;
//

// JWT auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOptions =>
{
    var config = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JwtAuthOptions>>().Value;

    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = config.Issuer,
        ValidAudience = config.Audience,        
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.key)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = false
    };
});
//

builder.Services.AddAuthorization();

var app = builder.Build();

//DB
using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<OrbitiumDataBaseContext>();

dbContext?.Database.EnsureCreated();
dbContext?.Database.Migrate();
//

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();