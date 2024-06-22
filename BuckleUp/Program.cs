using BuckleUp.DatabaseContext;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ITenantInfo, TenantInfo>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var multiTenantContextAccessor = serviceProvider.GetRequiredService<IMultiTenantContextAccessor<TenantInfo>>();
    var tenantInfo = multiTenantContextAccessor.MultiTenantContext?.TenantInfo;
    options.UseInMemoryDatabase("InMemory");
    options.EnableSensitiveDataLogging(); // Enable sensitive data logging for debugging

    // Additional DbContext configuration if needed
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMultiTenant<TenantInfo>()
    .WithHeaderStrategy("TenantId")
    .WithInMemoryStore(options =>
    {
        options.Tenants.Add(new TenantInfo { Id = "1", Identifier = "Apple", Name = "App" });
        options.Tenants.Add(new TenantInfo { Id = "2", Identifier = "Samsung", Name = "Sam"});
    });

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
