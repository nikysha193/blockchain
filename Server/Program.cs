using Microsoft.Extensions.Hosting;
using Server.Network;

var builder =
    Host.CreateApplicationBuilder(args);

// Берём строку подключения к PostgreSQL
// из appsettings.json.
string connectionString =
    builder.Configuration
        .GetConnectionString("PostgreSQL")
    ?? throw new InvalidOperationException(
        "PostgreSQL connection string not found."
    );

// Создаём наш собственный TCP Server.
var tcpServer =
    new ServerSocket(connectionString);

// Запускаем TCP Server.
// Он постоянно ждёт подключения Client.
await tcpServer.StartAsync();