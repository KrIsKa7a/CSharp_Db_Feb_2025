// ReSharper disable PossibleMultipleEnumeration
namespace MiniORM
{
    using System.Collections;

    public class DbSet<TEntity> : ICollection<TEntity>
        where TEntity : class, new()
    {
        internal DbSet(IEnumerable<TEntity> entities)
        {
            this.ChangeTracker = new ChangeTracker<TEntity>(entities);
            this.Entities = entities.ToList();
        }

        internal ChangeTracker<TEntity> ChangeTracker { get; set; }

        internal ICollection<TEntity> Entities { get; set; }

        public int Count
            => this.Entities.Count;

        public bool IsReadOnly
            => this.Entities.IsReadOnly;

        public void Add(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(ErrorMessages.ItemNotNull);
            }

            this.Entities.Add(item);
            this.ChangeTracker.Add(item); // Notification to the change tracker about the added entity record
        }

        public void Clear()
        {
            while (this.Entities.Any())
            {
                TEntity entityToRemove = this.Entities.First();
                this.Remove(entityToRemove);
            }
        }

        public bool Contains(TEntity item)
        {
            return this.Entities.Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            this.Entities.CopyTo(array, arrayIndex);
        }

        public bool Remove(TEntity item)
        {
            bool isRemoved = this.Entities.Remove(item);
            if (isRemoved)
            {
                this.ChangeTracker.Remove(item); // Notification to ChangeTracker about the removed entity record
            }

            return isRemoved;
        }

        public bool RemoveRange(IEnumerable<TEntity> range)
        {
            foreach (TEntity entityToRemove in range)
            {
                bool result = this.Remove(entityToRemove);
                if (!result)
                {
                    return false; // Stop the removing since we have invalid parameter
                }
            }

            return true;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
