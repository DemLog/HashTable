using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DynamicStructures.Test.Tester.classes;
using DynamicStructures.Test.Tester.interfaces;

namespace DynamicStructures.Test.Tester
{
    public class TimeTester : ITester<double>, ITimeTester
    {
        public TimeTester()
        {
            AllResults = new List<TestResult<double>>();
        }
        
        public TestResult<double> LastResult { get; protected set; }
        public IList<TestResult<double>> AllResults { get; protected set; }
        
        public void Test(Action algorithm, int iterNumber, string name)
        {
            var time = new Stopwatch();
            var localResults = new double[iterNumber];
            for (int i = 0; i < iterNumber; i++)
            {
                time.Restart();
                algorithm.Invoke();
                time.Stop();
                localResults[i] = time.Elapsed.TotalMilliseconds;
            }
            var resultId = AllResults.Count(x => x.AlgorithmName == name) + 1;
            var generalResult = localResults.Min();
            var testResult = new TestResult<double>(resultId, name, generalResult, localResults);
            LastResult = testResult;
            lock (AllResults) 
            {
                AllResults.Add(testResult); 
            }
        }
        
        public void SaveAsExcel(string path, string name, bool emissionsEnabled = true)
        {
            path = Path.Combine(path, name + ".xlsx");
            FileInfo file = new(path);
            var groupedResults = AllResults.GroupBy(x => x.AlgorithmName);

            foreach (var group in groupedResults)
            {
                var groupAr = group.ToArray();
                // if (!emissionsEnabled) Services.DeleteEmissions(groupAr);
                SaveManager.SaveTable(file, groupAr, "Количество (n)", "Время (Миллисекунды)");
            }
        }
        

    }
}