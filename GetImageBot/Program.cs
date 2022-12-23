class Program
{
    static void Main(string[] args)
    {
        var flickrApi = new FlickrAPI("c77b1b60abbb5751a82a01e545280c8d");
        var bot = new Bot("5899586717:AAFHiuFz6Hp5hl4h150jkqNzWZIvvd5cCkk", flickrApi);
    }
}