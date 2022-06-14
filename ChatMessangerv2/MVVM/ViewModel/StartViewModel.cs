using ChatMessangerv2.Core;
using ChatMessangerv2.MVVM.Model;
using ChatMessangerv2.MVVM.View;
using ChatMessangerv2.Net;
using ChatMessangerv2.Net.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;

namespace ChatMessangerv2.MVVM.ViewModel
{
    public class StartViewModel: OnPropertyChangedClass
    {
        //private string _login;
        //private string _password;
        //private readonly UserAuth user;
        //private readonly ChatCommon chat = new ChatCommon();
        public static string Token { get; private set; }
        public MainViewModel MainViewModel { get; set; }
        public string LoginText { get; set; }
        public string PasswordText { get; set; }
        //public string Login { get => _login; set=> SetProperty(ref _login, value); }
        //public string Password { get => _password; set=> SetProperty(ref _password, value); }
        public RelayCommand RegisterToServer { get; set; }
        public RelayCommand AuthoriseToServer { get; set; }

        private ServerHttp _server;
        public StartViewModel()
        {
            //this.user = user;
            //user.ValueChanged += ModelValueChanged;
            //user.AllValueChanged();

            RegisterToServer = new RelayCommand(o => Register());
            AuthoriseToServer = new RelayCommand(o => Authorise());
        }
        //private void ModelValueChanged(object sender, string valueName, object oldValue, object newValue)
        //{
        //    switch (valueName)
        //    {
        //        case nameof(UserAuth.Login): Login = (string)newValue; break;
        //        case nameof(UserAuth.Password): Password = (string)newValue; break;
        //    }
        //}

        //protected override void PropertyNewValue<T>(ref T fieldProperty, T newValue, string propertyName)
        //{
        //    base.PropertyNewValue(ref fieldProperty, newValue, propertyName);

        //    switch (propertyName)
        //    {
        //        case nameof(Login): user.SendValue(nameof(UserAuth.Login), Login); break;
        //        case nameof(Password): user.SendValue(nameof(UserAuth.Password), Password); break;
        //    }

        //}
        public async Task Register()
        {
            _server = new ServerHttp();
            var result = await _server.Register(new NetUser() { Login = LoginText, Password = PasswordText});
            var status = result.StatusCode;
            switch(status)
            {
                case HttpStatusCode.OK:
                    MessageBox.Show("Вы успешно зарегистрировались",
                        "Уведомление",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    Token = result.Headers.GetValues("Token").First();
                    MainView mv = new MainView();
                    User.YouUser = new User();
                    User.YouUser.Login = LoginText;
                    User.YouUser.Password = PasswordText;
                    MainViewModel = new MainViewModel();
                    mv.DataContext = MainViewModel;
                    mv.Show();
                    break;
                case HttpStatusCode.BadRequest:
                    var error = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result) as ProblemDetails;
                    MessageBox.Show(error.Detail);
                    break;
                case HttpStatusCode.InternalServerError:
                    MessageBox.Show("Сервер не отвечает");
                    break;
            }
        }
        public async Task Authorise()
        {
            _server = new ServerHttp();
            var result = await _server.Authorise(new NetUser() { Login = LoginText, Password = PasswordText});
            var status = result.StatusCode;
            switch (status)
            {
                case HttpStatusCode.OK:
                    Token = result.Headers.GetValues("Token").First();
                    MainView mv = new MainView();
                    User.YouUser = new User();
                    User.YouUser.Login = LoginText;
                    User.YouUser.Password = PasswordText;
                    MainViewModel = new MainViewModel();
                    mv.DataContext = MainViewModel;
                    mv.Show();
                    break;
                case HttpStatusCode.BadRequest:
                    var error = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result) as ProblemDetails;
                    MessageBox.Show(error.Detail);
                    break;
                case HttpStatusCode.InternalServerError:
                    MessageBox.Show("Сервер не отвечает");
                    break;
            }
        }
    }
}
