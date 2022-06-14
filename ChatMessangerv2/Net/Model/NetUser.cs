using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessangerv2.Net.Model
{
    public class NetUser
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }
    }
}
