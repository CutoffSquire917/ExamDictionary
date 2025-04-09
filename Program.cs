using log4net;
using log4net.Config;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;
using System.Reflection;
using static VerbalDictionary;

static class MyExtension
{
    public static string ToTitle(this string str) => char.IsLetter(str[0]) ? char.ToUpper(str[0]).ToString() + str.Substring(1) : str;
}

internal class Program
{

    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    static void Main(string[] args)
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

        int enter = 0;
        VerbalDictionary? verbalDictionary = null;
        while (enter != 4)
        {
            try
            {
                if (verbalDictionary == null)
                {
                    Console.Write("Enter name for dictionary: ");
                    verbalDictionary = new(Console.ReadLine());
                    Console.Clear();
                }   
                Console.WriteLine($"\n\t{verbalDictionary.Name.ToTitle()}\n");
                enter = ChoicePanel("Add", "Remove", "Show all", "Save", "Exit");
                

                switch (enter)
                {
                    case 0:
                        Console.Clear();
                        Console.WriteLine($"\n\t Add to {verbalDictionary.Name.ToTitle()}\n");
                        Console.Write("Enter word: ");
                        string word = Console.ReadLine();
                        List<string> translates = new();
                        while (enter != 1)
                        {
                            Console.Write("Translate: ");
                            translates.Add(Console.ReadLine());

                            Console.WriteLine();
                            enter = ChoicePanel("Continue", "End");
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            Console.WriteLine(new string(' ', Console.WindowWidth));
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                        }
                        verbalDictionary.AddTranslation(word, translates.ToArray());
                        break;
                    case 1:
                        Console.Clear();
                        if (verbalDictionary.Translations.Count <= 0)
                        {
                            break;
                        }
                        Console.WriteLine($"\n\t Remove from {verbalDictionary.Name.ToTitle()}\n");
                        int tempEnter = ChoicePanel(verbalDictionary.Translations.Keys.ToArray().Append("Exit").ToArray());
                        if (tempEnter == verbalDictionary.Translations.Count)
                        {
                            break;
                        }
                        string tempKey = verbalDictionary.Translations.ElementAt(tempEnter).Key;

                        if (verbalDictionary.Translations[tempKey].Count > 1) {
                            tempEnter = ChoicePanel(verbalDictionary.Translations[tempKey].ToArray().Append($"Delete {tempKey}").Append("Exit").ToArray());
                            if (tempEnter == verbalDictionary.Translations[tempKey].Count) {
                                verbalDictionary.Remove(tempKey);
                            }
                            else if (tempEnter == verbalDictionary.Translations[tempKey].Count + 1)
                            {
                                break;
                            }
                            else {
                                verbalDictionary.Translations[tempKey].RemoveAt(tempEnter);
                            }
                        }
                        else {
                            tempEnter = ChoicePanel($"Delete {tempKey}", "Exit");
                            switch (tempEnter)
                            {
                                case 0:
                                    verbalDictionary.Remove(tempKey);
                                    break;
                                case 1:
                                    break;
                            }
                        }
                        break;
                    case 2:
                        Console.WriteLine(verbalDictionary);
                        Console.Write("\nPress any key: ");
                        Console.ReadKey(true);
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine($"\n\t Save {verbalDictionary.Name.ToTitle()}\n");
                        Console.Write("Enter path:\n\\");
                        verbalDictionary.Preservation(Console.ReadLine());
                        break;
                }
            }

            catch (KeyNotFoundException keyNotFound_ex)
            {
                Console.Clear();
                log.Info($"\nDate - {DateTime.Now.Date.ToShortDateString()}" +
                $"\nTime - {DateTime.Now.ToString("HH:mm:ss")}" +
                $"\nLog level - {LogLevel.Information}" +
                $"\nMessage - {keyNotFound_ex.Message}" +
                $"\nDetails: {keyNotFound_ex.GetType()}" +
                $"\n......\n......\n");
                Thread.Sleep(2000);
            }
            catch (FileNotFoundException fileNotFound_ex)
            {
                Console.Clear();
                log.Info($"\nDate - {DateTime.Now.Date.ToShortDateString()}" +
                $"\nTime - {DateTime.Now.ToString("HH:mm:ss")}" +
                $"\nLog level - {LogLevel.Information}" +
                $"\nMessage - {fileNotFound_ex.Message}" +
                $"\nDetails: {fileNotFound_ex.GetType()}" +
                $"\n......\n......\n");
                Thread.Sleep(2000);
            }
            catch (ArgumentNullException argumentNull_ex)
            {
                Console.Clear();
                log.Warn($"\nDate - {DateTime.Now.Date.ToShortDateString()}" +
                $"\nTime - {DateTime.Now.ToString("HH:mm:ss")}" +
                $"\nLog level - {LogLevel.Warning}" +
                $"\nMessage - {argumentNull_ex.Message}" +
                $"\nStackTrace - {argumentNull_ex.StackTrace}" +
                $"\nTargetSite - {argumentNull_ex.TargetSite}" +
                $"\nDetails: {argumentNull_ex.GetType()}" +
                $"\n......\n......\n");
                Thread.Sleep(2000);
            }
            catch (FormatException format_ex)
            {
                Console.Clear();
                log.Error($"\nDate - {DateTime.Now.Date.ToShortDateString()}" +
                $"\nTime - {DateTime.Now.ToString("HH:mm:ss")}" +
                $"\nLog level - {LogLevel.Error}" +
                $"\nMessage - {format_ex.Message}" +
                $"\nStackTrace - {format_ex.StackTrace}" +
                $"\nTargetSite - {format_ex.TargetSite}" +
                $"\nDetails: {format_ex.GetType()}" +
                $"\n......\n......\n");
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.Clear();
                log.Error($"\nDate - {DateTime.Now.Date.ToShortDateString()}" +
                $"\nTime - {DateTime.Now.ToString("HH:mm:ss")}" +
                $"\nLog level - {LogLevel.Error}" +
                $"\nMessage - {ex.Message}" +
                $"\nStackTrace - {ex.StackTrace}" +
                $"\nTargetSite - {ex.TargetSite}" +
                $"\nDetails: {ex.GetType()}" +
                $"\n......\n......\n");
                Thread.Sleep(2000);
            }
            Console.Clear();
        }
        if (verbalDictionary != null) { verbalDictionary.Preservation(); }

    }
    
    static int ChoicePanel(params string[] choices)
    {
        if (choices.Length <= 0) { return 0; }
        int index = 0;
        ConsoleKeyInfo choice = new();
        Console.CursorVisible = false;
        int startTop = Console.CursorTop;
        void DrawChoices()
        {
            for (int i = 0; i < choices.Length; i++)
            {
                Console.SetCursorPosition(0, startTop + i);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, startTop + i);
                if (index == i) { Console.Write(choices[i] + "  <--"); }
                else { Console.Write(choices[i]); }
            }
        }
        DrawChoices();

        while (choice.Key != ConsoleKey.Enter)
        {
            choice = Console.ReadKey(true);
            switch (choice.Key)
            {
                case ConsoleKey.W or ConsoleKey.UpArrow:
                    index = (index == 0) ? choices.Length - 1 : index - 1;
                    break;
                case ConsoleKey.S or ConsoleKey.DownArrow:
                    index = (index == choices.Length - 1) ? 0 : index + 1;
                    break;
            }
            DrawChoices();
        }

        for (int i = 0; i < choices.Length; i++)
        {
            Console.SetCursorPosition(0, startTop + i);
            Console.Write(new string(' ', Console.WindowWidth));
        }
        Console.SetCursorPosition(0, startTop);
        Console.CursorVisible = true;
        return index;
    }
}


