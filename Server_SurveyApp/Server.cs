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
using TestEntitySurvey.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore;
namespace Server
{
    public class Server
    {
        private TcpListener? _listener;
        private List<TcpClient> _clients = new List<TcpClient>();
        private CancellationTokenSource? _cancellationTokenSource;
        private SurveyDbContext _surveyDbContext;
        private List<Survey> _surveys = new List<Survey>();


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

        public async Task SendMessageToClient(string message, string clientIdentifier = null)
        {
            if (clientIdentifier == null)
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
            else
            {
                
                TcpClient targetClient = null;

                lock (_clients)
                {
                    targetClient = _clients.FirstOrDefault(client =>
                        client.Client.RemoteEndPoint?.ToString() == clientIdentifier);
                }

                if (targetClient != null && targetClient.Connected)
                {
                    try
                    {
                        var stream = targetClient.GetStream();
                        var responseBuffer = Encoding.UTF8.GetBytes(message);
                        await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
                        Console.WriteLine($"Sent message to client: {targetClient.Client.RemoteEndPoint}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message to client {clientIdentifier}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Client with identifier {clientIdentifier} not found or disconnected.");
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

        private async Task<bool>HandleAdmin(string username,string password)
        {
            var manager = new SurveyDbManagerUser(_surveyDbContext);
            return await manager.IsAdminAsync(username,password);

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

        private async Task<string> HandleCreateSurvey(string surveyData)
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                surveyData,
                @"^(\S+)\s+""([^""]+)""\s+(.+)$"
            );

            if (!match.Success)
            {
                Console.WriteLine("Invalid survey data format");
                return "ERROR_INVALID_SURVEY_DATA";
            }

            var topic = match.Groups[1].Value;          
            var description = match.Groups[2].Value;  
            var options = match.Groups[3].Value.Split('|');

            var surveyId = Guid.NewGuid().ToString();

            var surveyManager = new SurveyDbManagerSurvey(_surveyDbContext);

           await surveyManager.AddSurveyAsync(topic, description, options);  
            
           _surveyDbContext.SaveChanges();
     
            var broadcastMessage = $"NEW_SURVEY {surveyId} {topic} \"{description}\" {string.Join("|", options)}";

            

            await SendMessageToClient(broadcastMessage);
            return "CREATE_SUCCESS";
        }
        private async Task<string> HandleDeleteSurveyAsync(string surveyId)
        {
        
            var manager = new SurveyDbManagerSurvey(_surveyDbContext);
            await manager.DeleteSurveyAsync(int.Parse(surveyId));

            
            var broadcastMessage = $"DELETE_SURVEY {surveyId}";
            await SendMessageToClient(broadcastMessage);

            return "DELETE_SUCCESS";
        }

        private async Task<string> ProcessMessageAsync(string message)
        {
            var parts = message.Split(' ');
            var command = parts[0];
            var username = parts.Length > 1 ? parts[1] : string.Empty;
            var password = parts.Length > 2 ? parts[2] : string.Empty;

            switch (command)
            {
                case "LOGIN":
                    bool isAdminLog = await Task.Run(() => HandleAdmin(username, password));
                    if (isAdminLog) return "LOGIN_SUCCESS_ADMIN";

                    bool isValidLog = await Task.Run(() => HandleLogin(username, password)); 
                    return isValidLog ? "LOGIN_SUCCESS" : "ERROR_WRONG_CREDENTIALS";

                case "REGISTER":
                    bool isValidReg = await Task.Run(() => HandleRegister(username, password)); 
                    return isValidReg ? "REGISTER_SUCCESS" : "ERROR_USER_EXISTS";

                case "CREATE":
                    return await HandleCreateSurvey(message.Substring(7));

                case "DELETE":
                    return await HandleDeleteSurveyAsync(message.Substring(7));

                default:
                    Console.WriteLine("Unknown command received: " + command);
                    return "ERROR_UNKNOWN_COMMAND";
            }
        }

        //////
        private void SaveSurveysToJson(List<Survey> surveys)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(surveys, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText("surveys.json", json);
            Console.WriteLine("Surveys saved to surveys.json");
        }

        private List<Survey> LoadSurveysFromJson()
        {
            if (System.IO.File.Exists("surveys.json"))
            {
                string json = System.IO.File.ReadAllText("surveys.json");
                List<Survey> surveys = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Survey>>(json);
                return surveys ?? new List<Survey>();
            }
            return new List<Survey>();
        }

        private async Task<string> HandleGetSurveys()
        {
            var surveys = await _surveyDbContext.Surveys.ToListAsync(); 
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(surveys, Newtonsoft.Json.Formatting.Indented);
            return json;
        }







    }
}
