using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dnc.Services.RestClient.Options
{
    public class RestClientOptions
    {
        public HttpClient HttpClient { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public string BaseAddress { get; set; }
        public string UserScope { get; set; }
        public string AppScope { get; set; }
        public string Audience { get; set; }
    }
}
