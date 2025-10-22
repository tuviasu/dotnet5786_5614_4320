namespace Stage0;

partial class Program
{
    static partial void Welcome4320()
    {
        Console.WriteLine("enter your user name");
        string userName = Console.ReadLine()!;
        Console.WriteLine($"{userName}, welcome to my first application");
    }
}
