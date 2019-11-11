using System;
using System.Text;
using System.Windows;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DigtalSignatureDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static String file_content;
        String public_key;
        SignatureGenerator signatureGenerator;
        public MainWindow()
        {
            InitializeComponent();
            Console.ReadLine();
        }

        private void CreateConnectionButtonHandler(object sender, RoutedEventArgs e)
        {
               ServerTask();       
        }

        private void ConnectButtonHandler(Object sender, RoutedEventArgs e)
        {
            clientTask();
        }

        public void clientTask()
        {
            try
            {
                Console.WriteLine("Client connected");
                IPAddress ipAddr = IPAddress.Parse(ipaddress_input.Text);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, int.Parse(port_input.Text));

                Socket senderSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                try
                {
                    senderSocket.Connect(localEndPoint);

                    status_ouput.AppendText("Socket connected to -> {0} " + senderSocket.RemoteEndPoint.ToString() + "\n");

                    byte[] messageSent = Encoding.ASCII.GetBytes("aomw askldjad" + "<EOF>");
                    int byteSent = senderSocket.Send(messageSent);

                    byte[] messageReceived = new byte[4096];

                    int byteRecv = senderSocket.Receive(messageReceived);

                    status_ouput.AppendText("Message from Server -> {0}" + Encoding.ASCII.GetString(messageReceived, 0, byteRecv) + "\n");

                    senderSocket.Shutdown(SocketShutdown.Both);
                    senderSocket.Close();


                }
                catch (ArgumentNullException ane)
                {
                    MessageBox.Show($"ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    MessageBox.Show($"SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        public async void ServerTask()
        {
            Console.WriteLine("Server started");
            IPAddress[] ipHost = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress ip = null;
            foreach (IPAddress ipAddr in ipHost)
            {

                if (ipAddr.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = ipAddr;
                    Console.WriteLine(ip);

                    ipaddress_input.Text = ipAddr.ToString();
                }
            }

            Console.WriteLine("After foreach loop");
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ipaddress_input.Text), int.Parse(port_input.Text));
            status_ouput.AppendText("Shell >> Ip Address: " + ipaddress_input.Text.ToString() + ":" + port_input.Text.ToString() + "\n");
            
            
            Socket listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    SocketAsyncEventArgs el = new SocketAsyncEventArgs();
                    el.Completed += test;

                    Socket clientSocket = listener.AcceptAsync();

                    SignatureGenerator sg = new SignatureGenerator();
                    sg.HashFile(file_content);
                    byte[] bytes = new Byte[4096];
                    string data = null;
                    signatureGenerator = new SignatureGenerator();
                    while (true)
                    {
                        int numByte = clientSocket.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, numByte);
                        Console.WriteLine("data: " + data);

                        if (data.IndexOf("<EOF>") > -1)
                            break;
                    }

                    status_ouput.AppendText("Shell >> Text received -> {0} " + data + "\n");
                    byte[] message = Encoding.ASCII.GetBytes("Anzzi <EOF>");
                    signatureGenerator.HashFile(file_content);


                    //---------------------------- T O   B E   T E S T E D   ---------------------------------

                    byte[] message2 = Encoding.ASCII.GetBytes(file_content);





                    //---------------------------- E N D  O F  F I L E ---------------------------------------

                    


                    clientSocket.Send(message);

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }




            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }

        public void test(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            Console.WriteLine("Testerajbajhjabhj");
        }
        private void ReadFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            bool? result = dlg.ShowDialog();

            string filename = null;
            if (result == true)
            {
                filename = dlg.FileName;
                selected_file.Text = System.IO.Path.GetFileName(filename);
            }

            using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var sr = new StreamReader(fileStream, Encoding.UTF8);
                file_content = sr.ReadToEnd();
                status_ouput.Text = "";
                status_ouput.Text = file_content;
                selected_file.FontSize = 16;
            }


        }

        private void BrowseFileButtonHandler(object sender, RoutedEventArgs e)
        {
            ReadFile();
        }

        //public void SendButtonHandler(Object sender, RoutedEventArgs e)
        //{
        //    String hashedFile = signatureGenerator.HashFile(file_content);
            
        //    if(MainWindow.file_content == ""){
        //        selected_file.Text = "Please choose the file first";
        //    }
        //    else{
        //        String content_to_hash = file_content; 
        //    }
        //}

    }
}
