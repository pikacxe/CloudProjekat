using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

using Common.DTOs;

using Newtonsoft.Json;

namespace PortfolioService.Controllers
{
    public class CurrencyController : Controller
    {
        // GET: Currency
        public ActionResult Index()
        {
            return View();
        }

        // GET: Currency/Search
        [HttpGet]
        public async Task<ActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<CryptoCurrency>());
            }

            // Fetch the list of cryptocurrencies
            var cryptoList = await GetCryptoList();
            if (cryptoList == null)
            {
                return View("Error");
            }

            // Filter cryptocurrencies by the search query using IndexOf for case-insensitive comparison
            var filteredCryptos = cryptoList
                .Where(c => c.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (!filteredCryptos.Any())
            {
                return View(new List<CryptoCurrency>());
            }

            // Get the IDs of the filtered cryptocurrencies
            var ids = string.Join(",", filteredCryptos.Select(c => c.Id));

            // Fetch the prices for the filtered cryptocurrencies
            var cryptoPrices = await GetCryptoPrices(ids);
            if (cryptoPrices == null)
            {
                return View("Error");
            }

            // Combine the results
            var result = filteredCryptos.Select(c => new CryptoCurrency
            {
                Id = c.Id,
                Name = c.Name,
                Price = cryptoPrices.ContainsKey(c.Id) ? cryptoPrices[c.Id].Usd : (decimal?)null
            }).ToList();

            return View(result);
        }

        private async Task<List<Crypto>> GetCryptoList()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync("https://api.coingecko.com/api/v3/coins/list");
                return JsonConvert.DeserializeObject<List<Crypto>>(response);
            }
        }

        private async Task<Dictionary<string, CryptoPrice>> GetCryptoPrices(string ids)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"https://api.coingecko.com/api/v3/simple/price?ids={ids}&vs_currencies=usd");
                return JsonConvert.DeserializeObject<Dictionary<string, CryptoPrice>>(response);
            }
        }
    }
}
