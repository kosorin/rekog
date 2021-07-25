using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using VersOne.Epub;

await new Api().DownloadAll();

internal class Api
{
    private const string BaseUrl = "https://www.cbdb.cz";
    private const string OutputFolder = "books";
    private const string BookExtension = "epub"; // Currently is supported only "epub"
    private const int Delay = 100; // To prevent "DDoS"
    private const int BookTrimPages = 2; // To exclude usually "not content" pages

    public async Task DownloadAll()
    {
        Directory.CreateDirectory(OutputFolder);

        var client = new HttpsClient();
        var document = new HtmlDocument();
        for (var i = 1;; i++)
        {
            Console.WriteLine($"Fetching page {i}");

            var booksFound = 0;

            try
            {
                document.LoadHtml(client.DownloadString(BaseUrl + $"/elektronicke-knihy-ebooky-zdarma/{i}"));
                var bookNodes = document.DocumentNode.SelectNodes("//div[@class='grlist_item_in']");
                if (bookNodes == null || bookNodes.Count == 0)
                {
                    break;
                }

                foreach (var bookNode in bookNodes)
                {
                    var anchorNode = bookNode.SelectSingleNode("a[2]");
                    if (anchorNode?.Attributes["href"] is not { } hrefAttribute)
                    {
                        continue;
                    }

                    var href = hrefAttribute.Value;
                    var idMatch = Regex.Match(href, @"^/kniha-(?<Id>[0-9]+)");
                    if (!idMatch.Success)
                    {
                        continue;
                    }

                    var id = idMatch.Groups["Id"].Value;
                    var name = anchorNode.SelectSingleNode("b").InnerHtml;
                    var book = new Book(id, name);

                    try
                    {
                        await DownloadBook(book);
                        await ConvertBook(book);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Book error: {ex}");
                    }

                    booksFound++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"List error: {ex}");
                break;
            }

            if (booksFound == 0)
            {
                break;
            }

            await Task.Delay(Delay);
        }
    }

    private async Task DownloadBook(Book book)
    {
        var filePath = OutputFolder + "/" + book.GetFileName(BookExtension);
        if (File.Exists(filePath))
        {
            return;
        }

        var client = new HttpsClient();
        var document = new HtmlDocument();

        Console.WriteLine($"Checking {book.Id} - {book.Name}");
        document.LoadHtml(client.DownloadString(BaseUrl + $"/ajax_server/book.php?action=get_products&book={book.Id}"));
        var downloadNode = document.DocumentNode.SelectSingleNode($"//div[@class='']/a[text() = '{BookExtension}']");
        if (downloadNode == null)
        {
            return;
        }

        var downloadUrl = BaseUrl + downloadNode.Attributes["href"].Value;
        Console.WriteLine($"Downloading {downloadUrl} ({filePath})");

        client.DownloadFile(downloadUrl, filePath);
        await Task.Delay(Delay);
    }

    private async Task ConvertBook(Book book)
    {
        var filePath = OutputFolder + "/" + book.GetFileName(BookExtension);
        if (!File.Exists(filePath))
        {
            return;
        }

        var outputFilePath = filePath + ".txt";
        if (File.Exists(outputFilePath))
        {
            return;
        }

        await using var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        await using var writer = new StreamWriter(fileStream);

        Console.WriteLine($"Converting {filePath}");
        var document = new HtmlDocument();
        var bookRef = await EpubReader.OpenBookAsync(filePath);
        var readingOrder = await bookRef.GetReadingOrderAsync();
        foreach (var contentFileRef in readingOrder.Skip(BookTrimPages).Take(readingOrder.Count - 2 * BookTrimPages))
        {
            await using var contentStream = contentFileRef.GetContentStream();

            document.Load(contentStream);

            var textNodes = document.DocumentNode.SelectNodes("//body//text()");
            if (textNodes == null)
            {
                continue;
            }

            foreach (var node in textNodes)
            {
                await writer.WriteLineAsync(HttpUtility.HtmlDecode(node.InnerText.Trim()));
            }
        }
    }
}

internal record Book(string Id, string Name)
{
    public string InfoUrl => $"https://www.cbdb.cz/ajax_server/book.php?action=get_products&book={Id}";

    public string GetFileName(string extension)
    {
        var fileName = Regex.Replace(Name, @"\s+", "_");
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '-');
        }
        return fileName + "." + extension;
    }
}

internal class HttpsClient : WebClient
{
    protected override WebRequest GetWebRequest(Uri address)
    {
        var request = (HttpWebRequest)base.GetWebRequest(address);

        if (request == null)
        {
            throw new NullReferenceException();
        }

        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        request.AllowAutoRedirect = true;
        return request;
    }
}
