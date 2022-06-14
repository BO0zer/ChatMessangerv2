using ChatMessangerv2.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    [Serializable]
    public enum ACTION
    {
        SEND,
        CHNG,
        DEL,
        GET
    }
    /// <summary>
    /// Json класс с сообщениями и амортизационными токенами
    /// </summary>
    [Serializable]
    public class TransferMessages : Transfer
    {
        internal ACTION action;

        /// <summary>
        /// передаваемое сообщение
        /// </summary>
        public NetMessage Message { get; set; }
    }
}
