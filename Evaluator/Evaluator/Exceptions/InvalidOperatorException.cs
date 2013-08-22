using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Evaluator.Exceptions
{
    [Obsolete]
    public sealed class InvalidOperatorException : Exception
    {
        public InvalidOperatorException()
            : base("Invalid or uninitalized operator for expression.")
        {
        }

        public InvalidOperatorException(string message) : base(message)
        {
        }

        public InvalidOperatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidOperatorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
