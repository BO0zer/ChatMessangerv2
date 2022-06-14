using ChatMessangerv2.MVVM.ViewModel;
using ChatMessangerv2.Net.Model;
using ChatMessangerv2.Net.Servers.MessageTransfer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessangerv2.Net
{
    class ServerTcp
    {
        TcpClient _tcpClient;
        private const string _ip = "127.0.0.1";
        private const int _port = 7891;
        public event Action msgRecievedEvent;
        public event Action msgUpdateEvent;
        public event Action msgDeleteEvent;
        public ServerTcp()
        {
            _tcpClient = new TcpClient();
        }
        public void ConnectToServer(string username)
        {
            if (!_tcpClient.Connected)
            {
                _tcpClient.Connect(_ip, _port);
            }
        }
        public void SendMessage(NetMessage message, ACTION act)
        {
            var stream = _tcpClient.GetStream();
            TransferMessages transferMessages = new TransferMessages();
            transferMessages.SetToken(StartViewModel.Token);
            transferMessages.action = act;
            transferMessages.Message = message;
            var json = JsonConvert.SerializeObject(transferMessages);
            BinaryFormatter binf = new BinaryFormatter();
            binf.Serialize(stream, json);
        }
        public TransferMessages GetMessage()
        {
            var stream = _tcpClient.GetStream();
            TransferMessages transferMessages = new TransferMessages();
            BinaryFormatter binf = new BinaryFormatter();
            transferMessages = binf.Deserialize(stream) as TransferMessages;
            return transferMessages;
        }
        private void GetMessages()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var msg = GetMessage();
                    switch (msg.action)
                    {
                        case ACTION.SEND:
                            break;
                        case ACTION.CHNG:
                            break;
                        case ACTION.DEL:
                            break;
                        case ACTION.GET:
                            msgRecievedEvent?.Invoke();
                            break;
                        default:
                            break;
                    }
                }
            });
        }
    }
}
