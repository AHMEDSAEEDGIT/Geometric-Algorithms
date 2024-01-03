using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            HelperMethods.delete_duplicte(ref points);
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }
            points.Sort((a, b) => (a.X != b.X)? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y));
            {
                int i = 1, j = 2;
                List<Point> idx = new List<Point>();
                while (HelperMethods.CheckTurn(new Line(points[0], points[i]), points[j]) == Enums.TurnType.Colinear)
                {
                    idx.Add(points[i]);
                    i++; j++;
                }
                foreach(Point p in idx)
                {
                    points.Remove(p);
                }
            }
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }
            List<Point> hull = new List<Point>();
            hull.Add(points[0]);
            if (HelperMethods.CheckTurn(new Line(points[0], points[1]), points[2]) == Enums.TurnType.Right)
            {
                hull.Add(points[2]);
                hull.Add(points[1]);
            }
            else
            {
                hull.Add(points[1]);
                hull.Add(points[2]);
            }
            points.RemoveRange(0, 3);
            outPoints = incremntalHull(hull, points);
        }
        List<Point> incremntalHull(List<Point> hull, List<Point> pts)
        {
            {
                int j = 0;
                double mx = hull[0].X;
                for (int i = 1; i < hull.Count; i++)
                    if (hull[i].X > mx)
                    {
                        mx = hull[i].X;
                        j = i;
                    }
                foreach (Point p in pts)
                {
                    j = addToHull(ref hull, p, j);
                }
            }
            {
                List<Point> idx = new List<Point>();
                for (int i = 0; i < hull.Count; i++)
                {
                    int j = (i + 1)%hull.Count;
                    int k = (i + 2)%hull.Count;
                    if (HelperMethods.CheckTurn(new Line(hull[i], hull[j]), hull[k]) == Enums.TurnType.Colinear)
                    {
                        idx.Add(hull[j]);
                    }
                }
                foreach (Point p in idx)
                {
                    hull.Remove(p);
                }
            }
            return hull;
        }
        int addToHull(ref List<Point> hull, Point p, int idx)// idx is lift most point in hull
        {
            int n = hull.Count;
            int j = idx;
            while(HelperMethods.CheckTurn(new Line(p, hull[j]), hull[(j + 1) % n]) == Enums.TurnType.Right
                || HelperMethods.CheckTurn(new Line(p, hull[j]), hull[(j-1+n) % n]) == Enums.TurnType.Right)
            {
                j++;
                if(j==n)
                {
                    j = 0;
                    break;
                }
            }
            int i = idx;
            while (HelperMethods.CheckTurn(new Line(p, hull[i]), hull[(i + 1) % n]) == Enums.TurnType.Left
                || HelperMethods.CheckTurn(new Line(p, hull[i]), hull[(i - 1 + n) % n]) == Enums.TurnType.Left)
            {
                if (i == 0) break;
                i--;
                if (i < 0) i += n;
            }
            if(i+1 == n)
            {
                hull.Add(p);
                return n;
            }
            i = (i + 1) % n;
            j = (j - 1 + n) % n;
            hull.RemoveRange(i, j-i+1);
            hull.Insert(i, p);
            return i;
        }
        
        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
