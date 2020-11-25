using NUnit.Framework;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using Microwave;
using NSubstitute;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT2_UIDoor
    {
        private IUserInterface _uut; //T
        private IButton _powerButton; //X
        private IButton _timeButton; //X
        private IButton _startCancelButton; //X
        private IDoor _door; //X

        private ILight _fakeLight; //S
        private ICookController _fakeCookController; //S
        private IDisplay _fakeDisplay; //S

        [SetUp]
        public void Setup()
        {
            _fakeLight = Substitute.For<ILight>();
            _fakeCookController = Substitute.For<ICookController>();
            _fakeDisplay = Substitute.For<IDisplay>();

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();

            _uut = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _fakeDisplay,
                _fakeLight,
                _fakeCookController);
        }

        #region Door
        [Test]
        public void DoorOpen_StateREADY_IsCorrect()
        {
            _door.Open();
            _fakeLight.Received(1).TurnOn();
        }

        [Test]
        public void DoorOpen_StateSETPOWER_IsCorrect()
        {
            //Power button is pressed, changing state to SETPOWER
            _powerButton.Press();

            _door.Open();

            _fakeLight.Received(1).TurnOn();
            _fakeDisplay.Received(1).Clear();
        }

        [Test]
        public void DoorOpen_StateSETTIME_IsCorrect()
        {
            //Power button is pressed, changing state to SETPOWER
            _powerButton.Press();
            //Time button is pressed, changing state to SETTIME
            _timeButton.Press();

            _door.Open();

            _fakeLight.Received(1).TurnOn();
            _fakeDisplay.Received(1).Clear();
        }

        [Test]
        public void DoorOpen_StateCOOKING_IsCorrect()
        {
            //Power button is pressed, changing state to SETPOWER
            _powerButton.Press();
            //Time button is pressed, changing state to SETTIME
            _timeButton.Press();
            //Start/Cancel butoon is pressed, changing state to COOKING
            _startCancelButton.Press();

            _door.Open();

            _fakeCookController.Received(1).Stop();
            _fakeDisplay.Received(1).Clear();
        }

        [Test]
        public void DoorClosed_StateDOOROPEN_IsCorrect()
        {
            _door.Open();

            _door.Close();

            _fakeLight.Received(1).TurnOff();
        }

        #endregion
    }
}
