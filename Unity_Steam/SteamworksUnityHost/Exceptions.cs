using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
    public class LicenseException : Exception
    {
        public LicenseException()
        {
        }

        public LicenseException(string message)
            : base(message)
        {
        }

        public LicenseException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}
