using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microwave.Classes.Interfaces
{
    public interface ICookController
    {
        //Added to fix double association interface issue
        public IUserInterface UI { set; }
        void StartCooking(int power, int time);
        void Stop();
    }
}
