using System.Text;
using System.Net;
using System.Net.Sockets;


class Server
{
    private TcpListener tcpListner;
    private List<TcpClient> clientList = new List<TcpClient>();
    public Server(int port)
    {
        tcpListner = new TcpListener(IPAddress.Any, port);
        tcpListner.Start();

        Console.WriteLine("Server is listening on port: " + port);

        //Thread for accepting clients
        Thread acceptThread = new Thread(new ThreadStart(AcceptClients));
        acceptThread.Start();
    }

    private void AcceptClients()
    {

        while (true)
        {
            TcpClient client = tcpListner.AcceptTcpClient();
            clientList.Add(client);

            //Thread for each connected client
            Thread clientThread = new Thread(() => HandleClientComm(client));
            clientThread.Start();
        }
    }

    private void HandleClientComm(TcpClient client)
    {
        try
        {
            using (NetworkStream clientStream = client.GetStream())
            using (StreamReader reader = new StreamReader(clientStream, Encoding.UTF8))
            using (StreamWriter writer = new StreamWriter(clientStream, Encoding.UTF8))
            {
                string recivedMessage = reader.ReadLine();

                while(recivedMessage != null)
                {
                    Console.WriteLine($"Received: {recivedMessage}");
                    //ForwardMessage(recivedMessage, writer);
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error handling client communication: " +  ex.Message);
        }
        finally
        {
            clientList.Remove(client);
            client.Close();
        }
    }
}