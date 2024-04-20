using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAChat.Utils
{
    public class ThisApplicationException : Exception
    {
        public ThisApplicationException(string message) : base(message)
        {
        }
        public ThisApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
