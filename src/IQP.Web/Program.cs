using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using IQP.Web.Middlewares;
using IQP.Web.Modules;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<IqpDbContext>(o =>
    o.UseNpgsql(builder.Configuration["PostgresConnectionString"]));

builder.Services.AddAuth(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();

builder.Services.AddCodeExecutionServices(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
        policyBuilder =>
            policyBuilder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<IqpDbContext>();
await dbContext.Database.MigrateAsync();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();