using System;
using System.Collections.Generic;

namespace MTree.Models
{
	internal sealed class Node<T> : MTreeElement<T>, IEquatable<Node<T>>
	{
		#region Properties
		internal MTree<T> Tree { get; }
		internal bool IsRoot { get; set; }
		internal bool IsLeaf { get; }
		internal List<MTreeElement<T>> Children { get; }
		internal int NodeID { get; }

		internal Node<T> Parent { get; set; }

		private void SetNewParent(Node<T> value)
		{
			double distToNewParent = Tree.GetDistance(value.Item, this.Item);
			value.ExpandCoveringRadius(distToNewParent);
			DistanceToParent = distToNewParent;
			Parent = value;
		}

		internal int NodeCount
		{
			get
			{
				if (IsLeaf) return 1;

				int numNodes = 1;

				for (int i = 0; i < Children.Count; i++)
					numNodes += (Children[i] as Node<T>).NodeCount;

				return numNodes;
			}
		}
		#endregion




		#region Constuctor
		public Node(MTree<T> tree, Node<T> parent, T item, double distToParent, bool isRoot, bool isLeaf)
			: base(item, distToParent, 0)
		{
			Tree = tree;
			Parent = parent;

			IsRoot = isRoot;
			IsLeaf = isLeaf;

			NodeID = tree.NewNodeID();

			Children = new List<MTreeElement<T>>(tree.MaxNodesSize + 1);
		}
		#endregion




		#region Misc Methods
		public bool Equals(Node<T> node)
		{
			if (node == null) return false;
			if (ReferenceEquals(this, node)) return true;

			return NodeID.Equals(node.NodeID);
		}

        internal override bool Contains(T item)
		{
			for (int i = 0; i < Children.Count; i++)
				if (Children[i].Contains(item))
					return true;
			
			return false;
		}
		#endregion




		#region Add Item
		internal void AddItemToTree(T newItem, double distItemToParent)
		{
			if (IsLeaf)
			{
				AddChildToNode(new Entry<T>(newItem, distItemToParent), distItemToParent);
			}
			else
			{
				/* Algorithm in original paper had two iterations through the Node's
				 * children. Current implementation reduces it to a single
				 * iteration, halving the number of distance calculations.
                 */

				Node<T> nextNodeToDescend = null;
				double distToNextNode = double.MaxValue;
				double distToCoveringRadOfNextNode = double.MaxValue;
				bool newItemInNextNodeRadius = false;

				for (int i = 0; i < Children.Count; i++)
				{
					double distItemToThisChild = Tree.GetDistance(Children[i].Item, newItem);

					if (distItemToThisChild <= Children[i].CoveringRadius)
					{
						if (newItemInNextNodeRadius)
						{
							if (distItemToThisChild < distToNextNode)
							{
								nextNodeToDescend = Children[i] as Node<T>;
								distToNextNode = distItemToThisChild;
							}
						}
						else
						{
							newItemInNextNodeRadius = true;
							nextNodeToDescend = Children[i] as Node<T>;
							distToNextNode = distItemToThisChild;
						}
					}
					else if (!newItemInNextNodeRadius)
					{
						if (distItemToThisChild - Children[i].CoveringRadius < distToCoveringRadOfNextNode)
						{
							nextNodeToDescend = Children[i] as Node<T>;
							distToNextNode = distItemToThisChild;
							distToCoveringRadOfNextNode = distItemToThisChild - Children[i].CoveringRadius;
						}
					}
				}

				if (!newItemInNextNodeRadius)
					nextNodeToDescend.ExpandCoveringRadius(distToNextNode);
				
				nextNodeToDescend.AddItemToTree(newItem, distToNextNode);
			}

			if (Children.Count > Tree.MaxNodesSize)
				SplitNode();

		}

		private void AddChildToNode(MTreeElement<T> newChild, double newChildDist)
		{
			newChild.DistanceToParent = newChildDist;
			Children.Add(newChild);

			double newRadius = Math.Max(CoveringRadius, newChildDist + newChild.CoveringRadius);

			ExpandCoveringRadius(newRadius);

			Node<T> newNode = newChild as Node<T>;

			if (newNode != null)
				newNode.SetNewParent(this);
			
		}
		#endregion




		#region Split Node
		private void SplitNode()
		{
			ChooseTwoNewReplacementNodes(out Node<T> newNode1, out Node<T> newNode2);

			PartitionThisNodesChildrenToNewNodes(newNode1, newNode2);

			if (IsRoot)
				CreateNewRootNode(newNode1, newNode2);


			Parent.Children.Remove(this);
			Parent.AddChildToNode(newNode1, Tree.GetDistance(Parent.Item, newNode1.Item));
			Parent.AddChildToNode(newNode2, Tree.GetDistance(Parent.Item, newNode2.Item));
		}

