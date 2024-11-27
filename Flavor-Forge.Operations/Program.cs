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