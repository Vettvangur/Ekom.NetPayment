# Umbraco.NetPayment


For configuration several dictionary keys are needed

values are umbraco node ids


- PaymentError

- PaymentSuccess


- PaymentProviders (Contains all payment providers)


Payment providers must have the following properties

- title

- portalUrl


- successUrl

- errorUrl


they can additionally contain

- paymentInfo

Payment Provider names must match the searched for names in Umbraco.NetPayment.Payment.Request

f.x. "Borgun" or "borgun"


A custom callback can be assigned to the static class

Payment.callback = PaymentCallback.Method;

This method is then called after a successful payment has been verified
