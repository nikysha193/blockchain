using Npgsql;

var builder = WebApplication.CreateBuilder(args);


// Получаем строку подключения к PostgreSQL
// из appsettings.json.
string connectionString =
    builder.Configuration.GetConnectionString("PostgreSQL")!;


// Разрешаем Client отправлять запросы на Server.
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


// Подключаем CORS.
app.UseCors("ClientPolicy");


// ==========================================
// TEST DATABASE API
// ==========================================

// Проверяем соединение Server с PostgreSQL.
app.MapGet("/api/test-db", async () =>
{
    await using var connection =
        new NpgsqlConnection(connectionString);

    await connection.OpenAsync();

    return Results.Ok("Database connected");
});


// ==========================================
// LOGIN API
// ==========================================

app.MapPost("/api/login", async (LoginRequest request) =>
{
    // Подключаемся к PostgreSQL.
    await using var connection =
        new NpgsqlConnection(connectionString);

    await connection.OpenAsync();


    // Ищем пользователя по Username.
    string sql = @"
        SELECT password_hash
        FROM users
        WHERE username = @username;
    ";


    await using var command =
        new NpgsqlCommand(sql, connection);


    // Передаём Username безопасно через параметр.
    command.Parameters.AddWithValue(
        "username",
        request.Username
    );


    // Получаем password_hash пользователя.
    var result =
        await command.ExecuteScalarAsync();


    // Если такого Username нет.
    if (result == null)
    {
        return Results.Unauthorized();
    }


    string passwordHash = result.ToString()!;


    // Проверяем введённый пароль
    // с hash из Database.
    bool passwordCorrect =
        BCrypt.Net.BCrypt.Verify(
            request.Password,
            passwordHash
        );


    // Если пароль неправильный.
    if (!passwordCorrect)
    {
        return Results.Unauthorized();
    }


    // Username и Password правильные.
    return Results.Ok(new
    {
        success = true,
        message = "Login successful"
    });
});


// ==========================================
// REGISTER API
// ==========================================

app.MapPost("/api/register", async (RegisterRequest request) =>
{
    // Проверяем, что все поля заполнены.
    if (string.IsNullOrWhiteSpace(request.Username) ||
        string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new
        {
            message = "All fields are required"
        });
    }


    // Превращаем обычный пароль в hash.
    string passwordHash =
        BCrypt.Net.BCrypt.HashPassword(request.Password);


    // Подключаемся к PostgreSQL.
    await using var connection =
        new NpgsqlConnection(connectionString);

    await connection.OpenAsync();


    // Добавляем нового пользователя.
    string sql = @"
        INSERT INTO users
            (username, email, password_hash)
        VALUES
            (@username, @email, @passwordHash);
    ";


    await using var command =
        new NpgsqlCommand(sql, connection);


    command.Parameters.AddWithValue(
        "username",
        request.Username
    );

    command.Parameters.AddWithValue(
        "email",
        request.Email
    );

    command.Parameters.AddWithValue(
        "passwordHash",
        passwordHash
    );


    try
    {
        // Выполняем INSERT.
        await command.ExecuteNonQueryAsync();


        return Results.Ok(new
        {
            success = true,
            message = "Registration successful"
        });
    }

    catch (PostgresException exception)
        when (exception.SqlState == "23505")
    {
        // Username или Email уже существует.
        return Results.Conflict(new
        {
            message = "Username or email already exists"
        });
    }
});


// ==========================================
// START SERVER
// ==========================================

// Это должно быть после всех API.
app.Run("http://localhost:5000");


// ==========================================
// LOGIN REQUEST
// ==========================================

// Данные, которые Client отправляет при Login.
public class LoginRequest
{
    public string Username { get; set; } = "";

    public string Password { get; set; } = "";
}


// ==========================================
// REGISTER REQUEST
// ==========================================

// Данные, которые Client отправляет при Register.
public class RegisterRequest
{
    public string Username { get; set; } = "";

    public string Email { get; set; } = "";

    public string Password { get; set; } = "";
}