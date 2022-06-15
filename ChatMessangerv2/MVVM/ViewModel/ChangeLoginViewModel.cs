using ChatMessangerv2.Core;
using ChatMessangerv2.MVVM.Model;
using ChatMessangerv2.Net;
using ChatMessangerv2.Net.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ChatMessangerv2.MVVM.ViewModel
{
    public class ChangeLoginViewModel
    {
        public string NewLogin { get; set; }
        public string OldPassword { get; set; }
        public RelayCommand ChangeLogin { get; set; }
        public RelayCommand GetBack { get; set; }
        private ServerHttp _server;
        public ChangeLoginViewModel()
        {
            ChangeLogin = new RelayCommand(o => ChangeLoginToServer());
            GetBack = new RelayCommand(Close);
        }
        public async Task ChangeLoginToServer()
        {
            _server = new ServerHttp();
            NetUser netUser = new NetUser() { Id = User.YouUser.Id, Login = NewLogin, Password = OldPassword };
            var result = await _server.NewLogin(netUser, NewLogin);
            var status = result.StatusCode;
            switch (status)
            {
                case HttpStatusCode.OK:
                    User.YouUser.Password = NewLogin;
                    MessageBox.Show("Пароль успешно изменен");
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
        public void Close(object paramater)
        {
            (paramater as Window).Close();
        }
    }
}
