namespace HashTable.Tester;

public abstract class HashTableTester
{
    public static (List<string>, List<string>) GeneratingValuesAndKeys(int sizeHash = 100000)
    {
        var start = Enumerable.Range(0, sizeHash).ToArray();
        var randomKeys = new Random();
        var randomValues = new Random();

        var keys = new List<string>();
        var values = new List<string>();
        
        for (var i = 0; i < sizeHash; i++)
            keys.Add($"{i}{start[i]}{randomKeys.Next()}");
        
        for (var j = 0; j < sizeHash; j++)
        {
            var s = "";
            for (var i = 0; i < 5; i++)
            {
                var a = (char)randomValues.Next(0, 255);
                s += a;

            }
            values.Add(s);
        }

        return (keys, values);
    }
}