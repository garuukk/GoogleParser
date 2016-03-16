using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace Svyaznoy.Core.Model
{
    public class EntityInventory : SqlInventory, Svyaznoy.Core.Model.IInventory, IDisposable
    {
        private readonly string _contextMetadata;

        protected Context _model;

        /// <summary>
        /// For transaction support begin transaction first
        /// </summary>
        protected Context Model
        {
            get
            {
                if (_model == null)
                {
                    _model = Context.Create(ConnectionString, _contextMetadata);

                    _model.Configuration.AutoDetectChangesEnabled = false;
                }
                return _model;
            }
        }

        public EntityInventory(string connectionString, string contextMetadata)
            : base(connectionString)
        {
            _contextMetadata = contextMetadata;
        }

        public void SubmitChanges()
        {
            SaveChanges();
        }

        /// <summary>
        /// Создает элемент в БД, выполняет SaveChanges
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Create<T>(T entity) where T : class, Svyaznoy.Core.Model.IEntity
        {
            Model.Set<T>().Add(entity);
            SaveChanges();
            return entity;
        }

        /// <summary>
        /// Создает элемент в БД
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void CreateOnSubmit<T>(T entity) where T : class, Svyaznoy.Core.Model.IEntity
        {
            Model.Set<T>().Add(entity);
        }

        public IQueryable<T> Select<T>(bool noTracking = false) where T : class, Svyaznoy.Core.Model.IEntity
        {
            if (noTracking)
            {
                return Model.Set<T>().AsNoTracking().AsQueryable();
            }
            else
            {
                return Model.Set<T>().AsQueryable();
            }
        }

        public void DeleteOnSubmit<T>(T entity, bool attach = false) where T : class, Svyaznoy.Core.Model.IEntity
        {
            if (attach)
            {
                Model.Set<T>().Attach(entity);
            }

            Model.Set<T>().Remove(entity);
        }

        public void DeleteAllOnSubmit<T>(IEnumerable<T> entities, bool attach = false)
            where T : class, Svyaznoy.Core.Model.IEntity
        {
            if (entities == null)
                return;

            if (attach)
            {
                foreach (var entity in entities.ToList())
                {
                    var entry = Model.Entry(entity);
                    if (entry.State == EntityState.Detached)
                    {
                        Model.Set<T>().Attach(entity);
                    }
                    Model.Set<T>().Remove(entity);
                }
            }
            else
            {
                foreach (var entity in entities.ToList())
                {
                    Model.Set<T>().Remove(entity);
                }    
            }
            
        }

        public IQueryable<T> Select<T>(Expression<Func<T, bool>> where, bool noTracking = false) where T : class, Svyaznoy.Core.Model.IEntity
        {
            if (noTracking)
                return Model.Set<T>().AsNoTracking().Where(where);
            else
                return Model.Set<T>().Where(where);
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> where, bool noTracking = false) where T : class, Svyaznoy.Core.Model.IEntity
        {
            if (noTracking)
                return Model.Set<T>().AsNoTracking().FirstOrDefault(where);
            else
                return Model.Set<T>().FirstOrDefault(where);
        }

        public override void Dispose()
        {
            try
            {
                base.Dispose();
            }
            finally
            {
                if (_model != null)
                {
                    _model.Dispose();
                }
            }
        }

        private int SaveChanges()
        {
            try
            {
                Model.ChangeTracker.DetectChanges();
                return Model.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
}