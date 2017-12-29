using System;
using System.Collections.Generic;

namespace MTree
{
	public class PriorityQueue<T>
	{
		#region Properties
		private Node<T> _start;
		private Node<T> _end;
		private int _itemCount;
		private int _capacity;
		#endregion

		#region Constructors
		public PriorityQueue() : this(int.MaxValue) { }

		public PriorityQueue(int maxCapacity)
		{
			_start = new Node<T>();
			_end = new Node<T>();

			_start.SetPrevAndNextNodes(null, _end);
			_end.SetPrevAndNextNodes(_start, null);

			_itemCount = 0;
			_capacity = maxCapacity;
		}
		#endregion

		#region Normal Queue Methods
		public void Enqueue(T item, double priority)
		{
			// new item has higher priority than last element in queue.
			if (_itemCount > 1 && Last().Key <= priority)
			{
				if (this.IsFull) return;
				else
				{
					Node<T> newItem = new Node<T>(item, priority);

					newItem.SetPrevAndNextNodes(_end.Prev, _end);
					_end.Prev.Next = newItem;
					_end.Prev = newItem;

					_itemCount++;
				}
			}
			else
			{
				Node<T> newItem = new Node<T>(item, priority);
				Node<T> curr = _start;
				int counter = 0;

				while (curr.Next != _end && priority > curr.Next.Priority && counter < this._capacity)
				{
					curr = curr.Next;
					counter++;
				}

				newItem.SetPrevAndNextNodes(curr, curr.Next);
				curr.Next.Prev = newItem;
				curr.Next = newItem;

				// if already full, then drop last element from queue.
				if (this.IsFull)
				{
					curr = _end.Prev;
					curr.Prev.Next = _end;
					_end.Prev = curr.Prev;
				}
				else
				{
					_itemCount++;
				}
			}
		}

		public KeyValuePair<double, T> Dequeue()
		{
			if (_start.Next == _end)
				throw new InvalidOperationException("PriorityQueue is empty.");

			Node<T> toReturn = _start.Next;

			toReturn.Next.Prev = _start;
			_start.Next = toReturn.Next;

			_itemCount--;

			return toReturn.KVPair;
		}

		public bool Remove(T item)
		{
			Node<T> curr = _start;

			while (curr.Next != _end)
			{
				curr = curr.Next;

				if (curr.Item.Equals((item)))
				{
					curr.Prev.Next = curr.Next;
					curr.Next.Prev = curr.Prev;
					_itemCount--;
					return true;
				}
			}

			return false;
		}

		public bool Contains(T item)
		{
			Node<T> curr = _start;

			while (curr.Next != _end)
			{
				curr = curr.Next;
				if (curr.Item.Equals(item)) return true;
			}

			return false;
		}

		public List<T> ToList()
		{
			List<T> retList = new List<T>(_itemCount);
			Node<T> curr = _start;

			while (curr.Next != _end)
			{
				curr = curr.Next;
				retList.Add(curr.Item);
			}

			return retList;
		}

		public bool HasNext
		{
			get { return _start.Next != _end; }
		}

		public int Count
		{
			get { return _itemCount; }
		}

		public bool IsFull
		{
			get { return _capacity == _itemCount; }
		}

		public bool IsEmpty
		{
			get { return _start.Next == _end; }
		}

		public int Capacity
		{
			get { return _capacity; }
		}

		public KeyValuePair<double, T> First()
		{
			if (IsEmpty)
				throw new InvalidOperationException("PriorityQueue is empty.");

			return _start.Next.KVPair;
		}

		public KeyValuePair<double, T> Last()
		{
			if (IsEmpty)
				throw new InvalidOperationException("PriorityQueue is empty.");

			return _end.Prev.KVPair;
		}


		#endregion

		#region MTree Specific Methods
		public void RemoveItemsAbovePriority(double maxPriority)
		{
			Node<T> curr = _end;
			int itemCounter = _itemCount;

			while (curr.Prev != _start && maxPriority < curr.Prev.Priority)
			{
				curr = curr.Prev;
				itemCounter--;
			}

			curr.Prev.Next = _end;
			_end.Prev = curr.Prev;

			_itemCount = itemCounter;
		}
		#endregion









		#region Inner Classes
		private class Node<U>
		{
			public Node<U> Next { set; get; }
			public Node<U> Prev { set; get; }
			private KeyValuePair<double, U> item;

			public Node()
			{
				Next = null;
				Prev = null;
				item = new KeyValuePair<double, U>(-1, default(U));
			}

			public Node(U item, double priority)
			{
				Next = null;
				Prev = null;
				this.item = new KeyValuePair<double, U>(priority, item);
			}

			public void SetPrevAndNextNodes(Node<U> prev, Node<U> next)
			{
				Prev = prev;
				Next = next;
			}

			public KeyValuePair<double, U> KVPair
			{
				get { return item; }
			}

			public U Item
			{
				get { return item.Value; }
			}

			public double Priority
			{
				get { return item.Key; }
			}
		}
		#endregion
	}
}
