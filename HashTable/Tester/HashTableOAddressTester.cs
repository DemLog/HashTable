using DynamicStructures.Test.Tester;
using HashTable.HashFunctions;

namespace HashTable.Tester;

public class HashTableOAddressTester: HashTableTester
{
    public static void Testing(HashProbingType hashProbingType, params HashFuncType[] hashFuncTypes)
    {
        const int SIZE_GENERATION = 10000;

        Console.WriteLine("\nНачало тестирования хеш-таблицы (открытая адресация)");
        Console.WriteLine($"Тестирование хеш функций класса: \"{GetNameProbingtype(hashProbingType)}\"");
        
        if (hashProbingType != HashProbingType.Double)
        {
            foreach (var hashFunc in hashFuncTypes)
            {
                Console.WriteLine($"\nТест хеш-таблицы с функцией хеширования: \"{HashFunc.HashFuncTypeNames[(int)hashFunc]}\"");
                var ht = new HashTableOAddress<string, string>(10000, hashProbingType, hashFunc);

                Console.WriteLine("Генерация данных в таблицу");
                var (keys, values) = GeneratingValuesAndKeys(SIZE_GENERATION);

                Console.WriteLine("Добавление данных в таблицу");
                for (var i = 0; i < keys.Count; i++)
                {
                    ht.Add(keys[i], values[i]);
                }
                Console.WriteLine($"Самый длинный кластер в таблице: {ht.MaxClusterLength}");
            }
        }
        else if (hashFuncTypes.Length == 2)
        {
            Console.WriteLine($"\nТест хеш-таблицы с функциями хеширования: \"{HashFunc.HashFuncTypeNames[(int)hashFuncTypes[0]]}\" и \"{HashFunc.HashFuncTypeNames[(int)hashFuncTypes[1]]}\"");
            var ht = new HashTableOAddress<string, string>(10000, hashProbingType, hashFuncTypes[0], hashFuncTypes[1]);
            
            Console.WriteLine("Генерация данных в таблицу");
            var (keys, values) = GeneratingValuesAndKeys(SIZE_GENERATION);

            Console.WriteLine("Добавление данных в таблицу");
            for (var i = 0; i < keys.Count; i++)
            {
                ht.Add(keys[i], values[i]);
            }
            
            Console.WriteLine($"Самый длинный кластер в таблице: {ht.MaxClusterLength}");
        }
    }
    
    public static void TestForTable(string name, HashProbingType hashProbingType, int iterCount)
    {
        var testerTime = new TimeTester();
        var testerMemory = new MemoryTester();
        Action<int, HashProbingType> func = StartSpecialTest;

        for (int i = 1; i <= 1000; i++)
        {
            Console.WriteLine("Тест алгоритма: {0} | Итерация: {1}", name, i);
            testerTime.Test(() => func.Invoke(i, hashProbingType), iterCount, name);
            testerMemory.Test(() => func.Invoke(i, hashProbingType), iterCount, name);
        }
        testerTime.SaveAsExcel(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), name + " - время");
        testerTime.AllResults.Clear();
        
        testerMemory.SaveAsExcel(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), name + " - память");
        testerMemory.AllResults.Clear();
    }
    
    private static void StartSpecialTest(int size, HashProbingType hashProbingType)
    {
        var ht = new HashTableOAddress<string, string>(1000, hashProbingType);
        var (keys, values) = HashTableTester.GeneratingValuesAndKeys(size);
        for (var i = 0; i < keys.Count; i++)
        {
            ht.Add(keys[i], values[i]);
        }
    }

    private static string GetNameProbingtype(HashProbingType hashProbingType)
    {
        return hashProbingType switch
        {
            HashProbingType.Linear => "Линейное исследование",
            HashProbingType.Quadratic => "Квадратичное исследование",
            HashProbingType.Double => "Двлйное хеширование",
            _ => "Линейное исследование"
        };
    }
}