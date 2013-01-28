using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
	public abstract class PriorityQueue<T>
	{
		public static PriorityQueue<T> CreateMinPriorityQueue(IComparer<T> comparer = null, int? initialCapacity = null)
		{
			return new MinPriorityQueue(comparer, initialCapacity);
		}

		public static PriorityQueue<T> CreateMaxPriorityQueue(IComparer<T> comparer = null, int? initialCapacity = null)
		{
			return new MaxPriorityQueue(comparer, initialCapacity);
		}

		public int Count
		{
			get { return m_itemCount; }
		}

		public void Enqueue(T item)
		{
			if (m_itemCount == m_heap.Length)
				Resize();

			// add to end of heap and bubble up
			m_heap[m_itemCount] = item;
			BubbleUp(m_itemCount++);
		}

		public T Dequeue()
		{
			if (m_itemCount == 0)
				throw new InvalidOperationException();

			// save head item (replace with default)
			T result = default(T);
			ObjectUtility.Swap(ref m_heap[0], ref result);

			// move last item to head and bubble down
			ObjectUtility.Swap(ref m_heap[0], ref m_heap[--m_itemCount]);
			BubbleDown(0);
			
			return result;
		}
		
		public T Peek()
		{
			if (m_itemCount == 0)
				throw new InvalidOperationException();

			return m_heap[0];
		}

		protected PriorityQueue(IComparer<T> comparer, int? initialCapacity)
		{
			m_comparer = comparer ?? Comparer<T>.Default;
			m_heap = new T[initialCapacity ?? c_defaultInitialCapacity];
		}

		protected abstract bool IsHeapPropertySatisfied(IComparer<T> comparer, T parent, T child);
		
		private void Resize()
		{
			int currentPower = (int) Math.Ceiling(Math.Log(m_heap.Length, 2));
			currentPower++;
			int newSize = (int) Math.Pow(2, currentPower);
			T[] newHeap = new T[newSize];
			m_heap.CopyTo(newHeap, 0);
			m_heap = newHeap;
		}

		private void BubbleUp(int currentIndex)
		{
			// swap the current item with its parent, until the heap property is satisfied
			while (currentIndex > 0)
			{
				int parentIndex = (currentIndex + 1) / 2 - 1;
				if (IsHeapPropertySatisfied(m_comparer, m_heap[parentIndex], m_heap[currentIndex]))
					break;

				ObjectUtility.Swap(ref m_heap[currentIndex], ref m_heap[parentIndex]);
				currentIndex = parentIndex;
			}
		}

		private void BubbleDown(int currentIndex)
		{
			// swap the current item with its best child, until the heap property is satisfied
			int leftChild = currentIndex * 2 + 1;
			while (leftChild < m_itemCount)
			{
				// consider swapping the current item with a child; must chose a child which will satisfy the heap property over the other
				int swappableChild = leftChild;
				int rightChild = leftChild + 1;
				if (rightChild < m_itemCount && IsHeapPropertySatisfied(m_comparer, m_heap[rightChild], m_heap[leftChild]))
					swappableChild = rightChild;

				if (IsHeapPropertySatisfied(m_comparer, m_heap[currentIndex], m_heap[swappableChild]))
					break;

				ObjectUtility.Swap(ref m_heap[currentIndex], ref m_heap[swappableChild]);
				currentIndex = swappableChild;
				leftChild = currentIndex * 2 + 1;
			}
		}

		class MinPriorityQueue : PriorityQueue<T>
		{
			public MinPriorityQueue(IComparer<T> comparer, int? initialCapacity)
				: base(comparer, initialCapacity) { }

			protected override bool IsHeapPropertySatisfied(IComparer<T> comparer, T parent, T child)
			{
				return comparer
					.Is(parent)
					.LessThanOrEqualTo(child);
			}
		}

		class MaxPriorityQueue : PriorityQueue<T>
		{
			public MaxPriorityQueue(IComparer<T> comparer, int? initialCapacity)
				: base(comparer, initialCapacity) { }

			protected override bool IsHeapPropertySatisfied(IComparer<T> comparer, T parent, T child)
			{
				return comparer
					.Is(parent)
					.GreaterThanOrEqualTo(child);
			}
		}

		const int c_defaultInitialCapacity = 8;

		readonly IComparer<T> m_comparer;

		T[] m_heap;
		int m_itemCount;
	}
}
