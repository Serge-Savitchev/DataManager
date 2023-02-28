using System.Diagnostics;

// Raw string literal text

Console.WriteLine(""" "A" """);
Console.WriteLine(@"""ABC""");

Console.WriteLine(@" RETURNING ""Id""");

var variable = @"
V1
   ""V2""
V3

                    1
";

var multyLines = @$"Line 1
            Line 2
""Line 3""
       Line 4
       {variable}";

Console.WriteLine(multyLines);

int fileId = 1, userDataId = 2;

var request1 = $"SELECT \"Oid\" FROM \"UserFiles\" WHERE \"Id\"={fileId} AND \"UserDataId\"={userDataId}";
Console.WriteLine(request1);

var request2 = $"""SELECT "Oid" FROM "UserFiles" WHERE "Id"={fileId} AND "UserDataId"={userDataId}""";
Console.WriteLine(request2);

var request3 = @$"SELECT ""Oid"" FROM ""UserFiles""
WHERE ""Id""={fileId} AND ""UserDataId""={userDataId}";
Console.WriteLine(request3);


Console.ReadLine();
return;


const int _bufferSize = 1024 * 1024 * 10;

Console.WriteLine(Directory.GetCurrentDirectory());

byte[] _buffer = Array.Empty<byte>();

if (args.Length > 0 && args[0].Contains("usebuffer", StringComparison.InvariantCultureIgnoreCase))
{
    _buffer = new byte[_bufferSize];
}

do
{
    Console.Write("Enter request or <Enter> to exit: ");
    string? input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        break;
    }

    Stopwatch timer = new Stopwatch();
    timer.Start();

    try
    {

        using var client = new HttpClient();

        using HttpResponseMessage responseMessage = await client.GetAsync(input, HttpCompletionOption.ResponseHeadersRead);

        responseMessage.EnsureSuccessStatusCode();

        string fileName = responseMessage.Content.Headers!.ContentDisposition!.FileNameStar!;

        await using var streamToReadFrom = await responseMessage.Content.ReadAsStreamAsync();
        await using var outputStream = new FileStream(fileName, FileMode.Create);
        await using BufferedStream bufferedStream = new(streamToReadFrom, _bufferSize);

        if (_buffer.Length > 0)
        {
            int read, count = 0;
            while ((read = await bufferedStream.ReadAsync(_buffer)) > 0)
            {
                Console.WriteLine($"{++count}\t{read}");
                await outputStream.WriteAsync(_buffer.AsMemory(0, read));
            }
        }
        else
        {
            bufferedStream.CopyTo(outputStream);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }

    timer.Stop();
    Console.WriteLine($"Timer: {timer.Elapsed.Minutes}:{timer.Elapsed.Seconds} Total seconds: {timer.Elapsed.TotalSeconds}");

} while (true);

