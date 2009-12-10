using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OX.Copyable.Tests
{
    public enum Gender
    {
        Male,
        Female
    }
    public class Human
    {
        private string _name;
        private Gender _gender;
        private List<Human> _children;

        public string Name { get { return _name; } set { _name = value; } }
        public Gender Gender { get { return _gender; } set { _gender = value; } }
        public List<Human> Children { get { return _children; } set { _children = value; } }
    }
}
