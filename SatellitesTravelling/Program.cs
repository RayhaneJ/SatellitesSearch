using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using SatellitesTravelling;
using System.Collections.Generic;
using GeoCoordinatePortable;
using QuickGraph.Algorithms.Search;
using QuickGraph;
using System.Data;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Collections;
using SatellitesTravelling.SatellitesModels;
using System.Xml.Serialization;
using SatellitesTravelling.LaunchSitesModels;
using System.IO;
using ConsoleTools;

namespace Satellites
{
    class Program
    {
        private static string fetchSatellitesUrl = "https://api.n2yo.com/rest/v1/satellite/above/41.702/-76.014/0/90/18/&apiKey=QSFW4U-BF8HWV-47A5CR-4OKK";
        private static Kml kml;
        private static ConsoleMenu menu;
        private static ConsoleMenu subMenu;
        private static List<Satellite> satellites;

        static async Task Main(string[] args)
        {
            InitializeMenus(args);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "http://www.opengis.net/kml/2.1");

            using (Stream reader = new FileStream("doc.kml", FileMode.Open))
            {
                kml = (Kml)new XmlSerializer(typeof(Kml)).Deserialize(reader);
            }
            
            kml.Document.Folder.ForEach(f => menu.Add(f.Name, () => ShowLaunchStations(f.Name, args)));
            menu.Show();

            //HttpClient httpClient = new();
            //var response = await httpClient.GetAsync(fetchSatellitesUrl);
            //var jsonResponse = await response.Content.ReadAsStringAsync();
            //var data = JsonSerializer.Deserialize<Root>(jsonResponse);

            //var satellites = SatelliteRepository.GetSatellites().ToList();
            //var edges = PopulateEdgesFromSatellites(satellites);

            //List<int> vertices = satellites.Select(s => s.Id).ToList();
            //List<Edge> MinimumSpanningTree = Kruskal.Kruskals_MST(edges, vertices);

            //UndirectedGraph<int, UndirectedEdge<int>> graph = CreateGraphFromTree(vertices, MinimumSpanningTree);
            //RunDFS(graph);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void ShowLaunchStations(string continent, string[] args)
        {
            var launchStations = kml.Document.Folder.Where(f => f.Name == continent)
                .SelectMany(f => f.Placemark).ToList();

            var subMenu = new ConsoleMenu(args, level: 1)
                           .Configure(config =>
                           {
                               config.Selector = "--> ";
                               config.EnableFilter = false;
                               config.Title = "Select your Launch Station";
                               config.EnableWriteTitle = true;
                               config.EnableBreadcrumb = true;
                           });

            subMenu.Add("Retour", () => subMenu.CloseMenu());
            launchStations.ForEach(s => subMenu.Add(s.Name, () => Test()));
            subMenu.Show();
        }

        public void Run()
        {

        }

        public void ShowSatellites(string launchStation)

        private static List<Edge> PopulateEdgesFromSatellites(List<Satellite> satellites)
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

        private static UndirectedGraph<int, UndirectedEdge<int>> CreateGraphFromTree(List<int> vertices, List<Edge> MinimumSpanningTree)
        {
            var graph = new UndirectedGraph<int, UndirectedEdge<int>>();

            vertices.ForEach(v => graph.AddVertex(v));
            MinimumSpanningTree.ForEach(e => graph.AddEdge(new UndirectedEdge<int>(e.Vertex1, e.Vertex2)));

            return graph;
        }

        private static void RunDFS(UndirectedGraph<int, UndirectedEdge<int>> graph)
        {
            var dfs = new UndirectedDepthFirstSearchAlgorithm<int, UndirectedEdge<int>>(graph);
            dfs.TreeEdge += DfsTreeEdge;
            dfs.Compute(0);
        }

        private static void DfsTreeEdge(object sender, UndirectedEdgeEventArgs<int, UndirectedEdge<int>> e)
        {

        }

        private static void InitializeMenus(string[] args)
        {
            menu = new ConsoleMenu(args, level: 0)
                           .Configure(config =>
                           {
                               config.Selector = "--> ";
                               config.EnableFilter = true;
                               config.Title = "Select a Geographical zone";
                               config.EnableWriteTitle = true;
                               config.EnableBreadcrumb = true;
                           });

        }
    }
}
