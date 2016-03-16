namespace Svyaznoy.Core.Model
{
    using System;

    public interface IRegistratorChanges : IEntity
    {
        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Идентификатор регистрируемого объекта
        /// </summary>
        Guid ObjectId { get; set; }

        /// <summary>
        /// Тип регистрируемого объекта
        /// </summary>
        string EntityName { get; set; }

        /// <summary>
        /// Название измененного свойства
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// Старое значение записи
        /// </summary>
        string OldValue { get; set; }

        /// <summary>
        /// Новое значение записи
        /// </summary>
        string NewValue { get; set; }

        /// <summary>
        /// Дата создания записи
        /// </summary>
        DateTime CreatedUtcDate { get; set;}

        /// <summary>
        /// Состояние объекта
        /// </summary>
        int EntityState { get; set; }
    }
}
