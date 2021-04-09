using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Optional;

namespace Fhi.Smittesporing.Varsling.Eksternetjenester
{
    public static class WebApiExtensions
    {
        public static HttpContent SomJson(this object data)
        {
            var json = JsonConvert.SerializeObject(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public static async Task<TResult> GetJson<TResult>(this HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(responseBody);
        }

        public static async Task<Option<TResult>> OptionalGetJson<TResult>(this HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Option.None<TResult>();
            }

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return Option.Some<TResult>(JsonConvert.DeserializeObject<TResult>(responseBody));
        }

        public static async Task<TResult> PostJson<TResult>(this HttpClient httpClient, string url, object request)
        {
            var response = await httpClient.PostAsync(url, request.SomJson());

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(responseBody);
        }

        public static async Task<HttpResponseMessage> PostJson(this HttpClient httpClient, string url, object request)
        {
            return await httpClient.PostAsync(url, request.SomJson());
        }

        public static string LeggTilQuery(this string url, object query)
        {
            var queryParams = query.GetType().GetProperties()
                .Where(p => p.GetValue(query, null) != null)
                .Select(p => $"{Uri.EscapeDataString(char.ToLower(p.Name[0]) + p.Name.Substring(1))}={Uri.EscapeDataString(GetQueryStringValue(p.GetValue(query)))}")
                .ToArray();

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
            return url + queryString;
        }

        public static string LeggTilOptionalQuery<T>(this string url, Option<T> optionalQuery)
        {
            return optionalQuery.Match(none: () => url, some: query => url.LeggTilQuery(query));
        }

        private static string GetQueryStringValue(object queryElement)
        {
            if (queryElement is DateTime queryDate)
            {
                return queryDate.ToString("O");
            }

            return queryElement.ToString();
        }
    }
}