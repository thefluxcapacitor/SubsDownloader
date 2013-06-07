namespace SubsDownloader
{
    using System;
    using System.Net;

    public class CookieAwareWebClient : WebClient
    {
        private readonly CookieContainer container;

        public CookieAwareWebClient(CookieContainer container)
        {
            this.container = container;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            var webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.CookieContainer = this.container;
            }

            return request;
        }
    }
}
