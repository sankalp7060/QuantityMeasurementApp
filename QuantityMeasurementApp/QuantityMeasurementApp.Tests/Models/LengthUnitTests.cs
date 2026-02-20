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
        public void GetConversionFactorToFeet_YardUnit_ReturnsThree()
        {
            double factor = LengthUnit.YARD.GetConversionFactorToFeet();
            Assert.AreEqual(3.0, factor, 0.0001);
        }

        [TestMethod]
        public void GetConversionFactorToFeet_CentimeterUnit_ReturnsCorrectValue()
        {
            double factor = LengthUnit.CENTIMETER.GetConversionFactorToFeet();
            // 1 cm = 1/(2.54*12) feet = 1/30.48 feet = 0.0328083989501312 feet
            double expected = 0.0328083989501312;
            Assert.AreEqual(expected, factor, 0.0000001);
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
        public void GetUnitSymbol_YardUnit_ReturnsYd()
        {
            string symbol = LengthUnit.YARD.GetUnitSymbol();
            Assert.AreEqual("yd", symbol);
        }

        [TestMethod]
        public void GetUnitSymbol_CentimeterUnit_ReturnsCm()
        {
            string symbol = LengthUnit.CENTIMETER.GetUnitSymbol();
            Assert.AreEqual("cm", symbol);
        }

        [TestMethod]
        public void GetUnitName_FeetUnit_ReturnsFeet()
        {
            string name = LengthUnit.FEET.GetUnitName();
            Assert.AreEqual("feet", name);
        }

        [TestMethod]
        public void GetUnitName_InchUnit_ReturnsInches()
        {
            string name = LengthUnit.INCH.GetUnitName();
            Assert.AreEqual("inches", name);
        }

        [TestMethod]
        public void GetUnitName_YardUnit_ReturnsYards()
        {
            string name = LengthUnit.YARD.GetUnitName();
            Assert.AreEqual("yards", name);
        }

        [TestMethod]
        public void GetUnitName_CentimeterUnit_ReturnsCentimeters()
        {
            string name = LengthUnit.CENTIMETER.GetUnitName();
            Assert.AreEqual("centimeters", name);
        }

        [TestMethod]
        public void GetConversionFactorToFeet_InvalidUnit_ThrowsException()
        {
            LengthUnit invalidUnit = (LengthUnit)99;
            Assert.ThrowsException<ArgumentException>(() =>
                invalidUnit.GetConversionFactorToFeet()
            );
        }
    }
}
