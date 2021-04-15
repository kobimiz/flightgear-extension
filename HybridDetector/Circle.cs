using System.Collections.Generic;

namespace HybridDetector
{
    internal class Circle
    {
        public Point c;
        public double r;

        public Circle(Point c, double r)
        {
            this.c = c;
            this.r = r;
        }

        public bool isInCircle(double x, double y)
        {
            return new Point(x, y).distance(c) <= r;
        }

        public static Circle trivial(List<Point> points)
        {
            if (points.Count == 0)
                return new Circle(new Point(0.0, 0.0), 0.0);
            if (points.Count == 1)
                return new Circle(new Point(points[0].x, points[0].y), 0.0f);
            if (points.Count == 2)
                return Circle.from2Points(points[0], points[1]);
            return Circle.from3Points(points[points.Count - 1], points[points.Count - 2], points[points.Count - 3]);
        }

        public static Circle from2Points(Point a, Point b)
        {
            double cx = (a.x + b.x) / 2.0;
            double cy = (a.y + b.y) / 2.0;
            double r = a.distance(b) / 2.0;
            return new Circle(new Point(cx, cy), r);
        }

        public static Circle from3Points(Point a, Point b, Point c)
        {
            if (Point.are3PointsInLine(a, b, c))
            {
                // find the two most distant points 
                return a.distance(b) > b.distance(c) ? from2Points(a, b) : from2Points(b, c);
            }
            Point abMiddle = Point.Middle(a, b);
            Point bcMiddle = Point.Middle(b, c);

            // do some (not so fancy) math
            double y1, x1;
            // if line is vertical
            if (a.x == b.x)
            {
                double e = Point.getSlope(b, c);
                y1 = abMiddle.y;
                x1 = e * (bcMiddle.y - y1) + bcMiddle.x;
            }
            else if (b.x == c.x)
            {
                double d = Point.getSlope(a, b);
                y1 = bcMiddle.y;
                x1 = d * (abMiddle.y - y1) + abMiddle.x;
            }
            else
            {
                double d = Point.getSlope(a, b);
                double e = Point.getSlope(b, c);

                y1 = (d * abMiddle.y - e * bcMiddle.y + abMiddle.x - bcMiddle.x) / (d - e);
                x1 = d * (abMiddle.y - y1) + abMiddle.x;
            }
            Point center = new Point(x1, y1);
            double r = center.distance(b);
            // check if construction with two points on the edge is shorter
            double abDist = a.distance(b);
            double bcDist = b.distance(c);
            double acDist = a.distance(c);

            if (abDist / 2.0 < r)
            {
                Circle temp = from2Points(a, b);
                if (temp.isInCircle(c.x, c.y))
                    return temp;
            }
            if (bcDist / 2.0 < r)
            {
                Circle temp = from2Points(b, c);
                if (temp.isInCircle(a.x, a.y))
                    return temp;
            }
            if (acDist / 2.0f < r)
            {
                Circle temp = from2Points(a, c);
                if (temp.isInCircle(b.x, b.y))
                    return temp;
            }

            return new Circle(center, r);
        }

        public static Circle welzlAlg(List<Point> points, List<Point> onEdge, int pointsSize)
        {
            if (onEdge.Count == 3 || pointsSize == 0)
                return trivial(onEdge);

            Point p = points[pointsSize - 1];

            Circle c = welzlAlg(points, onEdge, pointsSize - 1);
            if (c.isInCircle(p.x, p.y))
                return c;

            // point from the points set which is in the circle must be on the edge of the circle
            onEdge.Add(p);
            return welzlAlg(points, new List<Point>(onEdge), pointsSize - 1);
        }

        public static Circle findMinCircle(Point[] points)
        {
            if (points.Length == 0) return new Circle(new Point(0.0f, 0.0f), 0.0f);
            if (points.Length == 1) return new Circle(new Point(points[0].x, points[0].y), 0.0f);
            if (points.Length == 2) return from2Points(points[0], points[1]);

            List<Point> pointsVec = new List<Point>(points);
            List<Point> onEdge = new List<Point>();

            return welzlAlg(pointsVec, onEdge, pointsVec.Count);
        }

    }
}