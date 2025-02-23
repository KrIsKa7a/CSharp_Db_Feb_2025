namespace MiniORM
{
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    using static ErrorMessages;

    public class ChangeTracker<T>
        where T : class, new()
    {
        private readonly ICollection<T> allEntities;
        private readonly ICollection<T> added;
        private readonly ICollection<T> removed;

        public ChangeTracker(IEnumerable<T> entities)
        {
            this.allEntities = this.CloneEntities(entities);

            this.added = new HashSet<T>();
            this.removed = new HashSet<T>();
        }

        public IReadOnlyCollection<T> AllEntities
            => this.allEntities.ToList().AsReadOnly();

        public IReadOnlyCollection<T> Added
            => this.added.ToList().AsReadOnly();

        public IReadOnlyCollection<T> Removed
            => this.removed.ToList().AsReadOnly();

        /// <summary>
        /// This method tracks newly added entity records by adding them to the "added" collection.
        /// This method is not intended to actually persist the new entities in the DB.
        /// </summary>
        /// <param name="entity">New record of entity</param>
        public void Add(T entity)
        {
            this.added.Add(entity);
        }

        /// <summary>
        /// This method tracks removed entity records by adding them to the "removed" collection.
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(T entity)
        {
            this.removed.Add(entity);
        }

        public IEnumerable<T> GetModifiedEntities(DbSet<T> dbSet)
        {
            ICollection<T> modifiedEntities = new HashSet<T>();
            PropertyInfo[] primaryKeys = typeof(T)
                .GetProperties()
                .Where(pi => pi.HasAttribute<KeyAttribute>())
                .ToArray();
            foreach (T proxyEntity in this.allEntities)
            {
                IEnumerable<object> proxyPrimaryKeys =
                    GetPrimaryKeyValues(primaryKeys, proxyEntity);
                T dbSetEntity = dbSet
                    .Entities
                    .Single(e => GetPrimaryKeyValues(primaryKeys, e).SequenceEqual(proxyPrimaryKeys));

                bool isEntityModified = this.IsModified(proxyEntity, dbSetEntity);
                if (isEntityModified)
                {
                    modifiedEntities.Add(dbSetEntity);
                }
            }

            return modifiedEntities;
        }

        private static IEnumerable<object> GetPrimaryKeyValues(IEnumerable<PropertyInfo> primaryKeys, T entity)
        {
            ICollection<object> primaryKeyValues = new HashSet<object>();
            foreach (PropertyInfo primaryKeyInfo in primaryKeys)
            {
                object? primaryKeyValue = primaryKeyInfo.GetValue(entity);
                if (primaryKeyValue == null)
                {
                    throw new ArgumentNullException(primaryKeyInfo.Name, 
                        PrimaryKeyNullErrorMessage);
                }

                primaryKeyValues.Add(primaryKeyValue);
            }

            return primaryKeyValues;
        }

        private ICollection<T> CloneEntities(IEnumerable<T> entities)
        {
            ICollection<T> clonedEntities = new HashSet<T>();
            PropertyInfo[] propertiesToClone = typeof(T)
                .GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
                .ToArray();

            foreach (T entity in entities)
            {
                T entityClone = Activator.CreateInstance<T>();
                foreach (PropertyInfo propertyInfo in propertiesToClone)
                {
                    object? originalEntityValue = propertyInfo.GetValue(entity);
                    propertyInfo.SetValue(entityClone, originalEntityValue);
                }

                clonedEntities.Add(entityClone);
            }

            return clonedEntities;
        }

        private bool IsModified(T proxyEntity, T dbSetEntity)
        {
            PropertyInfo[] monitoredProperties = typeof(T)
                .GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
                .ToArray();
            foreach (PropertyInfo pi in monitoredProperties)
            {
                object? proxyEntityValue = pi.GetValue(proxyEntity);
                object? dbSetEntityValue = pi.GetValue(dbSetEntity);

                if (proxyEntityValue == null &&
                    dbSetEntityValue == null)
                {
                    continue;
                }

                if (!proxyEntityValue!.Equals(dbSetEntityValue))
                {
                    return true;
                }
            }

            return false;
        }
    }
}