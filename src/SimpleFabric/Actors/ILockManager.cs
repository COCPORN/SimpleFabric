﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors
{
    public interface ILockManager
    {
        void Lock();
        void Unlock();
    }
}
