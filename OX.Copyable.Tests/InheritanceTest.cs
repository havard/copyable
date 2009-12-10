namespace OX.Copyable.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InheritanceTest
    {
        class Foo
        {
            public int Value { get; set; }
        }
        
        class Bar : Foo
        {
        }

        [TestMethod]
        public void InheritedFieldsAreCopied()
        {
            var bar = new Bar() { Value = 42 };

            Bar copy = (Bar)bar.Copy();

            Assert.AreEqual(42, copy.Value);
        }

        [TestMethod]
        public void FormIsCopied()
        {
            Form1 form = new Form1();
            Form1 copy = (Form1)form.Copy();

            Assert.AreNotSame(form, copy);
        }
    }
}
