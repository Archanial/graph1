using graph1;
using static System.Console;

var graph = new[,]
{
    { 0, 1, 0, 0, 1 },
    { 1, 0, 1, 0, 1 },
    { 0, 1, 0, 1, 0 },
    { 0, 0, 1, 0, 1 },
    { 1, 1, 0, 1, 0 }
};

const int k = 3;
var ga = new Coloring(graph, k);
var solution = ga.Solve();
if (solution != null)
{
    WriteLine("Graph colored successfully with " + k + " colors:");
    WriteLine($"Combination: {string.Join(",", solution)}");
    var colors = ga.ColorGraph(solution);
    for (var index = 0; index < solution.Length; index++)
    {
        WriteLine($"Vertex {index}: color {colors[index]}");
    }
    WriteLine("Saving output as graph.png.");
    Coloring.VisualizeSolutionSkia(graph, colors, "graph.png");
}
else
    WriteLine("No solution found.");
