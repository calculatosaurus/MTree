using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTree;

namespace MTreeTests
{
	[TestClass]
	public class PriorityQueueTests
	{
		[TestMethod]
		public void InitTest()
		{
			PriorityQueue<DateTime> queue = new PriorityQueue<DateTime>();

			Assert.AreEqual(queue.Capacity, int.MaxValue, "PriorityQueue is not indicating the correct capacity after initialization with default constructor.");
			Assert.AreEqual(queue.Count, 0, "PriorityQueue is not indicating a count of zero elements after initializtion.");
			Assert.IsFalse(queue.HasNext, "Newly initialized PriorityQueue says it HasNext when it is empty.");

			try
			{
				var ret = queue.First();
				Assert.Fail("First is not throwing an exception when empty.");
			}
			catch (InvalidOperationException)
			{
				// Exception correctly thrown
			}

			try
			{
				var ret = queue.Last();
				Assert.Fail("Last is not throwing an exception when empty.");
			}
			catch (InvalidOperationException)
			{
				// Exception correctly thrown
			}

			PriorityQueue<DateTime> maxQueue = new PriorityQueue<DateTime>(10);

			Assert.AreEqual(maxQueue.Capacity, 10, "PriorityQueue with a set capacity does not say it has the correct capacity.");
			Assert.AreEqual(maxQueue.Count, 0, "PriorityQueue with a set capacity is not indicating a count of zero elements after initializtion");
			Assert.IsFalse(maxQueue.HasNext, "Newly initialized PriorityQueue  with a set capacity says it HasNext when it is empty.");

			try
			{
				var ret = maxQueue.First();
				Assert.Fail("First is not throwing an exception when empty.");
			}
			catch (InvalidOperationException)
			{
				// Exception correctly thrown
			}

			try
			{
				var ret = maxQueue.Last();
				Assert.Fail("Last is not throwing an exception when empty.");
			}
			catch (InvalidOperationException)
			{
				// Exception correctly thrown
			}
		}

		[TestMethod]
		public void FunctionalityTest_NoCapacity()
		{
			int itemCount = 10000;
			PriorityQueue<int> queue = new PriorityQueue<int>();

			// Insert elements in order
			for (int i = 0; i < itemCount; i++)
			{
				queue.Enqueue(i, i);
			}

			Assert.AreEqual(queue.Count, itemCount, "PriorityQueue is not giving the correct Count after adding " + itemCount + " items.");

			for (int i = itemCount - 1; i >= 0; i--)
			{
				Assert.IsTrue(queue.Contains(i), "PriorityQueue does not say it contains an item it has been given. i = " + i);
			}

			for (int i = 0; i < itemCount; i++)
			{
				Assert.AreEqual(queue.First().Key, i, "Priority of First item in queue is not correct.");
				Assert.AreEqual(queue.First().Key, i, "Value of First item in queue is not correct.");

				Assert.AreEqual(queue.Last().Key, itemCount - 1, "Priority of Last item in queue is not correct.");
				Assert.AreEqual(queue.Last().Value, itemCount - 1, "Value of Last item in queue is not correct.");

				Assert.IsTrue(queue.HasNext, "HasNext is returning false when there are still items in the queue.");

				KeyValuePair<double, int> item = queue.Dequeue();

				Assert.AreEqual(item.Key, i, "The priority of the item returned is not correct when calling Dequeue");
				Assert.AreEqual(item.Value, i, "The value of the item returned is not correct when calling Dequeue.");
				Assert.AreEqual(queue.Count, itemCount - (i + 1), "Count of elements after returning item number " + (i + 1) + " is not correct.");
			}

			Assert.IsFalse(queue.HasNext, "HasNext is returning true when queue should be empty.");
			Assert.AreEqual(queue.Count, 0, "Count is returning a non-zero value when queue should be empty.");

			try
			{
				queue.Dequeue();
				Assert.Fail("Dequeue did not thow an exception when the queue was empty.");
			}
			catch (InvalidOperationException e)
			{
				e.Message.GetType(); // to stop the annoying errir that e is declared and never used
			}

			// Insert elements out of order
			for (int i = itemCount - 1; i >= 0; i--)
			{
				queue.Enqueue(i + itemCount, i + itemCount);
				queue.Enqueue(i, i);
			}

			Assert.AreEqual(queue.Count, 2 * itemCount, "Count is not returning the correct value.");

			for (int i = 0; i < 2 * itemCount; i++)
			{
				Assert.AreEqual(queue.First().Key, i, "Priority of First item in queue is not correct.");
				Assert.AreEqual(queue.First().Key, i, "Value of First item in queue is not correct.");

				Assert.AreEqual(queue.Last().Key, 2 * itemCount - 1, "Priority of Last item in queue is not correct.");
				Assert.AreEqual(queue.Last().Value, 2 * itemCount - 1, "Value of Last item in queue is not correct.");

				Assert.IsTrue(queue.HasNext, "HasNext is returning false when there are still items in the queue.");

				KeyValuePair<double, int> item = queue.Dequeue();

				Assert.AreEqual(item.Key, i, "The priority of the item returned is not correct when calling Dequeue");
				Assert.AreEqual(item.Value, i, "The value of the item returned is not correct when calling Dequeue.");
				Assert.AreEqual(queue.Count, (2 * itemCount) - (i + 1), "Count of elements after returning item number " + (i + 1) + " is not correct.");
			}

			Assert.IsFalse(queue.HasNext, "HasNext is returning true when queue should be empty.");
			Assert.AreEqual(queue.Count, 0, "Count is returning a non-zero value when queue should be empty.");

			try
			{
				queue.Dequeue();
				Assert.Fail("Dequeue did not thow an exception when the queue was empty.");
			}
			catch (InvalidOperationException e)
			{
				e.Message.GetType(); // to stop the annoying errir that e is declared and never used
			}
			catch (Exception e)
			{
				Assert.Fail("Dequeue threw the wrong type of error. Message: " + e.Message);
			}
		}

