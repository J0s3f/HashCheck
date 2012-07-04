using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
namespace HashCheck
{
    public class MD5crack_c0llision : MD5crack
    {
        public override MD5crack_result crack(string hash_string)
        {
            MD5crack_result mD5crack_result = new MD5crack_result(hash_string);
            XElement response;
            try
            {
                response = this.GetResponse("http://api.dev.c0llision.net/xml/crack/md5/" + mD5crack_result.getHash());
            }
            catch (Exception)
            {
                return mD5crack_result;
            }
            mD5crack_result = this.parseResult(response, mD5crack_result);
            return mD5crack_result;
        }
        protected MD5crack_result parseResult(XElement result, MD5crack_result hash)
        {
            try
            {
                string value = result.Element("status").Element("code").Value;
                if (value != "200")
                {
                    return hash;
                }
                string value2 = result.Element("result").Element("cracked").Value;
                if (!bool.Parse(value2))
                {
                    return hash;
                }
                try
                {
                    hash.enterCrackResult(result.Element("result").Element("plaintext").Element("raw").Value);
                }
                catch (Exception)
                {
                }
            }
            catch (Exception e)
            { }
            return hash;
        }
        protected XElement GetResponse(string uri)
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;
            httpWebRequest.UserAgent = ".NET Cracker";
            httpWebRequest.KeepAlive = false;
            httpWebRequest.Timeout = 15000;
            HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
            if (httpWebRequest.HaveResponse && httpWebResponse != null)
            {
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                return XElement.Parse(streamReader.ReadToEnd());
            }
            throw new Exception("Error fetching data.");
        }
    }
}
