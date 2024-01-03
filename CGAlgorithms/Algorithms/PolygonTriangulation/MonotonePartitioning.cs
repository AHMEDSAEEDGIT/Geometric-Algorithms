using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities.DataStructures;
using CGUtilities;


namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotonePartitioning : Algorithm
    {
        public class Segment
        {
            public Point v;
            public int idx;

            public Segment(int i, Point p)
            {
                this.v = p;
                this.idx = i;

            }

            public Segment()
            {

            }
        }

        public class Event
        {
            public int idx;
            public Point v;
            public Enums.vertex_type type;
            public Segment edge;

            public Event(int i, Point P, Enums.vertex_type points_type, Segment s)
            {
                this.v = P;
                this.type = points_type;
                this.edge = s;
                this.idx = i;
            }
           
            public Event()
            {

            }
        }
        // start_data
        OrderedSet<Event> T = new OrderedSet<Event>(
               (a, b) =>
               {
                   return a.edge.v.X.CompareTo(b.edge.v.X);
               });
        OrderedSet<Event> Q = new OrderedSet<Event>(
                (a, b) =>
                {
                    if (a.v.Y == b.v.Y) return a.v.X.CompareTo(b.v.X);
                    return -a.v.Y.CompareTo(b.v.Y);
                });
        Dictionary<int, Event> helper = new Dictionary<int, Event>();
        Dictionary<int, Event> data = new Dictionary<int, Event>();
        Polygon P = new Polygon();
        List<Enums.vertex_type> points_type = new List<Enums.vertex_type>();
        // end_data

        void pre_processing(ref List<CGUtilities.Polygon> polygons)
        {
            P = polygonCounterClockwise(polygons[0]);
            get_type(P);

            for (int i = 0; i < P.lines.Count; i++)
                Q.Add(new Event(i, P.lines[i].Start, points_type[i], new Segment(i, P.lines[i].Start)));

            data.Add(0, new Event(P.lines.Count - 1, P.lines[P.lines.Count - 1].Start, points_type[P.lines.Count - 1], new Segment(P.lines.Count - 1, P.lines[P.lines.Count - 1].Start)));

            for (int i = 1; i < P.lines.Count; i++)
                data.Add(i, new Event(i - 1, P.lines[i - 1].Start, points_type[i - 1], new Segment(i - 1, P.lines[i - 1].Start)));
        }

        void handle_start(Event ev)
        {
            T.Add(ev);
            helper.Add(ev.edge.idx, ev);
        }

        void handle_split(Event ev, ref List<CGUtilities.Line> outLines)
        {
            Event val = null;
            foreach (var j in T)
                if (intersectsWithSweepLine(P, ev, j) && left(P, ev, j))
                    if (val == null)
                        val = j;
                    else if (right(P, ev, j, val))
                        val = j;          

            outLines.Add(new Line(ev.v, helper[val.edge.idx].v));
            helper[val.edge.idx] = ev;
            T.Add(ev);
            helper[ev.edge.idx] = ev;
        }

        void handle_end(Event ev, ref List<CGUtilities.Line> outLines)
        {
            if (helper[ev.edge.idx - 1].type == Enums.vertex_type.merge)
                outLines.Add(new Line(ev.v, helper[ev.edge.idx - 1].v));
            T.Remove(helper[ev.edge.idx - 1]);
        }

        void handle_merge(Event ev, ref List<CGUtilities.Line> outLines)
        {
            if (helper[ev.edge.idx - 1].type == Enums.vertex_type.merge)
                outLines.Add(new Line(ev.v, helper[ev.edge.idx - 1].v));
            T.Remove(data[ev.idx]);

            Event evo = null;
            foreach (var j in T)
                if (intersectsWithSweepLine(P, ev, j) && left(P, ev, j))
                    if (evo == null)
                        evo = j;

                    else if (right(P, ev, j, evo))
                        evo = j;                

            if (helper[evo.edge.idx].type == Enums.vertex_type.merge)
                outLines.Add(new Line(ev.v, helper[evo.edge.idx].v));
            
            helper[evo.edge.idx] = ev;
        }

        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            pre_processing(ref polygons);
     
            while (Q.Count > 0)
            {
                Event ev = Q.First();
                if (ev.type == Enums.vertex_type.start)
                    handle_start(ev);                    
                
                else if (ev.type == Enums.vertex_type.split)
                    handle_split(ev, ref outLines);

                else if (ev.type == Enums.vertex_type.end)
                    handle_end(ev, ref outLines);
                    
                else if (ev.type == Enums.vertex_type.merge)
                    handle_merge(ev, ref outLines);
                
                else
                    if (P.lines[((ev.idx - 1) + P.lines.Count) % P.lines.Count].Start.Y <= ev.v.Y)
                    {
                        Event evo = null;
                        foreach (Event j in T)
                        {
                            if (intersectsWithSweepLine(P, ev, j) && left(P, ev, j))
                            {
                                if (evo == null)
                                    evo = j;
                                
                                else if (right(P, ev, j, evo))
                                    evo = j;                                
                            }
                        }
                        
                        if (helper[evo.edge.idx].type == Enums.vertex_type.merge)
                            outLines.Add(new Line(ev.v, helper[evo.edge.idx].v));
                        
                        helper[evo.edge.idx] = ev;
                    }
                    else
                    {
                        if (helper[ev.edge.idx - 1].type == Enums.vertex_type.merge)
                            outLines.Add(new Line(ev.v, helper[ev.edge.idx - 1].v));

                        T.Remove(data[ev.idx]);
                        helper.Add(ev.edge.idx, ev);
                        T.Add(ev);                       
                    }
                
                Q.Remove(ev);
            }
        }

       void swap(ref Point p1, ref Point p2)
        {

        }
        public Polygon polygonCounterClockwise(Polygon polygons)
        {
            double all = 0, n = polygons.lines.Count;
            for (int i = 0; i < n; i++)
                all = all + ((polygons.lines[i].End.X - polygons.lines[i].Start.X) * (polygons.lines[i].End.Y + polygons.lines[i].Start.Y));
            if (all <= 0)
                return polygons;
           
            polygons.lines.Reverse();
            for (int i = 0; i < n; i++)
            {
                Point temp = polygons.lines[i].Start;
                polygons.lines[i].Start = polygons.lines[i].End;
                polygons.lines[i].End = temp;
            }
            
            return polygons;
        }
        void get_type(Polygon p)
        {
            int n = p.lines.Count;
            for (int i = 0; i < n; i++)
            {
                Point next = p.lines[(i + 1) % p.lines.Count].Start, C = p.lines[i].Start, prev = p.lines[((i - 1) + p.lines.Count) % p.lines.Count].Start;

                if (next.Y < C.Y && prev.Y < C.Y && OK(p, i))
                    points_type.Add(Enums.vertex_type.start);
                else if (next.Y < C.Y && prev.Y < C.Y && !OK(p, i))
                    points_type.Add(Enums.vertex_type.split);
                else if (next.Y > C.Y && prev.Y > C.Y && OK(p, i))
                    points_type.Add(Enums.vertex_type.end);
                else if (next.Y > C.Y && prev.Y > C.Y && !OK(p, i))
                    points_type.Add(Enums.vertex_type.merge);
                else
                    points_type.Add(Enums.vertex_type.regular);
            } 
        }

        public bool intersectsWithSweepLine(Polygon pol, Event e, Event j)
        {
            return (e.v.Y >= j.v.Y && e.v.Y <= (pol.lines[(j.idx + 1) % pol.lines.Count()].Start).Y) || (e.v.Y >= (pol.lines[(j.idx + 1) % pol.lines.Count()].Start).Y && e.v.Y <= j.v.Y);
        }
       
      
        public bool OK(Polygon pol, int Cur)
        {
            int next = (Cur + 1) % pol.lines.Count;
            int prev = ((Cur - 1) + pol.lines.Count) % pol.lines.Count;

            Point a = pol.lines[prev].Start, c = pol.lines[next].Start, b = pol.lines[Cur].Start;
           

            return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y) > 0;
        }

        public bool right(Polygon pol, Event e, Event evo, Event ev)
        {

            Point p1 = evo.v, p2 = pol.lines[(evo.idx + 1) % pol.lines.Count()].Start;

            double a = p1.Y - p2.Y, c = p1.X * p2.Y - p2.X * p1.Y, b = p2.X - p1.X;
            double x = (-c - b * e.v.Y) / a;

            p2 = pol.lines[(evo.idx + 1) % pol.lines.Count()].Start;
            p1 = ev.v;
            b = p2.X - p1.X;
            c = p1.X * p2.Y - p2.X * p1.Y;

            return x > (-c - b * e.v.Y) / (p1.Y - p2.Y);
        }

        public bool left(Polygon pol, Event e, Event j)
        {
            Point p1 = j.v, p2 = pol.lines[(j.idx + 1) % pol.lines.Count()].Start;
            double a = p1.Y - p2.Y, c = p1.X * p2.Y - p2.X * p1.Y, b = p2.X - p1.X;

            return (-c - b * e.v.Y) / a < e.v.X;
        }

        public override string ToString()
        {
            return "Monotone Partitioning";
        }
    }
}
