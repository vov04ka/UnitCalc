using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Units
{
    [TestClass()]
    public class Tests
    {
        /// <summary>
        /// Tests the IsSimple implementation
        /// </summary>
        [TestMethod()]
        public void CheckSimple()
        {
            CompositeUnit u = new CompositeUnit(Unit.Meters);
            Assert.IsTrue(u.IsSimple);
            u._components.Add(Unit.Seconds, -1);
            Assert.IsFalse(u.IsSimple);
        }

        /// <summary>
        /// Tests the Inverse() implementation
        /// </summary>
        [TestMethod()]
        public void CheckInverse()
        {
            CompositeUnit u = new CompositeUnit(new CompositeUnit(Unit.Meters), Unit.Seconds, -2);
            Assert.AreEqual(new CompositeUnit(new CompositeUnit(Unit.Seconds, 2), Unit.Meters, -1), u.Inverse());
        }

        [TestMethod()]
        public void ToSimple()
        {
            CompositeUnit u = new CompositeUnit(Unit.Meters);
            Assert.AreEqual(Unit.Meters, u.TryReduceToSimple());
        }

        [TestMethod(), ExpectedException(typeof(System.InvalidOperationException))]
        public void IllegalReduce()
        {
            CompositeUnit u = new CompositeUnit(new CompositeUnit(Unit.Meters), Unit.Seconds, 2);
            Unit uu = u.TryReduceToSimple();
        }

        [TestMethod()]
        public void ReduceToCoeff()
        {
            CompositeUnit u = new CompositeUnit(new CompositeUnit(Unit.DegreesC), Unit.Meters, -1);
            CompositeUnit uu = u * u.Inverse();
            
            Assert.AreEqual(Unit.None, uu.TryReduceToSimple());
        }

        [TestMethod()]
        public void EqualSimple()
        {
            CompositeUnit u1 = new CompositeUnit(Unit.Seconds);
            CompositeUnit u2 = new CompositeUnit(Unit.Seconds);
            Assert.AreEqual(u1, u2);
        }

        [TestMethod()]
        public void EqualComposite()
        {
            CompositeUnit u1 = new CompositeUnit(new CompositeUnit(Unit.Meters), Unit.Seconds, -1);
            CompositeUnit u2 = new CompositeUnit(new CompositeUnit(Unit.Seconds, -1), Unit.Meters, 1);
            CompositeUnit u3 = new CompositeUnit(new CompositeUnit(Unit.Meters, 2), Unit.Seconds, -1);
            Assert.AreEqual(u1, u2);
            Assert.AreEqual(u2, u1);
            Assert.AreNotEqual(u1, u3);
            Assert.AreNotEqual(u3, u2);
        }

        [TestMethod()]
        public void MultiplySimple()
        {
            CompositeUnit u1 = new CompositeUnit(Unit.Meters);
            CompositeUnit u2 = new CompositeUnit(Unit.Seconds);
            CompositeUnit u = u1 * u2;
            Assert.AreEqual(new CompositeUnit(new CompositeUnit(Unit.Meters), Unit.Seconds, 1), u);
            Assert.AreEqual(new CompositeUnit(new CompositeUnit(Unit.Meters), Unit.Seconds, 2), u * u2);
            Assert.AreEqual(new CompositeUnit(new CompositeUnit(Unit.Meters, 2), Unit.Seconds, 2), u * u);
        }

        [TestMethod()]
        public void MultiplyComposite()
        {
            CompositeUnit u1 = new CompositeUnit(new CompositeUnit(new CompositeUnit(Unit.Kilograms), Unit.Meters, -1), Unit.Seconds, -2);
            CompositeUnit u2 = new CompositeUnit(new CompositeUnit(Unit.Seconds), Unit.DegreesC, -1);
            Assert.AreEqual(new CompositeUnit(new CompositeUnit(new CompositeUnit(new CompositeUnit(Unit.Kilograms), Unit.Meters, -1), Unit.Seconds, -1), Unit.DegreesC, -1), u1 * u2);

            CompositeUnit u3 = new CompositeUnit(new CompositeUnit(Unit.Seconds, 2), Unit.Meters, -1);
            Assert.AreEqual(new CompositeUnit(new CompositeUnit(Unit.Kilograms), Unit.Meters, -2), u3 * u1);
        }

        [TestMethod()]
        public void DivideComposite()
        {
            CompositeUnit u1 = new CompositeUnit(new CompositeUnit(new CompositeUnit(Unit.Kilograms), Unit.Meters, -1), Unit.Seconds, -2);
            CompositeUnit u2 = new CompositeUnit(new CompositeUnit(Unit.Seconds), Unit.DegreesC, -1);
            Assert.AreEqual(new CompositeUnit(new CompositeUnit(new CompositeUnit(new CompositeUnit(Unit.Kilograms), Unit.Meters, -1), Unit.Seconds, -3), Unit.DegreesC, 1), u1 / u2);
        }
    }
}
