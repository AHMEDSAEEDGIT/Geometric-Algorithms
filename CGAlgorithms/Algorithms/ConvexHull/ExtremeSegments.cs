using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            HelperMethods.delete_duplicte(ref points); // pre processing to delete duplicate points

            if (points.Count == 1)
            {
                outPoints.Add(points[0]);
                return;
            }

            int len = points.Count;
            bool[] visited = new bool[len];
            Parallel.For(0, len, index => visited[index] = false); // memset(visited, 0, sizeof visited)

            for (int i = 0; i < len; i++)
            {
                for (int j = i + 1; j < len; j++)
                {
                    int l = 0, r = 0;
                    for (int k = 0; k < len; k++)
                    {
                        if (k != i && k != j)
                        {
                            if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Left)
                                l++;
                            if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Right)
                                r++;
                        }
                    }
                    if (l == 0 || r == 0)
                        visited[i] = visited[j] = true;
                }
            }
            for (int i = 0; i < len; i++)
            {
                for (int j = i + 1; j < len; j++)
                {
                    for (int k = 0; k < len; k++)
                    {
                        if (k != i && k != j && HelperMethods.PointOnSegment(points[k], points[i], points[j]))
                            visited[k] = false;

                    }
                }
            }

            for (int i = 0; i < len; i++)
            {
                if (visited[i] || len == 1)
                    outPoints.Add(points[i]);
            }
        }
        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
