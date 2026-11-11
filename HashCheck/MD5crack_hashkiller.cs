using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
namespace HashCheck
{
    public class MD5crack_hashkiller : MD5crack
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public override MD5crack_result crack(string hash_string)
        {
            MD5crack_result mD5crack_result = new MD5crack_result(hash_string);
            XElement response;
            try
            {
                response = this.GetResponse("http://hashkiller.com/api/api.php?md5=" + mD5crack_result.getHash());
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
                string value = result.Element("found").Value;
                if (!bool.Parse(value))
                {
                    return hash;
                }
                try
                {
                    hash.enterCrackResult(result.Element("plain").Value);
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
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("User-Agent", ".NET Cracker");
            request.Headers.ConnectionClose = true;

            using var cts = new System.Threading.CancellationTokenSource(15000);
            var response = _httpClient.SendAsync(request, cts.Token).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                return XElement.Parse(content);
            }
            throw new Exception("Error fetching data.");
        }
    }
}
