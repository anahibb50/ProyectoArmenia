using LigaArmenia.Application;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Infrastructure;
using LigaArmenia.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Composición de capas
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebSecurity(builder.Configuration, builder.Environment);
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Inicialización controlada del administrador inicial (solo si AdminSeed:Enabled = true)
using (var scope = app.Services.CreateScope())
{
    var adminSeeder = scope.ServiceProvider.GetRequiredService<IAdminSeeder>();
    await adminSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
