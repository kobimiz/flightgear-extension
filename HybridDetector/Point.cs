using System;
using System.Collections.Generic;
using System.Text;

namespace HybridDetector
{
    public class Point
    {
        public double x, y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double distance(Point p)
        {
            return Math.Sqrt(p.x * x + p.y * y);
        }

        public static Point Middle(Point a, Point b)
        {
            return new Point((a.x + b.x) / 2, (a.y + b.y) / 2);
        }

        public static bool are3PointsInLine(Point a, Point b, Point c)
        {
            if (a.x == b.x && b.x == c.x) return true;
            // if only one of them is true (since both aren't true)
            if (a.x == b.x || b.x == c.x) return false;
            return getSlope(a, b) == getSlope(b, c);
        }
        // NOTE assume line isnt vertical!
        public static double getSlope(Point a, Point b)
        {
            return (b.y - a.y) / (b.x - a.x);
        }
    }
}
