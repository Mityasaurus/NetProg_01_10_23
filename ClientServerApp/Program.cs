using System.Net;
using System.Net.Sockets;
using System.Text;

IPAddress localAddress = IPAddress.Parse("127.0.0.1");
Console.Write("Enter port of the server: ");
if (!int.TryParse(Console.ReadLine(), out var localPort)) return;
Console.Write("Enter port of the client: ");
if (!int.TryParse(Console.ReadLine(), out var remotePort)) return;
Console.WriteLine();

Task.Run(() => SendMessageAsync());

await ReceiveMessageAsync();

async Task SendMessageAsync()
{
    using UdpClient sender = new UdpClient();

    while (true)
    {
        Console.WriteLine("Enter name of pc component:");
        var message = Console.ReadLine();

        byte[] data = Encoding.UTF8.GetBytes(message);

        await sender.SendAsync(data, data.Length, new IPEndPoint(localAddress, remotePort));

        var response = await sender.ReceiveAsync();
        var responseValue = Encoding.UTF8.GetString(response.Buffer);
        Console.WriteLine($"Price for {message} - {responseValue}");
    }
}

async Task ReceiveMessageAsync()
{
    Dictionary<string, double> pcParts = new Dictionary<string, double>
    {
        { "CPU", 300.00 },
        { "GPU", 600.00 },
        { "RAM", 100.00 },
        { "SSD", 80.00 },
        { "HDD", 50.00 },
        { "Motherboard", 150.00 },
        { "Power Supply", 80.00 },
        { "Case", 70.00 },
        { "Monitor", 200.00 },
        { "Keyboard", 30.00 },
        { "Mouse", 20.00 }
    };

    using UdpClient receiver = new UdpClient(localPort);
    while (true)
    {
        var result = await receiver.ReceiveAsync();
        var message = Encoding.UTF8.GetString(result.Buffer);

        if (message.ToLower() == "end")
        {
            break;
        }

        string response = pcParts.TryGetValue(message, out double value) ? value.ToString() : "Invalid request";

        byte[] data = Encoding.UTF8.GetBytes(response);

        await receiver.SendAsync(data, data.Length, result.RemoteEndPoint);
    }
}
