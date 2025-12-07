using BP = Application.Features.BudgetPlanning;
using SBP = Application.Schema.BudgetPlanning;
using BT = Application.Features.BudgetTracking;
using SBT = Application.Schema.BudgetTracking;
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

builder.Services.AddScoped<SBP.GetBudget.IHandler, BP.GetBudgetHandler>();
builder.Services.AddScoped<SBP.GetBudgetItem.IHandler, BP.GetBudgetItemHandler>();
builder.Services.AddScoped<SBP.CreateBudget.IHandler, BP.CreateBudgetHandler>();
builder.Services.AddScoped<SBP.UpdateBudget.IHandler, BP.UpdateBudgetHandler>();
builder.Services.AddScoped<SBP.DeleteBudget.IHandler, BP.DeleteBudgetHandler>();

builder.Services.AddScoped<SBT.GetBudget.IHandler, BT.GetBudgetHandler>();
builder.Services.AddScoped<SBT.GetTransaction.IHandler, BT.GetTransactionHandler>();
builder.Services.AddScoped<SBT.GetAccount.IHandler, BT.GetAccountHandler>();
builder.Services.AddScoped<SBT.CreateTransaction.IHandler, BT.CreateTransactionHandler>();
builder.Services.AddScoped<SBT.UpdateTransaction.IHandler, BT.UpdateTransactionHandler>();
builder.Services.AddScoped<SBT.DeleteTransaction.IHandler, BT.DeleteTransactionHandler>();

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
