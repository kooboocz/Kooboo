//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Logging
{
    public interface ILogger
    {
        void LogDebug(string message);

        void LogInformation(string message);

        void LogWarning(string message);

        void LogError(Exception ex, string message);

        void LogCritical(Exception ex, string message);
    }
}
