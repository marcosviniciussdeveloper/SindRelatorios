using SindRelatorios.Application;
using SindRelatorios.Application.Providers;
using SindRelatorios.Components;
using SindRelatorios.Infrastructure;
using SindRelatorios.Providers; 
using SindRelatorios.Services; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



//  Registra o HttpClientFactory (para o HolidayService poder fazer chamadas API)
builder.Services.AddHttpClient(); 

//Injeção de Dependência
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<ICourseTemplateProvider, CourseTemplateProvider>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();