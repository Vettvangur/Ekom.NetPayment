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

Payment Provider names must match the searched for names in Umbraco.NetPayment.Payment.Request

f.x. "Borgun" or "borgun"


uWebshop.NetPayment.BorgunLoans

requires the following dictionary keys

- BorgunLoans
- InterestFree
- WithInterest
- ChristmasInvoice
