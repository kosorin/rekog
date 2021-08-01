using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

File.WriteAllLines("messages.txt", Directory
    .EnumerateFiles(".", "*.json")
    .Select(File.ReadAllText)
    .Select(json => JsonSerializer.Deserialize<Conversation>(json))
    .SelectMany(conversation => conversation!.Messages
        .Where(x => !x.IsUnsent)
        .Where(x => x.Type == "Generic")
        .Where(x => x.SenderName == "Ivan Ivanovi\u00c4\u008d Ivanovski")
        .Select(x => x.Content)
        .Select(x =>
        {
            var sourceEncoding = Encoding.GetEncoding("ISO-8859-1");
            var targetEncoding = Encoding.UTF8;
            return targetEncoding.GetString(sourceEncoding.GetBytes(x ?? string.Empty));
        }))
    .ToList());

internal class Conversation
{
    public Conversation(List<Participant> participants, List<Message> messages)
    {
        Participants = participants;
        Messages = messages;
    }

    [JsonPropertyName("participants")]
    public List<Participant> Participants { get; init; }

    [JsonPropertyName("messages")]
    public List<Message> Messages { get; init; }
}

internal class Participant
{
    public Participant(string? name)
    {
        Name = name;
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

internal class Message
{
    public Message(string? content, string senderName, string type, bool isUnsent)
    {
        Content = content;
        SenderName = senderName;
        Type = type;
        IsUnsent = isUnsent;
    }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("sender_name")]
    public string SenderName { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("is_unsent")]
    public bool IsUnsent { get; init; }
}
