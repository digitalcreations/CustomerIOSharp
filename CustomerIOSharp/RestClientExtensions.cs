using System;
using System.Threading.Tasks;
using RestSharp;

namespace CustomerIOSharp
{
    /// <summary>
    /// Extension methods to support async calls with RestSharp.
    /// </summary>
    /// <seealso cref="http://ianobermiller.com/blog/2012/07/23/restsharp-extensions-returning-tasks/" />
    public static class RestClientExtensions
    {
        private static Task<T> ExecuteAsync<T>(this RestClient client, IRestRequest request, Func<IRestResponse, T> selector)
        {
            var tcs = new TaskCompletionSource<T>();
            var loginResponse = client.ExecuteAsync(request, r =>
                {
                    if (r.ErrorException == null)
                    {
                        tcs.SetResult(selector(r));
                    }
                    else
                    {
                        tcs.SetException(r.ErrorException);
                    }
                });
            return tcs.Task;
        }

        public static Task<IRestResponse> ExecuteAsync(this RestClient client, IRestRequest request)
        {
            return client.ExecuteAsync(request, r => r);
        }
    }
}