using MainApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DEM_SAF_Tests
{
    [TestClass]
    public class MFHelperTests
    {
        [TestMethod]
        public void GetFolder_Test()
        {
            Helper helper = new Helper();
            string expectedResult, actualResult;

            expectedResult = "Test";
            actualResult = helper.GetFolder("D:\\data\\Test");
            Assert.AreEqual(expectedResult, actualResult);

            expectedResult = "Test";
            actualResult = helper.GetFolder("D:\\data\\Test\\");
            Assert.AreEqual(expectedResult, actualResult);            

            expectedResult = "AFolder.ext";
            actualResult = helper.GetFolder("D:\\data\\Test\\AFolder.ext");
            Assert.AreEqual(expectedResult, actualResult);

            expectedResult = "AFolder.ext";
            actualResult = helper.GetFolder("D:\\data\\Test\\AFolder.ext\\");
            Assert.AreEqual(expectedResult, actualResult);

            expectedResult = "Test";
            actualResult = helper.GetFolder("D:\\data\\Test\\AFile.ext");
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
