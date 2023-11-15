namespace Translator
{
    public class Program
    {
        public static string Sentence = null;
        public static string Desired_Language = null;
        static void Main(string[] args)
        {
            State();
        }

        public static void State()
        {
            Console.Clear();
            var task1 = Task.Run((Func<Task>)Library.Initialize_Library);
            task1.Wait();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Please state a Sentence to Translate");
            Console.ForegroundColor = ConsoleColor.White;
            Sentence = Console.ReadLine(); Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Please state a Language to translate to");
            Console.ForegroundColor = ConsoleColor.White;
            Desired_Language = Console.ReadLine(); Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Wait!");
            Console.ForegroundColor = ConsoleColor.White;

            var task2 = Task.Run((Func<Task>)Library.GetLanguageEvaluation);
            task2.Wait();

            var task3 = Task.Run((Func<Task>)Library.ConvertLanguage);
            task3.Wait();

            var task4 = Task.Run((Func<Task>)Library.GetTranslation);
            task4.Wait();
        }
    }
}