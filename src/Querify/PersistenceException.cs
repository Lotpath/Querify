using System;

namespace Querify
{
    public class PersistenceException : Exception
    {
        public PersistenceException(string message)
            : base(message)
        {
        }
    }
}