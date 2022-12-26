using System;

namespace DynamicStructures.Test.Tester.interfaces
{
    public interface IStepTester
    {
        public void Test(Func<(double, int)> algorithm, int iterNumber, string name);
    }
}