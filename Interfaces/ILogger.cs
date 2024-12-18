using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary.Interfaces
{
    public interface ILogger
    {
     
        static abstract void Error(string message);
        static abstract void Warning(string message);
        static abstract void Info(string message);
    
    }
}