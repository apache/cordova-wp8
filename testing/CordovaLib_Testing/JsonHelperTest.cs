using NUnit.Framework;
using System.Collections.Generic;
using WPCordovaClassLib.Cordova.JSON;

namespace CordovaLib_Testing
{
    [TestFixture]
    public class JsonHelperTest
    {
        [Test]
        public void SimpleJsonDeserializeTest()
        {
            string input = "[\"///CapturedImagesCache/WP_20141010_001.jpg\",\"http://foo.bar.net/foobar\",\"file\",\"WP_20141010_001.jpg\",\"image/jpg\",\"{}\",null,\"false\", \"1\",\"POST\",\"FileTransfer1982537557\"]";

            string[] args = JsonHelper.Deserialize<string[]>(input);

            Assert.AreEqual(11, args.Length);
            Assert.AreEqual("///CapturedImagesCache/WP_20141010_001.jpg", args[0]);
            Assert.AreEqual("http://foo.bar.net/foobar", args[1]);
            Assert.AreEqual("file", args[2]);
            Assert.AreEqual("WP_20141010_001.jpg", args[3]);
            Assert.AreEqual("image/jpg", args[4]);
            Assert.AreEqual("{}", args[5]);
            Assert.IsNull(args[6]);
            Assert.AreEqual("false", args[7]);
            Assert.AreEqual("1", args[8]);
            Assert.AreEqual("POST", args[9]);
            Assert.AreEqual("FileTransfer1982537557", args[10]);
        }

        [Test]
        public void SimpleJsonSerializeTest()
        {
            string input = "[\"///CapturedImagesCache/WP_20141010_001.jpg\",\"http://foo.bar.net/foobar\",\"file\",\"WP_20141010_001.jpg\",\"image/jpg\",\"{}\",null,\"false\",\"1\",\"POST\",\"FileTransfer1982537557\"]";

            string[] args = { "///CapturedImagesCache/WP_20141010_001.jpg", "http://foo.bar.net/foobar", "file", "WP_20141010_001.jpg", "image/jpg", "{}", null, "false", "1", "POST", "FileTransfer1982537557" };

            string compiled = JsonHelper.Serialize(args);

            Assert.AreEqual(input, compiled);
        }

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

            Assert.AreEqual("{\"x-wsse\":\"UsernameToken Username=\\\"foo@bar.net\\\", PasswordDigest=\\\"ZTQXXXXXXXXXXXXXXXXXXXXXXXXXXXX0NTJlMGE2M2I0NGJiYWM4NA==\\\", Nonce=\\\"XXXXXXXXXXXXXXXXXXXXXXXXXXXXA==\\\", Created=\\\"2014-10-11T18:48:07.232Z\\\"\"}", compiled);
        }
    }
}
