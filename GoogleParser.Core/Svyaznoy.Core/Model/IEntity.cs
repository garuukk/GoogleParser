namespace Svyaznoy.Core.Model
{
    using System;

    public interface IEntity
    {
    }

    /// <summary>
    /// Объект, который поддерживает историю изменений
    /// </summary>
    public interface ISupportHistoryEntity : IEntity
    {
        /// <summary>
        /// Пометка удаления
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        Guid Id { get; set; }
    }
}
