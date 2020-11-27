using NUnit.Framework;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using Microwave;
using NSubstitute;
using System;
using System.IO;

namespace Microwave.Test.Integration
{
    public class IT8_UIOutput
    {
        private IUserInterface _uut; //T
        private IButton _powerButton; //X
        private IButton _timeButton; //X
        private IButton _startCancelButton; //X
        private IDoor _door; //X
        private ILight _light; //X
        private ICookController _cookController; //x
        private IDisplay _display; //X
        private IPowerTube _powerTube; //X
        private ITimer _timer; //X
        private IOutput _output; //X

        private StringWriter str; //Used to test output

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_output);
            _cookController = new CookController(_timer, _display, _powerTube);

            _uut = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _cookController);

            // Finishes the double association with interfaces
            _cookController.UI = _uut;

            // Used to test Output
            str = new StringWriter();
            Console.SetOut(str);
        }

        [TestCase(1)]
        [TestCase(7)]
        [TestCase(14)]
        public void Output_DisplayShowPower_IsCorrect(int amountOfPresses)
        {
            for(int i = 0; i < amountOfPresses; i++)
            {
                _powerButton.Press();
            }

            // Asserts that the StringWriter contains the correct power and not higher
            Assert.That(str.ToString().Contains($"Display shows: {amountOfPresses*50} W"));
            Assert.That(!str.ToString().Contains($"Display shows: {(amountOfPresses+1) * 50} W"));
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void Output_DisplayShowTime_IsCorrect(int amountOfPresses)
        {
            _powerButton.Press();
            for (int i = 0; i < amountOfPresses; i++)
            {
                _timeButton.Press();
            }

            // Asserts that the StringWriter contains the time and not higher
            Assert.That(str.ToString().Contains($"Display shows: {(amountOfPresses*1):D2}:{0:D2}"));
            Assert.That(!str.ToString().Contains($"Display shows: {(amountOfPresses*1+1):D2}:{0:D2}"));
        }

        [Test]
        public void Output_DisplayClear_IsCorrect()
        {
            _powerButton.Press();
            _startCancelButton.Press();

            // Asserts that the StringWriter contains the text
            Assert.That(str.ToString().Contains($"Display cleared"));
        }

        [TestCase(1)]
        [TestCase(7)]
        [TestCase(14)]
        public void Output_StartCooking_PowerTubeTurnOn_IsCorrect(int amountOfPresses)
        {
            for (int i = 0; i < amountOfPresses; i++)
            {
                _powerButton.Press();
            }
            _timeButton.Press();
            _startCancelButton.Press();

            // Asserts that the StringWriter contains the correct PowerTube power
            Assert.That(str.ToString().Contains($"PowerTube works with {amountOfPresses*50} W"));
        }

        [Test]
        public void Output_StartCooking_PowerTubeTurnOff_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            // Asserts that the StringWriter contains the text
            Assert.That(str.ToString().Contains($"PowerTube turned off"));
        }

        [Test]
        public void Output_Light_TurnOn_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            // Asserts that the StringWriter contains the text
            Assert.That(str.ToString().Contains("Light is turned on"));
        }

        [Test]
        public void Output_Light_TurnOff_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            // Asserts that the StringWriter contains the text
            Assert.That(str.ToString().Contains("Light is turned off"));
        }
    }
}