using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
	public sealed class PriorityQueue<T>
	{
		// TODO: constructor with initial values? Or maybe a create method?
		public PriorityQueue(IComparer<T> comparer = null, int? initialCapacity = null)
		{
			m_comparer = comparer ?? Comparer<T>.Default;
			m_heap = new T[initialCapacity ?? c_defaultInitialCapacity];
		}

		public int Count
		{
			get { return m_itemCount; }
		}

		public void Enqueue(T item)
		{
			// add to end of heap
			if (m_itemCount == m_heap.Length)
				Resize();

			int currentIndex = m_itemCount;
			m_heap[currentIndex] = item;
			m_itemCount++;

			// TODO: support min heap

			HeapUtility.BubbleUpMax(m_heap, currentIndex, m_comparer);
		}

		public T Dequeue()
		{
			if (m_itemCount == 0)
				throw new InvalidOperationException();

			T result = m_heap[0];

			m_heap[0] = m_heap[m_itemCount - 1];
			m_heap[m_itemCount - 1] = default(T);
			m_itemCount--;

			HeapUtility.BubbleDownMax(m_heap, m_itemCount, 0, m_comparer);

			return result;
		}
		
		public T Peek()
		{
			if (m_itemCount == 0)
				throw new InvalidOperationException();

			return m_heap[0];
		}
		
		private void Resize()
		{
			int currentPower = (int) Math.Ceiling(Math.Log(m_heap.Length, 2));
			currentPower++;
			int newSize = (int) Math.Pow(2, currentPower);
			T[] newHeap = new T[newSize];
			m_heap.CopyTo(newHeap, 0);
			m_heap = newHeap;
		}

		static class HeapUtility
		{
			public static void BubbleUpMax(T[] heap, int currentIndex, IComparer<T> comparer)
			{
				while (currentIndex > 0)
				{
					int parentIndex = (currentIndex + 1) / 2 - 1;
					if (comparer
						.Is(heap[parentIndex])
						.GreaterThanOrEqual(heap[currentIndex]))
					{
						// heap property satisfied
						break;
					}

					ObjectUtility.Swap(ref heap[parentIndex], ref heap[currentIndex]);
					currentIndex = parentIndex;
				}
			}

			public static void BubbleDownMax(T[] heap, int count, int currentIndex, IComparer<T> comparer)
			{
				while (currentIndex < count - 1)
				{
					int leftChild = currentIndex * 2 + 1;
					int rightChild = leftChild + 1;

					int maxChild = leftChild;
					if (rightChild < count && comparer.Is(heap[rightChild]).GreaterThan(heap[leftChild]))
						maxChild = rightChild;

					if (comparer
						.Is(heap[currentIndex])
						.GreaterThanOrEqual(heap[maxChild]))
					{
						// heap property satisfied
						break;
					}

					ObjectUtility.Swap(ref heap[maxChild], ref heap[currentIndex]);
					currentIndex = maxChild;
				}
			}
		}

		const int c_defaultInitialCapacity = 8;

		readonly IComparer<T> m_comparer;

		T[] m_heap;
		int m_itemCount;
	}
}
