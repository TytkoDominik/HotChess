using Newtonsoft.Json;

[JsonObject("gamestate")]
public class Response
{
    [JsonProperty("nickname")]
    public string Nickname { get; set; }
}

public class ExampleResponse
{
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("gamestate")]
    public Response Response { get; set; }
}