using NUnit.Framework;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using Microwave;
using NSubstitute;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT1_UIButton
    {
        private IUserInterface _uut; //T
        private IButton _powerButton; //X
        private IButton _timeButton; //X
        private IButton _startCancelButton; //X

        private IDoor _fakeDoor;
        private ILight _fakeLight;
        private ICookController _fakeCookController;
        private IDisplay _fakeDisplay;

        [SetUp]
        public void Setup()
        {
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _fakeDoor = Substitute.For<IDoor>();
            _fakeLight = Substitute.For<ILight>();
            _fakeCookController = Substitute.For<ICookController>();
            _fakeDisplay = Substitute.For<IDisplay>();

            _uut = new UserInterface(
                _powerButton, 
                _timeButton, 
                _startCancelButton, 
                _fakeDoor, 
                _fakeDisplay, 
                _fakeLight, 
                _fakeCookController);
        }

        #region powerButton
        [Test]
        public void PowerButton_StateREADY_IsCorrect()
        {
            //Power button pressed, calls OnPowerPressed() in READY state
            _powerButton.Press();
            //Display.ShowPower called with argument 50 as initial value
            _fakeDisplay.Received(1).ShowPower(50);
        }

        [Test]
        public void PowerButton_StateSETPOWER_IsCorrect()
        {
            _powerButton.Press();
            //State is now SETPOWER, pressing again will call Display.ShowPower with arugment 100
            _powerButton.Press();
            _fakeDisplay.Received(1).ShowPower(100);
        }
        #endregion

        #region timeButton
        [Test]
        public void TimeButton_StateSETPOWER_IsCorrect()
        {
            //Power button pressed, changes state to SETPOWER
            _powerButton.Press();
            //Time button pressed, calls OnTimePressed()
            _timeButton.Press();
            _fakeDisplay.Received(1).ShowTime(1, 0);
        }

        [Test]
        public void TimeButton_StateSETTIME_IsCorrect()
        {
            //Power button pressed, changes state to SETPOWER
            _powerButton.Press();
            //Time button pressed, state is now SETTIME
            _timeButton.Press();
            //Time button pressed again, calls Display.ShowTime
            _timeButton.Press();
            _fakeDisplay.Received(1).ShowTime(2, 0);
        }
        #endregion

        #region startCancelButton
        [Test]
        public void StartCancelButton_StateSETPOWER_IsCorrect()
        {
            //Power button pressed, changes state to SETPOWER
            _powerButton.Press();
            //Time button pressed, calls OnTimePressed()
            _startCancelButton.Press();
            _fakeDisplay.Received(1).ShowTime(1, 0);
        }
        #endregion
    }
}