using System.Security.Claims;
using ClientCSharp;
using ClientCSharp.Auth;
using ClientCSharp.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder =
    WebApplication.CreateBuilder(args);


// Подключаем Blazor с интерактивными C# компонентами.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Общее состояние языка и темы.
builder.Services.AddScoped<AppState>();


// Хранилище одноразовых Login Ticket.
builder.Services.AddSingleton<LoginTicketStore>();


// Подключаем Cookie Authentication.
builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme
    )
    .AddCookie(options =>
    {
        // Имя нашей cookie в браузере.
        options.Cookie.Name =
            "SecureDocumentsAuth";

        // Cookie нельзя читать через JavaScript.
        options.Cookie.HttpOnly = true;

        // Срок Remember Me.
        options.ExpireTimeSpan =
            TimeSpan.FromDays(30);

        // При активном использовании
        // срок действия может продлеваться.
        options.SlidingExpiration = true;
    });


// Разрешаем приложению работать с авторизацией.
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true
    );

    app.UseHsts();
}


app.UseHttpsRedirection();


// Включаем Cookie Authentication.
app.UseAuthentication();


// Включаем Authorization.
app.UseAuthorization();


app.UseAntiforgery();

app.MapStaticAssets();


// ==========================================
// CREATE LOGIN COOKIE
// ==========================================

app.MapGet(
    "/auth/login",
    async (
        HttpContext httpContext,
        string ticket,
        LoginTicketStore ticketStore
    ) =>
    {
       if (!ticketStore.TryUseTicket(
            ticket,
            out LoginTicket loginTicket
        ))
    {
        return Results.Redirect("/");
    }


        // Записываем Username внутрь
        // защищённой authentication cookie.
        var claims =
    new List<Claim>
    {
        new Claim(
            ClaimTypes.Name,
            loginTicket.Username
        ),

        new Claim(
            "language",
            loginTicket.Language
        ),

        new Claim(
            "darkTheme",
            loginTicket.DarkTheme.ToString()
        ),

        new Claim(
            "sessionToken",
            loginTicket.SessionToken
        )
    };


        var identity =
            new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults
                    .AuthenticationScheme
            );


        var principal =
            new ClaimsPrincipal(identity);


        // Создаём cookie.
        await httpContext.SignInAsync(
            CookieAuthenticationDefaults
                .AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent =
                    loginTicket.RememberMe
            }
        );


        // После создания cookie
        // возвращаем пользователя на Login page.
        return Results.Redirect("/");
    }
);


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();