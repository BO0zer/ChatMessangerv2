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


        public void GetChats()
        {
            _serverHttp = new ServerHttp();
            //var result =  _serverHttp.GetChats(new NetUser() { Login = User.YouUser.Login, Password = User.YouUser.Password}, 0, 100).Result;
            //var status = result.StatusCode.ToString();
            var status = "200";
            switch (status)
            {
                case "200":
                    //var content = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result) as IEnumerable<NetChat>;
                    var content = new List<NetChat>()
                    { 
                new NetChat(){ ChatMembers = new List<NetUser>()
                {
                    new NetUser(){Login = "TestAdmin", Password = "1234"},
                    new NetUser(){ Login = "TestContact1"}
                },
                Id=Guid.Parse("3F2504E0-4F89-11D3-9A0C-0305E82C3300")
                },
                 new NetChat(){ ChatMembers = new List<NetUser>()
                {
                    new NetUser(){Login = "TestAdmin", Password = "1234"},
                    new NetUser(){ Login = "TestContact2"}
                },
                Id=Guid.Parse("3F2504E0-4F89-11D3-9A0C-0305E82C3301")
                },
                new NetChat(){ ChatMembers = new List<NetUser>()
                {
                    new NetUser(){Login = "TestAdmin", Password = "1234"},
                    new NetUser(){ Login = "TestContact3"}
                },
                Id=Guid.Parse("3F2504E0-4F89-11D3-9A0C-0305E82C3302")
                },
                    };
                    var chats = content.ToList();
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
                case "400":
                    //var error = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result) as ProblemDetails;
                    //MessageBox.Show(error.Detail);
                    break;
                case "500":
                    MessageBox.Show("Сервер не отвечает");
                    break;
            }
        }
        public void GetMessages()
        {
            Messages.Clear();
            if (SelectedChat.Messages != null)
            {             
                _serverHttp = new ServerHttp();
                //var result = _serverHttp.GetMessages(new NetChat()
                //{
                //    Id = SelectedChat.Id,
                //    ChatMembers = new List<NetUser>() { new NetUser() { Login = SelectedChat.UserMain.Login, Password = SelectedChat.UserMain.Password }, new NetUser() { Login = SelectedChat.UserContact.Login } },
                //    CreationTimeLocal = SelectedChat.CreationTimeLocal
                //}, 50, 0).Result;
                //var status = result.StatusCode.ToString();
                //var content = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result) as IEnumerable<NetMessage>;
                var status = "200";
                switch (status)
                {
                    case "200":
                        var content = new List<NetMessage>()
                        {
                            new NetMessage(){ Text = "Привет! Как дела?", User = new NetUser(){ Login = SelectedChat.UserContact.Login} },
                            new NetMessage(){ Text = "Привет! Норм а у тебя?", User = new NetUser(){ Login = User.YouUser.Login} },
                            new NetMessage(){ Text = "Да тоже все отлично", User = new NetUser(){ Login = SelectedChat.UserContact.Login} }
                        };
                        foreach (var msg in content)
                        {
                            Messages.Add(msg);
                            Chats.Where(o => o.Id.ToString() == SelectedChat.Id.ToString()).ToList()[0].Messages.Add(msg);
                        }
                        break;
                    case "400":
                        //var error = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result) as ProblemDetails;
                        //MessageBox.Show(error.Detail);
                        break;
                    case "500":
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
