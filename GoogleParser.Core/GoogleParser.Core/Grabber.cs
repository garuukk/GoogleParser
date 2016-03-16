using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GoogleParser.Core.Entities;
using GoogleParser.Core.Helpers;
using GoogleParser.Core.Parsers;
using GoogleParser.Core.Providers;
using OfficeOpenXml;
using Svyaznoy.Core;
using Svyaznoy.Core.Log;

namespace GoogleParser.Core
{
    public class Grabber
    {
        private ILogger _logger;
        private IMarketParser _marketParser;
        private IRequestHelper _requestHelper;
        private MobileApplicationsProvider _mobileApplicationsProvider;

        int quentityScannedCards = 0;

        private Action<RunResponse> _actionRunResponse;
        private DateTime _start;
        private DateTime _end;

        private IEnumerator<string> _collectionEnumerator;

        public Grabber(IMarketParser marketParser, IRequestHelper requestHelper, ILogger logger, string connectionString)
        {
            if (marketParser == null) throw new ArgumentNullException("marketParser");
            if (requestHelper == null) throw new ArgumentNullException("requestHelper");
            if (logger == null) throw new ArgumentNullException("logger");
            if (connectionString == null) throw new ArgumentNullException("connectionString");

            _logger = logger;
            _marketParser = marketParser;
            _requestHelper = requestHelper;

            _mobileApplicationsProvider = new MobileApplicationsProvider(connectionString);
        }

        public void Run(IEnumerable<string> collectionLinks, Action<RunResponse> actionRunResponse)
        {
            _actionRunResponse = actionRunResponse;
            _start = DateTime.Now;

            if (collectionLinks.IsNullOrEmpty())
                RunResponseIsComplited();

            _collectionEnumerator = collectionLinks.GetEnumerator();

            RunNextCollection();
        }

        private void RunNextCollection()
        {
            if (!_collectionEnumerator.MoveNext())
            {
                RunResponseIsComplited();
                return;
            }

            var currentCollection = _collectionEnumerator.Current;
            try
            {
                _logger.Info("Начата обработка коллекции {0}", _collectionEnumerator.Current);
                _requestHelper.SendRequestByAvesomium(currentCollection, CollectionPageIsLoaded, true);
            }
            catch (Exception exc)
            {
                _logger.Fatal(string.Format("Link: {0}", currentCollection), exc);
            }
        }

        private void RunResponseIsComplited()
        {
            _end = DateTime.Now;
            var runResponse = new RunResponse {QuentityScannedCards = quentityScannedCards, ScannedTime = _end - _start};
            _actionRunResponse.Invoke(runResponse);
        }

        private void CollectionPageIsLoaded(string html, string collectionLink)
        {
            var cardsLinks = _marketParser.ParseCollectionPage(html);

            if (cardsLinks != null)
            {
                foreach (var cardLink in cardsLinks)
                {
                    try
                    {
                        var link = cardLink.First() == '/'
                            ? "https://play.google.com" + cardLink
                            : "https://play.google.com/" + cardLink;
                        var cardPageHtml = _requestHelper.SendRequest(link);
                        var card = _marketParser.ParseAppPage(cardPageHtml);

                        if (card != null)
                        {
                            card.Url = link;
                            _mobileApplicationsProvider.Save(card);

                            quentityScannedCards++;
                        }
                        else
                        {
                            _logger.Fatal(string.Format("Не удалось распарсить ответ по Link: {0} \n {1}", link,
                                cardPageHtml));
                        }

                        _logger.Info("Обработано приложение {0}", link);

                        Thread.Sleep(1000);
                    }
                    catch (Exception exc)
                    {
                        _logger.Fatal(string.Format("Link: {0}", cardLink), exc);
                    }
                }
            }

            _logger.Info("Завершена обработка коллекции {0}", collectionLink);

            RunNextCollection();
        }

        public string ExportToExcel(string outputDir)
        {
            var cards = _mobileApplicationsProvider.GetCards();

            var fileData = CreateExcelFile(cards);

            string fileName = "Export_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx";

            if(!Directory.Exists(outputDir))
                throw new ApplicationException("Не найден путь '" + outputDir + "'");

            string filePath = Path.Combine(outputDir, fileName);

            File.WriteAllBytes(filePath, fileData);

            return fileName;
        }

        private byte[] CreateExcelFile(IEnumerable<ApplicationCard> cards)
        {
            Stream stream = new MemoryStream();
            
            var package = new ExcelPackage(stream);
            
            var sheet = package.Workbook.Worksheets.Add("GoogleApps");

            int headerRow = 1;
            sheet.Cells[headerRow, 1].Value = "Название";
            sheet.Cells[headerRow, 2].Value = "Ссылка";
            sheet.Cells[headerRow, 3].Value = "Разработчик";
            sheet.Cells[headerRow, 4].Value = "Почта разработчика";
            sheet.Cells[headerRow, 5].Value = "Фактический адрес разработчика";
            sheet.Cells[headerRow, 6].Value = "Дата обновления";

            int currentRow = 2;
            foreach (var card in cards)
            {
                sheet.Cells[currentRow, 1].Value = card.Name;
                sheet.Cells[currentRow, 2].Value = card.Url;
                sheet.Cells[currentRow, 3].Value = card.DeveloperName;
                sheet.Cells[currentRow, 4].Value = card.DeveloperEmail;
                sheet.Cells[currentRow, 5].Value = card.DeveloperPhysicalAddress;

                DateTime date = !card.UpdatedUtcDate.HasValue ? card.CreatedUtcDate : card.UpdatedUtcDate.Value;

                sheet.Cells[currentRow, 6].Value = date.AddHours(3).ToString("dd-MM-yyyy HH:mm:ss");
                
                currentRow++;
            }

            package.Save();

            var fileData = stream.ReadAllBytes();

            return fileData;
        }
    }
}
