namespace OX.Copyable.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class ObjectHierarchyTest
    {
        class Node
        {
            private Node _prev;
            private Node _next;
            private string name;
            public Node Prev { get { return _prev; } set { _prev = value; } }
            public Node Next { get { return _next; } set { _next = value; } }
            public string Name { get { return name; } set { name = value; } }
        }

        [TestMethod]
        public void TestObjectHierarchyClone()
        {
            Node n1 = new Node();
            n1.Name = "Node 1";
            Node n2 = new Node();
            n2.Name = "Node 2";
            
            n1.Next = n2;
            Node n1c = (Node)n1.Copy();

            Assert.AreNotSame(n1, n1c);
            Assert.AreNotSame(n2, n1c.Next);
            Assert.IsNotNull(n1c.Next);
            Assert.AreEqual(n1.Name, n1c.Name);
            Assert.AreEqual(n2.Name, n1c.Next.Name);
        }

        [TestMethod]
        public void CyclicObjectIsCopiedWithSemanticsIntact()
        {
            Node n1 = new Node();
            n1.Name = "Node 1";
            Node n2 = new Node();
            n2.Name = "Node 2";

            n1.Next = n2;
            n2.Prev = n1;
            Node n1c = (Node)n1.Copy();

            Assert.AreNotSame(n1, n1c);
            Assert.AreNotSame(n2, n1c.Next);
            Assert.IsNotNull(n1c.Next);
            Assert.AreEqual(n1.Name, n1c.Name);
            Assert.AreEqual(n2.Name, n1c.Next.Name);
            Assert.AreSame(n1c, n1c.Next.Prev);
        }

        [TestMethod]
        public void HumanHierarchyIsCloned()
        {
            Human father = new Human();
            father.Gender = Gender.Male;
            father.Name = "Dad";
            father.Children = new List<Human>();

            Human son = new Human();
            son.Gender = Gender.Male;
            son.Name = "Sonny";

            father.Children.Add(son);

            // Crazy science
            Human sensation = (Human)father.Copy();

            Assert.AreNotSame(father, sensation);
            Assert.AreNotSame(father.Children, sensation.Children);
            Assert.IsNotNull(sensation.Children);
            Assert.AreEqual(1, sensation.Children.Count);
            Assert.AreNotSame(father.Children[0], sensation.Children[0]);
            Assert.AreEqual(father.Name, sensation.Name);
            Assert.AreEqual(father.Children[0].Name, sensation.Children[0].Name);
        }
    }
}
