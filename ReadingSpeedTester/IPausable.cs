using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadingSpeedTester
{
    interface IPausable
    {
        bool isActive();
        bool isPaused();
        void pause();
        void activate();
        void toggleActivity();
    }
}
