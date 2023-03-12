using AutoMapper;
using DataManagerAPI.Helpers;
using DataManagerAPI.Middleware;
using DataManagerAPI.NLogger.Extensions;
using DataManagerAPI.Repository;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services;
using DataManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

    var filePath = Path.Combine(AppContext.BaseDirectory, "DataManagerAPI.xml");
    c.IncludeXmlComments(filePath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme.
Enter token in the text input below.
Example: ""1safsfsdfdfd""",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddSelectedDBContext(builder.Configuration);

// Auto Mapper Configurations
MapperConfiguration mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUserDataService, UserDataService>();

builder.Services.AddScoped<IUserFilesService, UserFilesService>();

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IUserPasswordService, UserPasswordService>();
builder.Services.AddSingleton<ILoggedOutUsersCollectionService, LoggedOutUsersCollectionservice>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
                      policy.RequireClaim("Role", RoleIds.Admin.ToString()));
    options.AddPolicy("PowerUser", policy =>
                      policy.RequireClaim("Role", RoleIds.Admin.ToString(), RoleIds.PowerUser.ToString()));
    options.AddPolicy("User", policy =>
                      policy.RequireClaim("Role", RoleIds.Admin.ToString(), RoleIds.PowerUser.ToString(), RoleIds.User.ToString()));
    options.AddPolicy("ReadOnlyUser", policy =>
                      policy.RequireClaim("Role", RoleIds.Admin.ToString(), RoleIds.PowerUser.ToString(), RoleIds.ReadOnlyUser.ToString()));
});

builder.Services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, options => { });

builder.Services.AddProblemDetails();

builder.SetupNLogConfiguration();

WebApplication app = builder.Build();

//app.UseHttpLogging();   // enable HTTP request/response logging

app.UseNLogConfiguration();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Used for integration tests.
/// </summary>
public partial class Program { }
