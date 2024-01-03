using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            HelperMethods.delete_duplicte(ref points); // pre processing to delete duplicate points

            if (points.Count == 1)
            {
                outPoints.Add(points[0]);
                return;
            }
            /************ algorithm *************/
            int len = points.Count;
            bool[] visited = new bool[len];
            Parallel.For(0, len, index => visited[index] = true); // memset(visited, 0, sizeof visited)

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    for (int k = 0; k < len; k++)
                    {
                        for (int l = 0; l < len; l++)
                        {
                            if (l != i && l != j && l != k && HelperMethods.PointInTriangle(points[l], points[i], points[j], points[k]) != Enums.PointInPolygon.Outside)
                                visited[l] = false;
                        }
                    }
                }
            }

            // check if point colinear in segment delete it
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
                if (visited[i])
                    outPoints.Add(points[i]);
            }

        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
