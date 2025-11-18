using SindRelatorios.Application;
using SindRelatorios.Application.Providers;
using SindRelatorios.Components;
using SindRelatorios.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SindRelatorios.Providers;
using SindRelatorios.Services; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- Serviços da Aplicação ---
builder.Services.AddHttpClient(); 
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<ICourseTemplateProvider, CourseTemplateProvider>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();

// --- Conexão com o Banco de Dados (Supabase) ---

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SindDbContext>(options =>
    options.UseNpgsql(connectionString)
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); 

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();