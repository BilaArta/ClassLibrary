using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary.Interfaces
{
    public interface ILogger
    {
     
        void Log(string message);
        void LogError(string message);
        void LogWarning(string message);
        void LogInfo(string message);
    
    }
}