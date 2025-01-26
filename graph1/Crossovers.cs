namespace graph1;

public abstract class Crossovers
{
    public static int[] CrossoverOx(int[] parent1, int[] parent2)
    {
        var child = new int[parent1.Length];
        var taken = new bool[parent1.Length]; 
        var rand = new Random();

        // Copy random fragment
        var start = rand.Next(0, parent1.Length - 1);
        var end = rand.Next(start + 1, parent1.Length);

        // Copy segment from parent1, -1 is uninitialized
        Array.Fill(child, -1);
        for (var i = start; i < end; i++)
        {
            child[i] = parent1[i];
            taken[parent1[i]] = true;
        }

        // Fill rest
        var currentIndex = 0;
        for (var i = 0; i < parent1.Length; i++)
        {
            if (child[i] != -1)
                continue;

            while (taken[parent2[currentIndex]])
            {
                currentIndex++;
                if (currentIndex >= parent1.Length) 
                    throw new IndexOutOfRangeException("Parent2 traversal exceeded bounds!"); // Safety check
            }

            child[i] = parent2[currentIndex];
            currentIndex++;
        }

        return child;
    }
    
    public static int[] CrossoverPmx(int[] parent1, int[] parent2)
    {
        var child = new int[parent1.Length];
        Array.Fill(child, -1);
        var rand = new Random();
        var start = rand.Next(0, parent1.Length - 1);
        var end = rand.Next(start + 1, parent1.Length);
        var mapping = new Dictionary<int, int>();
        for (var i = start; i < end; i++)
        {
            child[i] = parent1[i];
            mapping[parent1[i]] = parent2[i];
        }
    
        for (var i = start; i < end; i++)
        {
            if (!mapping.ContainsKey(parent2[i]))
                continue;
            var val = parent2[i];
            while (mapping.ContainsKey(val))
            {
                val = mapping[val];
            }
            mapping[parent2[i]] = val;
        }
    
        for (var i = 0; i < parent1.Length; i++)
        {
            if (child[i] != -1)
                continue;
            
            var candidate = parent2[i];
            while (mapping.ContainsKey(candidate))
            {
                candidate = mapping[candidate];
            }
            child[i] = candidate;
        }
    
        return child;
    }
}