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

- reportUrl

- paymentInfo
