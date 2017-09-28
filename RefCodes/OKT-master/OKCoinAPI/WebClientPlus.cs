using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OKCoinAPI
{
    public class WebClientPlus : WebClient
    {
        private int timeout = 0;
        private WebResponse webResponse = null;

        public WebClientPlus(int timeout)
            : base()
        {
            this.timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Timeout = this.timeout;
            request.ReadWriteTimeout = this.timeout;
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            this.webResponse = base.GetWebResponse(request);
            return this.webResponse;
        }

        public HttpStatusCode HttpStatusCode
        {
            get
            {
                if (this.webResponse == null)
                {
                    return HttpStatusCode.NotImplemented;
                }
                else
                {
                    return ((HttpWebResponse)this.webResponse).StatusCode;
                }
            }
        }
    }
}
