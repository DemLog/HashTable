using System;

namespace DynamicStructures.Test.Tester.interfaces
{
    public interface ITimeTester
    {
        public void Test(Action algorithm, int iterNumber, string name);
    }
}