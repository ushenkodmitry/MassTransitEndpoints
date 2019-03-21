namespace MassTransit.JiraSerivedeskConnector
{
    public sealed class ServerOptions
    {
        public string BaseAddress { get; set; }

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string RequestTokenUrl => $"{BaseAddress}/plugins/servlet/oauth/request-token";

        public string AccessTokenUrl => $"{BaseAddress}/plugins/servlet/oauth/access-token";

        public string AuthorizeUrl => $"{BaseAddress}/plugins/servlet/oauth/authorize";
    }
}