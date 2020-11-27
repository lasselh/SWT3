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
    public class IT4_UICookController
    {
        private IUserInterface _uut; //T
        private IButton _powerButton; //X
        private IButton _timeButton; //X
        private IButton _startCancelButton; //X
        private IDoor _door; //X
        private ILight _light; //X
        private ICookController _cookController; //x

        private IDisplay _fakeDisplay; //S
        private IOutput _fakeOutput; //S
        private IPowerTube _fakePowerTube; //S
        private ITimer _fakeTimer; //S

        [SetUp]
        public void Setup()
        {
            _fakeDisplay = Substitute.For<IDisplay>();
            _fakeOutput = Substitute.For<IOutput>();
            _fakePowerTube = Substitute.For<IPowerTube>();
            _fakeTimer = Substitute.For<ITimer>();

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_fakeOutput);
            _cookController = new CookController(_fakeTimer, _fakeDisplay, _fakePowerTube);

            _uut = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _fakeDisplay,
                _light,
                _cookController);

            // Finishes the double association with interfaces
            _cookController.UI = _uut;
        }

        [Test]
        public void CookController_StartCooking_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _fakePowerTube.Received(1).TurnOn(50);
            _fakeTimer.Received(1).Start(60);
        }

        [Test]
        public void CookController_StopCooking_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _fakePowerTube.Received(1).TurnOff();
            _fakeTimer.Received(1).Stop();
        }

        [Test]
        public void CookController_TimerExpired_IsCooking_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _fakeTimer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _fakePowerTube.Received(1).TurnOff();
            _fakeOutput.Received(1).OutputLine("Light is turned off");
            _fakeDisplay.Received(1).Clear();
        }

        [Test]
        public void CookController_TimerExpired_IsNotCooking_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _door.Open();

            _fakeTimer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _fakePowerTube.DidNotReceive().TurnOff();
            _fakeOutput.DidNotReceive().OutputLine("Light is turned off");
            _fakeDisplay.Received(1).Clear();
        }

        [TestCase(30)]
        [TestCase(90)]
        [TestCase(120)]
        public void CookController_TimerTick_IsCooking_IsCorrect(int timeRemaining)
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _fakeTimer.TimeRemaining.Returns(timeRemaining);

            _fakeTimer.TimerTick += Raise.EventWith(this, EventArgs.Empty);

            _fakeDisplay.Received(1).ShowTime(timeRemaining / 60, timeRemaining % 60);
        }

        [Test]
        public void CookController_TimerTick_IsNotCooking_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _fakeTimer.TimeRemaining.Returns(30);

            _fakeTimer.TimerTick += Raise.EventWith(this, EventArgs.Empty);

            _fakeDisplay.DidNotReceive().ShowTime(_fakeTimer.TimeRemaining / 60, 
                                              _fakeTimer.TimeRemaining % 60);
        }
    }
}
