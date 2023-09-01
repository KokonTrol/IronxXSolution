namespace CheckingGamesUpdates
{
    class Program
    {

        static async Task Main(string[] args)
        {
            await CGULibrary.Checking.Start(args, true);
        }
    }
}
