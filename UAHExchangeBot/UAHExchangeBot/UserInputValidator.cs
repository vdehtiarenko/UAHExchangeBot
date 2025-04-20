using Microsoft.Extensions.Configuration;

namespace UAHExchangeBot
{
    public class UserInputValidator
    {
        private readonly List<string> _validCurrencyCodes;

        public UserInputValidator(IConfiguration configuration)
        {
            _validCurrencyCodes = configuration.GetSection("ValidCurrencyCodes").Get<List<string>>();
        }

        public bool VerifyInputData(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
            {
                throw new ArgumentException("Input data cannot be empty.");
            }

            string[] words = inputData.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 2)
            {
                if (words[0].Length == 3 && DateTime.TryParseExact(words[1], "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsValidCurrencyCode(string currencyCode)
        {
            if(string.IsNullOrEmpty(currencyCode))
            {
                throw new ArgumentException("Currency code cannot be empty.");
            }

            return _validCurrencyCodes.Contains(currencyCode.ToUpper());
        }
    }
}