		[TestMethod]
		public void FunctionalityTest_WithCapacity()
		{
			int itemCount = 10000;
			int maxItems = 500;
			PriorityQueue<int> queue = new PriorityQueue<int>(maxItems);

			// Insert elements in order
			for (int i = 0; i < itemCount; i++)
			{
				queue.Enqueue(i, i);
			}

			Assert.AreEqual(queue.Count, maxItems, "PriorityQueue is not giving the correct Count after adding " + maxItems + " items.");

			for (int i = itemCount - 1; i >= 0; i--)
			{
				if (i >= maxItems) Assert.IsFalse(queue.Contains(i), "PriorityQueue does not say it contains an item it has been given. i = " + i);
				else Assert.IsTrue(queue.Contains(i), "PriorityQueue does not say it contains an item it has been given. i = " + i);
			}

			for (int i = 0; i < maxItems; i++)
			{
				Assert.AreEqual(queue.First().Key, i, "Priority of First item in queue is not correct.");
				Assert.AreEqual(queue.First().Key, i, "Value of First item in queue is not correct.");

				Assert.AreEqual(queue.Last().Key, maxItems - 1, "Priority of Last item in queue is not correct.");
				Assert.AreEqual(queue.Last().Value, maxItems - 1, "Value of Last item in queue is not correct.");

				Assert.IsTrue(queue.HasNext, "HasNext is returning false when there are still items in the queue.");

				KeyValuePair<double, int> item = queue.Dequeue();

				Assert.AreEqual(item.Key, i, "The priority of the item returned is not correct when calling Dequeue");
				Assert.AreEqual(item.Value, i, "The value of the item returned is not correct when calling Dequeue.");
				Assert.AreEqual(queue.Count, maxItems - (i + 1), "Count of elements after returning item number " + (i + 1) + " is not correct.");
			}

			Assert.IsFalse(queue.HasNext, "HasNext is returning true when queue should be empty.");
			Assert.AreEqual(queue.Count, 0, "Count is returning a non-zero value when queue should be empty.");

			try
			{
				queue.Dequeue();
				Assert.Fail("Dequeue did not thow an exception when the queue was empty.");
			}
			catch (InvalidOperationException e)
			{
				e.Message.GetType(); // to stop the annoying errir that e is declared and never used
			}
			catch (Exception e)
			{
				Assert.Fail("Dequeue threw the wrong type of error. Message: " + e.Message);
			}

			// Insert elements out of order
			for (int i = itemCount - 1; i >= 0; i--)
			{
				queue.Enqueue(i + itemCount, i + itemCount);
				queue.Enqueue(i, i);
			}

			Assert.AreEqual(queue.Count, maxItems, "Count is not returning the correct value.");

			for (int i = 0; i < maxItems; i++)
			{
				Assert.AreEqual(queue.First().Key, i, "Priority of First item in queue is not correct.");
				Assert.AreEqual(queue.First().Key, i, "Value of First item in queue is not correct.");

				Assert.AreEqual(queue.Last().Key, maxItems - 1, "Priority of Last item in queue is not correct.");
				Assert.AreEqual(queue.Last().Value, maxItems - 1, "Value of Last item in queue is not correct.");

				Assert.IsTrue(queue.HasNext, "HasNext is returning false when there are still items in the queue.");

				KeyValuePair<double, int> item = queue.Dequeue();

				Assert.AreEqual(item.Key, i, "The priority of the item returned is not correct when calling Dequeue");
				Assert.AreEqual(item.Value, i, "The value of the item returned is not correct when calling Dequeue.");
				Assert.AreEqual(queue.Count, maxItems - (i + 1), "Count of elements after returning item number " + (i + 1) + " is not correct.");
			}

			Assert.IsFalse(queue.HasNext, "HasNext is returning true when queue should be empty.");
			Assert.AreEqual(queue.Count, 0, "Count is returning a non-zero value when queue should be empty.");

			try
			{
				queue.Dequeue();
				Assert.Fail("Dequeue did not thow an exception when the queue was empty.");
			}
			catch (InvalidOperationException e)
			{
				e.Message.GetType(); // to stop the annoying errir that e is declared and never used
			}
			catch (Exception e)
			{
				Assert.Fail("Dequeue threw the wrong type of error. Message: " + e.Message);
			}
		}

