using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
    /// <summary>
    /// License Exception
    /// </summary>
    public class LicenseException : Exception
    {
        /// <summary>
        /// License exception
        /// </summary>
        public LicenseException()
        {
        }
        /// <summary>
        /// License exception
        /// </summary>
        /// <param name="message">Message shown to the user</param>
        public LicenseException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// License exception
        /// </summary>
        /// <param name="message">Message shown to the user</param>
        /// <param name="e">Exception</param>
        public LicenseException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}
