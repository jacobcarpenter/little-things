using Combinatorics.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures
{
	public sealed class MyDictionary<TKey, TValue>
	{
		public MyDictionary(IEqualityComparer<TKey> comparer = null, int? initialCapacity = null)
		{
			m_comparer = comparer ?? EqualityComparer<TKey>.Default;
			m_buckets = new DictionaryEntry[initialCapacity ?? c_defaultInitialCapacity];
		}

		public void Add(TKey key, TValue value)
		{
			if (m_count == BucketCount)
				Resize();

			int bucketIndex = GetBucketIndex(key);

			DictionaryEntry currentEntry = m_buckets[bucketIndex];
			while (currentEntry != null)
			{
				if (IsKeyForEntry(key, currentEntry))
					throw new ArgumentException("Entry for specified key already exists.", "key");

				currentEntry = currentEntry.NextEntry;
			}

			var entry = new DictionaryEntry(key, value, nextEntry: m_buckets[bucketIndex]);
			m_buckets[bucketIndex] = entry;
			m_count++;
		}

		public bool Remove(TKey key)
		{
			int bucketIndex = GetBucketIndex(key);

			DictionaryEntry lastEntry = null;
			DictionaryEntry currentEntry = m_buckets[bucketIndex];
			while (currentEntry != null)
			{
				if (IsKeyForEntry(key, currentEntry))
				{
					if (lastEntry == null)
						m_buckets[bucketIndex] = currentEntry.NextEntry;
					else
						lastEntry.NextEntry = currentEntry.NextEntry;

					m_count--;
					return true;
				}

				lastEntry = currentEntry;
				currentEntry = currentEntry.NextEntry;
			}

			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			int bucketIndex = GetBucketIndex(key);

			DictionaryEntry currentEntry = m_buckets[bucketIndex];
			while (currentEntry != null)
			{
				if (IsKeyForEntry(key, currentEntry))
				{
					value = currentEntry.Value;
					return true;
				}

				currentEntry = currentEntry.NextEntry;
			}

			value = default(TValue);
			return false;
		}

		private int BucketCount
		{
			get { return m_buckets.Length; }
		}

		private void Resize()
		{
			DictionaryEntry[] oldEntries = m_buckets;

			int nextSize = oldEntries.Length * 2 + 1; // close enough ...
			m_buckets = new DictionaryEntry[nextSize];

			for (int i = 0; i < oldEntries.Length; i++)
			{
				DictionaryEntry currentEntry = oldEntries[i];
				while (currentEntry != null)
				{
					DictionaryEntry nextEntry = currentEntry.NextEntry;
					
					int newBucketIndex = GetBucketIndex(currentEntry.Key);
					
					currentEntry.NextEntry = m_buckets[newBucketIndex];
					m_buckets[newBucketIndex] = currentEntry;

					currentEntry = nextEntry;
				}
			}
		}

		private bool IsKeyForEntry(TKey key, DictionaryEntry entry)
		{
			return m_comparer.Equals(key, entry.Key);
		}

		private int GetBucketIndex(TKey key)
		{
			int hashCode = m_comparer.GetHashCode(key);
			// TODO: unchecked uint math ?
			return Math.Abs(hashCode % BucketCount);
		}

		sealed class DictionaryEntry
		{
			public DictionaryEntry(TKey key, TValue value, DictionaryEntry nextEntry = null)
			{
				Key = key;
				Value = value;
				NextEntry = nextEntry;
			}

			public TKey Key { get; private set; }

			public TValue Value { get; private set; }

			public DictionaryEntry NextEntry { get; set; }
		}

		const int c_defaultInitialCapacity = 5;

		readonly IEqualityComparer<TKey> m_comparer;

		int m_count;
		DictionaryEntry[] m_buckets;
	}
}
