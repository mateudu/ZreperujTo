using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZreperujTo.UWP.Helpers
{
    class ZreperujToHelper
    {
        private readonly string _serviceUrl = @"https://zreperujto.azurewebsites.net";
        private readonly HttpClient _client = new HttpClient();
    }
}
