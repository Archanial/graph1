namespace graph1;

public static class GraphParser
{
    public static (int[,], int?) ParseFromFile(string path)
    {
        if(!File.Exists(path))
            throw new Exception("File not found");
        
        var lines = File.ReadAllLines(path);
        int[,] array = null!;
        var k = 0;
        foreach (var line in lines)
        {
            if(string.IsNullOrWhiteSpace(line))
                continue;

            string[] split;
            switch (line[0])
            {
                case 'c':
                    continue;
                case 'p':
                    split = line.Split(' ');
                    var numVertices = int.Parse(split[2]);
                    array = new int[numVertices, numVertices];
                    break;
                case 'e':
                    split = line.Split(' ');
                    array[int.Parse(split[1]) - 1, int.Parse(split[2]) - 1] = 1;
                    break;
                case 'k':
                    k = int.Parse(line.Split(' ')[1]);
                    break;
            }
        }

        return (array, k);
    }
}