using graph1;
using static System.Console;

// var graph = new[,]
// {
//     { 0, 1, 0, 0, 1 },
//     { 1, 0, 1, 0, 1 },
//     { 0, 1, 0, 1, 0 },
//     { 0, 0, 1, 0, 1 },
//     { 1, 1, 0, 1, 0 }
// };

var parsed = GraphParser.ParseFromFile(@"D:\repos\graph1\anna.col.txt");
parsed.Item2 ??= 10;

//const int k = 3;
var ga = new Coloring(parsed.Item1, parsed.Item2.Value, Crossovers.CrossoverPmx, Mutations.InverseMutation);
var solution = ga.Solve();
if (solution != null)
{
    WriteLine("Graph colored successfully with " + parsed.Item2 + " colors:");
    WriteLine($"Combination: {string.Join(",", solution)}");
    var colors = ga.ColorGraph(solution);
    for (var index = 0; index < solution.Length; index++)
    {
        WriteLine($"Vertex {index}: color {colors[index]}");
    }
    WriteLine("Saving output as graph.png.");
    Coloring.VisualizeSolutionSkia(parsed.Item1, colors, "graph.png", 1500);
}
else
    WriteLine("No solution found.");
