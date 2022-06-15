using ChatMessangerv2.MVVM.Model;
using ChatMessangerv2.MVVM.ViewModel;
using ChatMessangerv2.Net.Model;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransferLibrary;

namespace ChatMessangerv2.Net
{
    class ServerTcp
    {
        TcpClient _tcpClient;
        private const string _ip = "127.0.0.1";
        private const int _port = 40000;
        public event Action msgRecievedEvent;
        public event Action msgUpdateEvent;
        public event Action msgDeleteEvent;
        private NetworkStream _stream;
        public ServerTcp()
        {
            _tcpClient = new TcpClient();

        }
        public void ConnectToServer()
        {
            if (!_tcpClient.Connected)
            {
                _tcpClient.Connect(_ip, _port);
                _stream = _tcpClient.GetStream();
                //GetMessages();
            }
        }
        public void SendMessage(Message message, ACTION act)
        {
            TransferLibrary.TransferMessages transferMessages = new TransferLibrary.TransferMessages();
            transferMessages.SetToken(StartViewModel.Token);
            transferMessages.Action = TransferLibrary.ACTION.SEND;
            transferMessages.Message = new TransferLibrary.CModels.Message()
            {
                Text = message.Text,
                Sender = new TransferLibrary.CModels.User()
                {
                    Id = message.Sender.Id,
                    Login = message.Sender.Login,
                    Password = message.Sender.Password
                },
                Attachments = message.Attachments,
                AttachmentsCount = message.AttachmentsCount,
                IsViewed = message.IsViewed,
                Id = message.Id,
                ChatId = message.ChatId
            };
            transferMessages.ForAll = null;
            BinaryFormatter binf = new BinaryFormatter();

            binf.Serialize(_stream, transferMessages);

        }
        public Servers.MessageTransfer.TransferMessages GetMessage()
        {
            BinaryFormatter binf = new BinaryFormatter();
            TransferMessages transferMessages = new TransferMessages();
            transferMessages.Message = new TransferLibrary.CModels.Message() { Sender = new TransferLibrary.CModels.User() { Id = MyUser.YouUser.Id } };
            binf.Serialize(_stream, transferMessages);
            transferMessages = binf.Deserialize(_stream) as TransferMessages;
            var transferMessagesMy = new Servers.MessageTransfer.TransferMessages()
            { 
                Action = transferMessages.Action, 
                ForAll = transferMessages.ForAll,
                Message = new Message()
                {
                    Attachments = transferMessages.Message.Attachments,
                    AttachmentsCount = transferMessages.Message.AttachmentsCount,
                    ChatId = transferMessages.Message.ChatId,
                    Id = transferMessages.Message.Id,
                    IsViewed = transferMessages.Message.IsViewed,
                    Sender = new User() 
                    { 
                        Id = transferMessages.Message.Sender.Id, 
                        Login = transferMessages.Message.Sender.Login,
                        Password = transferMessages.Message.Sender.Password
                    },
                    Text = transferMessages.Message.Text
                }
            };
            return transferMessagesMy;
        }
        private void GetMessages()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var msg = GetMessage();
                    switch (msg.Action)
                    {
                        case ACTION.CHNG:
                            msgDeleteEvent?.Invoke();
                            break;
                        case ACTION.DEL:
                            msgUpdateEvent?.Invoke();
                            break;
                        case ACTION.GET:
                            msgRecievedEvent?.Invoke();
                            break;
                    }
                    Thread.Sleep(1000);
                }
            });
        }
    }
}
