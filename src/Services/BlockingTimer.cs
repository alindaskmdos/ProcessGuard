using System;
using System.Threading;

namespace ProcessGuard.Services
{
    public class BlockerTimer
    {
        private Timer _timer;
        private DateTime _startTime;
        private DateTime _endTime;
        private bool _isBlocking;

        public event Action BlockingStarted;
        public event Action BlockingEnded;

        public BlockerTimer(DateTime startTime, DateTime endTime)
        {
            _startTime = startTime;
            _endTime = endTime;

            _timer = new Timer(CheckTimeAndProcess, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private void CheckTimeAndProcess(object state)
        {
            DateTime currentTime = DateTime.Now;

            bool shouldBlock = IsWithinBlockingPeriod(currentTime);

            if (shouldBlock && !_isBlocking)
            {
                StartBlocking();
            }
            else if (!shouldBlock && _isBlocking)
            {
                StopBlocking();
            }
        }

        private bool IsWithinBlockingPeriod(DateTime currentTime)
        {
            TimeSpan currentTimeOfDay = currentTime.TimeOfDay;
            TimeSpan startTimeOfDay = _startTime.TimeOfDay;
            TimeSpan endTimeOfDay = _endTime.TimeOfDay;

            if (startTimeOfDay <= endTimeOfDay)
            {
                return currentTimeOfDay >= startTimeOfDay && currentTimeOfDay <= endTimeOfDay;
            }
            else
            {
                return currentTimeOfDay >= startTimeOfDay || currentTimeOfDay <= endTimeOfDay;
            }
        }

        private void StartBlocking()
        {
            _isBlocking = true;
            BlockingStarted?.Invoke();
        }

        private void StopBlocking()
        {
            _isBlocking = false;
            BlockingEnded?.Invoke();
        }

        public void UpdateSchedule(DateTime newStartTime, DateTime newEndTime)
        {
            _startTime = newStartTime;
            _endTime = newEndTime;
        }

        public void Dispose()
        {
            BlockingStarted = null;
            BlockingEnded = null;
            _timer?.Dispose();
        }
    }
}