using BuckleUp.DatabaseContext;
using BuckleUp.Models;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ITenantInfo, TenantInfo>();
builder.Services.AddTransient<ITenantInfo, Tenant>();
builder.Services.AddScoped<IMultiTenantStore<Tenant>, CustomTenantStore>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var multiTenantContextAccessor = serviceProvider.GetRequiredService<IMultiTenantContextAccessor<Tenant>>();
    var tenantInfo = multiTenantContextAccessor.MultiTenantContext?.TenantInfo;
    options.UseSqlServer("ConnectionString");
    options.EnableSensitiveDataLogging(); // Enable sensitive data logging for debugging

    // Additional DbContext configuration if needed
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMultiTenant<Tenant>()
    .WithHeaderStrategy("TenantId")
    .WithStore<CustomTenantStore>(ServiceLifetime.Scoped);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMultiTenant();

// In Program.cs

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
