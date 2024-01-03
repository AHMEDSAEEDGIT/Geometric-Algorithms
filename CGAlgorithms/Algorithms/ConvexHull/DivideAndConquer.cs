using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            HelperMethods.delete_duplicte(ref points);
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }
            points.Sort((a, b) => (a.X != b.X) ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y));
            outPoints = DAC(points);
            Console.WriteLine(outPoints.Count);
        }
        public List<Point> merge(List<Point> hull1, List<Point> hull2)
        {
            int n1 = hull1.Count, n2 = hull2.Count;
            if (n1 ==2 || n2 ==2)
                return brute(hull1.Concat(hull2).ToList());
            int i1 = 0, i2 = 0;
            for (int i = 1; i < n1; i++)
                if (hull1[i].X > hull1[i1].X)
                    i1 = i;
            for (int i = 1; i < n2; i++)
                if (hull2[i].X < hull2[i2].X)
                    i2 = i;
            int ind1 = i1;
            int ind2 = i2;
            bool f = false;
            while(!f)
            {
                f = true;
                while (HelperMethods.CheckTurn(new Line(hull2[ind2], hull1[ind1]), hull1[(ind1 + 1) % n1]) != Enums.TurnType.Left)
                {
                    ind1 = (ind1 + 1) % n1;
                    //if (ind1 == 0) break;
                    
                }
                while (HelperMethods.CheckTurn(new Line(hull1[ind1], hull2[ind2]), hull2[(ind2 - 1+n2) % n2]) != Enums.TurnType.Right)
                {
                    f = false;
                    ind2 = (ind2 - 1 + n2) % n2;
                    //if (ind2 == 0) break;
                }
            }
            int up1 = ind1, up2 = ind2;
            ind1 = i1;
            ind2 = i2;
            f = false;
            while (!f)
            {
                f = true;
                while (HelperMethods.CheckTurn(new Line(hull1[ind1], hull2[ind2]), hull2[(ind2 + 1) % n2]) != Enums.TurnType.Left)
                {
                    ind2 = (ind2 + 1) % n2;

                }
                while (HelperMethods.CheckTurn(new Line(hull2[ind2], hull1[ind1]), hull1[(ind1 - 1 + n1) % n1]) != Enums.TurnType.Right)
                {
                    ind1 = (ind1 - 1 + n1) % n1;
                    f = false;
                }
            }
            int down1 = ind1, down2 = ind2;
            List<Point> ret = new List<Point>();
            ret.Add(hull1[up1]);
            while(up1 != down1)
            {
                up1 = (up1 + 1) % n1;
                ret.Add(hull1[up1]);
            }
            ret.Add(hull2[down2]);
            while (up2 != down2)
            {
                down2 = (down2 + 1) % n2;
                ret.Add(hull2[down2]);
            }
            return ret;
        }
        public List<Point> DAC(List<Point> points)
        {
            if (points.Count < 8)
                return brute(points);
            List<Point> points1 = new List<Point>();
            List<Point> points2 = new List<Point>();
            for (int i = 0; i < points.Count / 2; i++)
                points1.Add(points[i]);
            for (int i = points.Count / 2 ; i < points.Count; i++)
                points2.Add(points[i]);
            List<Point> hull1 = DAC(points1);
            List<Point> hull2 = DAC(points2);
            return merge(hull1, hull2);
        }
        public List<Point> brute(List<Point> points)
        {
            QuickHull q = new QuickHull();
            List<Point> l = new List<Point>();
            List<Line> x = new List<Line>();
            List<Polygon> y = new List<Polygon>();
            q.Run(points, null, null, ref l, ref x, ref y);
            return l;
        }
        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
