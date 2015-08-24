using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CachingDemo.Test
{
    [TestClass]
    public class Cache
    {
        [TestMethod]
        public void CanStoreString()
        {
            var cacheKey = "testItem";
            var cacheValue = "test value";

            Assert.IsTrue(global::CachingDemo.Cache.Store(cacheKey, cacheValue), "Can not store a string!");
        }

        [TestMethod]
        public void CanGetString()
        {
            var cacheKey = "testItem";
            var cacheValue = "test value";

            Assert.IsTrue(global::CachingDemo.Cache.Store(cacheKey, cacheValue), "Can not store a string!");

            Assert.AreEqual(cacheValue, global::CachingDemo.Cache.Get(cacheKey), "Can not get the string from the cache!");
        }

        private class BigBadObject
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }
        [TestMethod]
        public void CanStoreObject()
        {
            var cacheKey = "testObject";
            var cacheObject = new BigBadObject() {ID = new Guid().ToString(), Name = "Test Object"};

            Assert.IsTrue(global::CachingDemo.Cache.Store(cacheKey, cacheObject), "Can not store an object!");

            Assert.AreEqual(cacheObject.ID, ((BigBadObject)global::CachingDemo.Cache.Get(cacheKey)).ID, "Can not get the object from the cache!");
        }
    }
}
