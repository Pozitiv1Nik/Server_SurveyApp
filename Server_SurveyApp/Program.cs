using System.Net;

namespace Server_SurveyApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var server = new Server.Server();
            var ipAddress = "127.0.0.1";
            var port = 3456;
            try
            {
                server.Start(ipAddress, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            while (true)
            {
                Console.WriteLine("Choose an action:");
                Console.WriteLine("1. Send Message to Clients");
                Console.WriteLine("2. Stop Server");
                Console.Write("Enter your choice: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter message to send: ");
                        var message = Console.ReadLine();
                        if (!string.IsNullOrEmpty(message))
                        {
                            await server.SendMessageToClient(message);
                        }
                        else
                        {
                            Console.WriteLine("Message cannot be empty.");
                        }
                        break;
                    case "2":
                        server.Stop();
                        return;
                    default:
                        Console.WriteLine("NI");
                        break;
                }
            }
        }
    }
}