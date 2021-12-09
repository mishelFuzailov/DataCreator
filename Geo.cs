using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
	public class GeographicCoordinate
	{
		private const double Tolerance = 10.0 * .1;

		public GeographicCoordinate(double longitude, double latitude)
		{
			this.Longitude = longitude;
			this.Latitude = latitude;
		}

		public double Latitude { get; set; }
		public double Longitude { get; set; }

		public static bool operator ==(GeographicCoordinate a, GeographicCoordinate b)
		{
			// If both are null, or both are same instance, return true.
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			var latResult = Math.Abs(a.Latitude - b.Latitude);
			var lonResult = Math.Abs(a.Longitude - b.Longitude);
			return (latResult < Tolerance) && (lonResult < Tolerance);
		}

		public static bool operator !=(GeographicCoordinate a, GeographicCoordinate b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			// Check for null values and compare run-time types.
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var p = (GeographicCoordinate)obj;
			var latResult = Math.Abs(this.Latitude - p.Latitude);
			var lonResult = Math.Abs(this.Longitude - p.Longitude);
			return (latResult < Tolerance) && (lonResult < Tolerance);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (this.Latitude.GetHashCode() * 397) ^ this.Longitude.GetHashCode();
			}
		}
	}

	/**
	 *  Create polygon from string
	 */

	public class GeographicP
	{
		private static IEnumerable<GeographicCoordinate> ConvertStringArrayToGeographicCoordinates(string pointString)
		{
			var points = pointString.Split(',');
			var coordinates = new List<GeographicCoordinate>();

			for (var i = 0; i < points.Length / 2; i++)
			{
				var geoPoint = points.Skip(i * 2).Take(2).ToList();
				coordinates.Add(new GeographicCoordinate(double.Parse(geoPoint.First()), double.Parse(geoPoint.Last())));
			}

			return coordinates;
		}



		public static string GetPolygonsFormat(string[] values)
		{
			string pointString = GetPolygonSTR(values);
			IEnumerable<GeographicCoordinate> coordinates = ConvertStringArrayToGeographicCoordinates(pointString);
			var coordinateList = coordinates.ToList();
			if (coordinateList.First() != coordinateList.Last())
			{
				throw new Exception("First and last point do not match. This is not a valid polygon");
			}

			var count = 0;
			var sb = new StringBuilder();
			sb.Append(@"POLYGON((");
			foreach (var coordinate in coordinateList)
			{
				if (count == 0)
				{
					sb.Append(coordinate.Longitude + " " + coordinate.Latitude);
				}
				else
				{
					sb.Append("," + coordinate.Longitude + " " + coordinate.Latitude);
				}

				count++;
			}

			sb.Append(@"))");

			return sb.ToString();
		}




		public static string GetPolygonSTR(string[] values)
		{
			string x1 = values[0];
			string y1 = values[1];
			string x2 = values[2];
			string y2 = values[3];
			if (values.Length == 4)
            {
				return x1 + "," + y1 + "," + x2 + "," + y1 + "," + x2 + "," + y2 + "," + x1 + "," + y2 + "," + x1 + "," + y1;
			}

			string x3 = values[4];
			string y3 = values[5];
			string x4 = values[6];
			string y4 = values[7];
			return x1 + "," + y1 + "," + x2 + "," + y2 + "," + x3 + "," + y3 + "," + x4 + "," + y4 + "," + x1 + "," + y1;
		}



		public static System.Data.Entity.Spatial.DbGeography GetPolygon(string sb)
        {
			return System.Data.Entity.Spatial.DbGeography.PolygonFromText(sb, 4326);
		}




		public static System.Data.Entity.Spatial.DbGeography GetPoint(double x, double y)
		{
			var point = string.Format("POINT({0} {1})", x, y);
			System.Data.Entity.Spatial.DbGeography p = System.Data.Entity.Spatial.DbGeography.PointFromText(point, 4326);
			return p;
		}




		public static string CreatePointStr(double x, double y)
		{
			return string.Format("POINT({0} {1})", x, y);
		}



		// return the nig_info
		public static string GetNIG_info(double[] points)
        {
			//If it point
			if (points.Length == 2)
			{
				return CreatePointStr(points[0], points[1]);
			}

			if (points[2] == -1 && points[3] == -1)
            {
				return CreatePointStr(points[0], points[1]);
            }

			// if its polygon
			int len = points.Length;
			string[] po = new string[len];
			for (int i = 0; i < len; i++)
            {
				po[i] = points[i].ToString();
            }
			return GetPolygonsFormat(po);
		}



		// return  WGS_INFO
		public static System.Data.Entity.Spatial.DbGeography GetWGS_info(double[] points)
		{
			//If it point
			if (points.Length == 2)
			{
				return GetPoint(points[0], points[1]);
			}

			if (points[2] == -1 && points[3] == -1)
			{
				return GetPoint(points[0], points[1]);
			}

			// if its polygon
			int len = points.Length;
			string[] po = new string[len];
			for (int i = 0; i < len; i++)
			{
				po[i] = points[i].ToString();
			}
			return GetPolygon(GetPolygonsFormat(po));
		}

	}
}
