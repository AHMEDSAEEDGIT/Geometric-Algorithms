using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            HelperMethods.delete_duplicte(ref points); // pre processing to delete duplicate points

            if (points.Count == 1)
            {
                outPoints.Add(points[0]);
                return;
            }

            /****** get min point with respect to Y  ******/

            List<Tuple<double, double, int>> lst = new List<Tuple<double, double, int>>();

            for (int i = 0; i < points.Count; i++)
            {
                lst.Add(new Tuple<double, double, int>(points[i].Y, points[i].X, i));
            }

            lst.Sort();
            /************************************************/

            int len = points.Count;
            bool[] visited = new bool[len];
            Parallel.For(0, len, index => visited[index] = false); // memset(visited, 0, sizeof visited)

            int idx = lst[0].Item3, cur, last = 0;
            Point start = points[idx];
            cur = idx;

            /***** algorithm *****/
            do
            {
                visited[cur] = true;
                outPoints.Add(points[cur]);
                double mn_angle = 512;
                int mn_idx = 0;
                for (int i = 0; i < len; i++)
                {
                    if (cur == i || last == i || (visited[i] && (i != idx)))
                        continue;
                    if (cur == idx)
                    {
                        double angle = HelperMethods.calculateAngle_2(points[cur], points[i]);
                        if (angle < mn_angle)
                        {
                            mn_angle = angle;
                            mn_idx = i;
                        }
                    }
                    else
                    {

                        double angle = HelperMethods.calculateAngle_3(points[cur], points[last], points[i]);

                        if (angle < mn_angle)
                        {
                            mn_angle = angle;
                            mn_idx = i;
                        }
                    }
                }

                last = cur;
                cur = mn_idx;

            }
            while (idx != cur);
        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