		[TestMethod]
		public void RemovalTest()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();
			int numItems = 10000;
			int itemToRemove = 999;
			int maxPriority = 500;

			for (int i = 0; i < numItems; i++)
			{
				queue.Enqueue(i, i);
			}

			queue.Remove(itemToRemove);

			Assert.IsFalse(queue.Contains(itemToRemove), "Removed item is still in queue.");
			Assert.AreEqual(numItems - 1, queue.Count, "Count does not give the correct value after removing item.");
			Assert.AreEqual(0, queue.First().Key, "First is giving the wrong value after removing item.");
			Assert.AreEqual(numItems - 1, queue.Last().Key, "Last is giving the wrong value after removing item.");

			queue.RemoveItemsAbovePriority(maxPriority);

			for (int i = 0; i <= maxPriority; i++)
			{
				Assert.AreEqual(queue.First().Key, i, "Priority of First item in queue is not correct.");
				Assert.AreEqual(queue.First().Key, i, "Value of First item in queue is not correct.");

				Assert.AreEqual(queue.Last().Key, maxPriority, "Priority of Last item in queue is not correct.");
				Assert.AreEqual(queue.Last().Value, maxPriority, "Value of Last item in queue is not correct.");

				Assert.IsTrue(queue.HasNext, "HasNext is returning false when there are still items in the queue.");

				KeyValuePair<double, int> item = queue.Dequeue();

				Assert.AreEqual(item.Key, i, "The priority of the item returned is not correct when calling Dequeue");
				Assert.AreEqual(item.Value, i, "The value of the item returned is not correct when calling Dequeue.");
				Assert.AreEqual(queue.Count, maxPriority - i, "Count of elements after returning item number " + (i + 1) + " is not correct.");
			}

			Assert.IsFalse(queue.HasNext, "HasNext is returning true when queue should be empty.");
			Assert.AreEqual(queue.Count, 0, "Count is returning a non-zero value when queue should be empty.");
		}
	}
}
