using BudgetPlanning = Application.Features.BudgetPlanning;
using BudgetTracking = Application.Features.BudgetTracking;
using Application.Shared.Persistence;
using Components;
using Components.Shared;
using Microsoft.EntityFrameworkCore;
using Radzen;
using WebServerApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Radzen Services
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ThemeService>();

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<AppStateManager>();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("TrackerDb")));
builder.Services.AddScoped<BudgetPlanning.GetBudget.GetBudgetHandler>();
builder.Services.AddScoped<BudgetPlanning.GetCategory.GetCategoryHandler>();
builder.Services.AddScoped<BudgetPlanning.CreateBudget.CreateBudgetHandler>();
builder.Services.AddScoped<BudgetPlanning.UpdateBudget.UpdateBudgetHandler>();
builder.Services.AddScoped<BudgetPlanning.DeleteBudget.DeleteBudgetHandler>();
builder.Services.AddScoped<BudgetTracking.GetCategory.GetCategoryHandler>();
builder.Services.AddScoped<BudgetTracking.GetTransaction.Handler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Dashboard).Assembly);

app.Run();
