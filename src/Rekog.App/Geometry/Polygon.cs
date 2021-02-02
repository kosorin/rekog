using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.App.Geometry
{
    public class Polygon
    {
        public Polygon(IList<Point> vertices)
        {
            if (vertices.Count == 0)
            {
                throw new ArgumentException("Vertex list is empty.", nameof(vertices));
            }

            var isClosed = vertices.First() == vertices.Last();

            var minimumVertices = isClosed ? 4 : 3;
            if (vertices.Count < minimumVertices)
            {
                throw new ArgumentException($"Polygon must have at least {minimumVertices} vertices.", nameof(vertices));
            }

            Vertices = new List<Point>(vertices.Count + (isClosed ? 0 : 1));
            Vertices.AddRange(vertices);
            if (!isClosed)
            {
                Vertices.Add(vertices.First());
            }

            Edges = GetEdges(Vertices);

            for (var i = 0; i < Edges.Count; i++)
            {
                for (var j = 0; j < Edges.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    var edge1 = Edges[i];
                    var edge2 = Edges[j];
                    if (GetIntersections(edge1, edge2) is { Count: > 0 })
                    {
                        throw new ArgumentException("Polygon must be simple (a simple polygon is one which does not intersect itself).", nameof(vertices));
                    }
                }
            }
        }

        public List<Point> Vertices { get; }

        public List<LineSegment> Edges { get; }

        public List<Polygon> Union(Polygon polygon)
        {
            return UnionHelper.Union(this, polygon);
        }

        private static List<LineSegment> GetEdges(IList<Point> vertices)
        {
            var edges = new List<LineSegment>(vertices.Count);
            for (var i = 1; i < vertices.Count; i++)
            {
                var first = vertices[i - 1];
                var second = vertices[i];
                if (first == second)
                {
                    continue;
                }
                edges.Add(new LineSegment(first, second));
            }
            return edges;
        }

        private static Dictionary<Point, HashSet<Point>> CreateGraph(List<LineSegment> edges)
        {
            var graph = GetVertices(edges).ToDictionary(vertex => vertex, _ => new HashSet<Point>());
            foreach (var edge in edges.Where(edge => edge.First != edge.Second))
            {
                graph[edge.First].Add(edge.Second);
                graph[edge.Second].Add(edge.First);
            }
            return graph;
        }

        private static List<Point> GetVertices(IList<LineSegment> edges)
        {
            return edges
                .SelectMany(edge => new[] { edge.First, edge.Second })
                .Distinct()
                .ToList();
        }

        private static List<Point> GetIntersections(LineSegment edge1, LineSegment edge2)
        {
            return edge1.GetIntersections(edge2)
                .Where(vertex => !IsVertexIntersection(vertex, edge1, edge2))
                .ToList();
        }

        private static bool IsVertexIntersection(Point vertex, LineSegment edge1, LineSegment edge2)
        {
            return (edge1.First == vertex || edge1.Second == vertex) && (edge2.First == vertex || edge2.Second == vertex);
        }

        private class UnionHelper
        {
            public static List<Polygon> Union(Polygon polygon1, Polygon polygon2)
            {
                var graph = BuildGraph(polygon1, polygon2);
                if (graph == null)
                {
                    var outer1 = FindOuterPolygon(polygon1, polygon2);
                    var outer2 = FindOuterPolygon(polygon2, polygon1);
                    if (outer1 == outer2)
                    {
                        return new List<Polygon> { outer1 };
                    }
                    else
                    {
                        return new List<Polygon> { polygon1, polygon2 };
                    }
                }
                return new List<Polygon> { ConstructPolygon(graph) };
            }

            private static Dictionary<Point, HashSet<Point>>? BuildGraph(Polygon polygon1, Polygon polygon2)
            {
                var graphEdges = new List<LineSegment>();

                var edges1 = new Queue<LineSegment>(polygon1.Edges);
                var edges2 = new List<LineSegment>(polygon2.Edges);

                var anyPolygonIntersection = false;

                while (edges2.Any() && edges1.TryDequeue(out var edge1))
                {
                    var anyEdgeIntersection = false;

                    for (var i = 0; i < edges2.Count; i++)
                    {
                        var edge2 = edges2[i];

                        var intersections = edge1.GetIntersections(edge2);
                        var edgeIntersections = intersections.Where(vertex => !IsVertexIntersection(vertex, edge1, edge2)).ToList();
                        var vertexIntersectionCount = intersections.Length - edgeIntersections.Count;
                        if (edgeIntersections.Count > 0)
                        {
                            edges2.RemoveAt(i);

                            SplitEdge(edge1, edgeIntersections).ForEach(edges1.Enqueue);
                            SplitEdge(edge2, edgeIntersections).ForEach(edges2.Add);

                            anyPolygonIntersection = true;
                            anyEdgeIntersection = true;
                            break;
                        }
                        else if (vertexIntersectionCount > 0)
                        {
                            anyPolygonIntersection = true;
                        }
                    }

                    if (!anyEdgeIntersection)
                    {
                        graphEdges.Add(edge1);
                    }
                }

                if (!anyPolygonIntersection)
                {
                    return null;
                }

                graphEdges = graphEdges
                    .Concat(edges1)
                    .Concat(edges2)
                    .ToList();

                return CreateGraph(graphEdges);
            }

            private static List<LineSegment> SplitEdge(LineSegment edge, List<Point> vertices)
            {
                if (vertices.Any())
                {
                    var newVertices = new List<Point>(vertices.Count + 2)
                    {
                        edge.First
                    };
                    newVertices.AddRange(vertices.OrderBy(x => x.DistanceTo(edge.First)));
                    newVertices.Add(edge.Second);
                    return GetEdges(newVertices);
                }
                else
                {
                    return new() { edge };
                }
            }

            private static Polygon FindOuterPolygon(Polygon polygon1, Polygon polygon2)
            {
                // We already know that there is no intersection
                // So now just find outer polygon

                if (polygon1.Vertices.Max(p => p.X) < polygon2.Vertices.Min(p => p.X))
                {
                    return polygon1;
                }
                else if (polygon2.Vertices.Max(p => p.X) < polygon1.Vertices.Min(p => p.X))
                {
                    return polygon1;
                }

                var maxX = polygon1.Vertices.Max(p => p.X) + 1;

                foreach (var vertex in polygon2.Vertices)
                {
                    var ray = new LineSegment(vertex, new Point(maxX, vertex.Y));
                    var intersectionCount = polygon1.Edges
                        .Where(edge => Math.Min(edge.First.Y, edge.Second.Y) <= vertex.Y && vertex.Y <= Math.Max(edge.First.Y, edge.Second.Y))
                        .Sum(edge => ray.GetIntersections(edge).Length);
                    if ((intersectionCount % 2) == 1)
                    {
                        return polygon1;
                    }
                }

                return polygon2;
            }

            private static Polygon ConstructPolygon(Dictionary<Point, HashSet<Point>> graph)
            {
                var vertices = new List<Point>();

                var start = graph.Keys
                    .OrderBy(vertex => vertex.X)
                    .ThenBy(vertex => vertex.Y)
                    .First();

                var previous = start - new Point(1, 0);
                var current = start;
                vertices.Add(current);

                do
                {
                    var previousAngle = Math.Atan2(previous.Y - current.Y, previous.X - current.X);
                    var next = graph[current]
                        .Where(vertex => vertex != previous)
                        .OrderBy(vertex =>
                        {
                            var nextAngle = Math.Atan2(vertex.Y - current.Y, vertex.X - current.X);
                            return (previousAngle - nextAngle + (2 * Math.PI)) % (2 * Math.PI);
                        })
                        .First();
                    previous = current;
                    current = next;
                    vertices.Add(current);
                } while (current != start);

                return new Polygon(vertices);
            }
        }
    }
}
