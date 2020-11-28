using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketClient_DiCostanzoMartina
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);

            string strIPAddress = "";
            string strPort = "";
            IPAddress ipAddress = null;
            int nPort;



            //Dichiaro le Variabili per comunicare con il server
            string receivedString = "";
            byte[] sendBuff = new byte[128];
            byte[] recvBuff = new byte[128];
            int nReceivedBytes = 0;

            try
            {
                //Settagio da Console dell'EndPoint
                Console.WriteLine("Benvenuto nel Client Socket");
                Console.Write("IP del Server: ");
                strIPAddress = Console.ReadLine();
                Console.Write("Porta del Server: ");
                strPort = Console.ReadLine();

                if (!IPAddress.TryParse(strIPAddress.Trim(), out ipAddress))
                {
                    Console.WriteLine("IP non valido.");
                    return;
                }
                if (!int.TryParse(strPort.Trim(), out nPort))
                {
                    Console.WriteLine("Porta non valida.");
                    return;
                }
                if (nPort <= 0 || nPort >= 65535)
                {
                    Console.WriteLine("Porta non valida.");
                    return;
                }
                Console.WriteLine("End Point del Server " + ipAddress.ToString() + " " + nPort);

                //Connessione al Server
                client.Connect(ipAddress, nPort);
                string sendString = "";
                //Inizio chat con il server
                Console.WriteLine("Chatta con il server. ");

                while (true)
                {
                    // Prendo il messaggio & condizione di uscita
                    sendString = Console.ReadLine();

                    //Dico al Server di interrompersi
                    sendBuff = Encoding.ASCII.GetBytes(sendString);
                    client.Send(sendBuff);

                    if (sendString.ToUpper().Trim() == "Quiet")
                    {
                        break;
                    }

                    //Pulisco il buffer e ricevo il messaggio
                    Array.Clear(recvBuff, 0, recvBuff.Length);
                    nReceivedBytes = client.Receive(recvBuff);
                    receivedString = Encoding.ASCII.GetString(recvBuff);
                    Console.WriteLine("S: " + receivedString);
                }


                Console.WriteLine("EndPoint del server " + ipAddress.ToString() + "" + nPort);
                client.Connect(ipAddress, nPort);

                byte[] buff = new byte[128];
                sendString = "";
                string receiveString = "";
                int receivBytes = 0;
                while (true)
                {
                    Console.WriteLine("Mando un messaggio: ");
                    sendString = Console.ReadLine();
                    Encoding.ASCII.GetBytes(sendString).CopyTo(buff, 0);
                    client.Send(buff);
                    if (sendString.ToUpper().Trim() == "QUIT")
                    {
                        break;
                    }

                    Array.Clear(buff, 0, buff.Length);
                    receivBytes = client.Receive(buff);
                    receiveString = Encoding.ASCII.GetString(buff, 0, receivBytes);
                    Console.WriteLine("M: " + receiveString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                /* In ogni occasione chiudo la connessione per sicurezza */
                if (client != null)
                {
                    if (client.Connected)
                    {
                        client.Shutdown(SocketShutdown.Both);//disabilita la send e receive
                    }
                    client.Close();
                    client.Dispose();
                }
            }

            Console.WriteLine("Premi Enter per chiudere...");
            Console.ReadLine();

        }
        
    }
}
