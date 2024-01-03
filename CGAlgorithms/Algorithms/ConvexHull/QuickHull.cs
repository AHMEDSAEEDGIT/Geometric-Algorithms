using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        List<Point> outpts = new List<Point>();
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            double mn = points[0].Y, mx = points[0].Y;
            Point mnp = points[0], mxp = points[0];
            foreach (Point p in points)
            {
                if (p.Y > mx)
                {
                    mx = p.Y;
                    mxp = p;
                }
                if(p.Y < mn)
                {
                    mn = p.Y;
                    mnp = p;
                }
            }
            outpts.Add(mnp);
            quickHull(points, new Line(mnp, mxp));
            if(mnp != mxp)
            outpts.Add(mxp);
            quickHull(points, new Line(mxp, mnp));
            outPoints = outpts;
        }
        void quickHull(List<Point> inpts, Line ab)
        {
            List<Point> right = getRightPoints(inpts, ab);
            Point mx = getMaxPoint(right, ab);
            if (mx == null)
                return;
            quickHull(right, new Line(ab.Start, mx));
            outpts.Add(mx);
            quickHull(right, new Line(mx, ab.End));
        }
        Point getMaxPoint(List<Point> inpts, Line ab)
        {
            double mx = 0;
            Point ret = null;
            foreach (Point p in inpts)
            {
                double d = HelperMethods.CrossProduct(p.Vector(ab.Start), p.Vector(ab.End))
                    /ab.Start.Vector(ab.End).Magnitude();
                d = Math.Abs(d);
                if(d > mx)
                {
                    mx = d;
                    ret = p;
                }
            }
            return ret;
        }
        List<Point> getRightPoints(List<Point> inpts, Line ab)
        {
            List<Point> ret = new List<Point>();
            foreach (Point p in inpts)
            {
                if (HelperMethods.CheckTurn(ab, p) == Enums.TurnType.Right)
                    ret.Add(p);
            }
            return ret;
        }
        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
