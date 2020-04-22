﻿using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Vendr.Contrib.PaymentProviders.Yourpay.Api.Models;

namespace Vendr.Contrib.PaymentProviders.Yourpay.Api
{
    public class YourpayClient
    {
        private YourpayClientConfig _config;

        public YourpayClient(YourpayClientConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Generate token for payment window
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public PaymentTokenResult GenerateToken(object data)
        {
            return Request($"/v4.3/generate_token", (req) => req
                .SetQueryParams(data)
                .GetJsonAsync<PaymentTokenResult>());
        }

        /// <summary>
        /// Get payment data
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns></returns>
        public PaymentData GetPaymentData(string id)
        {
            return Request($"/v4.3/payment_data?id={id}", (req) => req
                .GetJsonAsync<PaymentData>());
        }

        /// <summary>
        /// Release payment
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns></returns>
        public PaymentResultBase ReleasePayment(string id)
        {
            return Request($"/v4.3/payment_release?id={id}", (req) => req
                .GetJsonAsync<PaymentResultBase>());
        }

        /// <summary>
        /// Capture payment
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <param name="amount">Amount to capture</param>
        /// <returns></returns>
        public PaymentResultBase CapturePayment(string id, decimal amount)
        {
            return Request($"/v4.3/payment_action?id={id}&amount={amount}", (req) => req
                .GetJsonAsync<PaymentResultBase>());
        }

        /// <summary>
        /// Refund payment
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <param name="amount">Negative amount for refund</param>
        /// <returns></returns>
        public PaymentResultBase RefundPayment(string id, decimal amount)
        {
            return Request($"/v4.3/payment_action?id={id}&amount={amount}", (req) => req
                .GetJsonAsync<PaymentResultBase>());
        }

        private TResult Request<TResult>(string url, Func<IFlurlRequest, Task<TResult>> func)
        {
            var result = default(TResult);

            try
            {
                var req = new FlurlRequest(_config.BaseUrl + url)
                        .ConfigureRequest(x =>
                        {
                            var jsonSettings = new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                DefaultValueHandling = DefaultValueHandling.Include,
                                MissingMemberHandling = MissingMemberHandling.Ignore
                            };
                            x.JsonSerializer = new NewtonsoftJsonSerializer(jsonSettings);
                        }).
                        SetQueryParam("merchant_token", _config.Token);

                result = func.Invoke(req).Result;
            }
            catch (FlurlHttpException ex)
            {
                throw;
            }

            return result;
        }
    }
}