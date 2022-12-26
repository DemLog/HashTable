using DynamicStructures.Test.Tester.classes;
using DynamicStructures.Test.Tester.interfaces;

namespace DynamicStructures.Test.Tester;

public class MemoryTester : ITester<long>
{
    public MemoryTester()
    {
        AllResults = new List<TestResult<long>>();
    }
    public TestResult<long> LastResult { get; private set; }

    public IList<TestResult<long>> AllResults { get; private set; }

    public void Test(Action algorithm, int iterationNumber, string name)
    {
        long result = 0;
        var localResults = new long[iterationNumber];
        for (int i = 0; i < iterationNumber; i++)
        {
            var startMemory = GC.GetTotalMemory(false);
            algorithm.Invoke();
            var endMemory = GC.GetTotalMemory(false);
            result = result + endMemory - startMemory;
        }
        result = result < 0 ? LastResult.Result : result /= iterationNumber;
        var resultId = AllResults.Count(x => x.AlgorithmName == name) + 1;
        TestResult<long> testResult = new(resultId,name, result, localResults);
        LastResult = testResult;
        lock (AllResults)
        {
            AllResults.Add(testResult);
        }
    }

    public void SaveAsExcel(string path, string name, bool EnableEmissions = true)
    {
        path = Path.Combine(path, name + ".xlsx");
        FileInfo file = new(path);
        var groupedResults = AllResults.GroupBy(x => x.AlgorithmName);
    
        foreach (var group in groupedResults)
        {
            SaveManager.SaveTable(file, group.ToArray(), "Количество (n)", "память (bytes)");
        }
    }
}