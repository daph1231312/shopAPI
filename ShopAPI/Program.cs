using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShopAPI.Data;
using ShopAPI.Middleware;
using ShopAPI.Repositories;
using ShopAPI.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=shop.db"));

// ── Repositories (Dependency Injection) ──────────────────────────────────────
// We register the interface → concrete class.
// Controllers ask for the interface; DI provides the class. Easy to swap for tests.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILinkService, LinkService>();

// ── Controllers + JSON ───────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ── Caching ───────────────────────────────────────────────────────────────────
builder.Services.AddResponseCaching();

// ── CORS (allows React dev server on port 5173) ───────────────────────────────
builder.Services.AddCors(options =>
    options.AddPolicy("AllowReact", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()));

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShopAPI",
        Version = "v1",
        Description = "E-commerce REST API — Richardson Maturity Level 4 (HATEOAS)"
    });
    // Include XML comments in Swagger UI
    var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xml));
});

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// Auto-apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseResponseCaching();
app.UseCors("AllowReact");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopAPI v1"));
}

app.MapControllers();
app.Run();

// Make Program class accessible to test project
public partial class Program { }
