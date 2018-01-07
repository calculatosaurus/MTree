using System;
using System.Collections.Generic;
using MTree.Models;

namespace MTree
{
	public class MTree<T>
	{
		#region Properties
		internal Node<T> Root { set; get; }
		public Func<T, T, double> GetDistance;

		public int MaxNodesSize { get; }
		private int _nodeIDCounter;

		/// <summary>
		/// Gets the number of items the this MTree.
		/// </summary>
		/// <value>The count.</value>
		public int Count { private set; get; }

		/// <summary>
		/// Gets the number of nodes in this MTree.
		/// </summary>
		/// <value>The node count.</value>
		public int NodeCount
		{
			get
			{
				if (Root == null) return 0;

				return Root.NodeCount;
			}
		}
		#endregion


		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MTree.MTree`1"/> class with a default
		/// maximum node size of 25.
		/// </summary>
		/// <param name="DistanceFunction">The distance metric used to determine the "similarity"
		/// of two item in the MTree.</param>
		public MTree(Func<T, T, double> DistanceFunction)
			: this(DistanceFunction, 25) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MTree.MTree`1"/> class with the given
        /// maximum node size.
        /// </summary>
        /// <param name="DistanceFunction">The distance metric used to determine the "similarity"
        /// of two item in the MTree.</param>
        /// <param name="maxNodeSize">The maximum number of items allowed in each node before the
        /// node is split. </param>
        public MTree(Func<T, T, double> DistanceFunction, int maxNodeSize)
		{
			if (maxNodeSize < 3)
				throw new ArgumentException(
					"maxNodeSize must be 3 or greater in order for MTree to function properly.");

			Root = null;

			GetDistance = DistanceFunction;
			MaxNodesSize = maxNodeSize;

			Count = 0;
			_nodeIDCounter = 0;
		}
		#endregion


		#region Misc Methods
		/// <summary>
		/// Searches this MTree to see if it contains the specified item.
		/// </summary>
		/// <returns>A bool indicating if the given item was contained in the MTree.</returns>
		/// <param name="item">Item.</param>
		public bool Contains(T item)
		{
			if (Root == null) return false;

			return Root.Contains(item);
		}

		internal int NewNodeID()
		{
			_nodeIDCounter++;
			return _nodeIDCounter;
		}
		#endregion

		#region Primary Methods
		/// <summary>
		/// Add the specified item to the MTree.
		/// </summary>
		/// <returns></returns>
		/// <param name="newEntry">The item to be added to the MTree.</param>
		public void Add(T newEntry)
		{
			if (Root == null)
			{
				Root = new Node<T>(this, null, newEntry, 0, true, true);
			}

			double distEntryToRoot = GetDistance(Root.Item, newEntry);
			Root.AddItemToTree(newEntry, distEntryToRoot);

			Count++;
		}

		/// <summary>
		/// Returns all the items in the MTree within the given radius of the item provided.
		/// </summary>
		/// <returns>A List containing all the items from the MTree within the given radius.</returns>
		/// <param name="item"></param>
		/// <param name="range"></param>
		public List<T> RangeSearch(T item, double range)
		{
			if (Root == null) return null;

			List<T> nearbyItems = new List<T>();

			double distItemToRoot = GetDistance(Root.Item, item);
			Root.RangeSearch(item, range, distItemToRoot, nearbyItems);

			return nearbyItems;
		}

		/// <summary>
		/// Returns the k elements closest to the given item.
		/// </summary>
		/// <returns>A PriorityQueue containing the k elements closest to the given item, ordered
		/// from closest to farthest.</returns>
		/// <param name="item"></param>
		/// <param name="k">The number of items to return.<param>
		public PriorityQueue<T> KNearestNeighborSearch(T item, int k)
		{
			if (Root == null) return null;

			PriorityQueue<Node<T>> nodesToSearch = new PriorityQueue<Node<T>>();
			double dMin = Math.Max(0, GetDistance(Root.Item, item) - Root.CoveringRadius);
			nodesToSearch.Enqueue(Root, dMin);

			PriorityQueue<T> kNearestNeighbors = new PriorityQueue<T>(k);
			kNearestNeighbors.Enqueue(default(T), double.MaxValue);

			Node<T> nextNode;
			double distToNextNode;

			while (nodesToSearch.HasNext)
			{
				nextNode = nodesToSearch.Dequeue().Value;
				distToNextNode = GetDistance(nextNode.Item, item);
				nextNode.NodeSearch(item, distToNextNode, nodesToSearch, kNearestNeighbors);
			}

			kNearestNeighbors.RemoveItemsAbovePriority(double.MaxValue - 1);

			return kNearestNeighbors;
		}
		#endregion

	} // end class
}
