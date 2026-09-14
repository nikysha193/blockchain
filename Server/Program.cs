var builder = WebApplication.CreateBuilder(args);

// Разрешаем нашему Client обращаться к Server
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("ClientPolicy");


// Login API
app.MapPost("/api/login", (LoginRequest request) =>
{
    
    string correctUsername = "admin";
    string correctPassword = "1234";

    if (request.Username == correctUsername &&
        request.Password == correctPassword)
    {
        return Results.Ok(new
        {
            success = true,
            message = "Login successful"
        });
    }

    return Results.Unauthorized();
});


app.Run("http://localhost:5000");


public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}