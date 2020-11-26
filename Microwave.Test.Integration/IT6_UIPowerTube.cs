using NUnit.Framework;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using Microwave;
using NSubstitute;
using System;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT6_UIPowerTube
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

        private IOutput _fakeOutput; //S
        private ITimer _fakeTimer; //S

        [SetUp]
        public void Setup()
        {
            _fakeOutput = Substitute.For<IOutput>();
            _fakeTimer = Substitute.For<ITimer>();

            _powerTube = new PowerTube(_fakeOutput);
            _display = new Display(_fakeOutput);
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_fakeOutput);
            _cookController = new CookController(_fakeTimer, _display, _powerTube);

            _uut = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _cookController);

            //Finishes the double association with interfaces
            _cookController.UI = _uut;
        }

        [Test]
        public void PowerTube_TurnOn_PowerIs50_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _fakeOutput.Received(1).OutputLine($"PowerTube works with {50}");
        }

        [TestCase(3)]  // 150 Power
        [TestCase(7)]  // 350 Power
        [TestCase(14)] // 700 Power
        public void PowerTube_TurnOn_PowerIsOutOfRange_IsCorrect(int amountOfPresses)
        {
            for(int i = 0; i < amountOfPresses; i++)
            {
                _powerButton.Press();
            }

            _timeButton.Press();

            Assert.That(() => _startCancelButton.Press(), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void PowerTube_TurnOn_IsOn_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();

            _powerTube.TurnOn(50);

            Assert.That(() => _startCancelButton.Press(), Throws.TypeOf<ApplicationException>());
        }

        [Test]
        public void PowerTube_TurnOff_IsOn_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _fakeOutput.Received(1).OutputLine($"PowerTube turned off");
        }
    }
}
