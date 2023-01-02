using AutoMapper;
using DataManagerAPI.Helpers;
using DataManagerAPI.Middleware;
using DataManagerAPI.Models;
using DataManagerAPI.Repository;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DataManagerAPI"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter token in the text input below.\r\n\r\nExample: \"1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<UsersDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Auto Mapper Configurations
MapperConfiguration mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<IUserDataService, UserDataService>();
builder.Services.AddScoped<IUserDataRepository, UserDataRepository>();

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IUserPasswordService, UserPasswordService>();

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Admin", policy =>
                          policy.RequireClaim("Role", RoleIds.Admin.ToString()));
        options.AddPolicy("PowerUser", policy =>
                          policy.RequireClaim("Role", RoleIds.Admin.ToString(), RoleIds.PowerUser.ToString()));
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = ValidationTokenHelper.CreateTokenValidationParameters(useLifetime: true);
    });

WebApplication app = builder.Build();

//app.UseHttpLogging();   // enable HTTP request/response logging

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
