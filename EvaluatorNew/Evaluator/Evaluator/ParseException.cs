using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    public sealed class ParseException : Exception
    {
        public string FailedInput { get; private set; }

        public ParseException() : base("An expression could not be parsed.")
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception inner) : base(message, inner)
        {
        }

        public ParseException(string message, string failedInput) : base(message)
        {
            this.FailedInput = failedInput;
        }

        public ParseException(string message, string failedInput, Exception inner) : base(message, inner)
        {
            this.FailedInput = failedInput;
        }
    }
}
