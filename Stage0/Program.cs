namespace Stage0;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("enter your user name");
        string userName = Console.ReadLine()!;
        Console.WriteLine($"{userName}, welcome to my first application");
        Console.ReadKey();
    }
}
