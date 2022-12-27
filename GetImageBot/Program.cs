using FlickrNet;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    private const string START = "/start";
    private const string ABOUT = "/about";

    private static TelegramBotClient _botClient;
    private static Flickr _flickr;
    private static Random _random = new Random();


    static void Main(string[] args)
    {
        _botClient = new TelegramBotClient("token");
        _flickr = new Flickr("token");


        CreateCommands();
        StartReceiving();
        Console.ReadLine();
    }

    private static void CreateCommands()
    {
        _botClient.SetMyCommandsAsync(new List<BotCommand>()
        {
            new()
            {
                Command = START,
                Description = "Запустить бота."
            },
            new()
            {
                Command = ABOUT,
                Description = "Что делает бот и как им пользоваться?"
            }
        });
    }

    private static void StartReceiving()
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

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
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

    private static Task HandleError(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }

    private static bool IsStartCommand(string messageText)
    {
        return messageText.ToLower() == START;
    }

    private static bool IsAboutCommand(string messageText)
    {
        return messageText.ToLower() == ABOUT;
    }

    private static async Task<string> GetPhotoUrlAsync(string request)
    {
        var photoSearchOptions = new PhotoSearchOptions
        {
            Text = request,
            SortOrder = PhotoSearchSortOrder.Relevance
        };

        PhotoCollection photos = await _flickr.PhotosSearchAsync(photoSearchOptions);
        var listPhotos = photos.ToList();

        var randomPhotos = _random.Next(0, listPhotos.Count);
        return listPhotos[randomPhotos].LargeUrl;
    }

    private static async Task SendPhotoAsync(long chatId, string request, CancellationToken cancellationToken)
    {
        var photoUrl = await GetPhotoUrlAsync(request);
        await _botClient.SendPhotoAsync(chatId: chatId,
            photo: new InputFileUrl(photoUrl),
            cancellationToken: cancellationToken);
    }
}