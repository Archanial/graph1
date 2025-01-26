using SkiaSharp;

namespace graph1;

public class Coloring(int[,] graph, int maxColors)
{
    private readonly int _numVertices = graph.GetLength(0);
    private const int PopulationSize = 100;
    private const int MaxGenerations = 1000;

    public int[] ColorGraph(int[] permutation)
    {
        var colors = new int[_numVertices];
        Array.Fill(colors, -1);

        foreach (var vertex in permutation)
        {
            var availableColors = new bool[maxColors];
            Array.Fill(availableColors, true);
            for (var neighbor = 0; neighbor < _numVertices; neighbor++)
            {
                if (graph[vertex, neighbor] == 1 && colors[neighbor] != -1)
                {
                    availableColors[colors[neighbor]] = false;
                }
            }

            for (var color = 0; color < maxColors; color++)
            {
                if (!availableColors[color])
                    continue;
                
                colors[vertex] = color;
                break;
            }
        }

        return colors;
    }

    private int Fitness(int[] permutation) => ColorGraph(permutation).Count(c => c == -1);

    private int[] Crossover(int[] parent1, int[] parent2)
    {
        var child = new int[_numVertices];
        var taken = new bool[_numVertices]; 
        var rand = new Random();

        // Copy random fragment
        var start = rand.Next(0, _numVertices - 1);
        var end = rand.Next(start + 1, _numVertices);

        // Copy segment from parent1, -1 is unitialized
        Array.Fill(child, -1);
        for (var i = start; i < end; i++)
        {
            child[i] = parent1[i];
            taken[parent1[i]] = true;
        }

        // Fill rest
        var currentIndex = 0;
        for (var i = 0; i < _numVertices; i++)
        {
            if (child[i] != -1)
                continue;

            while (taken[parent2[currentIndex]])
            {
                currentIndex++;
                if (currentIndex >= _numVertices) 
                    throw new IndexOutOfRangeException("Parent2 traversal exceeded bounds!"); // Safety check
            }

            child[i] = parent2[currentIndex];
            currentIndex++;
        }

        return child;
    }

    private void ScrambleMutation(int[] individual)
    {
        var rand = new Random();
        var start = rand.Next(0, _numVertices);
        var end = rand.Next(start, _numVertices);
        Array.Reverse(individual, start, end - start);
    }

    public int[]? Solve()
    {
        var rand = new Random();
        var population = new List<int[]>();
        for (var i = 0; i < PopulationSize; i++)
        {
            var perm = Enumerable.Range(0, _numVertices).OrderBy(_ => rand.Next()).ToArray();
            population.Add(perm);
        }

        for (var generation = 0; generation < MaxGenerations; generation++)
        {
            // Sort by fitness
            population.Sort((a, b) => Fitness(a).CompareTo(Fitness(b)));

            // Did we solve?
            if (Fitness(population[0]) == 0)
            {
                return population[0];
            }

            // Creating new population with the best performers of last one
            List<int[]> newPopulation =
            [
                population[0],
                population[1]
            ];
            
            // Crossing
            while (newPopulation.Count < PopulationSize)
            {
                var parent1 = population[rand.Next(0, PopulationSize)];
                var parent2 = population[rand.Next(0, PopulationSize)];
                var child = Crossover(parent1, parent2);
                newPopulation.Add(child);
            }

            // Mutation
            foreach (var individual in newPopulation)
            {
                // 10% chance for mutation
                if (rand.NextDouble() < 0.1)
                {
                    ScrambleMutation(individual);
                }
            }

            population = newPopulation;
        }

        return null;
    }
    
    public static void VisualizeSolutionSkia(int[,] graph, int[] colors, string outputPath, int imageSize = 500)
    {
        var numVertices = graph.GetLength(0);
        const float radius = 20;
        var center = (float)imageSize / 2;
        var circleRadius = (float)imageSize / 3;
        var positions = new SKPoint[numVertices];
        for (var i = 0; i < numVertices; i++)
        {
            var angle = (float)(2 * Math.PI * i / numVertices);
            positions[i] = new SKPoint(
                center + circleRadius * (float)Math.Cos(angle),
                center + circleRadius * (float)Math.Sin(angle)
            );
        }

        var colorPalette = new[]
        {
            SKColors.Red, SKColors.Blue, SKColors.Green, SKColors.Orange, SKColors.Purple, 
            SKColors.Yellow, SKColors.Pink, SKColors.Teal, SKColors.Brown, SKColors.Gray
        };

        var font = new SKFont
        {
            Size = 14
        };

        using var bitmap = new SKBitmap(imageSize, imageSize);
        using var canvas = new SKCanvas(bitmap);
        using var edgePaint = new SKPaint();
        edgePaint.Color = SKColors.Black;
        edgePaint.StrokeWidth = 2;
        edgePaint.IsAntialias = true;
        using var nodePaint = new SKPaint();
        nodePaint.IsAntialias = true;
        using var textPaint = new SKPaint();
        textPaint.Color = SKColors.White;
        textPaint.IsAntialias = true;
        canvas.Clear(SKColors.White);
        for (var i = 0; i < numVertices; i++)
        {
            for (var j = i + 1; j < numVertices; j++)
            {
                if (graph[i, j] == 1)
                {
                    canvas.DrawLine(positions[i], positions[j], edgePaint);
                }
            }
        }

        for (var i = 0; i < numVertices; i++)
        {
            nodePaint.Color = colors[i] >= 0 ? colorPalette[colors[i] % colorPalette.Length] : SKColors.Gray;
            canvas.DrawCircle(positions[i], radius, nodePaint);
            canvas.DrawText(i.ToString(), positions[i].X, positions[i].Y + 5, SKTextAlign.Center, font, textPaint);
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes(outputPath, data.ToArray());
    }
}