using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UAHExchangeBot;
using Microsoft.Extensions.Configuration;
using UAHExchangeBot.Services;
using System.Globalization;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

string botToken = configuration["TelegramBot:Token"];

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(botToken);
var me = await bot.GetMe();
bot.OnMessage += OnMessage;
bot.OnError += OnError;

await bot.SetMyCommandsAsync(new[]
{
    new BotCommand { Command = "/start", Description = "Start the bot"},
    new BotCommand { Command = "/help", Description = "Get instructions on how to use the bot" },
    new BotCommand { Command = "/currencies", Description = "Show all available currency codes" }
});

Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel();

async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception);
}

async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text != null)
    {
        if (msg.Text.StartsWith("/start"))
        {
            HandleStartCommand(msg.Chat.Id);

        }
        else if (msg.Text == "/help")
        {
            HandleHelpCommand(msg.Chat.Id);
        }
        else if (msg.Text.StartsWith("/currencies"))
        {
            HandleCurrenciesCommand(msg.Chat.Id);
        }
        else
        {
            var userInputValidator = new UserInputValidator(configuration);

            bool isInputValid = userInputValidator.VerifyInputData(msg.Text);

            if (!isInputValid)
            {
                await bot.SendMessage(msg.Chat, "Incorrect format! Please enter the currency code and date in the format: <currency code> <date (dd.mm.yyyy)>\nEample: USD 18.02.2021");
            }
            else
            {
                string[] words = msg.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                bool isCurrencyCodeValid = userInputValidator.IsValidCurrencyCode(words[0]);

                if (!isCurrencyCodeValid)
                {
                    await bot.SendMessage(msg.Chat, "Invalid currency code! To see the list of supported currency codes, use the command `/currencies`.");

                }
                else
                {
                    string currencyCode = words[0].ToUpper();

                    DateTime dateTime;
                    bool isValidDate = DateTime.TryParseExact(words[1], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);

                    var bankService = new BankService(configuration);

                    string responseMessage = await bankService.GetExchange(currencyCode, dateTime);

                    await bot.SendMessage(msg.Chat, responseMessage);
                }
            }
        }
    }
}

async Task HandleStartCommand(long chatId)
{
    string message = 
@"Welcome! I can help you check exchange rates for foreign currencies to UAH.

To get started, please enter the currency code (e.g. USD) and the date (in format dd.MM.yyyy), like this: `USD 18.02.2021`.

If you need help, just type `/help`.";

    await bot.SendMessage(chatId, message); 
}

async Task HandleHelpCommand(long chatId)
{
    string message = 
@"Here’s how to use me:

1. Enter the currency code (3-letter code) such as `USD`, `EUR`, `GBP`, etc.
2. Enter the date in the format `dd.MM.yyyy`.
For example, `USD 18.02.2021` to get the exchange rate for USD on February 18, 2021.

I will reply with the exchange rate to UAH for that currency on the specified date.

If the input is invalid, I will notify you accordingly.

To see the list of available currencies, use the command `/currencies`.";

    await bot.SendMessage(chatId, message);
}

async Task HandleCurrenciesCommand(long chatId)
{
    string message = 
@"USD - US Dollar
EUR - Euro
CHF - Swiss Franc
GBP - British Pound
PLZ - Polish Zloty
SEK - Swedish Krona
XAU - Gold
CAD - Canadian Dollar";

    await bot.SendMessage(chatId, message);
}

