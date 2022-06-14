using ChatMessangerv2.Core;
using ChatMessangerv2.MVVM.Model;
using ChatMessangerv2.Net;
using ChatMessangerv2.Net.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Threading;

namespace ChatMessangerv2.MVVM.ViewModel
{
    public class AddContactViewModel: OnPropertyChangedClass
    {

        private NetUser _selectedUser;
        public NetUser SelectedUser 
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                if (value != null)
                {
                    _selectedUser = value;
                }
            }
        }
        public string Login { get; set; }
        public int Offset { get; set; }
        private const int _propotion = 15;
        public RelayCommand AddContactToContacts { get; set; }
        public RelayCommand SearchUserToServer { get; set; }
        public RelayCommand ClickExite { get; set; }
        public RelayCommand Load { get; set; }
        public ObservableCollection<NetUser> Users { get; set; }

        private ServerHttp _server;
        public AddContactViewModel()
        {
            //this.chat = chat;
            //chat.ValueChanged += ModelValueChanged;
            //chat.AllValueChanged();
            //UserMain = userMain;
            Users = new ObservableCollection<NetUser>();
            AddContactToContacts = new RelayCommand(AddContact);
            SearchUserToServer = new RelayCommand(o => SearchUser());
            Load = new RelayCommand(o => LoadMore());
        }
        public void AddContact(object parameter)
        {
            if (SelectedUser != null)
            {
                Chat.CommonChat = new Chat() { CreationTimeLocal = DateTime.Now,  UserContact = new User() { Login = SelectedUser.Login} };
                (parameter as Window).DialogResult = true;
            }
            else (parameter as Window).DialogResult = false;

        }
        public async Task SearchUser()
        {
            Users.Clear();
            _server = new ServerHttp();
            var result = await _server.SearchUser(Login, Offset, _propotion);
            var status = result.StatusCode;
            switch(status)
            {
                case HttpStatusCode.OK:
                    var users = await result.Content.ReadFromJsonAsync<IEnumerable<NetUser>>(null, CancellationToken.None);
                    List<NetUser> listUsers = users.ToList();
                    for (int i = 0; i < listUsers.Count; i++)
                        Users.Add(listUsers[i]);
                    break;
                case HttpStatusCode.NoContent:
                    MessageBox.Show(
                        "Пользователи с данным логином не найдены");
                    break;
                case HttpStatusCode.BadRequest:
                    var error = await result.Content.ReadFromJsonAsync<ProblemDetails>(null, CancellationToken.None);
                    MessageBox.Show(error.Detail);
                    break;

                case HttpStatusCode.InternalServerError:
                    MessageBox.Show(
                        "Сервер не отвечает");
                    break;
            }
        }
        public void LoadMore()
        {
            Offset += _propotion;
            SearchUser();
        }
    }
}
