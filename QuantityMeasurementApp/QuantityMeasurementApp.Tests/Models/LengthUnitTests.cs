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
        private LengthUnitExtensions _unitExtensions = null!;

        [TestInitialize]
        public void Setup()
        {
            _unitExtensions = new LengthUnitExtensions();
        }

        // Tests LengthUnitExtensions.GetConversionFactorToFeet() for FEET
        [TestMethod]
        public void GetConversionFactorToFeet_FeetUnit_ReturnsOne()
        {
            double factor = _unitExtensions.GetConversionFactorToFeet(LengthUnit.FEET);
            Assert.AreEqual(1.0, factor, 0.0001);
        }

        // Tests LengthUnitExtensions.GetConversionFactorToFeet() for INCH
        [TestMethod]
        public void GetConversionFactorToFeet_InchUnit_ReturnsOneTwelfth()
        {
            double factor = _unitExtensions.GetConversionFactorToFeet(LengthUnit.INCH);
            Assert.AreEqual(1.0 / 12.0, factor, 0.0001);
        }

        // Tests LengthUnitExtensions.GetConversionFactorToFeet() for YARD
        [TestMethod]
        public void GetConversionFactorToFeet_YardUnit_ReturnsThree()
        {
            double factor = _unitExtensions.GetConversionFactorToFeet(LengthUnit.YARD);
            Assert.AreEqual(3.0, factor, 0.0001);
        }

        // Tests LengthUnitExtensions.GetConversionFactorToFeet() for CENTIMETER
        [TestMethod]
        public void GetConversionFactorToFeet_CentimeterUnit_ReturnsCorrectValue()
        {
            double factor = _unitExtensions.GetConversionFactorToFeet(LengthUnit.CENTIMETER);
            // 1 cm = 1/(2.54*12) feet = 1/30.48 feet = 0.0328083989501312 feet
            double expected = 0.0328083989501312;
            Assert.AreEqual(expected, factor, 0.0000001);
        }

        // Tests LengthUnitExtensions.GetUnitSymbol() for FEET (static UI method)
        [TestMethod]
        public void GetUnitSymbol_FeetUnit_ReturnsFt()
        {
            string symbol = LengthUnitExtensions.GetUnitSymbol(LengthUnit.FEET);
            Assert.AreEqual("ft", symbol);
        }

        // Tests LengthUnitExtensions.GetUnitSymbol() for INCH (static UI method)
        [TestMethod]
        public void GetUnitSymbol_InchUnit_ReturnsIn()
        {
            string symbol = LengthUnitExtensions.GetUnitSymbol(LengthUnit.INCH);
            Assert.AreEqual("in", symbol);
        }

        // Tests LengthUnitExtensions.GetUnitSymbol() for YARD (static UI method)
        [TestMethod]
        public void GetUnitSymbol_YardUnit_ReturnsYd()
        {
            string symbol = LengthUnitExtensions.GetUnitSymbol(LengthUnit.YARD);
            Assert.AreEqual("yd", symbol);
        }

        // Tests LengthUnitExtensions.GetUnitSymbol() for CENTIMETER (static UI method)
        [TestMethod]
        public void GetUnitSymbol_CentimeterUnit_ReturnsCm()
        {
            string symbol = LengthUnitExtensions.GetUnitSymbol(LengthUnit.CENTIMETER);
            Assert.AreEqual("cm", symbol);
        }

        // Tests LengthUnitExtensions.GetUnitName() for FEET (static UI method)
        [TestMethod]
        public void GetUnitName_FeetUnit_ReturnsFeet()
        {
            string name = LengthUnitExtensions.GetUnitName(LengthUnit.FEET);
            Assert.AreEqual("feet", name);
        }

        // Tests LengthUnitExtensions.GetUnitName() for INCH (static UI method)
        [TestMethod]
        public void GetUnitName_InchUnit_ReturnsInches()
        {
            string name = LengthUnitExtensions.GetUnitName(LengthUnit.INCH);
            Assert.AreEqual("inches", name);
        }

        // Tests LengthUnitExtensions.GetUnitName() for YARD (static UI method)
        [TestMethod]
        public void GetUnitName_YardUnit_ReturnsYards()
        {
            string name = LengthUnitExtensions.GetUnitName(LengthUnit.YARD);
            Assert.AreEqual("yards", name);
        }

        // Tests LengthUnitExtensions.GetUnitName() for CENTIMETER (static UI method)
        [TestMethod]
        public void GetUnitName_CentimeterUnit_ReturnsCentimeters()
        {
            string name = LengthUnitExtensions.GetUnitName(LengthUnit.CENTIMETER);
            Assert.AreEqual("centimeters", name);
        }

        // Tests LengthUnitExtensions.GetConversionFactorToFeet() for invalid enum value
        [TestMethod]
        public void GetConversionFactorToFeet_InvalidUnit_ThrowsException()
        {
            LengthUnit invalidUnit = (LengthUnit)99;
            Assert.ThrowsException<ArgumentException>(() =>
                _unitExtensions.GetConversionFactorToFeet(invalidUnit)
            );
        }
    }
}
