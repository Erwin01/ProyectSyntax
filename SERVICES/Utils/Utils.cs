using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SERVICES
{
    public static class Utils
    {
        public static T CallRestService2<T>(
            string url,
            HttpRestMethod method,
            HttpContent data = null,
            string authorization = "",
            string token = "",
            Dictionary<string, string> headers = null)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url);
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(authorization))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorization, token);
                }
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
                HttpResponseMessage response;
                if (method == HttpRestMethod.GET)
                {
                    response = client.GetAsync(string.Empty).Result;
                }
                else
                {
                    var stream = new StreamReader(data.ReadAsStreamAsync().Result);
                    var text = stream.ReadToEnd();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Content = data;
                    response = client.SendAsync(request).Result;
                }
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<T>().Result;

                }
                else
                {
                    throw new Exception(String.Format(Constants.FORMATS.MESSAGES.ERROR_IN_CALL, response.StatusCode, response.ReasonPhrase));
                }
            }
            catch (HttpRequestException ex)
            {

                throw ex;
            }
        }
        public static dynamic CallRESTService<T>(
            string url,
            HttpRestMethod method,
            HttpContent data = null,
            string authorization = "",
            string token = "",
            Dictionary<string,string> headers = null)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url);
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(authorization))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorization, token);
                }
                if(headers != null)
                {
                    foreach(var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                client.Timeout = TimeSpan.FromMinutes(10);
                HttpResponseMessage response;
                if (method == HttpRestMethod.GET)
                {   
                    response = client.GetAsync(string.Empty).Result;
                }
                else
                {
                    response = client.PostAsync(string.Empty, data).Result;
                }
                if (response.IsSuccessStatusCode)
                {
                    if(typeof(T) != typeof(Stream))
                    {
                        return response.Content.ReadAsAsync<T>().Result;
                    }
                    else
                    {
                        var streamResponse = response.Content.ReadAsStreamAsync().Result;
                        using (StreamReader reader = new StreamReader(streamResponse))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    throw new Exception(String.Format(Constants.FORMATS.MESSAGES.ERROR_IN_CALL, response.StatusCode, response.ReasonPhrase));
                }
            }
            catch (HttpRequestException ex)
            {

                throw ex;
            }
        }
    }
}
