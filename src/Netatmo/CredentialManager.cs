using Flurl;
using Flurl.Http;
using Flurl.Http.Content;
using Netatmo.Models;
using Netatmo.Models.Client;
using NodaTime;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

namespace Netatmo;

// https://dev.netatmo.com/apidocumentation/oauth
// https://melmanm.github.io/misc/2023/02/13/article6-oauth20-authorization-in-desktop-applicaions.html
public class CredentialManager(string baseUrl, string clientId, string clientSecret, IClock clock) : ICredentialManager
{
    public CredentialToken CredentialToken { get; private set; }
    public string AccessToken => CredentialToken?.AccessToken;

    private readonly string redirectUri = "http://localhost:8888/";

    public async Task GenerateToken(string username, string password, Scope[] scopes = null)
    {
        var scope = string.Join(" ", scopes?.Select(s => s.Value) ?? []);

        // TODO : Handle not success status codes (rate limit exceeded, api down, ect)
        var token = await baseUrl.AppendPathSegment("/oauth2/token")
            .PostUrlEncodedAsync(
                new
                {
                    grant_type = "password",
                    client_id = clientId,
                    client_secret = clientSecret,
                    username,
                    password,
                    scope
                })
            .ReceiveJson<Token>();

        CredentialToken = new CredentialToken(token, clock);
    }

    string CreateAuthorizeRequest(string redirectUri, string state)
    {
        var queryBuilder = new StringBuilder();
        queryBuilder.Append("client_id=").Append(clientId);
        queryBuilder.Append("&redirect_uri=").Append(redirectUri);
        queryBuilder.Append("&scope=").Append("read_station");
        queryBuilder.Append("&state=").Append(state);

        var uriBuilder = new UriBuilder(baseUrl) { Query = queryBuilder.ToString(), Path = "oauth2/authorize" };

        return uriBuilder.ToString();
    }

    public string Authorize()
    {
        string state = this.GetHashCode().ToString();
        var authorizeRequest = CreateAuthorizeRequest(redirectUri, state);

        //start system browser 
        Process.Start(new ProcessStartInfo(authorizeRequest) { UseShellExecute = true });

        using var listener = new HttpListener();
        listener.Prefixes.Add(redirectUri);
        listener.Start();

        //wait for server captures redirect_uri  
        HttpListenerContext context = listener.GetContext();
        HttpListenerRequest request = context.Request;

        if (!state.Equals(request.QueryString.Get("state")))
        {
            throw new Exception("Authorization state mismatch");
        }
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
    public void ProvideOAuth2Token(string accessToken, string refreshToken)
    {
        CredentialToken = new CredentialToken(new Token(20, accessToken, refreshToken), clock);
    }

    public void ProvideOAuth2Token(string accessToken)
    {
        ProvideOAuth2Token(accessToken, null);
    }

    public async Task RefreshToken()
    {
        // TODO : Handle not success status codes (rate limit exceeded, api down, ect)
        var token = await baseUrl.AppendPathSegment("/oauth2/token")
            .PostUrlEncodedAsync(new { grant_type = "refresh_token", client_id = clientId, client_secret = clientSecret, refresh_token = CredentialToken.RefreshToken })
            .ReceiveJson<Token>();

        CredentialToken = new CredentialToken(token, clock);
    }
}