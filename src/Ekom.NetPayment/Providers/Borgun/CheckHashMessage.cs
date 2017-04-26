namespace Umbraco.NetPayment.Borgun
{
    class CheckHashMessage
    {
        public string message;

        public CheckHashMessage(string orderId, string amount, string currency)
        {
            message = orderId + "|" + amount + "|" + currency;
        }

        public CheckHashMessage(string merchantId, string returnUrlSuccess, string returnUrlSuccessServer,
                         string orderId, string amount, string currency)
        {
            message = merchantId + "|" + returnUrlSuccess + "|" + returnUrlSuccessServer + "|" + orderId + "|"
                                 + amount + "|" + currency;
        }
    }
}
