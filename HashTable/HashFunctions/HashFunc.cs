using System;
using System.Security.Cryptography;
using System.Text;
using Extensions.Data;
using Murmur;

namespace HashTable.HashFunctions
{
    public static class HashFunc
    {
        public static string[] HashFuncTypeNames = new string[]
        {
            "Метод деления",
            "Метод умножения",
            "SHA256",
            "HMACMD5",
            "XXHash32",
            "MurmurHash"
        };

        public static double GoldenRatioConst { get; } = (Math.Sqrt(5) - 1) / 2;

        public static int GetHashCodeDivFunc(object key, int sizeTable) => Math.Abs(key.GetHashCode() % sizeTable);

        public static int GetHashCodeMultiFunc(object key, int sizeTable) =>
            (int) Math.Abs(sizeTable * (key.GetHashCode() * GoldenRatioConst % 1));

        public static int GetHashCodeSHA256Func(object key, int sizeTable)
        {
            using var sha256 = SHA256.Create();
            var bytes_sha256_in = Encoding.UTF8.GetBytes(key.ToString());
            var bytes_sha256_out = sha256.ComputeHash(bytes_sha256_in);
            var resultSHA256 = BitConverter.ToInt32(bytes_sha256_out, 0);

            return Math.Abs(resultSHA256 % sizeTable);
        }

        public static int GetHashCodeHMACMD5Func(object key, int sizeTable)
        {
            var secrectKey = Encoding.UTF8.GetBytes(key.ToString());
            using var md5 = new HMACMD5(secrectKey);
            var bytes_md5_in = Encoding.UTF8.GetBytes(key.ToString());
            var bytes_md5_out = md5.ComputeHash(bytes_md5_in);
            var str_md5_out = BitConverter.ToString(bytes_md5_out);
            str_md5_out = str_md5_out.Replace("-", "");

            var sum = 0;
            for (var i = 0; i < str_md5_out.Length; i++)
            {
                sum += Convert.ToInt32(str_md5_out[i]);
            }

            return Math.Abs(sum % sizeTable);
        }

        public static int GetHashCodeXXHash32Func(object key, int sizeTable)
        {
            byte[] input = Encoding.UTF8.GetBytes(key.ToString());
            byte[] result = null;
            using (HashAlgorithm xxhash = XXHash32.Create())
            {
                result = xxhash.ComputeHash(input);
            }

            return Math.Abs(BitConverter.ToInt32(result, 0) % sizeTable);
        }

        public static int GetHashCodeMurmurHashFunc(object key, int sizeTable)
        {
            byte[] data = Guid.NewGuid().ToByteArray();
            HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
            byte[] hash = murmur128.ComputeHash(data);
            return Math.Abs(BitConverter.ToInt32(hash, 0) % sizeTable);
        }

        public static Func<object, int, int> GetHashFunc(HashFuncType hashFuncType)
        {
            return hashFuncType switch
            {
                HashFuncType.Div => GetHashCodeDivFunc,
                HashFuncType.Multi => GetHashCodeMultiFunc,
                HashFuncType.SHA256 => GetHashCodeSHA256Func,
                HashFuncType.HMACMD5 => GetHashCodeHMACMD5Func,
                HashFuncType.XXHash32 => GetHashCodeXXHash32Func,
                HashFuncType.MurmurHash => GetHashCodeMurmurHashFunc,
                _ => GetHashCodeDivFunc
            };
        }
    }
}