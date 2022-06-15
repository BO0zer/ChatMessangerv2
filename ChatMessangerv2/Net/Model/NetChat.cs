using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessangerv2.Net.Model
{
    class NetChat
    {
        /// <summary>
        /// ID чата
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Время создания
        /// </summary>
        public DateTime CreationTimeLocal { get; set; }
        /// <summary>
        /// Участники чата
        /// </summary>
        public IEnumerable<User> ChatMembers { get; set; }
    }
}
