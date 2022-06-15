using ChatMessangerv2.Core;
using ChatMessangerv2.MVVM.Model;
using ChatMessangerv2.Net;
using ChatMessangerv2.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Windows;

namespace ChatMessangerv2.MVVM.ViewModel
{
    public class ChangePasswordViewModel
    {
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public RelayCommand ChangePassword { get; set; }
        public RelayCommand GetBack { get; set; }
        private ServerHttp _server;
        public ChangePasswordViewModel()
        {
            ChangePassword = new RelayCommand(o => ChangePasswordToServer());
            GetBack = new RelayCommand(Close);
        }
        public async Task ChangePasswordToServer()
        {
            _server = new ServerHttp();
            NetUser netUser = new NetUser() { Id = User.YouUser.Id, Login = User.YouUser.Login, Password = OldPassword };
            var result = await _server.NewPassword(netUser, NewPassword);
            var status = result.StatusCode;
            switch(status)
            {
                case HttpStatusCode.OK:
                    User.YouUser.Password = NewPassword;
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
            (paramater as Window).DialogResult = true;
        }
    }
}
