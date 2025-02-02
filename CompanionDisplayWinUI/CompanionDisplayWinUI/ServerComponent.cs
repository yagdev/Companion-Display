using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web.Http;
namespace CompanionDisplayWinUI
{
    // This will likely be on 25.2's source code, so pretend you didn't see anything :P (18/1/2025)
    class ServerComponent
    {
        public delegate void handleServerReceiveData(string reply);
        private Socket serverToClient;
        private Socket listenerSocket;
        private Socket clientSocket;
        public event handleServerReceiveData callReceived;
        public static bool serverOnline = false;
        public async void startServer()
        {
            try
            {
                serverOnline = true;
                startClientToServer();
                startServerToClient();
            }
            catch
            {
                serverOnline = false;
            }
        }
        public async void startClientToServer()
        {
            IPHostEntry ipEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());
            IPAddress ip0 = null;
            foreach (IPAddress ip in ipEntry.AddressList)
            {
                if (ip.ToString().StartsWith("192"))
                {
                    ip0 = ip;
                }
            }
            IPEndPoint iPEndPoint = new(ip0, 2534);
            serverToClient = new(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverToClient.Bind(iPEndPoint);
            serverToClient.Listen();
            listenForCommands();
        }
        public async void listenForCommands()
        {
            var handler = await serverToClient.AcceptAsync();
            while (true)
            {
                var buffer = new byte[16384];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var command = Encoding.UTF8.GetString(buffer, 0, received);
                if (command != null)
                {
                    callReceived?.Invoke(command);
                    var responseByte = Encoding.UTF8.GetBytes(checkRemoteCommand(command));
                    await handler.SendAsync(responseByte, SocketFlags.None);
                }
            }
        }
        public async void startServerToClient()
        {
            IPHostEntry ipEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());
            IPAddress ip0 = null;
            foreach (IPAddress ip in ipEntry.AddressList)
            {
                if (ip.ToString().StartsWith("192"))
                {
                    ip0 = ip;
                }
            }
            IPEndPoint iPEndPoint = new(ip0, 2535);
            listenerSocket = new(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(iPEndPoint);
            listenerSocket.Listen(10);
            await AcceptClientsAsync();
        }
        private async Task AcceptClientsAsync()
        {
            while (serverOnline)
            {
                clientSocket = await listenerSocket.AcceptAsync(); // Accept a new client
                Console.WriteLine("Client connected.");

                _ = HandleClientAsync(clientSocket); // Start handling the client
            }
        }
        private async Task HandleClientAsync(Socket client)
        {
            var buffer = new byte[16384];
            try
            {
                while (serverOnline)
                {
                    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                    if (received == 0) break; // Client disconnected
                    var command = Encoding.UTF8.GetString(buffer, 0, received);
                    callReceived?.Invoke(command);
                    // Respond to the client
                    string response = "Echo";
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await client.SendAsync(responseBytes, SocketFlags.None);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket exception: {ex.Message}");
            }
        }
        public async Task SendCommandAsync(string command)
        {
            var commandBytes = Encoding.UTF8.GetBytes(command);
            await clientSocket.SendAsync(commandBytes, SocketFlags.None);
        }
        private string checkRemoteCommand(string command)
        {
            switch (command)
            {
                case "mediareq":
                    return (Globals.SongName + "\n" + Globals.SongDetails + "\n" + Globals.AlbumName + "\n" + Globals.SongLyrics + "\n" + Globals.SongTime + "\n" + Globals.SongEnd + "\n" + Globals.SongProgress + "\n" + Globals.SongBackground);
                default:
                    return ("Hello from Companion Display Desktop. Invalid command.");
            }
        }
        public void StopServer()
        {
            serverOnline = false;
            listenerSocket?.Close();
            clientSocket?.Close();
            Console.WriteLine("Server stopped.");
        }
    }
}
