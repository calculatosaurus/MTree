using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTree;

namespace MTreeTests
{
	[TestClass]
	public class PerformanceTests
	{
		public int SearchRadius = 250;
		int KNeighbors = 50;
		CartesianPoint TestOrigin = new CartesianPoint(0, 0);
		GeographicPoint TestLocation = new GeographicPoint(41.0, -88.1);
		int numTestPoints = 5_000;
		int[] testNodeSizes = { 5, 10, 25, 50, 100, 150 };

		[TestMethod]
		public void CartesianPoint_OptimalNodeSize()
		{
			Stopwatch watch = new Stopwatch();

			List<CartesianPoint> testPoints = GenerateTestCartesianPoints();

			foreach (int nodeSize in testNodeSizes)
			{
				Console.WriteLine("Maximum node size: " + nodeSize);
				Console.WriteLine("----------------------------");

				MTree<CartesianPoint> tree = new MTree<CartesianPoint>(CartesianPoint.PythagoreanTheorem, nodeSize);

				watch.Reset();
				watch.Start();

				PopulateTree(tree, testPoints);

				watch.Stop();

				Console.WriteLine("Build:\t\t" + FormatTime(watch.Elapsed.Ticks));

				watch.Reset();
				watch.Start();

				PerformRangeSearch(tree, testPoints);

				watch.Stop();

				Console.WriteLine("Range:\t\t" + FormatTime(watch.Elapsed.Ticks));

				watch.Reset();
				watch.Start();

				PerformKNeighborSearch(tree, testPoints);

				watch.Stop();

				Console.WriteLine("K Neighbor:\t" + FormatTime(watch.Elapsed.Ticks));
				Console.WriteLine("\n\n");
			}
		}

		[TestMethod]
		public void GeographicPoint_OptimalNodeSize()
		{
			Stopwatch watch = new Stopwatch();

			List<GeographicPoint> testPoints = GenerateTestGeographicPoints();

			foreach (int nodeSize in testNodeSizes)
			{
				Console.WriteLine("Maximum node size: " + nodeSize);
				Console.WriteLine("----------------------------");

				MTree<GeographicPoint> tree = new MTree<GeographicPoint>(GeographicPoint.HaversineFormula, nodeSize);

				watch.Reset();
				watch.Start();

				PopulateTree(tree, testPoints);

				watch.Stop();

				Console.WriteLine("Build:\t\t" + FormatTime(watch.Elapsed.Ticks));

				watch.Reset();
				watch.Start();

				PerformRangeSearch(tree, testPoints);

				watch.Stop();

				Console.WriteLine("Range:\t\t" + FormatTime(watch.Elapsed.Ticks));

				watch.Reset();
				watch.Start();

				PerformKNeighborSearch(tree, testPoints);

				watch.Stop();

				Console.WriteLine("K Neighbor:\t" + FormatTime(watch.Elapsed.Ticks));
				Console.WriteLine("\n\n");
			}
		}


		private List<CartesianPoint> GenerateTestCartesianPoints()
		{
			List<CartesianPoint> testPoints = new List<CartesianPoint>(numTestPoints);
			Random rand = new Random();
			
			for (int i = 0; i < numTestPoints; i++)
			{
				int xMultiplier = rand.Next(-1, 2);
				while (xMultiplier == 0) xMultiplier = rand.Next(-1, 2);

				int yMultiplier = rand.Next(-1, 2);
				while (yMultiplier == 0) yMultiplier = rand.Next(-1, 2);

				double xMod = rand.NextDouble() * SearchRadius * 2;
				double yMod = rand.NextDouble() * SearchRadius * 2;
				
				double x = TestOrigin.X + (xMod * yMultiplier);
				double y = TestOrigin.Y + (yMod * yMultiplier);

				testPoints.Add(new CartesianPoint(x, y));
			}

			return testPoints;
		}

		private List<GeographicPoint> GenerateTestGeographicPoints()
		{
			List<GeographicPoint> testPoints = new List<GeographicPoint>(numTestPoints);
			Random rand = new Random();

			for (int i = 0; i < numTestPoints; i++)
			{
				int latMultiplier = rand.Next(-1, 2);
				while (latMultiplier == 0) latMultiplier = rand.Next(-1, 2);

				int lonMultiplier = rand.Next(-1, 2);
				while (lonMultiplier == 0) lonMultiplier = rand.Next(-1, 2);

				double latMod = rand.NextDouble();
				double lonMod = rand.NextDouble();

				double x = TestOrigin.X + (latMod * lonMultiplier);
				double y = TestOrigin.Y + (lonMod * lonMultiplier);

				testPoints.Add(new GeographicPoint(x, y));
			}

			return testPoints;
		}

		private void PopulateTree<T>(MTree<T> tree, List<T> points)
		{
			foreach(T point in points)
			{
				tree.Add(point);
			}
		}

		private void PerformRangeSearch<T>(MTree<T> tree, List<T> points)
		{
			foreach(T point in points)
			{
				var returns = tree.RangeSearch(point, SearchRadius);
			}
		}

		private void PerformKNeighborSearch<T>(MTree<T> tree, List<T> points)
		{
			foreach(T point in points)
			{
				var returns = tree.KNearestNeighborSearch(point, KNeighbors);
			}
		}

		private string FormatTime(long ticks)
		{
			return string.Format("{0:n0}", ticks);
		}
	}
}
