namespace Server.Auth;

public class SessionStore
{
    private readonly Dictionary<string, UserSession> sessions =
        new Dictionary<string, UserSession>();


    public string CreateSession(
        int userId,
        string username
    )
    {
        // Создаём случайный Token.
        string token =
            Guid.NewGuid().ToString("N");


        // Запоминаем:
        //
        // Token -> User.
        sessions[token] =
            new UserSession
            {
                UserId = userId,
                Username = username
            };


        // Возвращаем Token.
        return token;
    }


    public bool TryGetSession(
        string token,
        out UserSession session
    )
    {
        if (sessions.TryGetValue(
                token,
                out UserSession? savedSession
            ))
        {
            session = savedSession;

            return true;
        }


        session =
            new UserSession();

        return false;
    }


    public void RemoveSession(
        string token
    )
    {
        sessions.Remove(token);
    }
}


public class UserSession
{
    public int UserId { get; set; }

    public string Username { get; set; } = "";
}