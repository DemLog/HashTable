namespace DynamicStructures.Test.Tester.classes
{
    public static class DoubleExtension
    {
        public static double Average(this double[] vector)
        {
            double temp = 0;
            double result;
            for (int i = 0; i < vector.Length; i++)
            {
                temp = temp + vector[i];
            }
            result = temp / vector.Length;
            return result;
        }
    }
}