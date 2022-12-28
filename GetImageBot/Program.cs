class Program
{
    static void Main(string[] args)
    {
        var bot = new Bot("5899586717:AAFHiuFz6Hp5hl4h150jkqNzWZIvvd5cCkk");
        
        bot.CreateCommands();
        bot.StartReceiving();
        Console.ReadLine();
    }
}