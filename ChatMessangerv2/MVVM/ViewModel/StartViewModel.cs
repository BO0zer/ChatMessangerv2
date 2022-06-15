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
using System.Net.Http.Json;
using System.Threading;

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

            RegisterToServer = new RelayCommand(o => Register(o));
            AuthoriseToServer = new RelayCommand(o => Authorise(o));
        }
        public async Task Register(object parameter)
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
                    Authorise(parameter);
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
        public async Task Authorise(object parameter)
        {
            _server = new ServerHttp();
            var result = await _server.Authorise(new NetUser() { Login = LoginText, Password = PasswordText});
            var status = result.StatusCode;
            switch (status)
            {
                case HttpStatusCode.OK:
                    Token = result.Headers.GetValues("Token").First();
                    MainView mv = new MainView();
                    var user = await result.Content.ReadFromJsonAsync<NetUser>(null, CancellationToken.None);
                    User.YouUser = user;
                    MainViewModel = new MainViewModel();
                    mv.DataContext = MainViewModel;
                    mv.Show();
                    (parameter as Window).Close();
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
}
