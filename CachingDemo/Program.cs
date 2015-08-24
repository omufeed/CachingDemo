using System;

namespace CachingDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CachingDemo");

            var cacheKey = "TestCacheKey";
            var cacheValue = "Test value";

            Console.WriteLine($"Storing {cacheValue} as {cacheKey}...");
            Cache.Store(cacheKey, cacheValue);
            Console.WriteLine("Stored");
            Console.WriteLine($"Getting {cacheKey} from cache...");
            Console.WriteLine($"{cacheKey} is {Cache.Get(cacheKey)}");

        }
    }
}
