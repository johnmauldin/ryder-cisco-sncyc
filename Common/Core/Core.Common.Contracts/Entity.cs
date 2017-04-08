using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Contracts
{
    public abstract class Entity<TId>
    {
        public virtual TId Id { get; protected set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity<TId>);
        }

        /// <summary>
        /// A Transient Object is an object that has not been saved to the database
        /// and does not have an Identity set.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsTransient(Entity<TId> obj)
        {
            return obj != null && Equals(obj.Id, default(TId));
        }

        /// <summary>
        /// Used to return the underlying type that a proxy is based.
        /// </summary>
        /// <returns></returns>
        private Type GetUnproxiedType()
        {
            return GetType();
        }

        /// <summary>
        /// Because NHibernate uses Proxy Objects to support Lazy Loading, there are subtle
        /// bugs that can arise when trying to compare an particular entity with a proxy.
        /// 
        /// Equality is based on POID as opposed to other members available on the Entity.
        ///  </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(Entity<TId> other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            // Both Entities have been saved to the database, and their POID values are equal.
            if (!IsTransient(this) && !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();

                // Make sure that both objects are of the same type.
                return thisType.IsAssignableFrom(otherType) ||
                       otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Equals(Id, default(TId)))
                return base.GetHashCode();

            return Id.GetHashCode();
        }
    }

    public abstract class Entity : Entity<int>
    {

    }

    public abstract class AssignableIdEntity<TId> : Entity<TId>
    {

        public virtual void SetId(TId id)
        {
            Id = id;
        }
    }

}
