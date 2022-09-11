using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfferExporter.Models;
using OfferExporter.Repositories.Abstract;
using OfferExporter.Repositories.Impl;
using OfferExporter.Services.Abstract;
using OfferExporter.Services.Impl;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddTransient<IOfferRespository, OfferRespository>();
        services.AddTransient<IPromotionRepository, PromotionRepository>();
        
        
        services.AddTransient<IOfferService, OfferService>();//OfferService
        services.AddTransient<IProductService, ProductService>();//ProductService

    }).Build();


var stopwatch = Stopwatch.StartNew();

Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} OfferExport batch started");

// ======================= //
// READ DATA FROM DATABASE //
// ======================= //

var offerResultRows = new List<GetAllOffersResultRow>();
var promotionTargetsResultRows = new List<GetAllPromotionTargetsResultRow>();


var offerRespository = host.Services.GetService<IOfferRespository>();
offerResultRows.AddRange(await offerRespository.GetAllOffersResultRows());

var promotionRepository = host.Services.GetService<IPromotionRepository>();
promotionTargetsResultRows.AddRange(await promotionRepository.GetAllOffersResultRows());

var offerService = host.Services.GetService<IOfferService>();
var productService = host.Services.GetService<IProductService>();
// =================== //
// REMOVE INVALID ROWS //
// =================== //

offerResultRows = offerService.RemoveEmptyRows().ToList();

offerResultRows = offerService.RemoveActiveSeller(offerResultRows).ToList();

offerResultRows = offerService.RemoveUnexportedData(offerResultRows).ToList();

Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Exclude invalid offers: {offerResultRows.Count} left");


// ========================================== //
// BUILD PRODUCTS FOR SERIALIZATION INTO JSON //
// ========================================== //
var products = productService.BuildProducts(offerResultRows);

// ================= //

Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Convert data into products: {products.Count} products");


// ============= //
// EXPORT OFFERS //
// ============= //
var outputDir = Path.Combine(Environment.CurrentDirectory, "_OUPUT");
var bytesOfNewLine = Encoding.UTF8.GetBytes(Environment.NewLine);
var jsonFiles = new List<string>();

Directory.CreateDirectory(outputDir);

var referentials = products.GroupBy(product => product.ReferentialName);
foreach (var referential in referentials)
{
    try
    {
        var jsonFileName = $"{referential.Key}.json";
        var jsonFilePath = Path.Combine(outputDir, jsonFileName);
        var jsonBytes = new List<byte>();

        // The exported file is not exactly a pure JSON. It will contains many JSON.
        // Each line of the file will contain a product serialized into JSON.
        foreach (var product in referential)
        {
            jsonBytes.AddRange(Utf8Json.JsonSerializer.Serialize(product,
                Utf8Json.Resolvers.StandardResolver.ExcludeNull));

            jsonBytes.AddRange(bytesOfNewLine);
        }

        File.WriteAllBytes(jsonFilePath, jsonBytes.ToArray());

        jsonFiles.Add(jsonFilePath);

        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Export {referential.Key} referential: {jsonFileName} ({jsonBytes.Count} bytes)");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Export {referential.Key} referential failed ! {ex.Message}");
    }
}


// ============================ //
// COMPRESS THE FILES INTO GZIP //
// ============================ //
foreach (var jsonFile in jsonFiles)
{
    var jsonFileName = Path.GetFileName(jsonFile);
    var gZipFile = jsonFile + ".gz";
    var md5File = gZipFile + ".md5";

    try
    {
        using (var fileStream = new FileStream(gZipFile, FileMode.Create))
        using (var gZipStream = new GZipStream(fileStream, CompressionMode.Compress))
        {
            var gZipFileName = Path.GetFileName(gZipFile);
            var jsonBytes = File.ReadAllBytes(jsonFile);

            gZipStream.Write(jsonBytes, 0, jsonBytes.Length);

            var gZipFileSize = new FileInfo(gZipFile).Length;

            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Compress {jsonFileName}: {gZipFileName} ({gZipFileSize} bytes)");
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Compress {jsonFileName} failed ! {ex.Message}");
    }
}


// ================== //
// CALCULATE MD5 HASH //
// ================== //
var md5 = MD5.Create();
var sb = new StringBuilder();

foreach (var jsonFile in jsonFiles)
{
    var jsonFileName = Path.GetFileName(jsonFile);
    var gZipFileName = jsonFileName + ".gz";
    var gZipFile = jsonFile + ".gz";
    var md5File = gZipFile + ".md5";

    try
    {
        var gZipBytes = File.ReadAllBytes(gZipFile);
        var hashBytes = md5.ComputeHash(gZipBytes);

        // Convert hashBytes into string then save the MD5 hash into file
        sb.Clear();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("X2"));
        }
        File.WriteAllText(md5File, sb.ToString());

        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Compute MD5 {gZipFileName}: {sb}");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Compute MD5 {gZipFileName} failed ! {ex.Message}");
    }
}


// ======== //
// CLEAN UP //
// ======== //
try
{
    foreach (var jsonFile in jsonFiles)
    {
        var gZipFile = jsonFile + ".gz";
        if (!File.Exists(gZipFile))
        {
            continue;
        }

        var gZipFileInfo = new FileInfo(gZipFile);
        if (gZipFile.Length == 0)
        {
            continue;
        }

        File.Delete(jsonFile);
    }

    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Clean up working files done");
}
catch (Exception ex)
{
    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Clean up working files failed ! {ex.Message}");
}


// === //
// END //
// === //
stopwatch.Stop();
Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} OfferExport batch completed in {stopwatch.Elapsed}");

Console.ReadKey();