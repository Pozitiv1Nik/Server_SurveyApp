using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Db_Survey;
using TestEntitySurvey;
using Db_Survey.Models;
namespace Server
{
    public class Server
    {
        private TcpListener? _listener;
        private List<TcpClient> _clients = new List<TcpClient>();
        private CancellationTokenSource? _cancellationTokenSource;
        private SurveyDbContext _surveyDbContext;

        public void Start(string ipAddress, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _listener.Start();
            _cancellationTokenSource = new CancellationTokenSource();
            Console.WriteLine($"Server started on {ipAddress}:{port}");

            var factory = new SurveyDbContextFactory();
            _surveyDbContext = factory.CreateDbContext(null);

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
                        var respons = await ProcessMessageAsync(message);
                        Console.WriteLine($"Message to sent: {respons}");
                        await SendMessageToClient(respons);

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

        
        private async  Task<bool> HandleLogin(string username, string password)
        {
            
            var manager = new SurveyDbManagerUser(_surveyDbContext);
            var user = await manager.GetUserByLoginAsync(username); 

            if (user != null && user.Password == password) 
            {
                Console.WriteLine($"Login successful for user: {username}");
                return true;
            }
            else
            {
                Console.WriteLine($"Login failed for user: {username}");
                return false;
            }
        }

        private async Task<bool> HandleRegister(string username, string password)
        {
            var manager = new SurveyDbManagerUser(_surveyDbContext);

            if (!await manager.CheckUserAsync(username, password))
            {
                var newUser = new User
                {
                    Login = username,
                    Password = password,
                    RoleId = 1
                };
                await manager.AddUserAsync(newUser);
                Console.WriteLine($"User {username} successfully registered.");
                return true;
            }
            else
            {
                Console.WriteLine($"Registration failed: user {username} already exists.");
                return false;
            }
        }

        //
        private async Task<string> ProcessMessageAsync(string message)
        {
            var parts = message.Split(' ');
            var command = parts[0];
            var username = parts.Length > 1 ? parts[1] : string.Empty;
            var password = parts.Length > 2 ? parts[2] : string.Empty;

            switch (command)
            {
                case "LOGIN":
                    bool isValidLog = await Task.Run(() => HandleLogin(username, password)); 
                    return isValidLog ? "LOGIN_SUCCESS" : "ERROR_WRONG_CREDENTIALS";

                case "REGISTER":
                    bool isValidReg = await Task.Run(() => HandleRegister(username, password)); 
                    return isValidReg ? "REGISTER_SUCCESS" : "ERROR_USER_EXISTS";

                default:
                    Console.WriteLine("Unknown command received: " + command);
                    return "ERROR_UNKNOWN_COMMAND";
            }
        }
    }
}
