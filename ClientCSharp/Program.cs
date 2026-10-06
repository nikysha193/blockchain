using ClientCSharp;
using ClientCSharp.Components;

var builder = WebApplication.CreateBuilder(args);


// Подключаем Blazor с интерактивными C# компонентами.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Нужен для HTTP-запросов от Client к нашему Server API.
builder.Services.AddHttpClient();


// Общее состояние языка и темы.
builder.Services.AddScoped<AppState>();


var app = builder.Build();


// Обработка ошибок вне режима разработки.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}


// Перенаправляет HTTP на HTTPS.
app.UseHttpsRedirection();


// Защита Blazor-форм.
app.UseAntiforgery();


// Подключает файлы из wwwroot.
app.MapStaticAssets();


// Запускает Blazor-компоненты приложения.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();