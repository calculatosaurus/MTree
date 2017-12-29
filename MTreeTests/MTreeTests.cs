using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTree;
using MTree.Models;

namespace MTreeTests
{
	[TestClass]
	public class MTreeTests
	{
		[TestMethod]
		public void InitTest()
		{
			int maxNodes = 25;
			MTree<int> tree = new MTree<int>((x, y) => Math.Abs(x + y), maxNodes);

			Assert.AreEqual(0, tree.Count, "Empty MTree is giving non-zero count when it is empty.");
			Assert.AreEqual(0, tree.NodeCount, "Emptry MTree is giving non-zero NodeCount when it is empty.");
			Assert.AreEqual(maxNodes, tree.MaxNodesSize, "New MTree is giving the incorrect value for MaxNodeSize.");
		}

		[TestMethod]
		public void ContainsTest()
		{
			int maxNodeSize = 25;
			int numItems = 10000;
			MTree<int> tree = new MTree<int>((x, y) => Math.Abs(x - y), maxNodeSize);

			for (int i = 0; i < numItems; i++)
			{
				tree.Add(i);
				Assert.AreEqual(i + 1, tree.Count, "Count is not returning the correct value.");
			}

			for (int i = 0; i < numItems; i++)
			{
				Assert.IsTrue(tree.Contains(i), "Contains is returning false for a value that has been inserted in the MTree.");
			}
		}

		[TestMethod]
		public void RangeSearchTest()
		{
			int maxItems = 10000;
			int maxNodes = 25;
			double distThreshold = 1000;
			Random rand = new Random();
			CartesianPoint testPoint = new CartesianPoint(-1, 0, 0);

			MTree<CartesianPoint> tree = new MTree<CartesianPoint>(CartesianPoint.GetDistance, maxNodes);
			List<CartesianPoint> itemsInThreshold = new List<CartesianPoint>();

			CartesianPoint newPoint;

			for (int i = 0; i < maxItems; i++)
			{
				int xMultiplier = rand.Next(-2, 1);
				while (xMultiplier == 0) xMultiplier = rand.Next(-2, 1);

				int yMultiplier = rand.Next(-2, 1);
				while (yMultiplier == 0) yMultiplier = rand.Next(-2, 1);

				double x = maxItems * xMultiplier * rand.NextDouble() * 0.05;
				double y = maxItems * yMultiplier * rand.NextDouble() * 0.05;

				newPoint = new CartesianPoint(i, x, y);

				tree.Add(newPoint);

				double dist = tree.GetDistance(newPoint, testPoint);

				if (dist <= distThreshold)
					itemsInThreshold.Add(newPoint);
			}

			List<CartesianPoint> neighbors = tree.RangeSearch(testPoint, distThreshold);

			Assert.AreEqual(itemsInThreshold.Count, neighbors.Count, "RangeSearch did not return the correct number of elements.");

			foreach (var neighbor in neighbors)
			{
				double dist = tree.GetDistance(neighbor, testPoint);

				Assert.IsTrue(dist <= distThreshold, "RangeSearch returned an item outside of the search radius.");
				Assert.IsTrue(itemsInThreshold.Contains(neighbor), "RangeSearch returned an invalid item");
			}

			foreach (var closeItem in itemsInThreshold)
			{
				Assert.IsTrue(neighbors.Contains(closeItem), "RangeSearch did not return an item within the search radius.");
			}
		}

		[TestMethod]
		public void KNearestNeighborsTest()
		{
			int maxItems = 10000;
			int k = 100;
			Random rand = new Random();
			CartesianPoint testPoint = new CartesianPoint(-1, 0, 0);

			MTree<CartesianPoint> tree = new MTree<CartesianPoint>(CartesianPoint.GetDistance);
			PriorityQueue<CartesianPoint> kNeighbors = new PriorityQueue<CartesianPoint>(k);

			CartesianPoint newPoint;

			for (int i = 0; i < maxItems; i++)
			{
				int xMultiplier = rand.Next(-2, 1);
				while (xMultiplier == 0) xMultiplier = rand.Next(-2, 1);

				int yMultiplier = rand.Next(-2, 1);
				while (yMultiplier == 0) yMultiplier = rand.Next(-2, 1);

				double x = maxItems * xMultiplier * rand.NextDouble() * 0.05;
				double y = maxItems * yMultiplier * rand.NextDouble() * 0.05;

				newPoint = new CartesianPoint(i, x, y);

				tree.Add(newPoint);

				double dist = tree.GetDistance(newPoint, testPoint);

				kNeighbors.Enqueue(newPoint, dist);
			}

			PriorityQueue<CartesianPoint> queryReturns = tree.KNearestNeighborSearch(testPoint, k);

			Assert.AreEqual(k, queryReturns.Count, "K neighor search did not return the correct number of items.");

			while (kNeighbors.HasNext)
			{
				var neighbor = kNeighbors.Dequeue();
				var result = queryReturns.Dequeue();

				Assert.AreEqual(neighbor.Value, result.Value, "K neighbor search returned wrong item.");
				Assert.AreEqual(neighbor.Key, result.Key, "K neighbor search returned correct item with wrong distance.");
			}

			Assert.IsFalse(queryReturns.HasNext, "K neighbor search returned too many items.");
		}

