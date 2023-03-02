using CsvHelper;
using CsvHelper.Configuration;
using System.Formats.Asn1;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

class Progrom
{
    static async Task<int> Main(string[] args)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
            var getPosts = await client.GetAsync("posts");
            if (getPosts.IsSuccessStatusCode)
            {
                var readPosts = await getPosts.Content.ReadFromJsonAsync<Posts[]>();
                await CreateJson(readPosts);
                await CreateCsv(readPosts);
            }

        }
        return 0;
    }

    private static async Task<bool> CreateJson(Posts[] posts)
    {
        var options = new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        var jsonString = JsonSerializer.Serialize(posts, options);
        File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}Posts.json", jsonString);
        Console.WriteLine($"JSON File Path:{AppDomain.CurrentDomain.BaseDirectory}Posts.json");
        return true;
    }
    private static async Task<bool> CreateCsv(Posts[] posts)
    {
        using (var ms = new MemoryStream())
        {
            var writer = new StreamWriter(ms, leaveOpen: true);
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(posts);
            }
            ms.Position = 0;
            ms.Seek(0, SeekOrigin.Begin);

            using (FileStream fs = new FileStream($"{AppDomain.CurrentDomain.BaseDirectory}Posts.csv", FileMode.OpenOrCreate))
            {
                ms.CopyTo(fs);
                fs.Flush();
            }
        }
        Console.WriteLine($"CSV File Path:{AppDomain.CurrentDomain.BaseDirectory}Posts.json");
        return true;
    }
}
public class Posts
{
    public int userId { get; set; }
    public int id { get; set; }
    public string title { get; set; }
    public string body { get; set; }
}