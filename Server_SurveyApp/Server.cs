using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    public class Server
    {
        private TcpListener? _listener;
        private List<TcpClient> _clients = new List<TcpClient>();
        private CancellationTokenSource? _cancellationTokenSource;

        public void Start(string ipAddress, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _listener.Start();
            _cancellationTokenSource = new CancellationTokenSource();
            Console.WriteLine($"Server started on {ipAddress}:{port}");

            _ = Task.Run(AcceptClientsAsync);
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _listener?.Stop();
            Console.WriteLine("Server stopped");
        }

        private async Task AcceptClientsAsync()
        {
            try
            {
                while (!_cancellationTokenSource!.Token.IsCancellationRequested)
                {
                    var client = await _listener!.AcceptTcpClientAsync();
                    lock (_clients)
                    {
                        _clients.Add(client);
                    }
                    Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
                    _ = Task.Run(() => HandleClientAsync(client));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in accepting clients: {ex.Message}");
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                var stream = client.GetStream();
                var buffer = new byte[1024];
                var clientEndpoint = client.Client.RemoteEndPoint?.ToString();

                try
                {
                    while (!_cancellationTokenSource!.Token.IsCancellationRequested)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token);
                        if (bytesRead == 0) break;

                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received message: {message}, from {clientEndpoint}");

                        ProcessMessage(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}. Error: {ex.Message}");
                }
            }
        }

        public async Task SendMessageToClient(string message)
        {
            List<TcpClient> clientsSnapshot;
            lock (_clients)
            {
                clientsSnapshot = new List<TcpClient>(_clients);
            }

            var responseBuffer = Encoding.UTF8.GetBytes(message);
            foreach (var client in clientsSnapshot)
            {
                try
                {
                    if (client.Connected)
                    {
                        var stream = client.GetStream();
                        await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
                        Console.WriteLine($"Sent message to client: {client.Client.RemoteEndPoint}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message to client: {ex.Message}");
                }
            }
        }
        //
        private bool ValidateLogin(string username, string password)
        {
            
            var testUsers = new Dictionary<string, string>
            {
                { "admin", "admin123" },
                { "user1", "password1" }
            };

            return testUsers.ContainsKey(username) && testUsers[username] == password;
        }
        //
        private bool RegisterUser(string username, string password)
        {
            
            var testUsers = new Dictionary<string, string>
            {
                { "admin", "admin123" },
                { "user1", "password1" }
            };

            if (testUsers.ContainsKey(username))
            {
                return false;
            }

            Console.WriteLine($"Registering user: {username} with password: {password}");
            return true;
        }
        //
        private void HandleLogin(string username, string password)
        {
            if (ValidateLogin(username, password))
            {
                Console.WriteLine($"Login successful for user: {username}");
            }
            else
            {
                Console.WriteLine($"Login failed for user: {username}");
            }
        }
        //
        private void HandleRegister(string username, string password)
        {
            if (RegisterUser(username, password))
            {
                Console.WriteLine($"User {username} successfully registered.");
            }
            else
            {
                Console.WriteLine($"Registration failed: user {username} already exists.");
            }
        }
        //
        private void ProcessMessage(string message)
        {
            var parts = message.Split(' ');
            var command = parts[0];
            var username = parts.Length > 1 ? parts[1] : string.Empty;
            var password = parts.Length > 2 ? parts[2] : string.Empty;

            switch (command.ToUpper())
            {
                case "LOGIN":
                    HandleLogin(username, password);
                    break;

                case "REGISTER":
                    HandleRegister(username, password);
                    break;

                default:
                    Console.WriteLine("Unknown command received: " + command);
                    break;
            }
        }
    }
}
