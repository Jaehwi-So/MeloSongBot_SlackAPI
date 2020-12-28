using System;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Collections.Generic;

/// <summary>
/// SlackBot의 요약 설명입니다.
/// </summary>
public class SlackBot
{
    private readonly Uri _uri = new Uri("https://hooks.slack.com/services/..");
    private readonly Encoding _encoding = new UTF8Encoding();
    
    public void PostMessage(string title, string text, string fallback, string username = null, string channel = null, string iconurl = null)
    {
        Payload payload = new Payload()
        {
            Channel = channel,
            Username = username,
            IconUrl = iconurl
        };
        
        var slackAttachment = new SlackAttachment
        {
            Fallback = fallback,
            Pretext = fallback,
            Color = "#dd00ff",
            Title = title,
            Text = text
        };

        payload.attachments = new List<SlackAttachment> { slackAttachment };

        PostMessage(payload);
    }
    
    private void PostMessage(Payload payload)
    {
        string payloadJson = JsonConvert.SerializeObject(payload, Formatting.Indented);

        using (WebClient client = new WebClient())
        {
            NameValueCollection data = new NameValueCollection();
            data["payload"] = payloadJson;

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var response = client.UploadValues(_uri, "POST", data);

            string responseText = _encoding.GetString(response);
        }
    }
}
