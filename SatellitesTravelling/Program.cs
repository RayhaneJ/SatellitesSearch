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
using Satellites.SatellitesModels;
using System.Threading;

namespace Satellites
{
    class Program
    {
        private static Kml kml;
        private static ConsoleMenu continentsMenu;
        private static ConsoleMenu satellitesMenu;
        private static ConsoleMenu launchStationsMenu;
        private static List<Satellite> satellites = new();
        private static List<int> vertex = new();

        static async Task Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            InitializeMenu(args);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "http://www.opengis.net/kml/2.1");

            using (Stream reader = new FileStream("doc.kml", FileMode.Open))
                kml = (Kml)new XmlSerializer(typeof(Kml)).Deserialize(reader);

            kml.Document.Folder.ForEach(f => continentsMenu.Add(f.Name, () => ShowLaunchStations(f.Name, args)));
            continentsMenu.Show();

            Console.WriteLine($"Path found from {satellites.Where(s => s.Id == vertex.FirstOrDefault()).Select(s => s.Name).FirstOrDefault()} " +
                $"to {satellites.Where(s => s.Id == vertex.LastOrDefault()).Select(s => s.Name).FirstOrDefault()}:");
            Console.Write(satellites.Where(s => s.Id == vertex.FirstOrDefault()).Select(s => s.Name).FirstOrDefault());
            vertex.Skip(1).ToList().ForEach(v => Console.Write($" > {satellites.Where(s => s.Id == v).Select(s => s.Name).FirstOrDefault()}"));

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void ShowLaunchStations(string continent, string[] args)
        {
            var launchStations = kml.Document.Folder.Where(f => f.Name == continent)
                .SelectMany(f => f.Placemark).ToList();

            launchStationsMenu = new ConsoleMenu(args, level: 1)
                           .Configure(config =>
                           {
                               config.Selector = "--> ";
                               config.EnableFilter = false;
                               config.Title = "Select your Launch Station";
                               config.EnableWriteTitle = true;
                               config.EnableBreadcrumb = true;
                           });

            launchStationsMenu.Add("Retour", () => launchStationsMenu.CloseMenu());
            launchStations.Take(20).ToList().ForEach(s => launchStationsMenu.Add(s.Name, () => ShowSatellites(s.Name, args)));
            launchStationsMenu.Show();
        }

        private static void ShowSatellites(string launchStation, string[] args)
        {
            Localisation localisation = kml.Document.Folder
                .SelectMany(f => f.Placemark.Where(p => p.Name == launchStation)
                .Select(p => new Localisation
                {
                    Latitude = Convert.ToDouble(p.Point.Coordinates.Split(',').ElementAt(0)),
                    Longitude = Convert.ToDouble(p.Point.Coordinates.Split(',').ElementAt(1))
                })).FirstOrDefault();

            HttpClient httpClient = new();
            var response = httpClient
                .GetAsync($"https://api.n2yo.com/rest/v1/satellite/above/{localisation.Latitude}/{localisation.Longitude}/0/90/18/&apiKey=QSFW4U-BF8HWV-47A5CR-4OKK")
                .GetAwaiter().GetResult();
            var jsonResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var data = JsonSerializer.Deserialize<Root>(jsonResponse);

            satellitesMenu = new ConsoleMenu(args, level: 1)
                           .Configure(config =>
                           {
                               config.Selector = "--> ";
                               config.EnableFilter = false;
                               config.Title = "Select your Satellites -- [Algorithm with run only for 3 satellites selected]";
                               config.EnableWriteTitle = true;
                               config.EnableBreadcrumb = true;
                           });
            satellitesMenu.Add("Find Shortest Path -- [Select the satellites first]", () => FindShortestPath());

            data.above.Take(20).ToList().ForEach(a => satellitesMenu.Add(a.satname, () => SelectSatellite(a)));

            satellitesMenu.Show();
        }

        private static void FindShortestPath()
        {
            if (satellites.Count > 3)
            {
                CloseAndClearMenus();

                var edges = PopulateEdgesFromSatellites();

                List<int> vertices = satellites.Select(s => s.Id).ToList();
                List<Edge> MinimumSpanningTree = Kruskal.Kruskals_MST(edges, vertices);

                UndirectedGraph<int, UndirectedEdge<int>> graph = CreateGraphFromTree(vertices, MinimumSpanningTree);
                RunDFS(graph);
            }
        }

        private static void CloseAndClearMenus()
        {
            satellitesMenu.CloseMenu();
            launchStationsMenu.CloseMenu();
            continentsMenu.CloseMenu();
            Console.Clear();
        }

        private static void SelectSatellite(Above above)
        {
            var satellite = new Satellite { Name = above.satname, Altitude = above.satalt, Latitude = above.satlat, Longitude = above.satlng };

            if (!satellites.Any(s => s.Name == above.satname))
            {
                satellites.Add(satellite);
                satellitesMenu.CurrentItem.Name = $"{satellitesMenu.CurrentItem.Name} [Selected]";

            }
            else
            {
                satellites.Remove(satellite);
                satellitesMenu.CurrentItem.Name = satellitesMenu.CurrentItem.Name.Replace("[Selected]", string.Empty);
            }
        }

        private static List<Edge> PopulateEdgesFromSatellites()
        {
            int i = 0;
            satellites.ForEach(delegate (Satellite s)
            {
                s.Id = i;
                i++;
            });

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
            vertex.Add(0);
            dfs.Compute(0);
        }

        private static void DfsTreeEdge(object sender, UndirectedEdgeEventArgs<int, UndirectedEdge<int>> e)
            => vertex.Add(e.Target);

        private static void InitializeMenu(string[] args)
        {
            continentsMenu = new ConsoleMenu(args, level: 0)
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
