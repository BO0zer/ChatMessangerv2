using ChatMessangerv2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessangerv2.MVVM.ViewModel
{
    public class ChangePasswordViewModel
    {
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public RelayCommand ChangePassword { get; set; }
        public RelayCommand GetBack { get; set; }
        public ChangePasswordViewModel()
        {
            ChangePassword = new RelayCommand(o => ChangePasswordToServer());
        }
        public async Task ChangePasswordToServer()
        {

        }
    }
}
