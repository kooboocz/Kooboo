﻿using Kooboo.Data.Interface;
using System; 

namespace Kooboo.Data.Ensurance
{ 
    public class QueueWorker : IBackgroundWorker
    {
        public int Interval
        {
            get
            {
                return 300;
            }
        }

        public DateTime LastExecute
        {
            get; set;
        }

        public void Execute()
        {
            EnsureManager.Execute(); 
        }
    }


}
