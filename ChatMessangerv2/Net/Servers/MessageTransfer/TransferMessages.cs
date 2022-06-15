using ChatMessangerv2.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferLibrary;

namespace ChatMessangerv2.Net.Servers.MessageTransfer
{
    [Serializable]
    public abstract class Transfer
    {
        public string Token { get; private set; }
        public ACTION Action { get; set; }
        public void SetToken(string t)
        {
            Token = t;
        }

    }
    /// <summary>
    /// Вариации действий
    /// </summary>
    /// <summary>
    /// Json класс с сообщениями и амортизационными токенами
    /// </summary>
    [Serializable]
    public class TransferMessages : Transfer
    {
        /// <summary>
        /// передаваемое сообщение
        /// </summary>
        public Message Message { get; set; }

        public bool? ForAll { get; set; }
    }
}
