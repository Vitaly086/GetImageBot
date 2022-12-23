using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class Bot
{
    private readonly FlickrAPI _flickrApi;
    private readonly TelegramBotClient _botClient;

    
    public Bot(string token, FlickrAPI flickrApi)
    {
        _flickrApi = flickrApi;
        _botClient = new TelegramBotClient(token);

        CreateCommands();
        StartReceiving();
        Console.ReadLine();
    }

    private void CreateCommands()
    {
        _botClient.SetMyCommandsAsync(new List<BotCommand>()
        {
            new()
            {
                Command = CustomBotCommands.START,
                Description = "Запустить бота."
            },
            new()
            {
                Command = CustomBotCommands.ABOUT,
                Description = "Что делает бот и как им пользоваться?"
            }
        });
    }

    private void StartReceiving()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var receiverOptions = new ReceiverOptions
        {
            // разрешено получать все виды апдейтов
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleError,
            receiverOptions,
            cancellationToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;

        if (string.IsNullOrEmpty(update.Message.Text))
        {
            await _botClient.SendTextMessageAsync(chatId,
                text: "Данный бот принимает только текстовые сообщения.\n" +
                      "Введите ваш запрос правильно.",
                cancellationToken: cancellationToken);
            return;
        }

        var messageText = update.Message.Text;

        if (IsStartCommand(messageText))
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Привет, я бот по поиску картинок. Введите ваш запрос.",
                cancellationToken: cancellationToken);
            return;
        }

        if (IsAboutCommand(messageText))
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Данный бот возвращает 1 картинку по запросу пользователя. \n" +
                      "Чтобы получить картинку, введите текстовый запрос.",
                cancellationToken: cancellationToken);
            return;
        }

        await SendPhotoAsync(chatId, messageText, cancellationToken);
    }

    private Task HandleError(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }

    private bool IsStartCommand(string messageText)
    {
        return messageText.ToLower() == CustomBotCommands.START;
    }

    private bool IsAboutCommand(string messageText)
    {
        return messageText.ToLower() == CustomBotCommands.ABOUT;
    }

    private async Task SendPhotoAsync(long chatId, string request, CancellationToken cancellationToken)
    {
        var photoUrl = await _flickrApi.GetPhotoUrlAsync(request);
        await _botClient.SendPhotoAsync(chatId: chatId,
            photo: new InputFileUrl(photoUrl),
            cancellationToken: cancellationToken);
    }
}