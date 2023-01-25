using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

HttpClient client = new HttpClient();

string prompt = string.Join(" ", args);

if (string.IsNullOrEmpty(prompt))
{
    Console.WriteLine("Please enter a question.");
    return;
}

var max_tokens = 60;
var temperature = 0.7;
var frequency_penalty = 0;
var presence_penalty = 0;
var top_p = 1;

var body = new
{
    prompt,
    max_tokens,
    temperature,
    frequency_penalty,
    presence_penalty,
    top_p,
    stop = "null"
};

var bodyJson = JsonSerializer.Serialize(body);
var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");


using (var request = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("OPENAI_ORG_URL")))
{
    var json = JsonSerializer.Serialize(request);
    request.Content = content;
    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    request.Headers.Add("api-key", Environment.GetEnvironmentVariable("OPENAI_ORG_KEY"));


    using (var response = await client.SendAsync(request).ConfigureAwait(false))
    {
        var responseString = await response.Content.ReadAsStringAsync();
        var responseJson = JsonSerializer.Deserialize<OpenAiResponse>(responseString);
        Console.WriteLine(responseJson.choices[0].text);
        Console.WriteLine();
        Console.WriteLine($"prompt_tokens {responseJson.usage.prompt_tokens}");
        Console.WriteLine($"completion_tokens {responseJson.usage.completion_tokens}");
        Console.WriteLine($"total_tokens {responseJson.usage.total_tokens}");
        Console.WriteLine();
    }
}


public class OpenAiResponse
{
    public string id { get; set; }
    public long created { get; set; }
    public string model { get; set; }
    public List<Choice> choices { get; set; }
    public Usage usage { get; set; }
}

public class Choice
{
    public string text { get; set; }
    public int index { get; set; }
    public string finish_reason { get; set; }
    public object logprobs { get; set; }
}

public class Usage
{
    public int completion_tokens { get; set; }
    public int prompt_tokens { get; set; }
    public int total_tokens { get; set; }
}