using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessangerv2.Net.Model
{
    [Serializable]
    public class Message
    {
        /// <summary>
        /// ID сообщения
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// ID отправителя
        /// </summary>
        public User Sender { get; set; }
        /// <summary>
        /// Просмотрено
        /// </summary>
        public Guid ChatId { get; set; }
        public bool? IsViewed { get; set; }
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Количество прикреплённых файлов
        /// </summary>
        public int AttachmentsCount { get; set; }
        /// <summary>
        /// Прикреплённые файлы
        /// </summary>
        public IEnumerable<string> Attachments { get; set; }
    }
}
