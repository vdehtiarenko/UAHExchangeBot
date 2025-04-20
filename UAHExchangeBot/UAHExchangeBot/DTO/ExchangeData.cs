namespace UAHExchangeBot.DTO
{
    public class ExchangeData
    {
        public string Date { get; set; }
        public string Bank { get; set; }
        public int BaseCurrency { get; set; }
        public string BaseCurrencyLit { get; set; }
        public List<ExchangeRate> ExchangeRate { get; set; }
    }
}