		private void ChooseTwoNewReplacementNodes(out Node<T> node1, out Node<T> node2)
		{
			/* Uses M_LB_Dist method described in paper. This had the best trade
			 * off between build time and search time when testing MTree with
			 * 5,000,000 items.
			 */

			MTreeElement<T> closestChild = Children[0];
			double distToClosestChild = Children[0].DistanceToParent;

			MTreeElement<T> farthestChild = closestChild;
			double distToFarthestChild = distToClosestChild;

			for (int i = 0; i < Children.Count; i++)
			{
				if (Children[i].DistanceToParent < distToClosestChild)
				{
					distToClosestChild = Children[i].DistanceToParent;
					closestChild = Children[i];
				}

				if (Children[i].DistanceToParent > distToFarthestChild)
				{
					distToFarthestChild = Children[i].DistanceToParent;
					farthestChild = Children[i];
				}
			}

			Children.Remove(closestChild);
			Children.Remove(farthestChild);

			node1 = new Node<T>(Tree, Parent, closestChild.Item, -1, false, IsLeaf);
			node1.AddChildToNode(closestChild, 0);

			node2 = new Node<T>(Tree, Parent, farthestChild.Item, -1, false, IsLeaf);
			node2.AddChildToNode(farthestChild, 0);
		}

		private void PartitionThisNodesChildrenToNewNodes(Node<T> newNode1, Node<T> newNode2)
		{
			/* Uses the General Hyperplane method described in paper.
			 */

			for (int i = 0; i < Children.Count; i++)
			{
				double distChildToNewNode1 = Tree.GetDistance(newNode1.Item, Children[i].Item);
				double distChildToNewNode2 = Tree.GetDistance(newNode2.Item, Children[i].Item);

				if (distChildToNewNode1 < distChildToNewNode2)
					newNode1.AddChildToNode(Children[i], distChildToNewNode1);
				else
					newNode2.AddChildToNode(Children[i], distChildToNewNode2);
			}
		}

		private void CreateNewRootNode(Node<T> newNode1, Node<T> newNode2)
		{
			T newRootData;

			if (newNode1.CoveringRadius > newNode2.CoveringRadius)
				newRootData = newNode1.Item;
			else
				newRootData = newNode2.Item;

			Tree.Root = new Node<T>(Tree, null, newRootData, 0, true, false);
			newNode1.SetNewParent(Tree.Root);
			newNode2.SetNewParent(Tree.Root);
			SetNewParent(Tree.Root);
		}
		#endregion




		#region Range Search
		public void RangeSearch(T item, double radius, double distItemToNode, List<T> neighbors)
		{
			if (IsLeaf)
			{
				for (int i = 0; i < Children.Count; i++)
				{
					if (Math.Abs(distItemToNode - Children[i].DistanceToParent) > radius)
						continue;

					double distItemToChild = Tree.GetDistance(item, Children[i].Item);

					if (distItemToChild <= radius)
					{
						neighbors.Add((Children[i].Item));
					}
					
				}

			}
			else
			{
				for (int i = 0; i < Children.Count; i++)
				{
					if (Math.Abs(distItemToNode - Children[i].DistanceToParent) > radius + Children[i].CoveringRadius)
						continue;

					double distQToChild = Tree.GetDistance(item, Children[i].Item);

					if (distQToChild <= radius + Children[i].CoveringRadius)
					{
						(Children[i] as Node<T>).RangeSearch(item, radius, distQToChild, neighbors);
					}
					
				}
			}
		}
		#endregion




		#region K Nearest Neighbor Search
		public void NodeSearch(T item, double distItemToNode, PriorityQueue<Node<T>> nodesToCheck, PriorityQueue<T> elementsToReturn)
		{
			double dynamicSearchRadius = elementsToReturn.Last().Key;

			if (IsLeaf)
			{
				for (int i = 0; i < Children.Count; i++)
				{
					if (Math.Abs(distItemToNode - Children[i].DistanceToParent) > dynamicSearchRadius)
						continue;

					double distQToChild = Tree.GetDistance(item, Children[i].Item);

					if (distQToChild <= dynamicSearchRadius)
					{
						elementsToReturn.Enqueue(Children[i].Item, distQToChild);
						dynamicSearchRadius = elementsToReturn.Last().Key;
						nodesToCheck.RemoveItemsAbovePriority(dynamicSearchRadius);
					}
					
				}
			}
			else
			{
				for (int i = 0; i < Children.Count; i++)
				{
					if (Math.Abs(distItemToNode - Children[i].DistanceToParent) > dynamicSearchRadius + Children[i].CoveringRadius)
						continue;

					double dMin = Math.Max(Tree.GetDistance(Children[i].Item, item) - Children[i].CoveringRadius, 0);

					if (dMin <= dynamicSearchRadius)
					{
						nodesToCheck.Enqueue(Children[i] as Node<T>, dMin);

						/* Original paper had code that inserted null elements into the queue
						 * without any explanation. This resulted in odd behavior. Through testing it
						 * was found that this wasn't necessary for KNN search to work properly.
						 */
					}
					
				}
			}
		}
		#endregion

	} // end class
}
