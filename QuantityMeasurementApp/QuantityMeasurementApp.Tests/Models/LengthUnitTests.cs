using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Contains unit tests for validating LengthUnit enum methods including conversion factors,
    /// unit symbols, unit names, and exception handling for invalid units.
    /// </summary>
    [TestClass]
    public class LengthUnitTests
    {
        private UnitConverter _unitConverter = null!;

        [TestInitialize]
        public void Setup()
        {
            _unitConverter = new UnitConverter();
        }

        // Tests LengthUnit.GetConversionFactorToFeet() for FEET
        [TestMethod]
        public void GetConversionFactorToFeet_FeetUnit_ReturnsOne()
        {
            double factor = _unitConverter.GetConversionFactorToFeet(LengthUnit.FEET);
            Assert.AreEqual(1.0, factor, 0.0001);
        }

        // Tests LengthUnit.GetConversionFactorToFeet() for INCH
        [TestMethod]
        public void GetConversionFactorToFeet_InchUnit_ReturnsOneTwelfth()
        {
            double factor = _unitConverter.GetConversionFactorToFeet(LengthUnit.INCH);
            Assert.AreEqual(1.0 / 12.0, factor, 0.0001);
        }

        // Tests LengthUnit.GetConversionFactorToFeet() for YARD
        [TestMethod]
        public void GetConversionFactorToFeet_YardUnit_ReturnsThree()
        {
            double factor = _unitConverter.GetConversionFactorToFeet(LengthUnit.YARD);
            Assert.AreEqual(3.0, factor, 0.0001);
        }

        // Tests LengthUnit.GetConversionFactorToFeet() for CENTIMETER
        [TestMethod]
        public void GetConversionFactorToFeet_CentimeterUnit_ReturnsCorrectValue()
        {
            double factor = _unitConverter.GetConversionFactorToFeet(LengthUnit.CENTIMETER);
            // 1 cm = 1/(2.54*12) feet = 1/30.48 feet = 0.0328083989501312 feet
            double expected = 0.0328083989501312;
            Assert.AreEqual(expected, factor, 0.0000001);
        }

        // Tests LengthUnit.GetUnitSymbol() for FEET
        [TestMethod]
        public void GetUnitSymbol_FeetUnit_ReturnsFt()
        {
            string symbol = LengthUnit.FEET.GetUnitSymbol();
            Assert.AreEqual("ft", symbol);
        }

        // Tests LengthUnit.GetUnitSymbol() for INCH
        [TestMethod]
        public void GetUnitSymbol_InchUnit_ReturnsIn()
        {
            string symbol = LengthUnit.INCH.GetUnitSymbol();
            Assert.AreEqual("in", symbol);
        }

        // Tests LengthUnit.GetUnitSymbol() for YARD
        [TestMethod]
        public void GetUnitSymbol_YardUnit_ReturnsYd()
        {
            string symbol = LengthUnit.YARD.GetUnitSymbol();
            Assert.AreEqual("yd", symbol);
        }

        // Tests LengthUnit.GetUnitSymbol() for CENTIMETER
        [TestMethod]
        public void GetUnitSymbol_CentimeterUnit_ReturnsCm()
        {
            string symbol = LengthUnit.CENTIMETER.GetUnitSymbol();
            Assert.AreEqual("cm", symbol);
        }

        // Tests LengthUnit.GetUnitName() for FEET
        [TestMethod]
        public void GetUnitName_FeetUnit_ReturnsFeet()
        {
            string name = LengthUnit.FEET.GetUnitName();
            Assert.AreEqual("feet", name);
        }

        // Tests LengthUnit.GetUnitName() for INCH
        [TestMethod]
        public void GetUnitName_InchUnit_ReturnsInches()
        {
            string name = LengthUnit.INCH.GetUnitName();
            Assert.AreEqual("inches", name);
        }

        // Tests LengthUnit.GetUnitName() for YARD
        [TestMethod]
        public void GetUnitName_YardUnit_ReturnsYards()
        {
            string name = LengthUnit.YARD.GetUnitName();
            Assert.AreEqual("yards", name);
        }

        // Tests LengthUnit.GetUnitName() for CENTIMETER
        [TestMethod]
        public void GetUnitName_CentimeterUnit_ReturnsCentimeters()
        {
            string name = LengthUnit.CENTIMETER.GetUnitName();
            Assert.AreEqual("centimeters", name);
        }

        // Tests LengthUnit.GetConversionFactorToFeet() for invalid enum value
        [TestMethod]
        public void GetConversionFactorToFeet_InvalidUnit_ThrowsException()
        {
            LengthUnit invalidUnit = (LengthUnit)99;
            Assert.ThrowsException<ArgumentException>(() =>
                _unitConverter.GetConversionFactorToFeet(invalidUnit)
            );
        }
    }
}
