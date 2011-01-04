
using System;
using System.Collections.Generic;

namespace HashMap
{
	public sealed class MyDictionary<TKey, TValue>
	{
		public MyDictionary()
			: this(EqualityComparer<TKey>.Default)
		{
		}

		public MyDictionary(IEqualityComparer<TKey> comparer)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			m_comparer = comparer;
			m_buckets = new int[c_defaultInitialSize];
			m_entries = new Entry[c_defaultInitialSize];

			for (int bucketIndex = 0; bucketIndex < m_buckets.Length; bucketIndex++)
				m_buckets[bucketIndex] = -1;
		}

		public int Count
		{
			get { return m_nextAvailable - m_removedCount; }
		}

		public TValue this[TKey key]
		{
			get { return GetValue(key); }
			set { SetValue(key, value); }
		}

		public void Add(TKey key, TValue value)
		{
			SetValue(key, value, throwIfExists: true);
		}

		public bool Remove(TKey key)
		{
			return RemoveEntry(key);
		}

		private int Capacity
		{
			get { return m_buckets.Length; }
		}

		private TValue GetValue(TKey key)
		{
			int hashcode = m_comparer.GetHashCode(key);
			int bucket = Math.Abs(hashcode % Capacity);
			int entryIndex = m_buckets[bucket];

			while (entryIndex != -1)
			{
				if (m_comparer.Equals(m_entries[entryIndex].Key, key))
					return m_entries[entryIndex].Value;

				entryIndex = m_entries[entryIndex].NextValue;
			}

			// did not find the appropriate value; throw
			throw new ArgumentException();
		}

		private void SetValue(TKey key, TValue value, bool throwIfExists = false)
		{
			int hashcode = m_comparer.GetHashCode(key);
			int bucket = Math.Abs(hashcode % Capacity);

			bool foundEntry = false;
			int entryIndex = m_buckets[bucket];

			if (entryIndex != -1)
			{
				// find the corresponding value entry for the key (if any) and break
				while (true)
				{
					if (m_comparer.Equals(m_entries[entryIndex].Key, key))
					{
						foundEntry = true;
						break;
					}

					if (m_entries[entryIndex].NextValue == -1)
						break;

					entryIndex = m_entries[entryIndex].NextValue;
				}
			}

			// either
			//   1) we found an entry with the corresponding key value; or
			//   2) there isn't one
			if (!foundEntry)
			{
				Entry entry = new Entry(key, hashcode, value);

				if (m_nextAvailable == Capacity)
				{
					ResizeAndAdd(entry);
				}
				else
				{
					m_entries[m_nextAvailable] = entry;

					if (entryIndex == -1)
						m_buckets[bucket] = m_nextAvailable;
					else
						m_entries[entryIndex].NextValue = m_nextAvailable;

					m_nextAvailable++;
				}
			}
			else
			{
				if (throwIfExists)
					throw new ArgumentException();

				m_entries[entryIndex].Value = value;
			}
		}

		private bool RemoveEntry(TKey key)
		{
			int hashcode = m_comparer.GetHashCode(key);
			int bucket = Math.Abs(hashcode % Capacity);

			int entryIndex = m_buckets[bucket];
			if (entryIndex != -1)
			{
				if (m_comparer.Equals(m_entries[entryIndex].Key, key))
				{
					// the bucket points directly to the entry; update it to point at the next entry in the chain
					//   (which may very well be -1, meaning no chain)
					m_buckets[bucket] = m_entries[entryIndex].NextValue;
					m_removedCount++;
					return true;
				}
				else
				{
					// the entry may be pointed to from the chain (rather than directly from a bucket)
					while (m_entries[entryIndex].NextValue != -1)
					{
						int nextEntryIndex = m_entries[entryIndex].NextValue;
						
						Entry nextEntry = m_entries[nextEntryIndex];
						if (m_comparer.Equals(nextEntry.Key, key))
						{
							m_entries[entryIndex].NextValue = nextEntry.NextValue;
							m_removedCount++;
							return true;
						}

						entryIndex = nextEntryIndex;
					}
				}
			}

			// no matching key found
			return false;
		}

		private void ResizeAndAdd(Entry entry)
		{
			// TODO: consider not resizing if there is high fragmentation (a high value for m_removedCount)

			int newSize = GetNextSize();
			int nextAvailable = 0;

			int[] newBuckets = new int[newSize];
			Entry[] newEntries = new Entry[newSize];

			for (int bucketIndex = 0; bucketIndex < newBuckets.Length; bucketIndex++)
				newBuckets[bucketIndex] = -1;

			// add the new entry first (no need to handle collisions)
			{
				int bucketIndex = Math.Abs(entry.HashCode % newSize);
				newBuckets[bucketIndex] = nextAvailable;
				newEntries[nextAvailable] = entry;
				nextAvailable++;
			}

			// process all of the existing entries
			for (int currentBucketIndex = 0; currentBucketIndex < m_nextAvailable; currentBucketIndex++)
			{
				int currentEntryIndex = m_buckets[currentBucketIndex];
				while (currentEntryIndex != -1)
				{
					int nextEntryIndex = m_entries[currentEntryIndex].NextValue;

					// clear old next value pointer
					m_entries[currentEntryIndex].NextValue = -1;

					// copy the entry to the new entries table
					newEntries[nextAvailable] = m_entries[currentEntryIndex];

					// look up the bucket for the current entry
					int bucketIndex = Math.Abs(m_entries[currentEntryIndex].HashCode % newSize);
					int entryIndex = newBuckets[bucketIndex];

					if (entryIndex == -1)
					{
						// no entry for the current bucket; set it
						newBuckets[bucketIndex] = nextAvailable;
					}
					else
					{
						// we've had a collision; set the next value pointer at the tail of the chain
						while (newEntries[entryIndex].NextValue != -1)
							entryIndex = newEntries[entryIndex].NextValue;

						newEntries[entryIndex].NextValue = nextAvailable;
					}

					nextAvailable++;
					currentEntryIndex = nextEntryIndex;
				}
			}

			m_nextAvailable = nextAvailable;
			m_removedCount = 0;
			m_buckets = newBuckets;
			m_entries = newEntries;
		}

		private int GetNextSize()
		{
			// for better distribution among the buckets, the size should be something prime;
			//   for our purposes, this is close enough...
			return Capacity * 2 + 1;
		}

		struct Entry
		{
			public Entry(TKey key, int cachedHashcode, TValue value)
			{
				m_key = key;
				m_hashcode = cachedHashcode;
				m_value = value;
				m_nextValue = -1;
			}

			public TKey Key
			{
				get { return m_key; }
			}

			public int HashCode
			{
				get { return m_hashcode; }
			}

			public TValue Value
			{
				get { return m_value; }
				set { m_value = value; }
			}

			public int NextValue
			{
				get { return m_nextValue; }
				set { m_nextValue = value; }
			}

			readonly TKey m_key;
			readonly int m_hashcode;

			// Caution: mutable!
			TValue m_value;
			int m_nextValue;
		}

		const int c_defaultInitialSize = 5; // something prime...

		readonly IEqualityComparer<TKey> m_comparer;

		int m_nextAvailable;
		int m_removedCount;

		// buckets hold indexes into the entries table
		int[] m_buckets;

		// when collisions occur, entries are chained
		Entry[] m_entries;
	}
}
