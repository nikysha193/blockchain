namespace ClientCSharp.Auth;

public class LoginTicketStore
{
    private readonly Dictionary<string, LoginTicket> tickets =
        new Dictionary<string, LoginTicket>();


    public string CreateTicket(
        string username,
        bool rememberMe,
        string language,
        bool darkTheme,
        string sessionToken
    )
    {
        string ticket =
            Guid.NewGuid().ToString("N");


        tickets[ticket] =
            new LoginTicket
            {
                Username = username,
                RememberMe = rememberMe,
                Language = language,
                DarkTheme = darkTheme,
                SessionToken = sessionToken
            };


        return ticket;
    }


    public bool TryUseTicket(
        string ticket,
        out LoginTicket loginTicket
    )
    {
        if (tickets.TryGetValue(
                ticket,
                out LoginTicket? savedTicket
            ))
        {
            loginTicket = savedTicket;


            // Ticket одноразовый.
            // После использования удаляем его.
            tickets.Remove(ticket);


            return true;
        }


        loginTicket = new LoginTicket();

        return false;
    }
}


public class LoginTicket
{
    public string Username { get; set; } = "";

    public bool RememberMe { get; set; }

    public string Language { get; set; } = "en";

    public bool DarkTheme { get; set; }

    public string SessionToken { get; set; } = "";
}