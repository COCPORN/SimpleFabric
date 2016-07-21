using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    class Timer : IActorTimer
    {
        System.Timers.Timer timer;
        Func<object, Task> asyncCallback;
        ActorBase actorBase;
        object state;
        public Timer(TimeSpan dueTime, TimeSpan period, Func<object, Task> asyncCallback, object state, ActorBase actorBase)
        {
            DueTime = dueTime;
            Period = period;
            this.asyncCallback = asyncCallback;
            this.actorBase = actorBase;
            this.state = state;
            timer = new System.Timers.Timer(dueTime.Milliseconds);
            timer.AutoReset = true;                        
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        bool setToPeriodTimer = false;
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (setToPeriodTimer == false)
            {
                timer.Interval = Period.Milliseconds;
            }
            actorBase.LockManager.Lock();
            try
            {
                asyncCallback(state);
            }
            finally
            {
                actorBase.LockManager.Unlock();
            }
        }

        public TimeSpan DueTime
        {
            get; private set;           
        }

        public TimeSpan Period
        {
            get; private set;
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
