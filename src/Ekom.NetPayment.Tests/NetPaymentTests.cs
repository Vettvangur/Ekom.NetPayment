using Microsoft.VisualStudio.TestTools.UnitTesting;
using Umbraco.NetPayment.Helpers;

namespace Umbraco.NetPayment.Tests
{
    [TestClass]
    public class NetPaymentTests
    {
        [TestMethod]
        public void CalculatesMD5Correctly()
        {
            var md5sum = CryptoHelpers.GetMD5StringSum("supyo");

            Assert.AreEqual(md5sum, "6358dc15702b9fe289b7329d7f26914f");
        }
    }
}
