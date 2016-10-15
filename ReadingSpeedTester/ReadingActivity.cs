﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadingSpeedTester
{
   public class ReadingActivity : IPausable
    {
        private bool active = false;
        private long totalPauseTimeMs = 0;
        private long totalActiveTimeMs = 0;
        DateTime lastStateChangingTime;

       public ReadingActivity()
        {
            lastStateChangingTime = DateTime.Now;
        }

        public long getTotalPauseTimeMs()
        {
            return totalPauseTimeMs;
        }

        public long getTotalActiveTimeMs()
        {
            return totalActiveTimeMs;
        }
        public void activate()
        {
            if (active) return;//already active, do nothing
            active = true;
            long pauseTime = TimeUtils.deltaBetweenTwoDatesMs(lastStateChangingTime, DateTime.Now);
            totalPauseTimeMs += pauseTime;
            lastStateChangingTime = DateTime.Now;
        }

        public bool isPaused()
        {
            return !active;
        }

        public void pause()
        {
            if (!active) return;
            active = false;
            long activeTime = TimeUtils.deltaBetweenTwoDatesMs(lastStateChangingTime, DateTime.Now);
            totalActiveTimeMs += activeTime;
            lastStateChangingTime = DateTime.Now;
        }

        public void toggleActivity()
        {
            if (active) pause();
            else activate();
        }

        bool IPausable.isActive()
        {
            return active;
        }

    }
}
