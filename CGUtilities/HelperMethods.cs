using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities.DataStructures;

namespace CGUtilities
{
    public class HelperMethods
    {
        /*****************************************************/
        public static void delete_duplicte(ref List<Point> points)
        {
            HashSet<Tuple<double, double>> hSet = new HashSet<Tuple<double, double>>();

            foreach (var i in points)
                try { hSet.Add(new Tuple<double, double>(i.X, i.Y)); }
                catch { };
            points = new List<Point>();

            foreach (var i in hSet)
                points.Add(new Point(i.Item1, i.Item2));
        }


        public static double calculateAngle_3(Point a, Point b, Point c)
        {
            double P1X = a.X, P1Y = a.Y, P2X = b.X, P2Y = b.Y, P3X = c.X, P3Y = c.Y;

            double numerator = P2Y * (P1X - P3X) + P1Y * (P3X - P2X) + P3Y * (P2X - P1X);
            double denominator = (P2X - P1X) * (P1X - P3X) + (P2Y - P1Y) * (P1Y - P3Y);
            double ratio = numerator / denominator;

            double angleRad = Math.Atan2(numerator, denominator);
            double angleDeg = (angleRad * 180) / Math.PI;

            if (angleDeg < 0)
            {
                angleDeg = 360 + angleDeg;
            }

            return 360 - angleDeg;
        }

        public static Point tovector(Point a, Point b)
        {
            return new Point(b.X - a.X, b.Y - a.Y);
        }

        public static double Get_Angle(Point PMinus, Point PPlus, Point P)
        {
            Point v1 = tovector(PMinus, P);
            Point v2 = tovector(PPlus, P);
            double cros = HelperMethods.CrossProduct(v1, v2);
            double dot = HelperMethods.DotProduct(v1, v2);
            double angle = Math.Atan2(cros, dot);
           
            if (angle < 0)
                angle += 360;

            return angle;
        }
        // last , cur, new
        public static double calculate_angle(Point a, Point b, Point c)
        {
            double v1x = b.X - a.X, v1y = b.Y - a.Y;
            double v2x = c.X - b.X, v2y = c.Y - b.Y;

            double dot = v1x * v2x + v1y * v2y;
            double cross = v1x * v2y - v1y * v2x;

            double angleRad = Math.Atan2(cross, dot);
            double angleDeg = (angleRad * 180) / Math.PI;

            return angleDeg < 0 ? angleDeg + 360 : angleDeg;
        }

        // calculate angle between two ponts and X axis
        public static double calculateAngle_2(Point a, Point b)
        {
            double x = b.X - a.X;
            double y = b.Y - a.Y;
            double radians = Math.Atan2(y, x);
            double angle = radians * (180 / Math.PI);

            return angle;
        }

