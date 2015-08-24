using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CachingDemo.Test
{
    [TestClass]
    public class Log
    {
        [TestMethod]
        public void CanAddError()
        {
            try
            {
                CachingDemo.Log.Error("Test error");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
