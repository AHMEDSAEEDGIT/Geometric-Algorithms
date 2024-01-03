using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {

        
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            HelperMethods.delete_duplicte(ref points);

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

            Point start = points[lst[0].Item3]; // min point 

            /******* sort input using angle ********/
            List<Tuple<double, double, double, int>> sorted = new List<Tuple<double, double, double, int>>();

            for (int i = 0; i < points.Count; i++)
            {
                double angle = HelperMethods.calculateAngle_2(start, points[i]);
                sorted.Add(new Tuple<double, double, double, int>(angle, points[i].X, points[i].Y, i));
            }

            sorted.Sort();

            /****************************************/

            /**** algorithm ****/
            Stack<Point> st = new Stack<Point>();
            int len = points.Count, idx = 2;

            st.Push(start);
            st.Push(points[sorted[1].Item4]);

            while (idx < len)
            {
                Point p = st.Peek();
                st.Pop();

                Point p2 = st.Peek();
                st.Push(p);

                Point pi = points[sorted[idx].Item4];

                if (HelperMethods.CheckTurn(new Line(p2, p), pi) == Enums.TurnType.Left)
                {
                    st.Push(pi);
                    idx++;
                }
                else if (HelperMethods.CheckTurn(new Line(p, p2), pi) == Enums.TurnType.Colinear)
                {
                    st.Pop();
                    st.Push(pi);
                    idx++;
                }
                else
                {
                    st.Pop();
                }
            }

            while (st.Count != 0)
            {
                outPoints.Add(st.Peek());
                st.Pop();
            }

        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
