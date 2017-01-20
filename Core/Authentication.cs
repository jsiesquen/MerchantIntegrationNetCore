//
//  Authentication.cs
//  merchant_integration
//  
//  Created by Juan Pablo Yiguerimián on 10/25/16.
//  Copyright © 2016 ForceITS. All rights reserved.
//
//  The software code, programs, and documentation are the confidential and
//  proprietary information of ForceITS Corporation ("Confidential Information").
//  You shall not disclose such Confidential Information and shall use it only
//  in accordance with the terms of the license agreement you entered into
//  with ForceITS or one of its licensed distributors.
//
//  ForceITS MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF THE
//  SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//  PURPOSE, OR NON-INFRINGEMENT. ForceITS SHALL NOT BE LIABLE FOR ANY DAMAGES
//  SUFFERED BY LICENSEE AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
//  THIS SOFTWARE OR ITS DERIVATIVES.
//

using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using merchant_integration.Core.Payloads;

namespace merchant_integration.Core
{

    /// <summary>
    /// This class interacts with Authentication Server to get a token
    /// that is needed to execute some processes, such as Payment.
    /// </summary>
    public class Authentication
    {

        /// <summary>
        /// This method connects to Authentication service and return a valid token
        /// in case of successful authentication. Otherwise, it returns an empty string.
        /// </summary>
        /// <param name="merchantId">The Id of the Merchant that will be used to authenticate</param>
        /// <param name="apiKey">The api key set for this merchant</param>
        /// <returns>A string that has the value of the token, or empty in case of authentication has failed.</returns>
        public string GetToken(string merchantId, string apiKey)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = GetAuthServiceUrl();
                AddAuthorizationHeader(httpClient, merchantId, apiKey);

                Task<HttpResponseMessage> httpResponse = GetAuthResponse(httpClient);
                Task<string> result = httpResponse.Result.Content.ReadAsStringAsync();
                AuthResult authResult = JsonConvert.DeserializeObject<AuthResult>(result.Result);

                return authResult.tok;
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("{0}. {1}", "An error occurred getting token", ex.Message));
            }
        }

        /// <summary>
        /// This methods add a header to the request that will contain
        /// a Basic Authorization including merchant Id an its api key.
        /// Both are separated by a ":" and encoded in base 64.
        /// </summary>
        /// <param name="httpClient">In this object will be added the authorization header</param>
        /// <param name="merchantId">The Id of the Merchant that will be used to authenticate</param>
        /// <param name="apiKey">The api key set for this merchant</param>
        private void AddAuthorizationHeader(HttpClient httpClient, string merchantId, string apiKey)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", merchantId, apiKey)))));
        }

        /// <summary>
        /// This method builds a URL that includes the protocol, the host and the port of the UI app.
        /// It reads the configuration to build it.
        /// </summary>
        /// <returns>An URL with this fashion 'http://host:port'</returns>
        private Uri GetAuthServiceUrl()
        {
            return new Uri(string.Format("{0}{1}:{2}", "http://", Program.Configuration["UIApp:Host"], Program.Configuration["UIApp:Port"]));
        }

        /// <summary>
        /// It builds a new URL from the base address of the httpClient
        /// that will point to the Authentication path of the service.
        /// It reads the configuration to get this path.
        /// </summary>
        /// <param name="httpClient">The http client which base address will be used to build the URL</param>
        /// <returns>An URL pointing to the authentication method, with this fashion 'http://host:port/method'</returns>
        private Uri GetAuthenticationUrl(HttpClient httpClient)
        {
            return new Uri(httpClient.BaseAddress, Program.Configuration["UIApp:AuthPath"]);
        }

        /// <summary>
        /// This methods execute an async post to the authentication service.
        /// </summary>
        /// <param name="httpClient">The http client used to send the request</param>
        /// <returns> object that represent the response. The request will be sent when the reponse result was queried.</returns>
        private Task<HttpResponseMessage> GetAuthResponse(HttpClient httpClient)
        {
            HttpContent httpContent = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
            return httpClient.PostAsync(GetAuthenticationUrl(httpClient), httpContent);
        }

    }
}
