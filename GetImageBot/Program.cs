class Program
{
    static void Main(string[] args)
    {
        var bot = new Bot("ВАШ ТОКЕН");
        
        bot.CreateCommands();
        bot.StartReceiving();
        Console.ReadLine();
    }
}