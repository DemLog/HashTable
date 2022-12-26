using HashTable.HashFunctions;
using HashTable.Tester;

/*HashTableChainedTester.Testing(HashFuncType.Div, HashFuncType.Multi, HashFuncType.SHA256, HashFuncType.HMACMD5, HashFuncType.XXHash32, HashFuncType.MurmurHash);*/
/*HashTableChainedTester.TestForTable("Метод цепочек", 1);*/

/*HashTableOAddressTester.Testing(HashProbingType.Linear, HashFuncType.SHA256);*/
/*HashTableOAddressTester.Testing(HashProbingType.Quadratic, HashFuncType.Div);*/
/*HashTableOAddressTester.Testing(HashProbingType.Double, HashFuncType.HMACMD5, HashFuncType.SHA256);*/

HashTableOAddressTester.TestForTable("Открытая адресация - Линейная", HashProbingType.Linear, 1);