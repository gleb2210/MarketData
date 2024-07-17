using WebAssessment.Models;
using WebAssessment.SignalR;
using WebAssessment.Simulator;
using WebAssessment.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTelerikBlazor();
builder.Services.AddSingleton<IMarketSimulator, MarketSimulator>();
builder.Services.AddScoped<MarketCache>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<MarketHub>();


var allowedOrigin = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("myAppCors", policy =>
    {
        policy.WithOrigins(allowedOrigin)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("myAppCors");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapHub<MarketHub>("/market-hub");

app.Run();
