using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;
using Npgsql;

namespace Server.Api;

public class ApiHandler
{
    private readonly string connectionString;


    // ==========================================
    // PASSWORD RESET CODES
    // ==========================================

    // Здесь временно хранятся коды
    // восстановления пароля.
    //
    // Email -> данные о коде.
    private static readonly ConcurrentDictionary<
        string,
        PasswordResetData
    > resetCodes =
        new(StringComparer.OrdinalIgnoreCase);


    public ApiHandler(string connectionString)
    {
        this.connectionString = connectionString;
    }


    // ==========================================
    // ACTION HANDLER
    // ==========================================

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

        if (action == "forgot_password_request")
        {
            return await ForgotPasswordRequestAsync(data);
        }

        if (action == "forgot_password_verify")
        {
            return VerifyResetCode(data);
        }

        if (action == "forgot_password_reset")
        {
            return await ForgotPasswordResetAsync(data);
        }


        return """
        {
            "success": false,
            "status": 400,
            "message": "Unknown action"
        }
        """;
    }


    // ==========================================
    // LOGIN
    // ==========================================

    private async Task<string> LoginAsync(
        JsonElement data
    )
    {
        string username =
            data.GetProperty("username")
                .GetString() ?? "";

        string password =
            data.GetProperty("password")
                .GetString() ?? "";


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
            new NpgsqlConnection(
                connectionString
            );

        await connection.OpenAsync();


        string sql = @"
            SELECT password_hash
            FROM users
            WHERE username = @username;
        ";


        await using var command =
            new NpgsqlCommand(
                sql,
                connection
            );


        command.Parameters.AddWithValue(
            "username",
            username
        );


        var result =
            await command.ExecuteScalarAsync();


        // Пользователь не найден.
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


    // ==========================================
    // REGISTER
    // ==========================================

    private async Task<string> RegisterAsync(
        JsonElement data
    )
    {
        string username =
            data.GetProperty("username")
                .GetString() ?? "";

        string email =
            data.GetProperty("email")
                .GetString() ?? "";

        string password =
            data.GetProperty("password")
                .GetString() ?? "";


        username = username.Trim();
        email = email.Trim();


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


        // Пароль превращаем в BCrypt Hash.
        string passwordHash =
            BCrypt.Net.BCrypt.HashPassword(
                password
            );


        await using var connection =
            new NpgsqlConnection(
                connectionString
            );

        await connection.OpenAsync();


        string sql = @"
            INSERT INTO users
                (username, email, password_hash)
            VALUES
                (@username, @email, @passwordHash);
        ";


        await using var command =
            new NpgsqlCommand(
                sql,
                connection
            );


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
            return """
            {
                "success": false,
                "status": 409,
                "message": "Username or email already exists"
            }
            """;
        }
    }


    // ==========================================
    // FORGOT PASSWORD
    // STEP 1 - REQUEST CODE
    // ==========================================

    private async Task<string> ForgotPasswordRequestAsync(
        JsonElement data
    )
    {
        string email =
            data.GetProperty("email")
                .GetString() ?? "";


        email = email.Trim();


        if (string.IsNullOrWhiteSpace(email))
        {
            return """
            {
                "success": false,
                "status": 400,
                "message": "Email is required"
            }
            """;
        }


        await using var connection =
            new NpgsqlConnection(
                connectionString
            );

        await connection.OpenAsync();


        // Проверяем, существует ли такой Email.
        string sql = @"
            SELECT id
            FROM users
            WHERE LOWER(email) = LOWER(@email);
        ";


        await using var command =
            new NpgsqlCommand(
                sql,
                connection
            );


        command.Parameters.AddWithValue(
            "email",
            email
        );


        var result =
            await command.ExecuteScalarAsync();


        // Если пользователь существует,
        // создаём код.
        if (result != null)
        {
            // Случайный 6-значный код.
            int randomNumber =
                RandomNumberGenerator.GetInt32(
                    100000,
                    1000000
                );


            string resetCode =
                randomNumber.ToString();


            // Код действует 10 минут.
            resetCodes[email] =
                new PasswordResetData
                {
                    Code = resetCode,

                    ExpiresAt =
                        DateTimeOffset.UtcNow
                            .AddMinutes(10)
                };


            // ==================================
            // TEMPORARY DEVELOPMENT VERSION
            // ==================================

            // Пока Email-сервис ещё не подключён,
            // показываем код в консоли Server.
            Console.WriteLine();
            Console.WriteLine(
                "=================================="
            );

            Console.WriteLine(
                "PASSWORD RESET CODE"
            );

            Console.WriteLine(
                $"Email: {email}"
            );

            Console.WriteLine(
                $"Code: {resetCode}"
            );

            Console.WriteLine(
                "Code expires in 10 minutes."
            );

            Console.WriteLine(
                "=================================="
            );

            Console.WriteLine();
        }


        // Даже если Email не существует,
        // не сообщаем это напрямую.
        return """
        {
            "success": true,
            "status": 200,
            "message": "If the email exists, a reset code was created"
        }
        """;
    }


    // ==========================================
    // FORGOT PASSWORD
    // STEP 2 - VERIFY CODE
    // ==========================================

    private string VerifyResetCode(
        JsonElement data
    )
    {
        string email =
            data.GetProperty("email")
                .GetString() ?? "";

        string code =
            data.GetProperty("code")
                .GetString() ?? "";


        email = email.Trim();
        code = code.Trim();


        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(code))
        {
            return """
            {
                "success": false,
                "status": 400,
                "message": "Email and code are required"
            }
            """;
        }


        // Ищем код для этого Email.
        if (!resetCodes.TryGetValue(
                email,
                out PasswordResetData? resetData
            ))
        {
            return """
            {
                "success": false,
                "status": 400,
                "message": "Invalid reset code"
            }
            """;
        }


        // Проверяем срок действия.
        if (DateTimeOffset.UtcNow >
            resetData.ExpiresAt)
        {
            resetCodes.TryRemove(
                email,
                out _
            );


            return """
            {
                "success": false,
                "status": 400,
                "message": "Reset code expired"
            }
            """;
        }


        // Проверяем сам код.
        if (resetData.Code != code)
        {
            return """
            {
                "success": false,
                "status": 400,
                "message": "Invalid reset code"
            }
            """;
        }


        // Код правильный.
        //
        // Пока его НЕ удаляем,
        // потому что он ещё понадобится
        // при окончательной смене пароля.
        return """
        {
            "success": true,
            "status": 200,
            "message": "Reset code verified"
        }
        """;
    }


    // ==========================================
    // FORGOT PASSWORD
    // STEP 3 - CHANGE PASSWORD
    // ==========================================

    private async Task<string> ForgotPasswordResetAsync(
        JsonElement data
    )
    {
        string email =
            data.GetProperty("email")
                .GetString() ?? "";

        string code =
            data.GetProperty("code")
                .GetString() ?? "";

        string newPassword =
            data.GetProperty("newPassword")
                .GetString() ?? "";


        email = email.Trim();
        code = code.Trim();


        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(code) ||
            string.IsNullOrWhiteSpace(newPassword))
        {
            return """
            {
                "success": false,
                "status": 400,
                "message": "All fields are required"
            }
            """;
        }


        if (newPassword.Length < 6)
        {
            return """
            {
                "success": false,
                "status": 400,
                "message": "Password must contain at least 6 characters"
            }
            """;
        }


        // ======================================
        // CHECK RESET CODE AGAIN
        // ======================================

        if (!resetCodes.TryGetValue(
                email,
                out PasswordResetData? resetData
            ))
        {
            return """
            {
                "success": false,
                "status": 400,
                "message": "Invalid reset code"
            }
            """;
        }


        // Проверяем время.
        if (DateTimeOffset.UtcNow >
            resetData.ExpiresAt)
        {
            resetCodes.TryRemove(
                email,
                out _
            );


            return """
            {
                "success": false,
                "status": 400,
                "message": "Reset code expired"
            }
            """;
        }


        // Проверяем код ещё раз.
        if (resetData.Code != code)
        {
            return """
            {
                "success": false,
                "status": 400,
                "message": "Invalid reset code"
            }
            """;
        }


        // ======================================
        // CREATE NEW PASSWORD HASH
        // ======================================

        string newPasswordHash =
            BCrypt.Net.BCrypt.HashPassword(
                newPassword
            );


        await using var connection =
            new NpgsqlConnection(
                connectionString
            );

        await connection.OpenAsync();


        // ======================================
        // UPDATE DATABASE
        // ======================================

        string sql = @"
            UPDATE users
            SET password_hash = @passwordHash
            WHERE LOWER(email) = LOWER(@email);
        ";


        await using var command =
            new NpgsqlCommand(
                sql,
                connection
            );


        command.Parameters.AddWithValue(
            "passwordHash",
            newPasswordHash
        );


        command.Parameters.AddWithValue(
            "email",
            email
        );


        int affectedRows =
            await command.ExecuteNonQueryAsync();


        if (affectedRows == 0)
        {
            resetCodes.TryRemove(
                email,
                out _
            );


            return """
            {
                "success": false,
                "status": 404,
                "message": "User not found"
            }
            """;
        }


        // ======================================
        // DELETE USED CODE
        // ======================================

        // После успешной смены пароля
        // код больше использовать нельзя.
        resetCodes.TryRemove(
            email,
            out _
        );


        return """
        {
            "success": true,
            "status": 200,
            "message": "Password changed successfully"
        }
        """;
    }


    // ==========================================
    // PASSWORD RESET DATA
    // ==========================================

    private class PasswordResetData
    {
        // 6-значный код.
        public string Code { get; set; } = "";


        // Время окончания действия кода.
        public DateTimeOffset ExpiresAt { get; set; }
    }
}