using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Final
{
    class Polygon
    {
        private string polygon_str;
        private List<Point> polygon;
        private Point downRight, downLeft, upLeft, upRight;
        public Polygon(string polygon_str)
        {
            this.polygon = new List<Point>();
            this.polygon_str = polygon_str;
            var polygonC = this.polygon_str.Substring(9, this.polygon_str.Length - 11).Split(',');
            // create points from str.
            for (int i = 0; i < 4; i++)
            {
                var p = polygonC[i].Split();
                Point point = new Point(p[0], p[1]);
                polygon.Add(point);
            }
            this.downRight = polygon[0];
            this.downLeft = polygon[1];
            this.upLeft = polygon[2];
            this.upRight = polygon[3];
        }

        public Polygon(string x1, string y1, string x2, string y2)
        {
            this.polygon = new List<Point>();
            this.downRight = new Point(x1, y1);
            polygon.Add(this.downRight);
            this.downLeft = new Point(x2, y1);
            polygon.Add(this.downLeft);
            this.upLeft = new Point(x2, y2);
            polygon.Add(this.upLeft);
            this.upRight = new Point(x1, y2);
            polygon.Add(this.upRight);

        }

        public Polygon(double[] points)
        {
            this.polygon = new List<Point>();
            this.downRight = new Point(points[0], points[1]);
            polygon.Add(this.downRight);
            this.downLeft = new Point(points[2], points[1]);
            polygon.Add(this.downLeft);
            this.upLeft = new Point(points[2], points[3]);
            polygon.Add(this.upLeft);
            this.upRight = new Point(points[0], points[3]);
            polygon.Add(this.upRight);

        }
        public Polygon() { }

        public Point getUpLeft()
        {
            return this.upLeft;
        }

        public Point getDownLeft()
        {
            return this.downLeft;
        }

        public Point getUpRigt ()
        {
            return this.upRight;
        }

        public Point getDownRight()
        {
            return this.downRight;
        }

        public List<Point> getPolygon()
        {
            return this.polygon;
        }

        public double[] GetPoints()
        {
            double x1, y1, x2, y2;
            x1 = this.downLeft.getX();
            y1 = this.downLeft.getY();
            x2 = this.upLeft.getX();
            y2 = this.upLeft.getY();
            double[] s = { x1, y1, x2, y2 };
            return s;
        }

        public void setDownLeft(Point p)
        {
            this.downLeft = p; ;
        }

        public void setDownRight(Point p)
        {
            this.downRight = p; ;
        }

        public void setUpRight(Point p)
        {
            this.upRight = p; ;
        }

        public void setUpLeft(Point p)
        {
            this.upLeft = p; ;
        }

        // If one polygon include the other one, return the biggest.
        // if they are the same return one of them
        // else, union them.
        public Polygon Include (Polygon polygon2)
        {
            if (this.Equals(polygon2)) return this;
            // CREATE new Polygon
            Polygon newPoly = new Polygon();
            newPoly.setDownLeft(this.downLeft.getBiggestPoint(polygon2.getDownLeft(), "downLeft"));
            newPoly.setUpLeft(this.downLeft.getBiggestPoint(polygon2.getUpLeft(), "upLeft"));
            newPoly.setDownRight(this.downLeft.getBiggestPoint(polygon2.getDownRight(), "downRight"));
            newPoly.setUpLeft(this.downLeft.getBiggestPoint(polygon2.getUpLeft(), "upRight"));

            return newPoly;
        }

        public bool Equals (Polygon polygon2)
        {
            List<Point> poly2 = polygon2.getPolygon();
            foreach (Point p in poly2)
            {
                if (!p.IsIn(this.polygon)) return false;
            }
            return true;
        }


        public Point getMiddlePoint ()
        {
            double x1, y1, x2, y2;
            x1 = this.downLeft.getX();
            y1 = this.downLeft.getY();
            x2 = this.upLeft.getX();
            y2 = this.upLeft.getY();

            double middleX = (x1 + x2) / 2;
            double middleY = (y1 + y2) / 2;
            return new Point(middleX,middleY); 
        }

        public double DistanceFromPoint(Point p , out Line line)
        {
            line = null;
            double minDistance = 0;
            // create line from two points
            for (int i = 0; i < 5; i++)
            {
                Line l = null;
                if (i < 3) l = new Line(polygon[i], polygon[i + 1]);
                else l = new Line(polygon[3], polygon[0]);
                double dis = l.LineDistanceFromPoint(p);
                if (dis < minDistance || i == 0)
                {
                    minDistance = dis;
                    line = l;
                }
            }
            return minDistance;
        }


        public bool IsIn (Polygon polygon2)
        {
            if ((polygon2.getUpLeft().getX() >= this.upLeft.getX()) && (polygon2.getUpLeft().getY() <= this.upLeft.getY())
                && (polygon2.getDownLeft().getX() >= this.downLeft.getX()) && (polygon2.getDownLeft().getY() >= this.downLeft.getY())
                && (polygon2.getDownRight().getX() <= this.downRight.getX()) && (polygon2.getDownRight().getY() >= this.downRight.getY())
                && (polygon2.getUpRigt().getX() <= this.upRight.getX()) && (polygon2.getUpRigt().getY() <= this.upRight.getY()))
            {
                return true;
            }
            return false;

        }
        public bool DistanceFromPolygonInRange (Polygon polygon2)
        {
            // check if the polygons are the same
            if (this.Equals(polygon2)) return true;
            // check if one of then include in the other
            if (this.IsIn(polygon2) || polygon2.IsIn(this)) return true;

            //else find the closest point in the new Polygon
            List<Point> poly2Points = polygon2.getPolygon();
            double minDistance = 0;
            Point point = poly2Points[0];
            int i = 0;
            double dis = 0;
            Line closestLine = null;
            foreach (Point p in poly2Points)
            {
                dis = this.DistanceFromPoint(p, out closestLine);
                if (i == 0)
                {
                    minDistance = dis;
                    i++;
                }
                if (dis < minDistance)
                {
                    minDistance = dis;
                    point = p;
                }
            }

            if (minDistance <= 500) return true;
            int j = poly2Points.IndexOf(point);
            int k = j - 1, h = j + 1;
            if (j == 3) h = 0;
            if (j == 0) k = 3;

            // find the colsest line
            Line line1 = new Line(point, poly2Points[h]);
            Line line2 = new Line(poly2Points[k], point);

            if (closestLine.FindDistanceBetweenSegments(line1) >= 500 || closestLine.FindDistanceBetweenSegments(line2) >= 500) return true;
            return false;
        }

        // Check if the point is in the polygon
        public bool PointIsIn (Point point)
        {
            double x = point.getX();
            double y = point.getY();
            if ((x<=downRight.getX()) && (y>=downRight.getY()) && 
                (x >= downLeft.getX()) && (y >= downLeft.getY()) &&
                (x <= upRight.getX()) && (y <= upRight.getY()) &&
                (x >= upLeft.getX()) && (y <= upLeft.getY()))
            {
                return true;
            }
            return false;
        } 

        // check of point is in range of 500 mt from this polygon.
        public bool PointInRange (Point point)
        {
            Line line;
            if (this.PointIsIn(point)) return true;
            if (this.DistanceFromPoint(point, out line) <= 500) return true;

            return false;
        }
    }

    class Point
    {
        private double X;
        private double Y;
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public Point(string x, string y)
        {
            this.X = double.Parse(x);
            this.Y = double.Parse(y);
        }
        public Point (string point_str)
        {
            var strNums = point_str.Substring(6, point_str.Length - 7).Split();
            this.X = double.Parse(strNums[0]);
            this.Y = double.Parse(strNums[1]);
        }

        public double getX()
        {
            return this.X;
        }

        public double getY()
        {
            return this.Y;
        }

        public void setX(int x)
        {
            this.X = x;
        }

        public void setY(int y)
        {
            this.Y = y;
        }


        public Point getBiggestPoint(Point p, string side)
        {
            double bigX = this.X;
            double bigY = this.Y;
            double pX = p.getX();
            double pY = p.getY();
            if (side == "upLeft")
            {
                // get the smallest X
                if (this.X > p.getX()) bigX = p.getX();
                // get the largest y
                if (this.Y < p.getY()) bigY = p.getY();
            }

            if (side == "downLeft")
            {
                // get the smallest X
                if (this.X > p.getX()) bigX = p.getX();
                // get the smallest y
                if (this.Y > p.getY()) bigY = p.getY();
            }

            if (side == "upRight")
            {
                // get the largest X
                if (this.X < p.getX()) bigX = p.getX();
                // get the largest y
                if (this.Y < p.getY()) bigY = p.getY();
            }

            if (side == "downRight")
            {
                // get the largest X
                if (this.X < p.getX()) bigX = p.getX();
                // get the largest y
                if (this.Y < p.getY()) bigY = p.getY();
            }

            return new Point(bigX, bigY);
        }

        public bool Equals(Point p)
        {
            if (this.X == p.getX() && this.Y == p.getY()) return true;
            return false;
        }

        //check if point is in list.
        public bool IsIn(List<Point> list)
        {
            foreach (Point p in list)
            {
                if (p.Equals(this)) return true;
            }
            return false;
        }

        // check if one point is in range of 500 m from the other
        public bool InRange (Point p)
        {
            // if the point are equals return true
            if (this.Equals(p)) return true;
            var dis = Math.Sqrt(Math.Pow((p.getX() - X), 2) + Math.Pow((p.getY() - Y), 2));
            if (dis <= 500) return true;
            return false;
        }

       
    }

    class Line
    {
        private Point start;
        private Point end;

        public Line() { }
        public Line (Point p1, Point p2)
        {
            this.start = p1;
            this.end = p2;
        }

        public void setStart (Point p)
        {
            this.start = p;
        }

        public void setEnd(Point p)
        {
            this.end = p;
        }

        public Point getStart ()
        {
            return this.start;
        }

        public Point getEnd()
        {
            return this.end;
        }

        public double LineDistanceFromPoint (Point p)
        {
            var l1X = this.start.getX();
            var l1Y = this.start.getY();
            var l2X = this.end.getX();
            var l2Y = this.end.getY();

            return Math.Abs((l2X - l1X) * (l1Y - p.getX()) - (l1X - p.getX()) * (l2Y - l1Y)) /
                Math.Sqrt(Math.Pow(l2X - l1X, 2) + Math.Pow(l2Y - l1Y, 2));

        }



        // Return the shortest distance between the two segments
        // p1 --> p2 and p3 --> p4.
        public double FindDistanceBetweenSegments(Line line2)
        {
            // See if the segments intersect.
            bool lines_intersect, segments_intersect;
            Point intersection;
            FindIntersection(line2 ,out lines_intersect, out segments_intersect, out intersection);
            if (segments_intersect)
            {
                return 0;
            }

            // Find the other possible distances.;
            double best_dist = double.MaxValue, test_dist;

            // Try p1.
            test_dist = FindDistanceToSegment(this.start, line2.getStart(), line2.getEnd());
            if (test_dist < best_dist)
            {
                best_dist = test_dist;
            }

            // Try p2.
            test_dist = FindDistanceToSegment(this.end, line2.getStart(), line2.getEnd());
            if (test_dist < best_dist)
            {
                best_dist = test_dist;
            }

            // Try p3.
            test_dist = FindDistanceToSegment(line2.getStart(), this.start, this.end);
            if (test_dist < best_dist)
            {
                best_dist = test_dist;
            }

            // Try p4.
            test_dist = FindDistanceToSegment(line2.getEnd(), this.start, this.end);
            if (test_dist < best_dist)
            {
                best_dist = test_dist;
            }

            return best_dist;
        }




        private void FindIntersection( Line line, out bool lines_intersect, out bool segments_intersect, out Point intersection)
        {
            // Get the segments' parameters.
            double dx12 = this.end.getX() - this.start.getX();
            double dy12 = this.end.getY() - this.start.getY();
            double dx34 = line.getEnd().getX() - line.getStart().getX();
            double dy34 = line.getEnd().getY() - line.getStart().getY();

            // Solve for t1 and t2
            double denominator = (dy12 * dx34 - dx12 * dy34);

            double t1 = ((this.start.getX() - line.getStart().getX()) * dy34 + (line.getStart().getY() - this.start.getY()) * dx34)/ denominator;
            if (double.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Point(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            double t2 =
                ((line.getStart().getX() - this.start.getX()) * dy12 + (this.start.getY() - line.getStart().getY()) * dx12) / -denominator;

            // Find the point of intersection.
            intersection = new Point(this.start.getX() + dx12 * t1, this.start.getY() + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

        }



        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        private double FindDistanceToSegment( Point pt, Point p1, Point p2)
        {
            double dx = p2.getX() - p1.getX();
            double dy = p2.getY() - p1.getY();
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                dx = pt.getX() - p1.getX();
                dy = pt.getY() - p1.getY();
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            double t = ((pt.getX() - p1.getX()) * dx + (pt.getY() - p1.getY()) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                dx = pt.getX() - p1.getX();
                dy = pt.getY() - p1.getY();
            }
            else if (t > 1)
            {
                dx = pt.getX() - p2.getX();
                dy = pt.getY() - p2.getY();
            }
            
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
