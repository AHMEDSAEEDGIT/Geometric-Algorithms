using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;
using CGUtilities.DataStructures;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    public class SweepLine : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            Sweep(lines, ref outPoints);
            HelperMethods.delete_duplicte(ref outPoints);
        }
        public class evnt
        {
            public Point p;
            public int type;
            public Line seg;
            public evnt(Point p, int t, Line s)
            {
                this.p = p;
                this.type = t;
                this.seg = s;
            }
        }
        public int cmp(evnt a, evnt b)
        {
            if (a.p.X < b.p.X - Constants.Epsilon)
                return -1;
            else if (a.p.X > b.p.X + Constants.Epsilon)
                return 1;
            else if (a.type > b.type)
                return -1;
            else if (a.type < b.type)
                return 1;
            else if (a.p.Y < b.p.Y - Constants.Epsilon)
                return -1;
            else if (a.p.Y > b.p.Y + Constants.Epsilon)
                return 1;
            else if (a.type == 0)
            {
                if (a.seg.End.X < b.seg.End.X - Constants.Epsilon)
                    return -1;
                else if (a.seg.End.X > b.seg.End.X + Constants.Epsilon)
                    return 1;
                else
                    return a.seg.End.Y.CompareTo(b.seg.End.Y);
            }
            else if (a.type == 1)
            {
                if (a.seg.Start.X < b.seg.Start.X - Constants.Epsilon)
                    return -1;
                else if (a.seg.Start.X > b.seg.Start.X + Constants.Epsilon)
                    return 1;
                else
                    return a.seg.Start.Y.CompareTo(b.seg.Start.Y);
            }
            else return 0;
        }
        public int lineCmp(Line a, Line b)
        {
            double x = Math.Max(a.Start.X, b.Start.X);
            double y1 = (a.End.Y - a.Start.Y) * (x - a.Start.X) / (a.End.X - a.Start.X) + a.Start.Y;
            double y2 = (b.End.Y - b.Start.Y) * (x - b.Start.X) / (b.End.X - b.Start.X) + b.Start.Y;
            if (y1 < y2 - Constants.Epsilon)
                return -1;
            else if (y1 > y2 + Constants.Epsilon)
                return 1;
            else
            {
                x = Math.Min(a.End.X, b.End.X);
                y1 = (a.End.Y - a.Start.Y) * (x - a.Start.X) / (a.End.X - a.Start.X) + a.Start.Y;
                y2 = (b.End.Y - b.Start.Y) * (x - b.Start.X) / (b.End.X - b.Start.X) + a.Start.Y;
                if (y1 < y2 - Constants.Epsilon)
                    return -1;
                else if (y1 > y2 + Constants.Epsilon)
                    return 1;
                else
                    return 0;
            }
        }
        public void handleEvent(Line seg1, Line seg2, Point x, OrderedSet<evnt> events)
        {
            // remove old ending
            events.Remove(new evnt(seg1.End, 1, seg1));
            events.Remove(new evnt(seg2.End, 1, seg2));
            // add new segments
            Line s1 = new Line((Point)x.Clone(), (Point)seg1.End.Clone());
            Line s2 = new Line((Point)x.Clone(), (Point)seg2.End.Clone());
            events.Add(new evnt(s1.Start, 0, s1));
            events.Add(new evnt(s2.Start, 0, s2));
            events.Add(new evnt(s1.End, 1, s1));
            events.Add(new evnt(s2.End, 1, s2));
            seg1.End = (Point)x.Clone();
            seg2.End = (Point)x.Clone();
            events.Add(new evnt(seg1.End, 1, seg1));
            events.Add(new evnt(seg2.End, 1, seg2));
        }
        public void Sweep(List<CGUtilities.Line> lines, ref List<CGUtilities.Point> Points)
        {
            OrderedSet<evnt> events = new OrderedSet<evnt>(cmp);
            OrderedSet<Line> sweep = new OrderedSet<Line>(lineCmp);
            foreach (Line seg in lines)
            {
                if (seg.Start.X > seg.End.X)
                {
                    Point tmp = seg.Start;
                    seg.Start = seg.End;
                    seg.End = tmp;
                }
                else if (seg.Start.X == seg.End.X)
                {
                    seg.Start.X -= 2 * Constants.Epsilon;
                    seg.End.X += 2 * Constants.Epsilon;
                }
                events.Add(new evnt(seg.Start, 0, seg));
                events.Add(new evnt(seg.End, 1, seg));
            }
            while (events.Count != 0)
            {
                var first = events.First();
                events.RemoveFirst();
                int type = first.type;
                Line item = first.seg;
                if (type == 0)
                {
                    sweep.Add(item);
                    var idx = sweep.IndexOf(item);
                    Point x = new Point(0, 0);
                    var i = idx - 1;
                    while (i >= 0)
                    {
                        if (!sweep[i].Start.Equals(item.Start) && HelperMethods.intersect(sweep[i], item, ref x))
                        {
                            Points.Add((Point)x.Clone());
                            handleEvent(sweep[i], item, (Point)x.Clone(), events);
                        }
                        if (!sweep[i].Start.Equals(item.Start)) break;
                        i--;
                    }
                    i = idx + 1;
                    while (i < sweep.Count)
                    {
                        if (!sweep[i].Start.Equals(item.Start) && HelperMethods.intersect(item, sweep[i], ref x))
                        {
                            Points.Add((Point)x.Clone());
                            handleEvent(sweep[i], item, (Point)x.Clone(), events);
                            break;
                        }
                        if (!sweep[i].Start.Equals(item.Start)) break;
                        i++;
                    }
                }
                else if (type == 1)
                {
                    var idx = sweep.IndexOf(item);
                    Point x = new Point(0, 0);
                    if (idx - 1 >= 0 && idx + 1 < sweep.Count)
                    {
                        if (!sweep[idx - 1].Start.Equals(sweep[idx + 1].Start) && HelperMethods.intersect(sweep[idx - 1], sweep[idx + 1], ref x))
                        {
                            Points.Add((Point)x.Clone());
                            handleEvent(sweep[idx - 1], sweep[idx + 1], (Point)x.Clone(), events);
                        }
                    }
                    sweep.Remove(item);
                }
            }
        }
        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}
