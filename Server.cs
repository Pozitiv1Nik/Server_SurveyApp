
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
    }
}