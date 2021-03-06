namespace System.Collections.Generic
{
    using Visitors;

    /// <summary>
    ///   A custom stack datastructure.
    /// </summary>
    public sealed class VisitableStack<T> : Stack<T>, IVisitableCollection<T>, IStack<T>
    {
        #region Construction
        /// <summary>
        ///   Initializes a new instance of the <see cref="Stack&lt;T&gt;" /> class.
        /// </summary>
        public VisitableStack()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Stack&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        public VisitableStack(int capacity) : base(capacity)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Stack&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="collection"> The collection to copy elements from. </param>
        /// <exception cref="T:System.ArgumentNullException">collection is null.</exception>
        public VisitableStack(IEnumerable<T> collection) : base(collection)
        {
        }
        #endregion

        #region IVisitableCollection<T> Members
        /// <summary>
        ///   Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor"> The visitor. </param>
        public void Accept(IVisitor<T> visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException("visitor");
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                visitor.Visit(enumerator.Current);
                if (visitor.HasCompleted)
                    return;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this collection is empty.
        /// </summary>
        /// <value> <c>true</c> if this collection is empty; otherwise, <c>false</c> . </value>
        public bool IsEmpty
        {
            get { return (Count == 0); }
        }

        /// <summary>
        ///   Gets a value indicating whether this collection is full.
        /// </summary>
        /// <value> <c>true</c> if this collection is full; otherwise, <c>false</c> . </value>
        public bool IsFull
        {
            get { return false; }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is of a fixed size.
        /// </summary>
        /// <value> <c>true</c> if this instance is fixed size; otherwise, <c>false</c> . </value>
        public bool IsFixedSize
        {
            get { return false; }
        }
        #endregion

        #region ICollection<T> Members
        /// <summary>
        ///   Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item"> The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see> . </param>
        /// <exception cref="T:System.NotSupportedException">The
        ///   <see cref="T:System.Collections.Generic.ICollection`1"></see>
        ///   is read-only.</exception>
        public void Add(T item)
        {
            Push(item);
        }

        /// <summary>
        ///   Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        /// <value> </value>
        /// <returns> true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false. </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///   Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item"> The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see> . </param>
        /// <returns> true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see> ; otherwise, false. This method also returns false if item is not found in the original <see
        ///    cref="T:System.Collections.Generic.ICollection`1"></see> . </returns>
        /// <exception cref="T:System.NotSupportedException">The
        ///   <see cref="T:System.Collections.Generic.ICollection`1"></see>
        ///   is read-only.</exception>
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region IComparable Members
        /// <summary>
        ///   Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj"> An object to compare with this instance. </param>
        /// <returns> A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj. </returns>
        /// <exception cref="T:System.ArgumentException">obj is not the same type as this instance.</exception>
        public int CompareTo(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (obj.GetType() == GetType())
            {
                var s = obj as VisitableStack<T>;
                return Count.CompareTo(s.Count);
            }
            return String.Compare(GetType().FullName, obj.GetType().FullName, StringComparison.Ordinal);
        }
        #endregion
    }
}