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
using System.Net.Http.Json;
using System.Threading;
using System.Net.Http;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;
using TransferLibrary;

namespace ChatMessangerv2.MVVM.ViewModel
{
    public class MainViewModel
    {
        public Message Message { get; set; }

        public ObservableCollection<Chat> Chats { get; set; }
        public Chat SelectedChat { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public RelayCommand AddContact { get; set; }
        public RelayCommand SendMessage { get; set; }  
        public RelayCommand UpdateMessage { get; set; }
        public RelayCommand DeleteMessage { get; set; }
        public RelayCommand DeleteContact { get; set; }
        public RelayCommand ExiteFromApp { get; set; }
        public RelayCommand EditPassword { get; set; }
        public RelayCommand EditLogin { get; set; }
        public RelayCommand GetMessagesForSelectedChat { get; set; }
        public RelayCommand OpenChangeLoginDialog { get; set; }
        public RelayCommand OpenChangePasswordDialog { get; set; }
        public RelayCommand GetBack { get; set; }

        private ServerTcp _serverTcp;
        private ServerHttp _serverHttp;


        public MainViewModel()
        {
            Message = new Message();
            Messages = new ObservableCollection<Message>();
            Chats = new ObservableCollection<Chat>();
            _serverTcp = new ServerTcp();
            //_serverTcp.msgRecievedEvent += MessageRecieved;

            GetChats();

            //DeleteContact = new RelayCommand();
            GetBack = new RelayCommand(GetBackToStart);
            OpenChangeLoginDialog = new RelayCommand(o => OpenChangeLogin());
            OpenChangePasswordDialog = new RelayCommand(o => OpenChangePassword());
            GetMessagesForSelectedChat = new RelayCommand(o => GetMessages());
            AddContact = new RelayCommand(o => OpenAddContact());
            SendMessage = new RelayCommand(o => SendMessageToChat());
            DeleteMessage = new RelayCommand(o => DeleteMsgToServer());
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
                    Id = Chat.CommonChat.Id,
                    CreationTimeLocal = Chat.CommonChat.CreationTimeLocal,
                    UserContact = Chat.CommonChat.UserContact,
                    UserMain = MyUser.YouUser
                });

                win.Close();
            }
        }


        public async Task GetChats()
        {
            _serverHttp = new ServerHttp();
            var result =  await _serverHttp.GetChats(0, 10);
            var status = result.StatusCode;
            switch (status)
            {
                case HttpStatusCode.OK:
                    var content = await result.Content.ReadFromJsonAsync<IEnumerable<NetChat>>();
                    var chats = content.ToList();
                    foreach(var cht in chats)
                    {
                        Chat chat = new Chat() 
                        {
                            Id = cht.Id,
                            CreationTimeLocal = cht.CreationTimeLocal,
                            UserMain = MyUser.YouUser,
                            UserContact = cht.ChatMembers.First(o=>o.Id != MyUser.YouUser.Id),
                            Messages = new ObservableCollection<Message>()   
                        };
                        Chats.Add(chat);
                    }
                    break;
                case HttpStatusCode.BadRequest:
                    var error = await result.Content.ReadFromJsonAsync<ProblemDetails>(null, CancellationToken.None);
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
                var result = await _serverHttp.GetMessages(SelectedChat.Id, 20, 0);
                var status = result.StatusCode;
                switch (status)
                {
                    case HttpStatusCode.OK:
                        var messages = await result.Content.ReadFromJsonAsync<IEnumerable<Message>>();
                        foreach (var msg in messages)
                        {
                            Messages.Add(msg);
                            Chats.Where(o => o.Id.ToString() == SelectedChat.Id.ToString()).First().Messages.Add(msg);
                        }
                        break;
                    case HttpStatusCode.BadRequest:
                        var error = await result.Content.ReadFromJsonAsync<ProblemDetails>(null, CancellationToken.None);
                        MessageBox.Show(error.Detail);
                        break;
                    case HttpStatusCode.InternalServerError:
                        MessageBox.Show("Сервер не отвечает");
                        break;
                }
            }
        }
        public async Task SendMessageToChat()
        {
            //if (SelectedChat != null)
            //{
            //    var client = new HttpClient();
            //    client.BaseAddress = new Uri("https://localhost:44359/api/");
            //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", StartViewModel.Token);
            //    var newMessage = new NewMessageDetails()
            //    {
            //        Text = Message.Text,
            //        ChatId = SelectedChat.Id
            //    };
            //    await client.PostAsJsonAsync("Messages", newMessage);
        //}
            _serverTcp.ConnectToServer();
            Message.Sender = MyUser.YouUser;
            Message.ChatId = SelectedChat.Id;
            _serverTcp.SendMessage(Message, ACTION.SEND);
            GetMessages();
           // Messages.Add(Message);
        }
        public async Task DeleteChatToServer()
        {

        }
        public async Task DeleteMsgToServer()
        {
            _serverTcp.ConnectToServer();
            Message.Sender = MyUser.YouUser;
            Message.ChatId = SelectedChat.Id;
            _serverTcp.SendMessage(Message, ACTION.DEL);
            GetMessages();
        }
        public void OpenChangeLogin()
        {
            ChangeLogin cl = new ChangeLogin();
            cl.DataContext = new ChangeLoginViewModel();
            cl.ShowDialog();
        }
        public void OpenChangePassword()
        {
            ChangePassword cl = new ChangePassword();
            cl.DataContext = new ChangePasswordViewModel();
            cl.ShowDialog();
        }
        public void GetBackToStart(object parameter)
        {
            StartViewModel svm = new StartViewModel();
            StartView sv = new StartView();
            sv.DataContext = svm;
            sv.Show();
            (parameter as Window).Close();
        }
        public void MessageRecieved()
        {
            Messages.Add( _serverTcp.GetMessage().Message);
            Application.Current.Dispatcher.Invoke(() => Messages.Add(_serverTcp.GetMessage().Message));
        }
    }
}
