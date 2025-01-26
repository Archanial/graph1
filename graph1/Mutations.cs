namespace graph1;

public abstract class Mutations
{
    public static void InverseMutation(int[] individual)
    {
        var rand = new Random();
        var start = rand.Next(0, individual.Length);
        var end = rand.Next(start, individual.Length);
        Array.Reverse(individual, start, end - start);
    }
    
    public static void ScrambleMutation(int[] individual)
    {
        var rand = new Random();
        var start = rand.Next(0, individual.Length);
        var end = rand.Next(start, individual.Length);
        var subArray = individual[start..end];
        for (var i = subArray.Length - 1; i > 0; i--)
        {
            var j = rand.Next(0, i + 1);
            (subArray[i], subArray[j]) = (subArray[j], subArray[i]);
        }
        Array.Copy(subArray, 0, individual, start, subArray.Length);
    }
}