using System.Collections.Generic;
using System.Linq;

using GeoCoordinatePortable;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using QuickGraph;
using QuickGraph.Algorithms.Search;

using SatellitesTravelling;
using SatellitesTravelling.SatellitesModels;

namespace SatelliteUnitTests
{
    [TestClass]
    public class TravellingSatteliteUnitTest
    {
        private List<int> vertex;

        [TestInitialize]
        public void InitializeTest() => vertex = new List<int>();

        [TestMethod]
        public void AlgorithmTest()
        {
            var satellites = SatelliteRepository.GetSatellites().ToList();
            var edges = PopulateEdgesFromSatellites(satellites);

            List<int> vertices = satellites.Select(s => s.Id).ToList();
            List<Edge> MinimumSpanningTree = Kruskal.Kruskals_MST(edges, vertices);

            UndirectedGraph<int, UndirectedEdge<int>> graph = CreateGraphFromTree(vertices, MinimumSpanningTree);
            RunDFS(graph);

            CollectionAssert.AreEquivalent(new List<int> { 0, 1, 3, 2, 4 }, vertices);
        }

        private List<Edge> PopulateEdgesFromSatellites(List<Satellite> satellites)
        {
            List<Edge> edges = new();

            foreach (var s1 in satellites)
            {
                foreach (var s2 in satellites.Except(new List<Satellite> { s1 }))
                {
                    GeoCoordinate g1 = new(s1.Latitude, s1.Longitude, s1.Altitude);
                    GeoCoordinate g2 = new(s2.Latitude, s2.Longitude, s2.Altitude);

                    //On test si la liste continent déja cet arc 
                    if (!edges.Any(e => e.Vertex1 == s2.Id && e.Vertex2 == s1.Id && e.Weight == g1.GetDistanceTo(g2)))
                        edges.Add(new Edge { Vertex1 = s1.Id, Vertex2 = s2.Id, Weight = g1.GetDistanceTo(g2) });
                }
            }

            return edges;
        }

        private UndirectedGraph<int, UndirectedEdge<int>> CreateGraphFromTree(List<int> vertices, List<Edge> MinimumSpanningTree)
        {
            var graph = new UndirectedGraph<int, UndirectedEdge<int>>();

            vertices.ForEach(v => graph.AddVertex(v));
            MinimumSpanningTree.ForEach(e => graph.AddEdge(new UndirectedEdge<int>(e.Vertex1, e.Vertex2)));

            return graph;
        }

        private void RunDFS(UndirectedGraph<int, UndirectedEdge<int>> graph)
        {
            var dfs = new UndirectedDepthFirstSearchAlgorithm<int, UndirectedEdge<int>>(graph);
            dfs.TreeEdge += (s, e) => vertex.Add(e.Target);
            dfs.Compute(0);
        }
    }
}
