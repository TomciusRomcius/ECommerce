export enum PaymentProvider {
  Stripe = 0
};

export default interface PaymentSessionModel {
  paymentSessionId: string;
  checkoutUrl: string;
  paymentSessionProvider: PaymentProvider;
}
