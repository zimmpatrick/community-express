using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace CESDKLicense
{
    public class CESDKLicense
    {
        public static void AppendLicenseData(XmlDocument xmldoc)
        {
            // Get the key pair from the key store.
            CspParameters parms = new CspParameters(1);
            parms.Flags = CspProviderFlags.UseMachineKeyStore;
            parms.KeyContainerName = "CommunityExpressSDK";
            parms.KeyNumber = 2;
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(parms);

            // Creating the XML signing object.
            SignedXml sxml = new SignedXml(xmldoc);
            sxml.SigningKey = csp;

            // Set the canonicalization method for the document.
            sxml.SignedInfo.CanonicalizationMethod =
              SignedXml.XmlDsigCanonicalizationUrl; // No comments.


            // Create an empty reference (not enveloped) for the XPath
            // transformation.
            Reference r = new Reference("");

            // Create the XPath transform and add it to the reference list.
            r.AddTransform(new XmlDsigEnvelopedSignatureTransform(false));

            // Add the reference to the SignedXml object.
            sxml.AddReference(r);

            // Compute the signature.
            sxml.ComputeSignature();

            // Get the signature XML and add it to the document element.
            XmlElement sig = sxml.GetXml();
            xmldoc.DocumentElement.AppendChild(sig);
        }
    }
}
