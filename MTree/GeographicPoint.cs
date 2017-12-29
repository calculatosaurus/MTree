using System;
using System.Device.Location;

namespace MTree
{
	public class GeographicPoint
	{
		public double Latitude { get; private set; }
		public double Longitude { get; private set; }
		public GeoCoordinate Location { get; private set; }

		public GeographicPoint(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
			Location = new GeoCoordinate(latitude, longitude);
		}

		public static Func<GeographicPoint, GeographicPoint, double> GetDistance
			= new Func<GeographicPoint, GeographicPoint, double>((p1, p2) => p1.Location.GetDistanceTo(p2.Location));

		public override bool Equals(object other)
		{
			GeographicPoint that = other as GeographicPoint;

			if (that == null) return false;

			return this.Location.Equals(that.Location);
		}

		public override int GetHashCode()
		{
			return this.Location.GetHashCode();
		}
	}
}
