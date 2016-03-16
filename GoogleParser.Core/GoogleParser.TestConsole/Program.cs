using System;
using System.Configuration;
using System.IO;
using System.Net.Mime;
using Awesomium.Core;
using GoogleParser.Core;
using GoogleParser.Core.Entities;
using GoogleParser.Core.Helpers;
using GoogleParser.Core.Parsers;
using Svyaznoy.Core.Log;
using ILogger = Svyaznoy.Core.Log.ILogger;

namespace GoogleParser.TestConsole
{
    class Program
    {
        private static Grabber grabber;
        private static bool exit;

        static void Main(string[] args)
        {
            IMarketParser marketParser = new GoogleMarketParse();
            IRequestHelper requestHelper = new RequestHelper();
            ILogger logger = new LoggerAppConfig(); 
            string connectionString = ConfigurationManager.ConnectionStrings["MarketApplicationsEntities"].ConnectionString;

            grabber = new Grabber(marketParser, requestHelper, logger, connectionString);

            exit = false;

            Console.WriteLine("Введите команду:");
            Run();
        }

        private static void Export()
        {
            Console.WriteLine("Укажите папку для экспорта:");

            string outputDir = Console.ReadLine();

            if (string.IsNullOrEmpty(outputDir))
            {
                Console.WriteLine("Указан пустой путь!");
                return;
            }

            string fileName = grabber.ExportToExcel(outputDir);

            Console.WriteLine("Экспорт завершен - {0}", fileName);
        }

        private static void Scan()
        {
            Console.WriteLine("Укажите полный путь к файлу с ссылками:");
            string filePath = Console.ReadLine();

            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Указан пустой путь!");
                return;
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Указанный файл не найден!");
                return;
            }

            var links = File.ReadAllLines(filePath);

            grabber.Run(links, ActionRunResponse);
        }

        private static void ActionRunResponse(RunResponse runResponse)
        {
            Console.WriteLine("Сканирование завершено!");
            Console.WriteLine("Просканировано {0} карточек приложений за {1}", runResponse.QuentityScannedCards, runResponse.ScannedTime);

            Run();
        }

        private static void Run()
        {
            while (!exit)
            {
                var command = Console.ReadLine();

                if (command != null)
                    command = command.ToLower();

                switch (command)
                {
                    case "exit":
                        exit = true;
                        WebCore.Shutdown();
                        break;
                    case "scan":
                        Scan();
                        break;
                    case "export":
                        Export();
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            }
        }
    }
}
