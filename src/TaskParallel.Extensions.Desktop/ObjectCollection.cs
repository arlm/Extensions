﻿// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Collections.Concurrent
{
    /// <summary>Provides a thread-safe object pool.</summary>
    /// <typeparam name="T">Specifies the type of the elements stored in the pool.</typeparam>
    [DebuggerDisplay("Count={Count}")]
    [DebuggerTypeProxy(typeof(IProducerConsumerCollection_DebugView<>))]
    public sealed class ObjectCollection<T> : ProducerConsumerCollection<T>
    {
        private readonly Func<T> _generator;

        /// <summary>Initializes an instance of the ObjectPool class.</summary>
        /// <param name="generator">The function used to create items when no items exist in the pool.</param>
        public ObjectCollection(Func<T> generator) : this(generator, new ConcurrentQueue<T>()) { }

        /// <summary>Initializes an instance of the ObjectPool class.</summary>
        /// <param name="generator">The function used to create items when no items exist in the pool.</param>
        /// <param name="collection">The collection used to store the elements of the pool.</param>
        public ObjectCollection(Func<T> generator, IProducerConsumerCollection<T> collection)
            : base(collection)
        {
            if (generator == null)
                throw new ArgumentNullException(nameof(generator));
            _generator = generator;
        }

        /// <summary>Adds the provided item into the pool.</summary>
        /// <param name="item">The item to be added.</param>
        public void PutObject(T item) { base.TryAdd(item); }

        /// <summary>Gets an item from the pool.</summary>
        /// <returns>The removed or created item.</returns>
        /// <remarks>If the pool is empty, a new item will be created and returned.</remarks>
        public T GetObject()
        {
            T value;
            return base.TryTake(out value) ? value : _generator();
        }

        /// <summary>Clears the object pool, returning all of the data that was in the pool.</summary>
        /// <returns>An array containing all of the elements in the pool.</returns>
        public T[] ToArrayAndClear()
        {
            var items = new List<T>();
            T value;
            while (base.TryTake(out value))
                items.Add(value);
            return items.ToArray();
        }

        protected override bool TryAdd(T item)
        {
            PutObject(item);
            return true;
        }

        protected override bool TryTake(out T item)
        {
            item = GetObject();
            return true;
        }
    }
}
