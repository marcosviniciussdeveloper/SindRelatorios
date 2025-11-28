using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using SindRelatorios.Application;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Application.Providers;
using SindRelatorios.Infrastructure.Data;
using SindRelatorios.Infrastructure.Repositories;
using SindRelatorios.Infrastructure.Services;
using SindRelatorios.Components;
using SindRelatorios.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Blazor com Erros Detalhados 
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => options.DetailedErrors = true);

// 2. Injeção de Dependências
builder.Services.AddHttpClient();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<ICourseTemplateProvider, CourseTemplateProvider>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IOpeningService, OpeningService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<IOpeningRepository, OpeningRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();

// Repositório Genérico
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Banco de Dados
builder.Services.AddDbContext<SindDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Configuração de Proxy (Para Fly.io / Docker)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();

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