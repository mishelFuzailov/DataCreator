using Project_Final;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Project_Final
{
    static class Converters
    {
        public const int eWGS84 = 0;
        public const int eGRS80 = 1;
        public const int eCLARK80M = 2;

        public const int gICS = 0;
        public const int gITM = 1;

        static Converters()
        {
            DatumList = new List<DATUM>();
            GridList = new List<GRID>();
            // WGS84 data
            DatumList.Add(new DATUM(
            6378137.0,				// a
            6356752.3142,			// b
            0.00335281066474748,  	// f = 1/298.257223563
            0.006694380004260807,	// esq
            0.0818191909289062,     // e
                                    // deltas to WGS84
            0,
            0,
            0
        ));

            // GRS80 data
            DatumList.Add(new DATUM(
                6378137.0,				// a
                6356752.3141,			// b
                0.0033528106811823,		// f = 1/298.257222101
                0.00669438002290272,	// esq
                0.0818191910428276,     // e
                                        // deltas to WGS84
                -48,
                55,
                52
            ));

            // Clark 1880 Modified data
            DatumList.Add(new DATUM(
                6378300.789,			// a
                6356566.4116309,		// b
                0.003407549767264,		// f = 1/293.466
                0.006803488139112318,	// esq
                0.08248325975076590,    // e
                                        // deltas to WGS84
                -235,
                -85,
                264
            ));

            GridList.Add(new GRID(
                // ICS data

                0.6145667421719,			// lon0 = central meridian in radians of 35.12'43.490"
                0.55386447682762762,		// lat0 = central latitude in radians of 31.44'02.749"
                1.00000,					// k0 = scale factor
                170251.555,					// false_easting
                2385259.0					// false_northing
            ));

            // ITM data
            GridList.Add(new GRID(
                0.61443473225468920,		// lon0 = central meridian in radians 35.12'16.261"
                0.55386965463774187,		// lat0 = central latitude in radians 31.44'03.817"
                1.0000067,					// k0 = scale factor
                219529.584,					// false_easting
                2885516.9488                // false_northing = 3512424.3388-626907.390
                                            // MAPI says the false northing is 626907.390, and in another place
                                            // that the meridional arc at the central latitude is 3512424.3388
            ));
        }

        public static double pi() { return 3.141592653589793; }
        public static double sin2(double x) { return Math.Sin(x) * Math.Sin(x); }
        public static double cos2(double x) { return Math.Cos(x) * Math.Cos(x); }
        public static double tan2(double x) { return Math.Tan(x) * Math.Tan(x); }
        public static double tan4(double x) { return tan2(x) * tan2(x); }


        public class DATUM
        {
            public double a;	// a  Equatorial earth radius
            public double b;	// b  Polar earth radius
            public double f;	// f= (a-b)/a  Flatenning
            public double esq;	// esq = 1-(b*b)/(a*a)  Eccentricity Squared
            public double e;	// sqrt(esq)  Eccentricity
            // deltas to WGS84
            public double dX;
            public double dY;
            public double dZ;

            public DATUM(double a, double b, double f, double esq, double e, double dx, double dy, double dz)
            {
                this.a = a;
                this.b = b;
                this.f = f;
                this.esq = esq;
                this.e = e;
                this.dX = dx;
                this.dY = dy;
                this.dZ = dz;
            }
        }

        public class GRID
        {
            public double lon0;
            public double lat0;
            public double k0;
            public double false_e;
            public double false_n;

            public GRID(double lon0, double lat0, double k0, double false_e, double false_n)
            {
                this.lon0 = lon0;
                this.lat0 = lat0;
                this.k0 = k0;
                this.false_n = false_n;
                this.false_e = false_e;
            }
        };

        public static List<DATUM> DatumList;
        public static List<GRID> GridList;


        public static System.Data.Entity.Spatial.DbGeography ITMtoWGS84_info(double[] points)
        {
            int pointsNum = points.Length;
            double[] wgsP;
            if (points[3] == -1)
            {
                wgsP = ITMtoWGS84(points[0], points[1]);
                return GeographicP.GetPoint(wgsP[0], wgsP[1]);
            }
            wgsP = FourITMtoWGS84(points[0], points[1], points[2], points[3]);
            List<string> p = new List<string>();
            for (int i=0; i < pointsNum; i++)
            {
                p.Add(wgsP[i].ToString());
            }
            if (p.Count == 4)
            {
                string[] po = { p[0], p[1], p[2], p[3] };
                return GeographicP.GetPolygon(GeographicP.GetPolygonsFormat(po));
            }
            string[] po2 = { p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7] };
            return GeographicP.GetPolygon(GeographicP.GetPolygonsFormat(po2));
        }


        public static double[] FourITMtoWGS84(double N1, double E1, double N2, double E2)
        {
            double[] temp1 = ITMtoWGS84(N1, E1);
            double[] temp2 = ITMtoWGS84(N2, E2);
            double[] ret = { temp1[0], temp1[1], temp2[0], temp2[1] };
            return ret;
        }

        //=================================================
        // Israel New Grid (ITM) to WGS84 conversion
        //=================================================
        public static double[] ITMtoWGS84(double N, double E)
        {
            // 1. Local Grid (ITM) -> GRS80
            double lat80, lon80;
            double[] temp = Grid2LatLon(N, E, gITM, eGRS80);
            lat80 = temp[0];
            lon80 = temp[1];
            // 2. Molodensky GRS80->WGS84
            double lat84, lon84;
            temp = Molodensky(lat80, lon80, eGRS80, eWGS84);
            lat84 = temp[0];
            lon84 = temp[1];

            // final results
            double lat = lat84 * 180 / pi();
            double lon = lon84 * 180 / pi();
            
            double[] ret = {lat, lon};
            return ret;
        }

        
        //====================================
        // Local Grid to Lat/Lon conversion
        //====================================
        public static double[] Grid2LatLon(double N, double E, int from, int to)
        {
            //================
            // GRID -> Lat/Lon
            //================
            double lat, lon;
            double y = N + GridList[from].false_n;
            double x = E - GridList[from].false_e;
            double M = y / GridList[from].k0;

            double a = DatumList[to].a;
            double b = DatumList[to].b;
            double e = DatumList[to].e;
            double esq = DatumList[to].esq;

            double mu = M / (a * (1 - e * e / 4 - 3 * Math.Pow(e, 4) / 64 - 5 * Math.Pow(e, 6) / 256));

            double ee = Math.Sqrt(1 - esq);
            double e1 = (1 - ee) / (1 + ee);
            double j1 = 3 * e1 / 2 - 27 * e1 * e1 * e1 / 32;
            double j2 = 21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32;
            double j3 = 151 * e1 * e1 * e1 / 96;
            double j4 = 1097 * e1 * e1 * e1 * e1 / 512;

            // Footprint Latitude
            double fp = mu + j1 * Math.Sin(2 * mu) + j2 * Math.Sin(4 * mu) + j3 * Math.Sin(6 * mu) + j4 * Math.Sin(8 * mu);

            double sinfp = Math.Sin(fp);
            double cosfp = Math.Cos(fp);
            double tanfp = sinfp / cosfp;
            double eg = (e * a / b);
            double eg2 = eg * eg;
            double C1 = eg2 * cosfp * cosfp;
            double T1 = tanfp * tanfp;
            double R1 = a * (1 - e * e) / Math.Pow(1 - (e * sinfp) * (e * sinfp), 1.5);
            double N1 = a / Math.Sqrt(1 - (e * sinfp) * (e * sinfp));
            double D = x / (N1 * GridList[from].k0);

            double Q1 = N1 * tanfp / R1;
            double Q2 = D * D / 2;
            double Q3 = (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * eg2 * eg2) * (D * D * D * D) / 24;
            double Q4 = (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 3 * C1 * C1 - 252 * eg2 * eg2) * (D * D * D * D * D * D) / 720;
            // result lat
            lat = fp - Q1 * (Q2 - Q3 + Q4);

            double Q5 = D;
            double Q6 = (1 + 2 * T1 + C1) * (D * D * D) / 6;
            double Q7 = (5 - 2 * C1 + 28 * T1 - 3 * C1 * C1 + 8 * eg2 * eg2 + 24 * T1 * T1) * (D * D * D * D * D) / 120;
            // result lon
            lon = GridList[from].lon0 + (Q5 - Q6 + Q7) / cosfp;
            double[] ret = { lat, lon };
            return ret;
        }

        

        //======================================================
        // Abridged Molodensky transformation between 2 datums
        //======================================================
        public static double[] Molodensky(double ilat, double ilon, int from, int to)
        {
            // from->WGS84 - to->WGS84 = from->WGS84 + WGS84->to = from->to
            double dX = DatumList[from].dX - DatumList[to].dX;
            double dY = DatumList[from].dY - DatumList[to].dY;
            double dZ = DatumList[from].dZ - DatumList[to].dZ;

            double slat = Math.Sin(ilat);
            double clat = Math.Cos(ilat);
            double slon = Math.Sin(ilon);
            double clon = Math.Cos(ilon);
            double ssqlat = slat * slat;


            double from_f = DatumList[from].f;
            double df = DatumList[to].f - from_f;
            double from_a = DatumList[from].a;
            double da = DatumList[to].a - from_a;
            double from_esq = DatumList[from].esq;
            double adb = 1.0 / (1.0 - from_f);
            double rn = from_a / Math.Sqrt(1 - from_esq * ssqlat);
            double rm = from_a * (1 - from_esq) / Math.Pow((1 - from_esq * ssqlat), 1.5);
            double from_h = 0.0; // we're flat!

            double dlat = (-dX * slat * clon - dY * slat * slon + dZ * clat
                           + da * rn * from_esq * slat * clat / from_a +
                           +df * (rm * adb + rn / adb) * slat * clat) / (rm + from_h);

            // result lat (radians)
            double olat = ilat + dlat;

            // dlon = (-dx * slon + dy * clon) / ((rn + from.h) * clat);
            double dlon = (-dX * slon + dY * clon) / ((rn + from_h) * clat);
            // result lon (radians)
            double olon = ilon + dlon;

            double[] ret = { olat, olon };
            return ret;
        }

    }
}
