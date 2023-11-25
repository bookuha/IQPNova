using System.Reflection;
using System.Text;
using FluentValidation;
using IQP.Application.Services;
using IQP.Application.Services.Validators;
using IQP.Domain.Entities;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using IQP.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<IqpDbContext>(o=>
    o.UseNpgsql("Server=127.0.0.1;Port=5432;Database=iqp;User Id=postgres;Password=oreooreo12;"));

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<IqpDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = builder.Configuration["Auth:AllowedUserNameCharacters"]!;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
});

builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.SaveToken = true;
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
        };
    });

builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddSwaggerGen(o =>
        {
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
            });
            o.MapType<ProblemDetails>(() => new OpenApiSchema
            {
                Type = "object",
                Properties =
                {
                    {"type", new OpenApiSchema {Type = "string"}},
                    {"title", new OpenApiSchema {Type = "string"}},
                    {"status", new OpenApiSchema {Type = "integer", Format = "int32"}},
                    {"detail", new OpenApiSchema {Type = "string"}},
                    {"traceId", new OpenApiSchema{Type = "string"}},
                },
            });
            o.MapType<ValidationProblemDetails>(() => new OpenApiSchema
            {
                Type = "object",
                Properties =
                {
                    {"type", new OpenApiSchema {Type = "string"}},
                    {"title", new OpenApiSchema {Type = "string"}},
                    {"status", new OpenApiSchema {Type = "integer", Format = "int32"}},
                    {"detail", new OpenApiSchema {Type = "string"}},
                    {"traceId", new OpenApiSchema {Type = "string"}},
                    {"errors", new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            AdditionalProperties = new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema { Type = "string" }
                            }
                        }
                    }
                },
            });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
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

builder.Services.AddAuthorization();

builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryCommandValidator>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<CategoriesService>();


builder.Services.AddControllers();
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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();