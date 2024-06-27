using BuckleUp.DatabaseContext;
using BuckleUp.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddTransient<ITenantInfo, TenantInfo>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

builder.Services.AddTransient<ITenantInfo, Tenant>();
builder.Services.AddTransient<Tenant>();



builder.Services.AddDbContext<TenantDbContext>(options =>
            options.UseSqlServer("Data Source=MAMTA\\SQLEXPRESS;database = BuckleUp; Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"));

builder.Services.AddMultiTenant<Tenant>()
    .WithHeaderStrategy("TenantId")
    .WithEFCoreStore<TenantDbContext, Tenant>();


builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    options.UseSqlServer("Data Source=MAMTA\\SQLEXPRESS;database = BuckleUp; Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

app.UseMiddleware<TenantMiddleware>();

app.UseMultiTenant();
//app.Use(async (context, next) =>
//{
//    var tenantContext = context.GetMultiTenantContext<Tenant>();
//    if (tenantContext?.TenantInfo == null)
//    {
//        // Handle scenario where tenant info is not found or invalid
//        context.Response.StatusCode = 400; // Bad Request
//        await context.Response.WriteAsync("Tenant information is missing or invalid.");
//        return;
//    }

//    // Proceed with next middleware or request handling
//    await next();
//});

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// In Program.cs

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
