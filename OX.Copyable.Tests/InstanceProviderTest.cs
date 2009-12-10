namespace OX.Copyable.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Drawing;
    
    class SolidBrushProvider : InstanceProvider<SolidBrush>
    {
        public static int NumCalls { get; set; }

        public override SolidBrush CreateTypedCopy(SolidBrush toBeCopied)
        {
            NumCalls++;
            return new SolidBrush(toBeCopied.Color);
        }
    }

    [TestClass]
    public class InstanceProviderTest
    {
        [TestMethod]
        public void BrushIsCopiedWithInstanceProvider()
        {
            SolidBrushProvider.NumCalls = 0;

            SolidBrush a = new SolidBrush(Color.Red);
            SolidBrush b = (SolidBrush)a.Copy();
            
            Assert.AreNotSame(a, b);
            Assert.IsTrue(SolidBrushProvider.NumCalls > 0);

        }

        [TestMethod]
        public void BrushIsCopiedWithSuppliedInstance()
        {
            SolidBrush a = new SolidBrush(Color.Black);
            SolidBrush b = new SolidBrush(Color.Red);
            a.Copy(b);

            Assert.AreNotSame(a, b);
            Assert.AreEqual(a.Color, b.Color);
        }
    }
}
