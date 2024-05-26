using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class CryptoInformationHelper
    {
        private readonly static string PrimaryCurrency = "USD";
        private readonly static string API_Url = "https://cex.io/api/last_price";

        public static async Task<double> CheckPrice(string currencyName)
        {
            using (HttpClient client = new HttpClient())
            {
                var res = await client.GetAsync($"{API_Url}/{currencyName}/{PrimaryCurrency}");
                if (res.IsSuccessStatusCode)
                {
                    var content = await res.Content.ReadFromJsonAsync<CryptoCurrencyInfo>();
                    return content.lprice;

                }
                return 0;
            }
        }

    }
}
