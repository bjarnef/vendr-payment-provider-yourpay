﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Vendr.Contrib.PaymentProviders.Yourpay.Api;
using Vendr.Contrib.PaymentProviders.Yourpay.Api.Models;
using Vendr.Core;
using Vendr.Core.Models;
using Vendr.Core.Web.Api;
using Vendr.Core.Web.PaymentProviders;

namespace Vendr.Contrib.PaymentProviders.Yourpay
{
    [PaymentProvider("yourpay-checkout-onetime", "Yourpay (One Time)", "Yourpay payment provider for one time payments")]
    public class YourpayCheckoutOneTimePaymentProvider : YourpayPaymentProviderBase<YourpayCheckoutOneTimeSettings>
    {
        public YourpayCheckoutOneTimePaymentProvider(VendrContext vendr)
            : base(vendr)
        { }

        public override bool FinalizeAtContinueUrl => true;

        public override PaymentFormResult GenerateForm(OrderReadOnly order, string continueUrl, string cancelUrl, string callbackUrl, YourpayCheckoutOneTimeSettings settings)
        {
            var currency = Vendr.Services.CurrencyService.GetCurrency(order.CurrencyId);
            var currencyCode = currency.Code.ToUpperInvariant();

            // Ensure currency has valid ISO 4217 code
            if (!Iso4217.CurrencyCodes.ContainsKey(currencyCode))
            {
                throw new Exception("Currency must be a valid ISO 4217 currency code: " + currency.Name);
            }

            var orderAmount = AmountToMinorUnits(order.TotalPrice.Value.WithTax);

            var paymentToken = order.Properties["yourpayPaymentToken"]?.Value ?? null;
            string paymentFormLink = string.Empty;

            try
            {
                var clientConfig = GetYourpayClientConfig(settings);
                var client = new YourpayClient(clientConfig);

                var merchantId = settings.TestMode ? settings.MerchantId : settings.ProductionMerchantId;

                var data = new YourpayTokenQuery
                {
                    MerchantNumber = merchantId,
                    //ShopPlatform = "Vendr",
                    Amount = Convert.ToInt32(orderAmount),
                    Currency = currencyCode,
                    CartId = order.OrderNumber,
                    AcceptUrl = continueUrl,
                    CallbackUrl = callbackUrl,
                    CustomerName = $"{order.CustomerInfo.FirstName} {order.CustomerInfo.LastName}"
                };

                if (!string.IsNullOrWhiteSpace(settings.Language))
                {
                    data.Language = settings.Language;
                }

                // Generate token
                var payment = client.GenerateToken(data);

                if (payment.Content != null)
                {
                    paymentToken = payment.Content.Token;
                    paymentFormLink = payment.Content.FullUrl;
                }
            }
            catch (Exception ex)
            {
                Vendr.Log.Error<YourpayCheckoutOneTimePaymentProvider>(ex, "Yourpay - error creating payment.");
            }

            return new PaymentFormResult()
            {
                MetaData = new Dictionary<string, string>
                {
                    { "yourpayPaymentToken", paymentToken },
                },
                Form = new PaymentForm(paymentFormLink, FormMethod.Get)
            };
        }

        public override CallbackResult ProcessCallback(OrderReadOnly order, HttpRequestBase request, YourpayCheckoutOneTimeSettings settings)
        {
            return new CallbackResult
            {
                TransactionInfo = new TransactionInfo
                {
                    AmountAuthorized = order.TotalPrice.Value.WithTax,
                    TransactionFee = 0m,
                    TransactionId = Guid.NewGuid().ToString("N"),
                    PaymentStatus = PaymentStatus.Authorized
                }
            };
        }

        public override ApiResult FetchPaymentStatus(OrderReadOnly order, YourpayCheckoutOneTimeSettings settings)
        {
            // Get payment: https://yourpay.docs.apiary.io/#/reference/0/payment-data/payment-data

            try
            {

                //return new ApiResult()
                //{
                //    TransactionInfo = new TransactionInfoUpdate()
                //    {
                //        TransactionId = GetTransactionId(payment),
                //        PaymentStatus = GetPaymentStatus(payment)
                //    }
                //};
            }
            catch (Exception ex)
            {
                Vendr.Log.Error<YourpayCheckoutOneTimePaymentProvider>(ex, "Yourpay - FetchPaymentStatus");
            }

            return ApiResult.Empty;
        }

        public override ApiResult CancelPayment(OrderReadOnly order, YourpayCheckoutOneTimeSettings settings)
        {
            // Release payment: https://yourpay.docs.apiary.io/#/reference/0/payment-release/payment-release

            try
            {

                //return new ApiResult()
                //{
                //    TransactionInfo = new TransactionInfoUpdate()
                //    {
                //        TransactionId = GetTransactionId(payment),
                //        PaymentStatus = GetPaymentStatus(payment)
                //    }
                //};
            }
            catch (Exception ex)
            {
                Vendr.Log.Error<YourpayCheckoutOneTimePaymentProvider>(ex, "Yourpay - CancelPayment");
            }

            return ApiResult.Empty;
        }

        public override ApiResult CapturePayment(OrderReadOnly order, YourpayCheckoutOneTimeSettings settings)
        {
            // Capture payment: https://yourpay.docs.apiary.io/#/reference/0/payment-actions/capture-payment

            try
            {
                //return new ApiResult()
                //{
                //    TransactionInfo = new TransactionInfoUpdate()
                //    {
                //        TransactionId = GetTransactionId(payment),
                //        PaymentStatus = GetPaymentStatus(payment)
                //    }
                //};
            }
            catch (Exception ex)
            {
                Vendr.Log.Error<YourpayCheckoutOneTimePaymentProvider>(ex, "Yourpay - CapturePayment");
            }

            return ApiResult.Empty;
        }

        public override ApiResult RefundPayment(OrderReadOnly order, YourpayCheckoutOneTimeSettings settings)
        {
            // Refund payment: https://yourpay.docs.apiary.io/#/reference/0/payment-actions/refund-payment

            try
            {

                //return new ApiResult()
                //{
                //    TransactionInfo = new TransactionInfoUpdate()
                //    {
                //        TransactionId = GetTransactionId(refund),
                //        PaymentStatus = GetPaymentStatus(refund)
                //    }
                //};
            }
            catch (Exception ex)
            {
                Vendr.Log.Error<YourpayCheckoutOneTimePaymentProvider>(ex, "Yourpay - RefundPayment");
            }

            return ApiResult.Empty;
        }
    }
}
