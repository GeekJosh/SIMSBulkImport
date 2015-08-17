using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserGen;

namespace UserGen_UnitTest
{
    [TestClass]
    public class Builder_UnitTest
    {
        [TestMethod]
        public void IsValidExpression_Invalid_MissingIncrement()
        {
            Builder _b = new Builder();
            _b.SetExpression = "[Forename]";
            bool result = _b.IsValidExpression;
            Assert.AreNotEqual(true, result);
        }

        [TestMethod]
        public void IsValidExpression_Valid()
        {
            Builder _b = new Builder();
            _b.SetExpression = "[Increment]";
            bool result = _b.IsValidExpression;
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void IsValidExpression_Invalid_MissingFrontBracket()
        {
            Builder _b = new Builder();
            _b.SetExpression = "Increment]";
            bool result = _b.IsValidExpression;
            Assert.AreNotEqual(true, result);
        }

        [TestMethod]
        public void IsValidExpression_Invalid_MissingBackBracket()
        {
            Builder _b = new Builder();
            _b.SetExpression = "[Increment";
            bool result = _b.IsValidExpression;
            Assert.AreNotEqual(true, result);
        }
    }
}
