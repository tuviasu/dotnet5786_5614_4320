namespace Stage0;

 partial class Program
{
    public static void Main(string[] args)
    {
        Welcome5614();
        Welcome4320();
        Console.ReadKey();
    }

    static partial void Welcome4320();
    private static void Welcome5614()
    {
        Console.WriteLine("enter your user name");
        string userName = Console.ReadLine()!;
        Console.WriteLine($"{userName}, welcome to my first application");
    }
}
