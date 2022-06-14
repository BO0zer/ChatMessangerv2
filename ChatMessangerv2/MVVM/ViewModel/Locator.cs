using ChatMessangerv2.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessangerv2.MVVM.ViewModel
{
    public class Locator
    {
        //private readonly UserAuth user = new UserAuth();
        //private readonly ChatCommon chat = new ChatCommon();
        public StartViewModel StartViewModel { get; }
        //public MainViewModel MainViewModel { get; }

        public AddContactViewModel AddContactViewModel { get; }

        public Locator()
        {
            StartViewModel = new StartViewModel();
            //MainViewModel = new MainViewModel();
            AddContactViewModel = new AddContactViewModel();
        }
    }
}
