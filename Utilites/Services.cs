using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilites
{
    public static class Services
    {
        public static async Task<string> CallApiUrlAsync(string url, Dictionary<string, string> headers = null, string content = null) 
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Set headers
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }

                    HttpResponseMessage response;
                    if (string.IsNullOrEmpty(content))
                    {
                        // Perform a GET request
                        response = await client.GetAsync(url);
                    }
                    else
                    {
                        // Perform a POST request with content in the request body
                        HttpContent requestBody = new StringContent(content, Encoding.UTF8, "application/json");
                        response = await client.PostAsync(url, requestBody);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        return responseData;
                    }
                    else
                    {
                        // Handle unsuccessful response
                        throw new Exception($"API request failed with status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exception that occurs during the API call
                throw new Exception($"API request failed: {ex.Message}");
            }
        }

    }
}
