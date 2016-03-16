using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Svyaznoy.Core.Model
{
    /// <summary>
    /// Предоставляет функциональные возможности к источнику данных 
    /// </summary>
    public interface IInventory : ISqlInventory, IDisposable
    {
        void SubmitChanges();

        /// <summary>
        /// Создает объект в БД, выполняет SubmitChanges
        /// </summary>
        /// <typeparam name="T">тип объекта</typeparam>
        /// <param name="entity">создаваемый объект</param>
        /// <returns>созданный объект</returns>
        T Create<T>(T entity) where T : class, Svyaznoy.Core.Model.IEntity;

        /// <summary>
        /// Создает объект в БД
        /// </summary>
        /// <typeparam name="T">тип объекта</typeparam>
        /// <param name="entity">создаваемый объект</param>
        void CreateOnSubmit<T>(T entity) where T : class, Svyaznoy.Core.Model.IEntity;

        /// <summary>
        /// Выбирает объекты
        /// </summary>
        /// <typeparam name="T">тип объекта</typeparam>
        /// <param name="noTracking"></param>
        /// <returns></returns>
        IQueryable<T> Select<T>(bool noTracking = false) where T : class, Svyaznoy.Core.Model.IEntity;

        /// <summary>
        /// Удаляет объект в БД
        /// </summary>
        /// <typeparam name="T">тип объекта</typeparam>
        /// <param name="entity">удаляемый объект</param>
        /// <param name="attach">прикрепить объект к контексту</param>
        void DeleteOnSubmit<T>(T entity, bool attach = false) where T : class, Svyaznoy.Core.Model.IEntity;

        /// <summary>
        /// Удалет объекты из БД
        /// </summary>
        /// <typeparam name="T">тип объекта</typeparam>
        /// <param name="entities">удаляемые объекты</param>
        /// <param name="attach">прикреить объекты к контексту</param>
        void DeleteAllOnSubmit<T>(IEnumerable<T> entities, bool attach = false) where T : class, Svyaznoy.Core.Model.IEntity;

        /// <summary>
        /// Выбирает объекты по условию
        /// </summary>
        /// <typeparam name="T">тип объекта</typeparam>
        /// <param name="where">условие выборки объектов</param>
        /// <param name="noTracking">кэшировать данные в контекст</param>
        /// <returns></returns>
        IQueryable<T> Select<T>(Expression<Func<T, bool>> where, bool noTracking = false) where T : class, Svyaznoy.Core.Model.IEntity;

        /// <summary>
        /// Выбрать первый объект подходящий по условие или вернуть значени по умолчанию
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="noTracking">кэшировать данные в контекст</param>
        /// <returns>Возвращается объект</returns>
        T FirstOrDefault<T>(Expression<Func<T, bool>> where, bool noTracking = false) where T : class, Svyaznoy.Core.Model.IEntity;
    }
}