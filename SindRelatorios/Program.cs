using Microsoft.EntityFrameworkCore;
using SindRelatorios.Application;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Application.Providers;
using SindRelatorios.Infrastructure.Data;
using SindRelatorios.Infrastructure.Repositories;
using SindRelatorios.Infrastructure.Services;
using SindRelatorios.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<ICourseTemplateProvider, CourseTemplateProvider>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddDbContext<SindDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

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