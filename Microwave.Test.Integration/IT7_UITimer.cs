using NUnit.Framework;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using Microwave;
using NSubstitute;
using System.Threading;
using System;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT7_UITimer
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

        private IOutput _fakeOutput; //S

        [SetUp]
        public void Setup()
        {
            _fakeOutput = Substitute.For<IOutput>();

            _timer = new Timer();
            _powerTube = new PowerTube(_fakeOutput);
            _display = new Display(_fakeOutput);
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_fakeOutput);
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
        }

        [Test]
        public void Timer_Start_TimerTick_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            // Timer.Start called with time = 1 minute = 60 seconds

            ManualResetEvent pause = new ManualResetEvent(false);

            _timer.TimerTick += (sender, args) => pause.Set();

            // Wait for a tick, but no longer, should occur
            Assert.That(pause.WaitOne(1100));
        }

        [Test]
        public void Timer_Stop_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            // Timer.Start called with time = 1 minute = 60 seconds 
            // Timer.Stop called immediately after, no ticks should occur

            ManualResetEvent pause = new ManualResetEvent(false);

            _timer.TimerTick += (sender, args) => pause.Set();

            // Wait for a tick, but no longer, should not occur
            Assert.That(!pause.WaitOne(1100));
        }

        [Test]
        public void Timer_Start_TimerExpired_IsExpired_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            // UserInterface calls CookControlers' StartCooking(), which calls Timer's Start()
            // with value 60 (seconds)

            ManualResetEvent pause = new ManualResetEvent(false);
            _timer.Expired += (sender, args) => pause.Set();

            // Waits for 61 seconds before asserting whether the timer has expired, which it should.
            Assert.That(pause.WaitOne(61000));
        }

        [Test]
        public void Timer_Start_TimerExpired_IsNotExpired_IsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            // UserInterface calls CookControlers' StartCooking(), which calls Timer's Start()
            // with value 60 (seconds)

            ManualResetEvent pause = new ManualResetEvent(false);
            _timer.Expired += (sender, args) => pause.Set();

            // Waits for 59 seconds before asserting whether the timer has expired, which it should not.
            Assert.That(!pause.WaitOne(59000));
        }
    }
}
