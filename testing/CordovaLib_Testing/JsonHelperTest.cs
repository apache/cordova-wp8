using NUnit.Framework;
using System.Collections.Generic;
using WPCordovaClassLib.Cordova.JSON;

namespace CordovaLib_Testing
{
    [TestFixture]
    public class JsonHelperTest
    {

        [Test]
        public void ComplexJsonDeserializeTest()
        {
            string complexHeader = "{\"x-wsse\":\"UsernameToken Username=\\\"foo@bar.net\\\", PasswordDigest=\\\"ZTQXXXXXXXXXXXXXXXXXXXXXXXXXXXX0NTJlMGE2M2I0NGJiYWM4NA==\\\", Nonce=\\\"XXXXXXXXXXXXXXXXXXXXXXXXXXXXA==\\\", Created=\\\"2014-10-11T18:48:07.232Z\\\"\"}";

            Dictionary<string, string> decompiled = JsonHelper.Deserialize<Dictionary<string, string>>(complexHeader);

            Assert.IsNotNull(decompiled);
            Assert.True(decompiled.ContainsKey("x-wsse"));
        }

        [Test]
        public void ComplexJsonSerializeTest()
        {
            Dictionary<string, string> decompiled = new Dictionary<string, string>();
            decompiled.Add("x-wsse", "UsernameToken Username=\"foo@bar.net\", PasswordDigest=\"ZTQXXXXXXXXXXXXXXXXXXXXXXXXXXXX0NTJlMGE2M2I0NGJiYWM4NA==\", Nonce=\"XXXXXXXXXXXXXXXXXXXXXXXXXXXXA==\", Created=\"2014-10-11T18:48:07.232Z\"");

            string compiled = JsonHelper.Serialize(decompiled);

            Assert.AreEqual(compiled, "{\"x-wsse\":\"UsernameToken Username=\\\"foo@bar.net\\\", PasswordDigest=\\\"ZTQXXXXXXXXXXXXXXXXXXXXXXXXXXXX0NTJlMGE2M2I0NGJiYWM4NA==\\\", Nonce=\\\"XXXXXXXXXXXXXXXXXXXXXXXXXXXXA==\\\", Created=\\\"2014-10-11T18:48:07.232Z\\\"\"}");
        }
    }
}
