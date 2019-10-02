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
            string expectedResult, actualResult;

            expectedResult = "Test";
            actualResult = Helper.GetFolder("D:\\data\\Test");
            Assert.AreEqual(expectedResult, actualResult);

            expectedResult = "Test";
            actualResult = Helper.GetFolder("D:\\data\\Test\\");
            Assert.AreEqual(expectedResult, actualResult);            

            expectedResult = "AFolder.ext";
            actualResult = Helper.GetFolder("D:\\data\\Test\\AFolder.ext");
            Assert.AreEqual(expectedResult, actualResult);

            expectedResult = "AFolder.ext";
            actualResult = Helper.GetFolder("D:\\data\\Test\\AFolder.ext\\");
            Assert.AreEqual(expectedResult, actualResult);

            expectedResult = "Test";
            actualResult = Helper.GetFolder("D:\\data\\Test\\AFile.ext");
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void FindInput_Test()
        {            
            string expectedResult, actualResult;

            expectedResult = @"D:\data\A_Process\Priority\GY8QJ.123";
            using (MainForm mainForm = new MainForm())
            {
                actualResult = mainForm.FindInput("123");
            }                
            Assert.AreEqual(expectedResult, actualResult);

            //expectedResult = "Test";
            //actualResult = Helper.GetFolder("D:\\data\\Test\\");
            //Assert.AreEqual(expectedResult, actualResult);

            //expectedResult = "AFolder.ext";
            //actualResult = Helper.GetFolder("D:\\data\\Test\\AFolder.ext");
            //Assert.AreEqual(expectedResult, actualResult);

            //expectedResult = "AFolder.ext";
            //actualResult = Helper.GetFolder("D:\\data\\Test\\AFolder.ext\\");
            //Assert.AreEqual(expectedResult, actualResult);

            //expectedResult = "Test";
            //actualResult = Helper.GetFolder("D:\\data\\Test\\AFile.ext");
            //Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
