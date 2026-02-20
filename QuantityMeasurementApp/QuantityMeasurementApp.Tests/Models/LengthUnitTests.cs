using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    [TestClass]
    public class LengthUnitTests
    {
        [TestMethod]
        public void GetConversionFactorToFeet_FeetUnit_ReturnsOne()
        {
            double factor = LengthUnit.FEET.GetConversionFactorToFeet();
            Assert.AreEqual(1.0, factor, 0.0001);
        }

        [TestMethod]
        public void GetConversionFactorToFeet_InchUnit_ReturnsOneTwelfth()
        {
            double factor = LengthUnit.INCH.GetConversionFactorToFeet();
            Assert.AreEqual(1.0 / 12.0, factor, 0.0001);
        }

        [TestMethod]
        public void GetUnitSymbol_FeetUnit_ReturnsFt()
        {
            string symbol = LengthUnit.FEET.GetUnitSymbol();
            Assert.AreEqual("ft", symbol);
        }

        [TestMethod]
        public void GetUnitSymbol_InchUnit_ReturnsIn()
        {
            string symbol = LengthUnit.INCH.GetUnitSymbol();
            Assert.AreEqual("in", symbol);
        }

        [TestMethod]
        public void GetConversionFactorToFeet_InvalidUnit_ThrowsException()
        {
            // This test verifies that passing an invalid enum value throws an exception
            LengthUnit invalidUnit = (LengthUnit)99;
            Assert.ThrowsException<ArgumentException>(() =>
                invalidUnit.GetConversionFactorToFeet()
            );
        }
    }
}
