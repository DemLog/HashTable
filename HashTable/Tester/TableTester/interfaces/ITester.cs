using System;
using System.Collections.Generic;
using DynamicStructures.Test.Tester.classes;

namespace DynamicStructures.Test.Tester.interfaces
{
    public interface ITester<TResult>
    {
        public TestResult<TResult> LastResult { get; }
        public IList<TestResult<TResult>> AllResults { get; }

        public void SaveAsExcel(string path, string name, bool enableEmissions = true);
    }
}