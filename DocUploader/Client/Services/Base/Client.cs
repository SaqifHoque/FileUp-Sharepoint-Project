using System.Net.Http;

namespace DocUploader.Client.Services.Base
{
    public partial class Client : IClient
    {
        public HttpClient httpClient
        {
            get
            {
                return _httpClient;
            }
        }
    }
}
