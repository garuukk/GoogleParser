namespace Svyaznoy.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Предоставляет функциональные возможности к источнику данных сохраняя изменения данных
    /// </summary>
    public interface IHistoryDataInventory : IInventory
    {
        /// <summary>
        /// Выбирает не удаленные объекты
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="noTracking"></param>
        IQueryable<T> SelectNotDeleted<T>(Expression<Func<T, bool>> where, bool noTracking = false)
            where T : class, ISupportHistoryEntity;

        /// <summary>
        /// Выбирает не удаленные объекты
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="noTracking"></param>
        IQueryable<T> SelectNotDeleted<T>(bool noTracking = false) where T : class, ISupportHistoryEntity;

        /// <summary>
        /// Помечает объект как удаленный в БД
        /// </summary>
        /// <typeparam name="T">тип объекта</typeparam>
        /// <param name="id">идентификатор удаляемого объекта</param>
        void DeleteOnSubmit<T>(Guid id) where T : class, Svyaznoy.Core.Model.ISupportHistoryEntity;

        /// <summary>
        /// Помечает объекты как удаленные из БД
        /// </summary>
        /// <typeparam name="T">тип объекта</typeparam>
        /// <param name="ids">идентификаторы удаляемых объектов</param>
        void DeleteAllOnSubmit<T>(IEnumerable<Guid> ids) where T : class, Svyaznoy.Core.Model.ISupportHistoryEntity;

        /// <summary>
        /// Сохраняет изменения записывая их в лог
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        void SubmitChangesWithLog(Guid userId);
    }
}
