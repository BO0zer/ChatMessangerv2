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

namespace ChatMessangerv2.MVVM.ViewModel
{
    public class AddContactViewModel: OnPropertyChangedClass
    {
        //private DateTime _creationDateTime;
        //private User _userContact;
        //private User _userMain;
        //private readonly ChatCommon chat;
        //public DateTime CreationDateTime { get => _creationDateTime; set => SetProperty(ref _creationDateTime, value); }
        //public User UserContact { get => _userContact; set => SetProperty(ref _userContact, value); }
        //public User UserMain { get => _userMain; set => SetProperty(ref _userMain, value); }

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
        public ObservableCollection<NetUser> Users { get; set; }

        private ServerHttp _server;
        public AddContactViewModel()
        {
            //this.chat = chat;
            //chat.ValueChanged += ModelValueChanged;
            //chat.AllValueChanged();
            //UserMain = userMain;
            AddContactToContacts = new RelayCommand(AddContact);
            SearchUserToServer = new RelayCommand(o => SearchUser());
            Users = new ObservableCollection<NetUser>()
            {
                new NetUser() { Login = "Vasya123", Password="gg"},
                new NetUser() { Login = "Vasya12", Password="gg"},
                new NetUser() { Login = "Vasya1", Password="gg"},
            };

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
        public void SearchUser()
        {
            Users.Clear();
            _server = new ServerHttp();
            var result = _server.SearchUser(Login, Offset, _propotion);
            switch(result.Status.ToString())
            {
                case "200":
                    IEnumerable<NetUser> users = JsonSerializer.Deserialize<IEnumerable<NetUser>>(result.Result.Content.ReadAsStringAsync().Result);
                    Users.Concat(users);
                    break;
                case "204":
                    MessageBox.Show(
                        "Пользователи с данным логином не найдены");
                    break;
                case "400":
                    //TODO: Обработку ошибок придумать логику
                    IDictionary<string, string> errors = JsonSerializer.Deserialize<IDictionary<string, string>>(result.Result.Content.ReadAsStringAsync().Result);
                    break;
                case "500":
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
        //private void ModelValueChanged(object sender, string valueName, object oldValue, object newValue)
        //{
        //    switch (valueName)
        //    {
        //        case nameof(ChatCommon.CreationTimeLocal): CreationDateTime = (DateTime)newValue; break;
        //        case nameof(ChatCommon.UserContact): UserContact = (User)newValue; break;
        //        case nameof(ChatCommon.UserMain): UserMain = (User)newValue; break;
        //    }
        //}

        //protected override void PropertyNewValue<T>(ref T fieldProperty, T newValue, string propertyName)
        //{
        //    base.PropertyNewValue(ref fieldProperty, newValue, propertyName);

        //    switch (propertyName)
        //    {
        //        case nameof(CreationDateTime): chat.SendValue(nameof(ChatCommon.CreationTimeLocal), CreationDateTime); break;
        //        case nameof(UserContact): chat.SendValue(nameof(ChatCommon.UserContact), UserContact); break;
        //        case nameof(UserMain): chat.SendValue(nameof(ChatCommon.UserMain), UserMain); break;
        //    }

        //}
    }
}
