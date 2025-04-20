using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using UAHExchangeBot.DTO;

namespace UAHExchangeBot.Services
{
    internal class BankService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly string _url;

        public BankService(IConfiguration configuration)
        {
            _url = configuration["ExchangeRatesAPI:BaseUrl"];
        }

        public async Task<string> GetExchange(string currency, DateTime onDate)
        {
            try
            {
                var endpoint = new Uri($"{ _url }{onDate.ToString("dd.MM.yyyy")}");
                var result = await _httpClient.GetAsync(endpoint);
                var json = await result.Content.ReadAsStringAsync();

                var exchangeData = JsonConvert.DeserializeObject<ExchangeData>(json);

                if (exchangeData.ExchangeRate.Count < 1)
                {
                    return $"No exchange rate data found for UAH to {currency} on {onDate.ToString()}.";
                }
                else
                {
                    var responseMessage = $"Date: {exchangeData.Date}\n" +
                                          $"Bank: {exchangeData.Bank}\n" +
                                          $"Base Currency: {exchangeData.BaseCurrencyLit}\n" +

                                          $"Currency: {currency}\n";
                    var rate = exchangeData.ExchangeRate.
                        FirstOrDefault(r => r.Currency == currency);

                    if (rate != null)
                    {
                        responseMessage += $"Sale - {rate.SaleRateNB} Purchase - {rate.PurchaseRateNB}\n";
                    }

                    return responseMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return "An unforeseen error occurred while processing your request. Please try again later.";
            }
        }
    }
}
