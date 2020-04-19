﻿using Vendr.Contrib.PaymentProviders.Yourpay.Api.Models;
using Vendr.Core;
using Vendr.Core.Models;
using Vendr.Core.Web.Api;
using Vendr.Core.Web.PaymentProviders;

namespace Vendr.Contrib.PaymentProviders.Yourpay
{
    public abstract class YourpayPaymentProviderBase<TSettings> : PaymentProviderBase<TSettings>
            where TSettings : YourpaySettingsBase, new()
    {
        public YourpayPaymentProviderBase(VendrContext vendr)
            : base(vendr)
        { }

        public override string GetCancelUrl(OrderReadOnly order, TSettings settings)
        {
            settings.MustNotBeNull("settings");
            settings.CancelUrl.MustNotBeNull("settings.CancelUrl");

            return settings.CancelUrl;
        }

        public override string GetContinueUrl(OrderReadOnly order, TSettings settings)
        {
            settings.MustNotBeNull("settings");
            settings.ContinueUrl.MustNotBeNull("settings.ContinueUrl");

            return settings.ContinueUrl;
        }

        public override string GetErrorUrl(OrderReadOnly order, TSettings settings)
        {
            settings.MustNotBeNull("settings");
            settings.ErrorUrl.MustNotBeNull("settings.ErrorUrl");

            return settings.ErrorUrl;
        }

        protected YourpayClientConfig GetYourpayClientConfig(YourpaySettingsBase settings)
        {
            return new YourpayClientConfig
            {
                BaseUrl = "https://webservice.yourpay.dk",
                Token = settings.MerchantToken
            };
        }
    }
}