		[TestMethod]
		public void ItemInsertVerification()
		{
			int maxItems = 1000;
			int maxNodes = 25;
			Random rand = new Random();

			MTree<CartesianPoint> tree = new MTree<CartesianPoint>(CartesianPoint.GetDistance, maxNodes);

			CartesianPoint newPoint;

			for (int i = 1; i <= maxItems; i++)
			{
				int xMultiplier = rand.Next(-2, 1);
				while (xMultiplier == 0) xMultiplier = rand.Next(-2, 1);

				int yMultiplier = rand.Next(-2, 1);
				while (yMultiplier == 0) yMultiplier = rand.Next(-2, 1);

				double x = maxItems * xMultiplier * rand.NextDouble() * 0.05;
				double y = maxItems * yMultiplier * rand.NextDouble() * 0.05;

				newPoint = new CartesianPoint(i, x, y);

				tree.Add(newPoint);

				Assert.AreEqual(i, tree.Count, "MTree does not give correct Count after adding a new element.");
				Assert.IsTrue(tree.Contains(newPoint), "MTree does not say it Contains new item that has been added.");

				NodeDescentAssert(tree.Root, 1, maxNodes);
			}
		}

		private void NodeDescentAssert(Node<CartesianPoint> nodeToCheck, int level, int maxNodeSize)
		{
			string errorMsg = "\nLevel: " + level + "\nNodeID: " + nodeToCheck.NodeID + "\nNodeItemID: " + nodeToCheck.Item.ID + "\nCoveringRadius: " + nodeToCheck.CoveringRadius + "\n";

			if (nodeToCheck.IsRoot)
			{
				Assert.IsNull(nodeToCheck.Parent, "Root node has a nun-null parent." + errorMsg);
				Assert.AreEqual(0, nodeToCheck.DistanceToParent, "Root node and non-zero distance to parent." + errorMsg);
			}

			if (nodeToCheck.Children.Count > 1)
				Assert.IsTrue(nodeToCheck.CoveringRadius > 0, "Node with more thane one child has a covering radius of 0." + errorMsg);
			else
				Assert.IsTrue(nodeToCheck.CoveringRadius >= 0, "Child has negative covering radius." + errorMsg);

			Assert.IsTrue(nodeToCheck.DistanceToParent >= 0, "Child has negative distance to parent." + errorMsg);
			Assert.IsTrue(nodeToCheck.Children.Count <= maxNodeSize, "Node has more children that MaxNodeSize." + errorMsg);

			double childDist;

			foreach (MTreeObject<CartesianPoint> child in nodeToCheck.Children)
			{
				childDist = nodeToCheck.Tree.GetDistance(nodeToCheck.Item, child.Item);
				string errorMsg2 = errorMsg + "ChildItemID: " + child.Item.ID + "\nDistanceToParent: " + child.DistanceToParent + "\nReal Distance: " + childDist + "\nCoveringRadius: " + child.CoveringRadius + "\n";

				Assert.AreEqual(childDist, child.DistanceToParent, "Child of node does not have correct distance to parent." + errorMsg2);
				Assert.IsTrue(childDist <= nodeToCheck.CoveringRadius, "Actual distance to parent is greater than node covering radius." + errorMsg2);
				Assert.IsTrue(child.DistanceToParent <= nodeToCheck.CoveringRadius, "Distance to parent is greater than node covering radius." + errorMsg2);

				if (child.GetType() == typeof(Node<CartesianPoint>))
					Assert.IsFalse(nodeToCheck.IsLeaf, "Node has a child that is of type Node<T> but is marked a leaf." + errorMsg2);
				else
					Assert.IsTrue(nodeToCheck.IsLeaf, "Node has a child that is not of type Node<T> but is not marked a leaf." + errorMsg2);

				if (!nodeToCheck.IsLeaf)
				{
					Node<CartesianPoint> nextNode = child as Node<CartesianPoint>;

					Assert.AreEqual(nodeToCheck.Item.ID, nextNode.Parent.Item.ID, "Child of node does not have this node as its parent.");

					NodeDescentAssert(nextNode, level + 1, maxNodeSize);
				}

			}
		}
	}
}
