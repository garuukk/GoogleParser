using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace Svyaznoy.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Linq.Expressions;

    public class HistoryDataInventory<TLogEntity> : EntityInventory, IHistoryDataInventory where TLogEntity : class, IRegistratorChanges
    {
        private readonly List<string> IgnoreLogProperties = new List<string>
                                                            {
                                                                "UpdatedUtcDate"
                                                                //, "CreatedDateUtc"
                                                            }; 

        public HistoryDataInventory(string connectionString, string contextMetadata)
            : base(connectionString, contextMetadata)
        {
        }

        public IQueryable<T> SelectNotDeleted<T>(Expression<Func<T, bool>> where, bool noTracking = false) where T : class, ISupportHistoryEntity
        {
            if (noTracking)
                return Model.Set<T>().AsNoTracking().Where(m => !m.IsDeleted).Where(where);
            else
                return Model.Set<T>().Where(m => !m.IsDeleted).Where(where);
        }

        public IQueryable<T> SelectNotDeleted<T>(bool noTracking = false) where T : class, ISupportHistoryEntity
        {
            if (noTracking)
            {
                return Model.Set<T>().AsNoTracking().AsQueryable().Where(t => !t.IsDeleted);
            }
            else
            {
                return Model.Set<T>().AsQueryable().Where(t => !t.IsDeleted);
            }
        }

        public void DeleteOnSubmit<T>(Guid id) where T : class, ISupportHistoryEntity
        {
            var entity = SelectNotDeleted<T>().FirstOrDefault(e => e.Id == id);

            if (entity == null) 
                return;

            entity.IsDeleted = true;
        }

        public void DeleteAllOnSubmit<T>(IEnumerable<Guid> ids) where T : class, ISupportHistoryEntity
        {
            if (!ids.HasValues())
                return;

            var entities = SelectNotDeleted<T>().Where(e => ids.Contains(e.Id));

            foreach (var entity in entities.ToList())
            {
                entity.IsDeleted = true;
            }
        }

        public void SubmitChangesWithLog(Guid userId)
        {
            try
            {
                Model.ChangeTracker.DetectChanges();
                DetectChangesAndSaveInLog(userId);
                Model.SaveChanges();
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

        /// <summary>
        /// Определить изменения и сохранить в лог
        /// </summary>
        /// <param name="userId"></param>
        private void DetectChangesAndSaveInLog(Guid userId)
        {
            ObjectContext ctx = ((IObjectContextAdapter)Model).ObjectContext;

            List<ObjectStateEntry> objectStateEntryList =
                ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added
                                                           | EntityState.Modified
                                                           | EntityState.Deleted)
                .ToList();

            foreach (ObjectStateEntry entry in objectStateEntryList)
            {
                var entity = entry.Entity as ISupportHistoryEntity;
                if (entity == null) 
                    continue;

                var entityName = entry.EntitySet.Name;
                var entityState = entry.State;
                var objectId = entity.Id;

                if (!entry.IsRelationship)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                        case EntityState.Deleted:
                            {
                                var registrator = CreateEntity(userId, entityName, entityState, objectId);
                                CreateOnSubmit(registrator);
                            }
                            break;
                        case EntityState.Modified:
                        {
                            var modifiedProperties =
                                entry.GetModifiedProperties()
                                    .Where(
                                        p =>
                                            IgnoreLogProperties.All(
                                                i => !string.Equals(i, p, StringComparison.InvariantCultureIgnoreCase))).ToList();

                            foreach (string propertyName in modifiedProperties)
                                {
                                    DbDataRecord original = entry.OriginalValues;
                                    string oldValue = original.GetValue(original.GetOrdinal(propertyName)).ToString();

                                    CurrentValueRecord current = entry.CurrentValues;
                                    string newValue = current.GetValue(current.GetOrdinal(propertyName)).ToString();

                                    if (!string.Equals(oldValue, newValue, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        var registrator = CreateEntity(userId, entityName, entityState, objectId);
                                        registrator.NewValue = newValue;
                                        registrator.OldValue = oldValue;
                                        registrator.PropertyName = propertyName;
                                        CreateOnSubmit(registrator);
                                    }
                                }
                                break;
                            }
                    }
                    
                }
            }
        }

        private TLogEntity CreateEntity(Guid userId, string entityName, EntityState entityState, Guid objectId)
        {
            Type _logEntityType = typeof(TLogEntity);
            TLogEntity registrator = (TLogEntity)_logEntityType.Assembly.CreateInstance(_logEntityType.FullName);

            registrator.Id = Guid.NewGuid();
            //registrator.UserId = userId;
            registrator.CreatedUtcDate = DateTime.UtcNow;
            registrator.EntityName = entityName;
            registrator.EntityState = (int)entityState;
            registrator.ObjectId = objectId;

            return registrator;
        }
    }
}
