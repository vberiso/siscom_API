using System;

namespace Siscom.Agua.Api.Exceptions
{
    class RunSpException : Exception
    {
        public RunSpException()
        {

        }

        public RunSpException(string name)
            : base(String.Format(name))
        {

        }

    }
}
