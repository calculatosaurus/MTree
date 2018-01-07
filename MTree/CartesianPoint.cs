using System;

namespace MTree
{
	public class CartesianPoint
	{
		public double X { get; private set; }
		public double Y { get; private set; }

		public CartesianPoint( double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		public static Func<CartesianPoint, CartesianPoint, double> PythagoreanTheorem
			= new Func<CartesianPoint, CartesianPoint, double>((a, b) =>
		{
			double xDiff = a.X - b.X;
			double yDiff = a.Y - b.Y;

			return Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
		});

		public override bool Equals(object obj)
		{
			CartesianPoint that = obj as CartesianPoint;

			if (that == null) return false;
			if (that.X != this.X) return false;
			if (that.Y != this.Y) return false;

			return true;
		}

		public override int GetHashCode()
		{
			return (int)Math.Pow(X, Y);
		}
	}
}
