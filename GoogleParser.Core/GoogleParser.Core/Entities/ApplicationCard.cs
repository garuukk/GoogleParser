using System;

namespace GoogleParser.Core.Entities
{
    /// <summary>
    /// Карточка приложения
    /// </summary>
    public class ApplicationCard : Entity
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на приложение
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Название приложения
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Название разработчика
        /// </summary>
        public string DeveloperName { get; set; }

        /// <summary>
        /// Почта разработчика
        /// </summary>
        public string DeveloperEmail { get; set; }

        /// <summary>
        /// Фактический адрес разработчика
        /// </summary>
        public string DeveloperPhysicalAddress { get; set; }
    }
}
