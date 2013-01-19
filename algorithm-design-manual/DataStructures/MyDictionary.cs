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
			m_buckets = new DictionaryEntry[initialCapacity ?? c_defaultInitialCapacity];
			m_comparer = comparer ?? EqualityComparer<TKey>.Default;
		}

		public void Add(TKey key, TValue value)
		{
			if (m_count == BucketCount)
				Resize();

			FindResult result;
			if (FindEntryForKey(key, out result))
				throw new ArgumentException("Entry for specified key already exists.", "key");

			DictionaryEntry entry = new DictionaryEntry(key, value, m_buckets[result.BucketIndex]);
			m_buckets[result.BucketIndex] = entry;
			m_count++;
		}

		public bool Remove(TKey key)
		{
			FindResult result;
			if (!FindEntryForKey(key, out result))
				return false;

			DictionaryEntry toRemove = result.Entry;
			DictionaryEntry preceding = result.PrecedingEntry;

			if (preceding == null)
				m_buckets[result.BucketIndex] = toRemove.NextEntry;
			else
				preceding.NextEntry = toRemove.NextEntry;

			m_count--;
			return true;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			FindResult result;
			if (FindEntryForKey(key, out result))
			{
				value = result.Entry.Value;
				return true;
			}

			value = default(TValue);
			return false;
		}

		private int BucketCount
		{
			get { return m_buckets.Length; }
		}

		private bool FindEntryForKey(TKey key, out FindResult findResult)
		{
			DictionaryEntry lastEntry = null;
			int bucketIndex = GetBucketIndex(key);
			DictionaryEntry currentEntry = m_buckets[bucketIndex];
			while (currentEntry != null)
			{
				if (IsKeyForEntry(key, currentEntry))
				{
					findResult = new FindResult(bucketIndex, currentEntry, lastEntry);
					return true;
				}

				lastEntry = currentEntry;
				currentEntry = currentEntry.NextEntry;
			}

			findResult = new FindResult(bucketIndex);
			return false;
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

		struct FindResult
		{
			public FindResult(int bucketIndex, DictionaryEntry entry = null, DictionaryEntry precedingEntry = null)
			{
				BucketIndex = bucketIndex;
				Entry = entry;
				PrecedingEntry = precedingEntry;
			}

			public readonly int BucketIndex;
			public readonly DictionaryEntry Entry;
			public readonly DictionaryEntry PrecedingEntry;
		}

		sealed class DictionaryEntry
		{
			public DictionaryEntry(TKey key, TValue value, DictionaryEntry nextEntry)
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
