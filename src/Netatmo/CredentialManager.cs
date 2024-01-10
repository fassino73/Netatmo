using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Netatmo.Models;
using Netatmo.Models.Client;
using NodaTime;

namespace Netatmo
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;

    public class CredentialManager 
    {
        private readonly string baseUrl;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string redirectUri;
        private readonly IClock clock;

        // https://dev.netatmo.com/apidocumentation/oauth
        // https://melmanm.github.io/misc/2023/02/13/article6-oauth20-authorization-in-desktop-applicaions.html
        public CredentialManager(string baseUrl, string clientId, string clientSecret, IClock clock)
        {
            this.baseUrl = baseUrl;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.clock = clock;
            this.redirectUri = "http://localhost:8888/";
        }

        public CredentialToken CredentialToken { get; private set; }
        public string AccessToken => CredentialToken?.AccessToken;

        string CreateAuthorizeRequest(string redirectUri)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("client_id=").Append(clientId);
            queryBuilder.Append("&redirect_uri=").Append(redirectUri);
            queryBuilder.Append("&scope=").Append("read_station");
            queryBuilder.Append("&state=").Append("mystate");

            var uriBuilder = new UriBuilder(baseUrl) { Query = queryBuilder.ToString(), Path = "oauth2/authorize" };

            return uriBuilder.ToString();
        }

        public string /*async Task*/ Authorize()
        {
            var authorizeRequest = CreateAuthorizeRequest(redirectUri);

            //start system browser 
            Process.Start(new ProcessStartInfo(authorizeRequest) { UseShellExecute = true });

            using var listener = new HttpListener();
            listener.Prefixes.Add(redirectUri);
            listener.Start();

            //wait for server captures redirect_uri  
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            var code = request.QueryString.Get("code");

            context.Response.Close();
            listener.Stop();

            return code;
        }

        public async Task GetAccessToken(string codeValue)
        {
            // TODO : Handle not success status codes (rate limit exceeded, api down, ect)
            var token = await baseUrl.AppendPathSegment("/oauth2/token").PostUrlEncodedAsync(new
            {
                grant_type = "authorization_code",
                client_id = clientId,
                client_secret = clientSecret,
                code = codeValue,
                redirect_uri = redirectUri,
                scope = "read_station"
            }).ReceiveJson<Token>();

            CredentialToken = new CredentialToken(token, clock);
        }

        public async Task GetRefreshToken()
        {
            // TODO : Handle not success status codes (rate limit exceeded, api down, ect)
            var token = await baseUrl.AppendPathSegment("/oauth2/token").PostUrlEncodedAsync(new
            {
                grant_type = "refresh_token",
                client_id = clientId,
                client_secret = clientSecret,
                refresh_token = CredentialToken.RefreshToken
            }).ReceiveJson<Token>();

            CredentialToken = new CredentialToken(token, clock);
        }
    }
}
