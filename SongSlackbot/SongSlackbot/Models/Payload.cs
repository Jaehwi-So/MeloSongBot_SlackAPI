using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// Payload의 요약 설명입니다.
/// </summary>
[JsonObject(Title = "attachments")]
public class Payload
{
    [JsonProperty("channel")]
    public string Channel { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("icon_url")]
    public string IconUrl { get; set; }
    
    
    //public Attachments attachments { get; set; } 

    public List<SlackAttachment> attachments { get; set; }
}


public class SlackAttachment
{
    [JsonProperty("fallback")]
    public string Fallback { get; set; }

    [JsonProperty("pretext")]
    public string Pretext { get; set; }

    [JsonProperty("color")]
    public string Color { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }
}