using System.IO;
using System.Threading.Tasks;
using Flurl.Http;
using Netatmo.Models.Client;
using Netatmo.Models.Client.Air;

namespace Netatmo
{
    public class AirClient : IAirClient
    {
        private readonly string baseUrl;
        private readonly CredentialManager credentialManager;

        public AirClient(string baseUrl, CredentialManager credentialManager)
        {
            this.baseUrl = baseUrl;
            this.credentialManager = credentialManager;
        }

        public Task<DataResponse<GetHomeCoachsData>> GetHomeCoachsData(string deviceId = null)
        {
            return baseUrl
                .WithSettings(Configuration.ConfigureRequest)
                .AppendPathSegment("/api/gethomecoachsdata")
                .WithOAuthBearerToken(credentialManager.AccessToken)
                .PostJsonAsync(new GetHomeCoachsDataRequest
                { 
                    DeviceId = deviceId
                })
                .ReceiveJson<DataResponse<GetHomeCoachsData>>();
        }
    }
}