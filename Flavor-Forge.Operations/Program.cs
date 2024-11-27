using Flavor_Forge.Entities;
using Flavor_Forge.Operations.Services;
using Flavor_Forge.Services;
using Flavor_Forge.Services.Repository;
using Flavor_Forge.Services.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Register Services
builder.Services.AddScoped<IUserServices, UserRepository>();
builder.Services.AddScoped<IRecipeServices, RecipeRepository>();
builder.Services.AddScoped<IIngredientServices, IngredientRepository>();
builder.Services.AddScoped<IAuthServices, AuthRepository>();
builder.Services.AddScoped<ICookiesServices, CookiesRepository>();
builder.Services.AddScoped<IImageServices, ImageRepository>();
builder.Services.AddHttpClient<ITheMealDbServices, TheMealDbRepository>();

// Register DB
builder.Services.AddDbContext<Flavor_ForgeDBContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConn")));

// For handling cookies
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<Flavor_ForgeDBContext>();

    // Ensure the database is created
    context.Database.EnsureCreated();

    // Add default admin user if not exists
    if (!context.Users.Any(u => u.Username == "admin"))
    {
        context.Users.Add(new User
        {
            Username = "admin",
            Password = HashPassword("admin123"), // Hash the password
        });
        context.SaveChanges();
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
/// <summary>
/// Hashes a password using SHA256. Replace this with a stronger hashing mechanism (e.g., BCrypt) in production.
/// </summary>
string HashPassword(string password)
{
    using var sha256 = System.Security.Cryptography.SHA256.Create();
    var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(bytes);
}