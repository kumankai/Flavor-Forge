using Flavor_Forge.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Register Services
builder.Services.AddScoped<IUserServices, UserRepository>();
builder.Services.AddScoped<IRecipeServices, RecipeRepository>();
builder.Services.AddScoped<IIngredientServices, IngredientRepository>();
builder.Services.AddScoped<IAuthServices, AuthRepository>();

// Register DB
builder.Services.AddDbContext<Flavor_ForgeDBContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConn")));

var app = builder.Build();

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
