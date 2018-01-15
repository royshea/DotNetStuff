using System;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;


namespace AzureQueueDemo
{
    class AzureQueueConnection
    {
        private HttpClient client { get; }
        private const string versionHeader = "x-ms-version";
        private const string versionName = "2009-09-19";

        // Load configuration
        private string storageAccountName = ConfigurationManager.AppSettings["storageAccountName"];
        private string azureQueueUri = ConfigurationManager.AppSettings["azureQueueUri"];
        private string storageAccountKey = ConfigurationManager.AppSettings["storageAccountKey"];

        public AzureQueueConnection()
        {
            client = new HttpClient();
            SetCommonHeaders();
        }

        private void SetCommonHeaders()
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(versionHeader, versionName);
        }

        private void PopulateRequestHeaders(string action, string resource)
        {
            client.BaseAddress = new Uri(azureQueueUri);

            // NOTE: This is time sensitive.  After this header has been set
            // the request must be received by Azure queue within a fixed
            // amount of time.
            var RequestDateString = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            if (client.DefaultRequestHeaders.Contains("x-ms-date"))
            {
                client.DefaultRequestHeaders.Remove("x-ms-date");
            }
            client.DefaultRequestHeaders.Add("x-ms-date", RequestDateString);

            // Create authorization header.
            string contentEncoding = "";
            string contentLanguage = "";
            string contentLength = "";
            string contentMd5 = "";
            string contentType = "";
            string date = "";
            string ifModifiedSince = "";
            string ifMatch = "";
            string ifNoneMatch = "";
            string ifUnmodifiedSince = "";
            string range = "";
            string canonicalizedHeaders = $"x-ms-date:{RequestDateString}\n{versionHeader}:{versionName}\n";
            string canonicalizedResource = resource;

            string canonicalizedStringToBuild =
               $"{action}\n" +
               $"{contentEncoding}\n" +
               $"{contentLanguage}\n" +
               $"{contentLength}\n" +
               $"{contentMd5}\n" +
               $"{contentType}\n" +
               $"{date}\n" +
               $"{ifModifiedSince}\n" +
               $"{ifMatch}\n" +
               $"{ifNoneMatch}\n" +
               $"{ifUnmodifiedSince}\n" +
               $"{range}\n" +
               $"{canonicalizedHeaders}" +
               $"{canonicalizedResource}";

            string signature;
            using (var hmac = new HMACSHA256(Convert.FromBase64String(storageAccountKey)))
            {
                byte[] dataToHmac = Encoding.UTF8.GetBytes(canonicalizedStringToBuild);
                signature = Convert.ToBase64String(hmac.ComputeHash(dataToHmac));
            }

            string authorizationHeader = string.Format($"{storageAccountName}:" + signature);
            if (client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Remove("Authorization");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);
        }

        public async Task<string> ListQueues()
        {
            const string action = "GET";
            string resoure = $"/{storageAccountName}/\ncomp:list";
            PopulateRequestHeaders(action, resoure);
            return await MakeGetRequest();
        }

        private async Task<string> MakeGetRequest()
        {
            string resultString;
            HttpResponseMessage response = await client.GetAsync("");
            if (response.IsSuccessStatusCode)
            {
                byte[] result = await response.Content.ReadAsByteArrayAsync();
                resultString = Encoding.UTF8.GetString(result);
            }
            else
            {
                resultString = response.ToString();
            }
            return resultString;
        }
    }
}
