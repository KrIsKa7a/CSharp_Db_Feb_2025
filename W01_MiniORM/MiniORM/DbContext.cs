// ReSharper disable PossibleMultipleEnumeration
namespace MiniORM
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    using Microsoft.Data.SqlClient;

    using static ErrorMessages;

    public class DbContext
    {
        private readonly DatabaseConnection dbConnection;
        private readonly IDictionary<Type, PropertyInfo> dbSetProperties;

        protected DbContext(string connectionString)
        {
            this.dbConnection = new DatabaseConnection(connectionString);
            this.dbSetProperties = this.DiscoverDbSets();
            using (new ConnectionManager(this.dbConnection))
            {
                this.InitializeDbSets();
            }
            this.MapAllRelations(); // This is done after connection close because it is in-memory
        }

        internal static readonly Type[] AllowedSqlTypes =
        {
            typeof(string),
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime),
            typeof(Guid)
        };

        public void SaveChanges()
        {
            IEnumerable<object> dbSetsObjects = this.dbSetProperties
                .Select(edb => edb.Value.GetValue(this)!)
                .ToArray();
            foreach (IEnumerable<object> dbSet in dbSetsObjects)
            {
                IEnumerable<object> invalidEntities = dbSet
                    .Where(e => !IsObjectValid(e))
                    .ToArray();
                if (invalidEntities.Any())
                {
                    throw new InvalidOperationException(String.Format(InvalidEntitiesInDbSetMessage,
                        invalidEntities.Count(), dbSet.GetType().Name));
                }
            }

            using (new ConnectionManager(this.dbConnection))
            {
                using SqlTransaction transaction = this.dbConnection
                    .StartTransaction();

                foreach (IEnumerable dbSet in dbSetsObjects)
                {
                    Type dbSetType = dbSet.GetType();
                    Type entityType = dbSetType.GetGenericArguments()[0];

                    MethodInfo persistMethod = typeof(DbContext)
                        .GetMethod("Persist", BindingFlags.NonPublic | BindingFlags.Instance)!
                        .MakeGenericMethod(entityType);

                    try
                    {
                        try
                        {
                            persistMethod.Invoke(this, new object[] { dbSet });
                        }
                        catch (TargetInvocationException tie) when (tie.InnerException != null)
                        {
                            throw tie.InnerException;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Performing Rollback due to Exception!!!");
                        transaction.Rollback();
                        throw;
                    }
                }

                transaction.Commit();
            }
        }

        private static bool IsObjectValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            ICollection<ValidationResult> validationErrors = new List<ValidationResult>();

            return Validator
                .TryValidateObject(obj, validationContext, validationErrors, true);
        }

        private IDictionary<Type, PropertyInfo> DiscoverDbSets()
        {
            return this.GetType()
                .GetProperties()
                .Where(pi => pi.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToDictionary(pi => pi.PropertyType.GetGenericArguments().First(), pi => pi);
        }

        private void InitializeDbSets()
        {
            foreach (KeyValuePair<Type, PropertyInfo> dbSetKvp in dbSetProperties)
            {
                Type dbSetType = dbSetKvp.Key;
                PropertyInfo dbSetProperty = dbSetKvp.Value;

                MethodInfo populateDbSetMethodInfo = typeof(DbContext)
                    .GetMethod("PopulateDbSet", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(dbSetType);
                populateDbSetMethodInfo.Invoke(this, new object?[] { dbSetProperty });
            }
        }

        private void MapAllRelations()
        {
            foreach (KeyValuePair<Type, PropertyInfo> dbSetKvp in dbSetProperties)
            {
                Type dbSetEntityType = dbSetKvp.Key;
                PropertyInfo dbSetPropertyInfo = dbSetKvp.Value;

                MethodInfo mapRelationsGenericMethodInfo = typeof(DbContext)
                    .GetMethod("MapRelations", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .MakeGenericMethod(dbSetEntityType);

                object? dbSetInstance = dbSetPropertyInfo.GetValue(this);
                if (dbSetInstance == null)
                {
                    throw new ArgumentNullException(dbSetPropertyInfo.Name,
                        String.Format(NullDbSetMessage, dbSetPropertyInfo.Name));
                }

                mapRelationsGenericMethodInfo.Invoke(this, new object?[] { dbSetInstance });
            }
        }

        private void Persist<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            string tableName = this.GetTableName(typeof(TEntity));
            IEnumerable<string> columnNames = this.dbConnection
                .FetchColumnNames(tableName);

            if (dbSet.ChangeTracker.Added.Any())
            {
                this.dbConnection
                    .InsertEntities(dbSet.ChangeTracker.Added, tableName, columnNames.ToArray());
            }

            IEnumerable<TEntity> modifiedEntities = dbSet
                .ChangeTracker
                .GetModifiedEntities(dbSet);
            if (modifiedEntities.Any())
            {
                this.dbConnection
                    .UpdateEntities(modifiedEntities, tableName, columnNames.ToArray());
            }

            if (dbSet.ChangeTracker.Removed.Any())
            {
                this.dbConnection
                    .DeleteEntities(dbSet.ChangeTracker.Removed, tableName, columnNames.ToArray());
            }
        }

        private void PopulateDbSet<TEntity>(PropertyInfo dbSetPropertyInfo)
            where TEntity : class, new()
        {
            IEnumerable<TEntity> dbSetEntities = this.LoadTableEntities<TEntity>();
            DbSet<TEntity> dbSetInstance = new DbSet<TEntity>(dbSetEntities);
            ReflectionHelper.ReplaceBackingField(this, dbSetPropertyInfo.Name, dbSetInstance);
        }

        private IEnumerable<TEntity> LoadTableEntities<TEntity>()
            where TEntity : class
        {
            Type tableType = typeof(TEntity);
            IEnumerable<string> columnNames = this.GetEntityColumnNames(tableType);
            string tableName = this.GetTableName(tableType);

            return this.dbConnection.FetchResultSet<TEntity>(tableName, columnNames.ToArray());
        }

        private IEnumerable<string> GetEntityColumnNames(Type entityType)
        {
            string tableName = this.GetTableName(entityType);
            IEnumerable<string> tableColumnNames = this.dbConnection
                .FetchColumnNames(tableName);

            IEnumerable<string> entityColumnNames = entityType
                .GetProperties()
                .Where(pi => tableColumnNames.Contains(pi.Name) &&
                             !pi.HasAttribute<NotMappedAttribute>() &&
                             AllowedSqlTypes.Contains(pi.PropertyType))
                .Select(pi => pi.Name)
                .ToArray();

            return entityColumnNames;
        }

        private string GetTableName(Type tableType)
        {
            Attribute? tableNameAttr = Attribute.GetCustomAttribute(tableType, typeof(TableAttribute));
            if (tableNameAttr == null)
            {
                return this.dbSetProperties[tableType].Name;
            }

            if (tableNameAttr is TableAttribute tableNameAttrConf)
            {
                return tableNameAttrConf.Name;
            }

            throw new ArgumentException(String.Format(NoTableNameFound, this.dbSetProperties[tableType].Name));
        }

        private void MapRelations<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            //TEntity = Department.
            Type entityType = typeof(TEntity);

            //We map all navigation properties.
            MapNavigationProperties(dbSet);

            foreach (PropertyInfo property in entityType.GetProperties())
            {
                string typeName = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                    ? $"ICollection<{property.PropertyType.GetGenericArguments()[0].Name}>"
                    : property.PropertyType.Name;

                Console.WriteLine($"Property: {property.Name} | Type: {typeName}");
                Console.WriteLine("---------------------------------------------------------------");
            }

            //Getting all the collections, whose generic type is ICollection ->
            //Meaning: One department can have more than one Employee => inside the Department we have
            //ICollection<Employee>, that is what we are looking for.

            var collections = entityType
                .GetProperties()
                .Where(pi =>
                    pi.PropertyType.IsGenericType &&
                    pi.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                .ToArray();

            foreach (PropertyInfo collection in collections)
            {
                //Taking the generic type arguments the current collection has ->
                //if we take the example with ICollection<Employee> => the generic argument would be 'Employee'.
                //We take only the first one, because we are 100% sure only one argument will be provided.
                Type collectionType = collection.PropertyType.GenericTypeArguments.First();

                //Creating a generic instance of the 'MapCollection' method of type <Department, Employee>
                MethodInfo mapCollectionGeneric = typeof(DbContext)
                    .GetMethod("MapCollection", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .MakeGenericMethod(entityType, collectionType);

                //Dynamically invoking the method
                mapCollectionGeneric.Invoke(this, new object[] { dbSet, collection });
            }
        }

        private void MapNavigationProperties<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            IEnumerable<PropertyInfo> foreignKeys = entityType
                .GetProperties()
                .Where(pi => pi.HasAttribute<ForeignKeyAttribute>());
            foreach (PropertyInfo fkPropertyInfo in foreignKeys)
            {
                string navigationPropName = fkPropertyInfo
                    .GetCustomAttribute<ForeignKeyAttribute>()!.Name;

                PropertyInfo? navigationPropertyInfo = entityType
                    .GetProperty(navigationPropName);
                if (navigationPropertyInfo == null)
                {
                    throw new ArgumentException(String.Format(InvalidNavigationPropertyName,
                        fkPropertyInfo.Name, navigationPropName));
                }

                object? navDbSetInstance = 
                    this.dbSetProperties[navigationPropertyInfo.PropertyType]
                        .GetValue(this);
                if (navDbSetInstance == null)
                {
                    throw new ArgumentException(String.Format(NavPropertyWithoutDbSetMessage,
                        navigationPropName, navigationPropertyInfo.PropertyType));
                }

                PropertyInfo navEntityPkPropInfo = navigationPropertyInfo
                    .PropertyType
                    .GetProperties()
                    .First(pi => pi.HasAttribute<KeyAttribute>());
                foreach (TEntity entity in dbSet)
                {
                    object? fkValue = fkPropertyInfo.GetValue(entity);
                    if (fkValue == null)
                    {
                        navigationPropertyInfo.SetValue(entity, null);
                        continue;
                    }

                    object? navPropValueEntity = ((IEnumerable<object>)navDbSetInstance)
                        .First(currNavPropEntity => navEntityPkPropInfo
                            .GetValue(currNavPropEntity)!
                            .Equals(fkValue));
                    navigationPropertyInfo.SetValue(entity, navPropValueEntity);
                }
            }
        }

        private void MapCollection<TDbSet, TCollection>(DbSet<TDbSet> dbSet, PropertyInfo collectionPropInfo)
            where TDbSet : class, new()
            where TCollection : class, new()
        {
            Type entityType = typeof(TDbSet);
            Type collectionType = typeof(TCollection);

            IEnumerable<PropertyInfo> collectionPrimaryKeys = collectionType
                .GetProperties()
                .Where(pi => pi.HasAttribute<KeyAttribute>());

            PropertyInfo primaryKey = collectionPrimaryKeys.First();

            PropertyInfo foreignKey = entityType
                .GetProperties()
                .First(pi => pi.HasAttribute<KeyAttribute>());
            if (collectionPrimaryKeys.Count() >= 2)
            {
                primaryKey = collectionType
                    .GetProperties()
                    .First(pi => collectionType
                        .GetProperty(pi.GetCustomAttribute<ForeignKeyAttribute>()!.Name)!
                        .PropertyType == entityType);
            }

            DbSet<TCollection> navDbSet = (DbSet<TCollection>)
                this.dbSetProperties[collectionType]
                    .GetValue(this)!;

            foreach (TDbSet dbSetEntity in dbSet)
            {
                object pkValue = foreignKey.GetValue(dbSetEntity)!;
                IEnumerable<TCollection> navCollectionEntities = navDbSet
                    .Where(navEntity => primaryKey.GetValue(navEntity)!.Equals(pkValue));

                var collectionEntitiesAsList = navCollectionEntities.ToList();

                ReflectionHelper.ReplaceBackingField(dbSetEntity, collectionPropInfo.Name, collectionEntitiesAsList);
            }
        }
    }
}
