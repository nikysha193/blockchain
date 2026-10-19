using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientCSharp.Network;

public class ClientSocket
{
    private const int Port = 6000;

    public async Task<string> SendAsync(string message)
    {
        // Создаём наш TCP Socket.
        using Socket socket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        // Подключаемся к Server
        // на этом компьютере, порт 6000.
        await socket.ConnectAsync(
            IPAddress.Loopback,
            Port
        );

        // Отправляем сообщение Server.
        await SendMessageAsync(
            socket,
            message
        );

        // Получаем ответ Server.
        string response =
            await ReceiveMessageAsync(socket);

        return response;
    }


    // ==========================================
    // SEND
    // ==========================================

    private static async Task SendMessageAsync(
        Socket socket,
        string message
    )
    {
        // Превращаем текст в байты.
        byte[] messageBytes =
            Encoding.UTF8.GetBytes(message);

        // Получаем размер сообщения.
        byte[] lengthBytes =
            BitConverter.GetBytes(
                messageBytes.Length
            );

        // Приводим число к сетевому порядку байтов.
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(lengthBytes);
        }

        // Сначала отправляем размер сообщения.
        await SendAllAsync(
            socket,
            lengthBytes
        );

        // Потом отправляем само сообщение.
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
        // Сначала получаем 4 байта,
        // в которых находится размер сообщения.
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

        // Узнаём размер будущего сообщения.
        int messageLength =
            BitConverter.ToInt32(
                lengthBytes,
                0
            );

        // Создаём место ровно нужного размера.
        byte[] messageBytes =
            new byte[messageLength];

        // Получаем всё сообщение полностью.
        await ReceiveExactAsync(
            socket,
            messageBytes
        );

        // Байты превращаем обратно в текст.
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