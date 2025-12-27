using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPagesBook.Data;
using RazorPagesBook.Models;
using RazorPagesBook.Services;
using RazorPagesMovie.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RazorPagesBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RazorPagesBookContext")));

builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<RazorPagesBookContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "RequireAdministratorRole");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin"));
});

builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapGet("/health", async (HttpContext context) => {
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
    try
    {
        return Results.Ok(new { status = "ok" });
    }
    catch (OperationCanceledException)
    {
        return Results.StatusCode(503);
    }
});


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.Use(async (context, next) => {
    var requestId = Guid.NewGuid().ToString();
    
    context.Response.Headers["X-Request-Id"] = requestId;
    await next();
});


app.UseExceptionHandler(errorApp => {
    errorApp.Run(async context => {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var requestId = context.Response.Headers["X-Request-Id"];

        await context.Response.WriteAsJsonAsync(new
        {
            error = "Internal Server Error",
            code = 500,
            requestId = requestId.ToString()
        });
    });
});

app.MapRazorPages().WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAdminAsync(scope.ServiceProvider, app.Configuration);
}

app.Run();
