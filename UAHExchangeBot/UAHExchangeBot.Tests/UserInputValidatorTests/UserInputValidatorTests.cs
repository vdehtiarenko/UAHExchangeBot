using Microsoft.Extensions.Configuration;

namespace UAHExchangeBot.Tests.UserInputValidatorTests
{
    public class UserInputValidatorTests
    {
        private readonly IConfiguration _configuration;

        public UserInputValidatorTests()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        [Fact]
        public void VerifyInputData_CorrectlyThrowsArgumentExceptionWhenInputDataIsEmpty()
        {
            // Arrange

            string inputData = null;
            var userInputValidator = new UserInputValidator(_configuration);

            // Act

            var exception = Assert.Throws<ArgumentException>(() => userInputValidator.VerifyInputData(inputData));

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Input data cannot be empty.", exception.Message);
        }

        [Fact]
        public void VerifyInputData_ReturnsFalseWhenCurrencyCodeIsNotThreeCharacters()
        {
            // Arrange

            string inputData = "USDD 18.02.2021";
            var userInputValidator = new UserInputValidator(_configuration);

            // Act

            var result = userInputValidator.VerifyInputData(inputData);

            // Assert

            Assert.False(result);
        }

        [Fact]
        public void VerifyInputData_ReturnsFalseWhenInputDataIsInIncorrectFormat()
        {
            // Arrange

            string inputData = "USD 2021-02-18";
            var userInputValidator = new UserInputValidator(_configuration);

            // Act

            var result = userInputValidator.VerifyInputData(inputData);

            // Assert

            Assert.False(result);
        }

        [Fact]
        public void VerifyInputData_ReturnsFalseWhenDateIsInvalid()
        {
            // Arrange

            string inputData = "USD 32.13.2021";
            var userInputValidator = new UserInputValidator(_configuration);

            // Act

            var result = userInputValidator.VerifyInputData(inputData);

            // Assert

            Assert.False(result);
        }

        [Fact]
        public void VerifyInputData_ReturnsFalseWhenInputHasTooManyWords()
        {
            // Arrange

            string inputData = "USD 18.02.2021 ExtraWord";
            var userInputValidator = new UserInputValidator(_configuration);


            // Act

            var result = userInputValidator.VerifyInputData(inputData);

            // Assert

            Assert.False(result);
        }

        [Fact]
        public void VerifyInputData_ReturnsTrueWhenInputDataIsCorrect()
        {
            // Arrange

            string inputData = "USD 18.02.2021";
            var userInputValidator = new UserInputValidator(_configuration);

            // Act

            var result = userInputValidator.VerifyInputData(inputData);

            // Assert

            Assert.True(result);
        }

        [Fact]
        public void IsValidCurrencyCode_ThrowsArgumentExceptionWhenCurrencyCodeIsEmpty()
        {
            // Arrange

            string currencyCode = null;
            var userInputValidator = new UserInputValidator(_configuration);

            // Act

            var exception = Assert.Throws<ArgumentException>(() => userInputValidator.IsValidCurrencyCode(currencyCode));

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Currency code cannot be empty.", exception.Message);
        }

        [Fact]
        public void IsValidCurrencyCode_ReturnsFalseWhenCurrencyCodeIsNotInList()
        {
            // Arrange

            string currencyCode = "ABC";
            var userInputValidator = new UserInputValidator(_configuration);

            // Act

            var result = userInputValidator.IsValidCurrencyCode(currencyCode);

            // Assert

            Assert.False(result);
        }

        [Fact]
        public void IsValidCurrencyCode_ReturnsTrueWhenCurrencyCodeIsInList()
        {
            // Arrange

            string currencyCode = "USD";
            var userInputValidator = new UserInputValidator(_configuration);

            // Act

            var result = userInputValidator.IsValidCurrencyCode(currencyCode);

            // Assert

            Assert.True(result);
        }
    }
}
