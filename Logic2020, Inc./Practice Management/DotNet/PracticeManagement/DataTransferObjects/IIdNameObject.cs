using System;
using System.Collections.Generic;

namespace DataTransferObjects
{
    internal class IdNameEqualityComparer<T> : IEqualityComparer<T> where T : IIdNameObject
    {
        #region Implementation of IEqualityComparer<T>

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.
        ///                 </param><param name="y">The second object of type <paramref name="T"/> to compare.
        ///                 </param>
        public bool Equals(T x, T y)
        {
            return x.Id != null && y.Id != null && x.Id.Value.Equals(y.Id.Value);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.
        ///                 </param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        ///                 </exception>
        public int GetHashCode(T obj)
        {
            return obj.Id != null ? obj.Id.Value : 0;
        }

        #endregion
    }

    /// <summary>
    /// Represents object that has Id and Name properties
    /// </summary>
    public interface IIdNameObject
    {
        int? Id { get; set; }
        string Name { get; set; }
    }
}

