using ChatMessangerv2.Net.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessangerv2.MVVM.Model
{
    public class Chat
    {
        public Guid Id { get; set; }
        public DateTime CreationTimeLocal { get; set; }
        public  User UserMain { get; set; }
        public  User UserContact { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public static Chat CommonChat { get; set; }
    }
}
