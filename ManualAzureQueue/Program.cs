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
    class Program
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            MakeWebRequest().Wait();
            return;
        }

        static async Task MakeWebRequest()
        {
            string azureQueueUri = ConfigurationManager.AppSettings["azureQueueUri"];
            client.BaseAddress = new Uri(azureQueueUri);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var RequestDateString = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            if (client.DefaultRequestHeaders.Contains("x-ms-date"))
            {
                client.DefaultRequestHeaders.Remove("x-ms-date");
            }
            client.DefaultRequestHeaders.Add("x-ms-date", RequestDateString);
            client.DefaultRequestHeaders.Add("x-ms-version", "2009-09-19");

            if (client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Remove("Authorization");
            }

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
            string canonicalizedHeaders = $"x-ms-date:{RequestDateString}\nx-ms-version:2009-09-19\n";
            string canonicalizedResource = $"/thehome/\ncomp:list";

            string canonicalizedStringToBuild =
               $"GET\n" +
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
            string storageAccountKey = ConfigurationManager.AppSettings["storageAccountKey"];
            using (var hmac = new HMACSHA256(Convert.FromBase64String(storageAccountKey)))
            {
                byte[] dataToHmac = Encoding.UTF8.GetBytes(canonicalizedStringToBuild);
                signature = Convert.ToBase64String(hmac.ComputeHash(dataToHmac));
            }

            string authorizationHeader = string.Format($"thehome:" + signature);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedKey", authorizationHeader);

            try
            {
                byte[] result = null;
                HttpResponseMessage response = await client.GetAsync("");
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsByteArrayAsync();
                    Console.WriteLine(response);
                    Console.WriteLine(Encoding.UTF8.GetString(result));
                }
                else
                {
                    Console.WriteLine(response);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }
    }
}

