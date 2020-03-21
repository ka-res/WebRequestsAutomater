using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SimpleInjector;
using WebRequestsAutomater.Common;
using WebRequestsAutomater.IoC;
using WebRequestsAutomater.Services.Interfaces;

namespace WebRequestsAutomater
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.ResetColor();
            var container = new Container();
            Configurator.RegisterComponents(container);

            Console.WriteLine("WebRequestsAutomater by ka_res\r\n==============================");
            
            var dataImporterService = container.GetInstance<IDataImporterService>();
            var voterHttpService = container.GetInstance<IVoterHttpService>();
            var data = dataImporterService.ImportData(out var lastItem);
            if (data == null || data.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"\r\n> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: no user provided!\r\n");
                Console.ResetColor();
                Console.WriteLine("Please, press any key to close...");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            Console.WriteLine($"\r\n> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: possessed {data.Count} users profiles\r\n");

            do
            {

                Console.WriteLine("Choose mode:");
                Console.WriteLine("\t v - vote for [sthg]");
                Console.WriteLine("\t l - like [sthg]");
                Console.WriteLine("\t x - vote & like [sthg]");
                Console.WriteLine("\t q - quit");

                Console.WriteLine($"\r\n> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: enter char & press enter...");
                var choice = Console.ReadKey();
                if (!Constants.PossibleChoices.Contains(choice.KeyChar))
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\r\n> invalid choice!\r\n");
                    Console.ResetColor();
                    continue;
                }

                var timingChoice = new ConsoleKeyInfo();
                if (choice.KeyChar != 'q')
                {
                    Console.WriteLine("\r\nShould use random timing? (y/n)");
                    timingChoice = Console.ReadKey();
                    if (!Constants.PossibleBoolChoices.Contains(timingChoice.KeyChar))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("\r\n> invalid choice!\r\n");
                        Console.ResetColor();
                        continue;
                    }
                }

                Console.WriteLine();

                switch (choice.KeyChar)
                {
                    case 'v':
                        var iv = 0;
                        var successBoardIv = new Dictionary<string, bool>();
                        foreach (var datum in data)
                        {
                            iv++;
                            Console.WriteLine($"\r\n> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: iteration no. [{iv}]");
                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: running V mode for {datum.Key}");

                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: initializing session...");
                            voterHttpService.RunLogin(datum.Key, datum.Value, out var cookieToken);
                            // TODO
                            var uniqueValue = 57;
                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: voting for [sthg] with ID {uniqueValue}...");
                            voterHttpService.VoteForProject(uniqueValue, cookieToken, out bool isSuccessful);
                            successBoardIv.Add(datum.Key, isSuccessful);

                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: logging out user {datum.Key}...");
                            voterHttpService.RunLogout(cookieToken);

                            var isLast = datum.Key == lastItem;
                            if (!isLast)
                            {
                                switch (timingChoice.KeyChar)
                                {
                                    case 'y':
                                        if (isSuccessful)
                                        {
                                            var waitingMs = Helpers.GetRandomTimeSpanInMiliseconds();
                                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: waiting {Helpers.FormatMilisecondsToMinutes(waitingMs)}...");
                                            Thread.Sleep(waitingMs);
                                        }
                                        else
                                        {
                                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: failure & continuation...");
                                        }
                                        break;
                                    case 'n':
                                        Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: fast mode in progress, continuing...");
                                        break;
                                    default:
                                        break;
                                }
                            }                                                      
                        }

                        Console.WriteLine();
                        foreach (var entry in successBoardIv)
                        {
                            if (!entry.Value)
                            {
                                Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: voting failure tracked for: {entry.Key}");
                            }
                        }

                        Console.WriteLine();
                        // TODO poprawic
                        Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: voting successful for {successBoardIv.Count}/{data.Count} entries");

                        break;
                    case 'l':
                        var il = 0;
                        var successBoardIl = new Dictionary<string, bool>();
                        foreach (var datum in data)
                        {
                            il++;
                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: iteration no. [{il}]");
                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: running L mode for {datum.Key}");

                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: initializing session...");
                            voterHttpService.RunLogin(datum.Key, datum.Value, out var cookieToken);
                            // TODO
                            var uniqueValues = new[] { 305 };
                            var stringBuilder = new StringBuilder();
                            foreach (var uniqueVal in uniqueValues)
                            {
                                stringBuilder.Append(uniqueVal + ", ");
                            }

                            var formattedUniques = stringBuilder.ToString().Substring(0, stringBuilder.ToString().Length - 2);

                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: liking [sthg] with IDs {formattedUniques}...");
                            voterHttpService.LikeArticles(uniqueValues, cookieToken, out bool isSuccessful);
                            successBoardIl.Add(datum.Key, isSuccessful);

                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: logging out user {datum.Key}...");
                            voterHttpService.RunLogout(cookieToken);

                            switch (timingChoice.KeyChar)
                            {
                                case 'y':
                                    if (isSuccessful)
                                    {
                                        var waitingMs = Helpers.GetRandomTimeSpanInMiliseconds();
                                        Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: waiting {Helpers.FormatMilisecondsToMinutes(waitingMs)}...");
                                        Thread.Sleep(waitingMs);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: failure & continuation...");
                                    }
                                    break;
                                case 'n':
                                    Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: fast mode in progress, continuing...");
                                    break;
                                default:
                                    break;
                            }
                        }

                        Console.WriteLine();
                        foreach (var entry in successBoardIl)
                        {
                            if (!entry.Value)
                            {
                                Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: liking failure tracked for: {entry.Key}");
                            }
                        }

                        Console.WriteLine();
                        // TODO poprawic
                        Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: voting successful for {successBoardIl.Count}/{data.Count} entries");

                        break;
                    case 'x':
                        var ivl = 0;
                        var successBoardIvlV = new Dictionary<string, bool>();
                        var successBoardIvlL = new Dictionary<string, bool>();
                        foreach (var datum in data)
                        {
                            ivl++;
                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: iteration no. [{ivl}]");
                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: running X mode for {datum.Key}");

                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: initializing session...");
                            voterHttpService.RunLogin(datum.Key, datum.Value, out var cookieToken);
                            // TODO
                            var uniqueValue = 57;
                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: voting for [sthg] with ID {uniqueValue}...");
                            voterHttpService.VoteForProject(uniqueValue, cookieToken, out bool isSuccessfulV);
                            successBoardIvlV.Add(datum.Key, isSuccessfulV);
                            // TODO
                            var uniqueValues = new[] { 305 };
                            var stringBuilder = new StringBuilder();
                            foreach (var uniqueVal in uniqueValues)
                            {
                                stringBuilder.Append(uniqueVal + ", ");
                            }

                            var formattedUniques = stringBuilder.ToString().Substring(0, stringBuilder.ToString().Length - 2);

                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: liking [sthg] with IDs {formattedUniques}...");
                            voterHttpService.LikeArticles(uniqueValues, cookieToken, out bool isSuccessfulL);
                            successBoardIvlL.Add(datum.Key, isSuccessfulL);

                            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: logging out user {datum.Key}...");
                            voterHttpService.RunLogout(cookieToken);

                            switch (timingChoice.KeyChar)
                            {
                                case 'y':
                                    if (isSuccessfulV)
                                    {
                                        var waitingMs = Helpers.GetRandomTimeSpanInMiliseconds();
                                        Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: waiting {Helpers.FormatMilisecondsToMinutes(waitingMs)}...");
                                        Thread.Sleep(waitingMs);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: failure & continuation...");
                                    }
                                    break;
                                case 'n':
                                    Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: fast mode in progress, continuing...");
                                    break;
                                default:
                                    break;
                            }
                        }

                        Console.WriteLine();
                        foreach (var entry in successBoardIvlV)
                        {
                            if (!entry.Value)
                            {
                                Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: voting failure tracked for: {entry.Key}");
                            }
                        }

                        Console.WriteLine();
                        foreach (var entry in successBoardIvlL)
                        {
                            if (!entry.Value)
                            {
                                Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: liking failure tracked for: {entry.Key}");
                            }
                        }

                        Console.WriteLine();
                        // TODO poprawic
                        Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: voting successful for {successBoardIvlV.Count}/{data.Count} entries");

                        break;
                    case 'q':
                        Environment.Exit(0);
                        break;
                }

                Console.WriteLine("\r\n[-]\r\n");
            } while (true);
        }
    }
}
