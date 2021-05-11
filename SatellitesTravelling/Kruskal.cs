using System.Collections.Generic;
using System.Linq;

namespace SatellitesTravelling
{
    public static class Kruskal
    {
        public static List<Edge> Kruskals_MST(List<Edge> edges, List<int> vertices)
        {
            //empty result list
            List<Edge> result = new List<Edge>();

            //making set
            DisjointSet.Set set = new DisjointSet.Set(100);
            foreach (int vertex in vertices)
                set.MakeSet(vertex);

            //sorting the edges order by weight ascending
            var sortedEdge = edges.OrderBy(x => x.Weight).ToList();

            foreach (Edge edge in sortedEdge)
            {
                //adding edge to result if both vertices do not belong to same set
                //both vertices in same set means it can have cycles in tree
                if (set.FindSet(edge.Vertex1) != set.FindSet(edge.Vertex2))
                {
                    result.Add(edge);
                    set.Union(edge.Vertex1, edge.Vertex2);
                }
            }

            return result;
        }
    }
}