        public static bool intersect(Line a, Line b, ref Point p)
        {
            double divisor = (a.Start.X - a.End.X) * (b.Start.Y - b.End.Y) - (a.Start.Y - a.End.Y) * (b.Start.X - b.End.X);
            if (Math.Abs(divisor) < Constants.Epsilon)// parallel lines
                return false;
            p.X = ((a.Start.X * a.End.Y - a.Start.Y * a.End.X) * (b.Start.X - b.End.X) - (a.Start.X - a.End.X) * (b.Start.X * b.End.Y - b.Start.Y * b.End.X)) / divisor;
            p.Y = ((a.Start.X * a.End.Y - a.Start.Y * a.End.X) * (b.Start.Y - b.End.Y) - (a.Start.Y - a.End.Y) * (b.Start.X * b.End.Y - b.Start.Y * b.End.X)) / divisor;
            if (Math.Abs(a.End.X - a.Start.X) > Constants.Epsilon)
            {
                double tmp = (p.X - a.Start.X) / (a.End.X - a.Start.X);
                if (tmp < Constants.Epsilon || tmp > 1.0 - Constants.Epsilon)// out of bounds of x of a
                    return false;
            }
            if (Math.Abs(a.End.Y - a.Start.Y) > Constants.Epsilon)
            {
                double tmp = (p.Y - a.Start.Y) / (a.End.Y - a.Start.Y);
                if (tmp < Constants.Epsilon || tmp > 1.0 - Constants.Epsilon)// out of bounds of y of a
                    return false;
            }
            if (Math.Abs(b.End.X - b.Start.X) > Constants.Epsilon)
            {
                double tmp = (p.X - b.Start.X) / (b.End.X - b.Start.X);
                if (tmp < Constants.Epsilon || tmp > 1.0 - Constants.Epsilon)// out of bounds of x of b
                    return false;
            }
            if (Math.Abs(b.End.Y - b.Start.Y) > Constants.Epsilon)
            {
                double tmp = (p.Y - b.Start.Y) / (b.End.Y - b.Start.Y);
                if (tmp < Constants.Epsilon || tmp > 1.0 - Constants.Epsilon)// out of bounds of y of b
                    return false;
            }
            return true;
        }
        /****************************************************/
        public static Enums.PointInPolygon PointInTriangle(Point p, Point a, Point b, Point c)
        {
            if (a.Equals(b) && b.Equals(c))
            {
                if (p.Equals(a) || p.Equals(b) || p.Equals(c))
                    return Enums.PointInPolygon.OnEdge;
                else
                    return Enums.PointInPolygon.Outside;
            }

            Line ab = new Line(a, b);
            Line bc = new Line(b, c);
            Line ca = new Line(c, a);

            if (GetVector(ab).Equals(Point.Identity)) return (PointOnSegment(p, ca.Start, ca.End)) ? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;
            if (GetVector(bc).Equals(Point.Identity)) return (PointOnSegment(p, ca.Start, ca.End)) ? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;
            if (GetVector(ca).Equals(Point.Identity)) return (PointOnSegment(p, ab.Start, ab.End)) ? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;

            if (CheckTurn(ab, p) == Enums.TurnType.Colinear)
                return PointOnSegment(p, a, b)? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;
            if (CheckTurn(bc, p) == Enums.TurnType.Colinear && PointOnSegment(p, b, c))
                return PointOnSegment(p, b, c) ? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;
            if (CheckTurn(ca, p) == Enums.TurnType.Colinear && PointOnSegment(p, c, a))
                return PointOnSegment(p, a, c) ? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;

            if (CheckTurn(ab, p) == CheckTurn(bc, p) && CheckTurn(bc, p) == CheckTurn(ca, p))
                return Enums.PointInPolygon.Inside;
            return Enums.PointInPolygon.Outside;
        }
        public static Enums.TurnType CheckTurn(Point vector1, Point vector2)
        {
            double result = CrossProduct(vector1, vector2);
            if (result < 0) return Enums.TurnType.Right;
            else if (result > 0) return Enums.TurnType.Left;
            else return Enums.TurnType.Colinear;
        }
        public static double CrossProduct(Point a, Point b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static double DotProduct(Point a, Point b)
        {
            return a.X * b.X - a.Y * b.Y;
        }

        public static bool PointOnRay(Point p, Point a, Point b)
        {
            if (a.Equals(b)) return true;
            if (a.Equals(p)) return true;
            var q = a.Vector(p).Normalize();
            var w = a.Vector(b).Normalize();
            return q.Equals(w);
        }
        public static bool PointOnSegment(Point p, Point a, Point b)
        {
            if (a.Equals(b))
                return p.Equals(a);

            if (b.X == a.X)
                return p.X == a.X && (p.Y >= Math.Min(a.Y, b.Y) && p.Y <= Math.Max(a.Y, b.Y));
            if (b.Y == a.Y)
                return p.Y == a.Y && (p.X >= Math.Min(a.X, b.X) && p.X <= Math.Max(a.X, b.X));
            double tx = (p.X - a.X) / (b.X - a.X);
            double ty = (p.Y - a.Y) / (b.Y - a.Y);

            return (Math.Abs(tx - ty) <= Constants.Epsilon && tx <= 1 && tx >= 0);
        }
        /// <summary>
        /// Get turn type from cross product between two vectors (l.start -> l.end) and (l.end -> p)
        /// </summary>
        /// <param name="l"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Enums.TurnType CheckTurn(Line l, Point p)
        {
            Point a = l.Start.Vector(l.End);
            Point b = l.End.Vector(p);
            return HelperMethods.CheckTurn(a, b);
        }
        public static Point GetVector(Line l)
        {
            return l.Start.Vector(l.End);
        }

        
    }
}
