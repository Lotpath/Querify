using System;

namespace Querify
{
    public class QuerifyException : Exception
    {
        public QuerifyException(string message)
            : base(message)
        {
        }
    }

    public class NoMatchFoundException : QuerifyException
    {
        public NoMatchFoundException(string message)
            : base(message)
        {
        }
    }
}