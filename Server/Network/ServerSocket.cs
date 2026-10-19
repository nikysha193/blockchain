using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Server.Api;

namespace Server.Network;

public class ServerSocket
{
    private const int Port = 6000;

    private readonly Socket serverSocket;
    private readonly ApiHandler apiHandler;

    public ServerSocket(string connectionString)
    {
        // ApiHandler выполняет Login,
        // Register и другие команды.
        apiHandler =
            new ApiHandler(connectionString);

        // Создаём наш собственный TCP Socket.
        serverSocket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        // Server принимает подключения
        // на порту 6000.
        var socketEndPoint =
            new IPEndPoint(
                IPAddress.Any,
                Port
            );

        // Привязываем Socket к порту.
        serverSocket.Bind(socketEndPoint);

        // Начинаем ждать подключения Client.
        serverSocket.Listen();
    }


    // ==========================================
    // START SERVER
    // ==========================================

    public async Task StartAsync()
    {
        while (true)
        {
            // Ждём подключения Client.
            Socket clientSocket =
                await serverSocket.AcceptAsync();

            // Обрабатываем подключившегося Client.
            await HandleClientAsync(
                clientSocket
            );
        }
    }


    // ==========================================
    // HANDLE CLIENT
    // ==========================================

    private async Task HandleClientAsync(
        Socket clientSocket
    )
    {
        using (clientSocket)
        {
            try
            {
                // Получаем полное сообщение Client.
                string message =
                    await ReceiveMessageAsync(
                        clientSocket
                    );

                // Разбираем JSON.
                using JsonDocument document =
                    JsonDocument.Parse(message);

                // Узнаём команду:
                // login, register и т.д.
                string action =
                    document.RootElement
                        .GetProperty("action")
                        .GetString() ?? "";

                // Передаём команду ApiHandler.
                string response =
                    await apiHandler.HandleAsync(
                        action,
                        document.RootElement
                    );

                // Отправляем ответ Client.
                await SendMessageAsync(
                    clientSocket,
                    response
                );
            }
            catch
            {
                string errorResponse = """
                {
                    "success": false,
                    "status": 500,
                    "message": "Server error"
                }
                """;

                try
                {
                    await SendMessageAsync(
                        clientSocket,
                        errorResponse
                    );
                }
                catch
                {
                    // Если соединение уже оборвалось,
                    // отправить ответ невозможно.
                }
            }
        }
    }


    // ==========================================
    // SEND
    // ==========================================

    private static async Task SendMessageAsync(
        Socket socket,
        string message
    )
    {
        // Текст превращаем в байты.
        byte[] messageBytes =
            Encoding.UTF8.GetBytes(message);

        // Получаем размер сообщения.
        byte[] lengthBytes =
            BitConverter.GetBytes(
                messageBytes.Length
            );

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(lengthBytes);
        }

        // Сначала отправляем размер.
        await SendAllAsync(
            socket,
            lengthBytes
        );

        // Потом само сообщение.
        await SendAllAsync(
            socket,
            messageBytes
        );
    }


    // ==========================================
    // RECEIVE
    // ==========================================

    private static async Task<string> ReceiveMessageAsync(
        Socket socket
    )
    {
        // Получаем размер сообщения.
        byte[] lengthBytes =
            new byte[4];

        await ReceiveExactAsync(
            socket,
            lengthBytes
        );

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(lengthBytes);
        }

        int messageLength =
            BitConverter.ToInt32(
                lengthBytes,
                0
            );

        // Создаём buffer нужного размера.
        byte[] messageBytes =
            new byte[messageLength];

        // Получаем сообщение полностью.
        await ReceiveExactAsync(
            socket,
            messageBytes
        );

        // Байты превращаем в текст.
        return Encoding.UTF8.GetString(
            messageBytes
        );
    }


    // ==========================================
    // SEND ALL BYTES
    // ==========================================

    private static async Task SendAllAsync(
        Socket socket,
        byte[] data
    )
    {
        int totalSent = 0;

        while (totalSent < data.Length)
        {
            int sent =
                await socket.SendAsync(
                    data.AsMemory(totalSent),
                    SocketFlags.None
                );

            if (sent == 0)
            {
                throw new SocketException(
                    (int)SocketError.ConnectionReset
                );
            }

            totalSent += sent;
        }
    }


    // ==========================================
    // RECEIVE ALL BYTES
    // ==========================================

    private static async Task ReceiveExactAsync(
        Socket socket,
        byte[] buffer
    )
    {
        int totalReceived = 0;

        while (totalReceived < buffer.Length)
        {
            int received =
                await socket.ReceiveAsync(
                    buffer.AsMemory(totalReceived),
                    SocketFlags.None
                );

            if (received == 0)
            {
                throw new SocketException(
                    (int)SocketError.ConnectionReset
                );
            }

            totalReceived += received;
        }
    }
}