using System.Text.Json;
using Npgsql;

namespace Server.Api;

public class ApiHandler
{
    private readonly string connectionString;

    public ApiHandler(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<string> HandleAsync(
        string action,
        JsonElement data
    )
    {
        if (action == "login")
        {
            return await LoginAsync(data);
        }

        if (action == "register")
        {
            return await RegisterAsync(data);
        }

        return """
        {
            "success": false,
            "status": 400,
            "message": "Unknown action"
        }
        """;
    }

    private async Task<string> LoginAsync(JsonElement data)
    {
        string username =
            data.GetProperty("username").GetString() ?? "";

        string password =
            data.GetProperty("password").GetString() ?? "";

        // Server сам проверяет,
        // что Username и Password не пустые.
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            return """
            {
                "success": false,
                "status": 400,
                "message": "Username and password are required"
            }
            """;
        }

        await using var connection =
            new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        string sql = @"
            SELECT password_hash
            FROM users
            WHERE username = @username;
        ";

        await using var command =
            new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "username",
            username
        );

        var result =
            await command.ExecuteScalarAsync();

        // Пользователь с таким Username не найден.
        if (result == null)
        {
            return """
            {
                "success": false,
                "status": 401,
                "message": "Incorrect username or password"
            }
            """;
        }

        string passwordHash =
            result.ToString()!;

        bool passwordCorrect =
            BCrypt.Net.BCrypt.Verify(
                password,
                passwordHash
            );

        // Пароль неправильный.
        if (!passwordCorrect)
        {
            return """
            {
                "success": false,
                "status": 401,
                "message": "Incorrect username or password"
            }
            """;
        }

        // Login успешный.
        return """
        {
            "success": true,
            "status": 200,
            "message": "Login successful"
        }
        """;
    }

    private async Task<string> RegisterAsync(JsonElement data)
    {
        string username =
            data.GetProperty("username").GetString() ?? "";

        string email =
            data.GetProperty("email").GetString() ?? "";

        string password =
            data.GetProperty("password").GetString() ?? "";

        // Server сам проверяет,
        // что все обязательные поля заполнены.
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return """
            {
                "success": false,
                "status": 400,
                "message": "All fields are required"
            }
            """;
        }

        // Обычный пароль превращаем в BCrypt hash.
        string passwordHash =
            BCrypt.Net.BCrypt.HashPassword(password);

        await using var connection =
            new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

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
            username
        );

        command.Parameters.AddWithValue(
            "email",
            email
        );

        command.Parameters.AddWithValue(
            "passwordHash",
            passwordHash
        );

        try
        {
            await command.ExecuteNonQueryAsync();

            return """
            {
                "success": true,
                "status": 200,
                "message": "Registration successful"
            }
            """;
        }
        catch (PostgresException exception)
            when (exception.SqlState == "23505")
        {
            // PostgreSQL code 23505 =
            // нарушено UNIQUE ограничение.
            // Значит Username или Email уже существует.
            return """
            {
                "success": false,
                "status": 409,
                "message": "Username or email already exists"
            }
            """;
        }
    }
}