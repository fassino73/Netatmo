using System;
using System.Threading.Tasks;
using NodaTime;

namespace Netatmo
{
    public class Client
    {
        public Client(IClock clock, string baseUrl, string clientId, string clientSecret)
        {
            CredentialManager = new CredentialManager(baseUrl, clientId, clientSecret, clock);
            Weather = new WeatherClient(baseUrl, CredentialManager);
            Energy = new EnergyClient(baseUrl, CredentialManager);
            Air = new AirClient(baseUrl, CredentialManager);
        }

        public IWeatherClient Weather { get; }
        public IEnergyClient Energy { get; }
        public IAirClient Air { get; }
        public CredentialManager CredentialManager { get; }
        
        public Task Authorize()
        {
            string code = CredentialManager.Authorize();
            return CredentialManager.GetAccessToken(code);
        }

        public Task RefreshToken()
        {
            return CredentialManager.GetRefreshToken();
        }
    }
}