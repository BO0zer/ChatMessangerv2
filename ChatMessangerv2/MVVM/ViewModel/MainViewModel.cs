using ChatMessangerv2.Core;
using ChatMessangerv2.MVVM.Model;
using ChatMessangerv2.MVVM.View;
using ChatMessangerv2.Net;
using ChatMessangerv2.Net.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;

namespace ChatMessangerv2.MVVM.ViewModel
{
    public class MainViewModel
    {
        public NetMessage Message { get; set; }

        public ObservableCollection<Chat> Chats { get; set; }
        public Chat SelectedChat { get; set; }
        public ObservableCollection<NetMessage> Messages { get; set; }
        public RelayCommand AddContact { get; set; }
        public RelayCommand SendMessage { get; set; }  
        public RelayCommand UpdateMessage { get; set; }
        public RelayCommand DeleteMessage { get; set; }
        public RelayCommand DeleteContact { get; set; }
        public RelayCommand ExiteFromApp { get; set; }
        public RelayCommand EditPassword { get; set; }
        public RelayCommand EditLogin { get; set; }
        public RelayCommand GetMessagesForSelectedChat { get; set; }

        private ServerTcp _serverTcp;
        private ServerHttp _serverHttp;
        //Лист контактов


        public MainViewModel()
        {
            Messages = new ObservableCollection<NetMessage>();
            Chats = new ObservableCollection<Chat>();
            _serverTcp = new ServerTcp();
            _serverTcp.msgRecievedEvent += MessageRecieved;

            GetChats();

            GetMessagesForSelectedChat = new RelayCommand(o => GetMessages());
            AddContact = new RelayCommand(o => OpenAddContact());
            SendMessage = new RelayCommand(o => SendMessageToChat());
        }

        private void MessageRecieved()
        {
            var msg = _serverTcp.GetMessage();
        }

        public void OpenAddContact()
        {
            AddContactView win = new AddContactView();
            var result =  win.ShowDialog();
            if (result == true)
            {
                win = new AddContactView();
                Chats.Add(new Chat()
                {
                    CreationTimeLocal = Chat.CommonChat.CreationTimeLocal,
                    UserContact = Chat.CommonChat.UserContact,
                    UserMain = User.YouUser
                });

                win.Close();
            }
        }


        public async void GetChats()
        {
            _serverHttp = new ServerHttp();
            var result =  await _serverHttp.GetChats(0, 100);
            var status = result.StatusCode;
            switch (status)
            {
                case HttpStatusCode.OK:
                    var content = await result.Content.ReadAsStringAsync();
                    var chats = (JsonConvert.DeserializeObject(content) as IEnumerable<NetChat>).ToList();
                    foreach(var cht in chats)
                    {
                        Chat chat = new Chat() 
                        {
                            Id = cht.Id,
                            CreationTimeLocal = cht.CreationTimeLocal,
                            UserMain = new User() { Login = User.YouUser.Login, Password = User.YouUser.Password},
                            UserContact = new User() { Login = cht.ChatMembers.Where(o=>o.Login != User.YouUser.Login).ToList()[0].Login},
                            Messages = new ObservableCollection<NetMessage>()   
                        };
                        Chats.Add(chat);
                    }
                    break;
                case HttpStatusCode.BadRequest:
                    content = await result.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject(content) as ProblemDetails;
                    MessageBox.Show(error.Detail);
                    break;
                case HttpStatusCode.InternalServerError:
                    MessageBox.Show("Сервер не отвечает");
                    break;
            }
        }
        public async Task GetMessages()
        {
            Messages.Clear();
            if (SelectedChat.Messages != null)
            {             
                _serverHttp = new ServerHttp();
                var result = await _serverHttp.GetMessages(50, 0);
                var status = result.StatusCode;
                switch (status)
                {
                    case HttpStatusCode.OK:
                        var content = await result.Content.ReadAsStringAsync();
                        var messages = JsonConvert.DeserializeObject(content) as IEnumerable<NetMessage>;
                        foreach (var msg in messages)
                        {
                            Messages.Add(msg);
                            Chats.Where(o => o.Id.ToString() == SelectedChat.Id.ToString()).ToList()[0].Messages.Add(msg);
                        }
                        break;
                    case HttpStatusCode.BadRequest:
                        content = await result.Content.ReadAsStringAsync();
                        var error = JsonConvert.DeserializeObject(content) as ProblemDetails;
                        MessageBox.Show(error.Detail);
                        break;
                    case HttpStatusCode.InternalServerError:
                        MessageBox.Show("Сервер не отвечает");
                        break;
                }
            }
        }
        public void SendMessageToChat()
        {
            _serverTcp.SendMessage(Message, Net.Servers.MessageTransfer.ACTION.SEND);
        }
    }
}
