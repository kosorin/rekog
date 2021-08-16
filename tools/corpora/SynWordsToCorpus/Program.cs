using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

if (args.Length < 2)
{
    Console.WriteLine("Usage: .exe <input-path> <output-path>");
}

var input = args[0];
var output = args[1];

var config = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    Delimiter = ";",
    HasHeaderRecord = false,
};

using var writer = new StreamWriter(output);
using var reader = new StreamReader(input);
using var csv = new CsvReader(reader, config);

Console.WriteLine("start");
var line = 0;
while (csv.Read())
{
    line++;
    if (line % 100_000 == 0)
    {
        Console.WriteLine(line);
    }

    var word = csv.GetField<string>(1);
    var count = csv.GetField<int>(2);

    for (var i = 0; i < count; i++)
    {
        writer.WriteLine(word);
    }
}
Console.WriteLine("end");
