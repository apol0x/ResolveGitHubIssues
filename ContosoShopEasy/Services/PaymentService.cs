using ContosoShopEasy.Models;
using ContosoShopEasy.Data;

namespace ContosoShopEasy.Services
{
    public class PaymentService
    {
        private readonly OrderRepository _orderRepository;

        public PaymentService(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public bool ProcessPayment(string cardNumber, string cardHolderName, string expiryDate, string cvv, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            string normalizedCardNumber = cardNumber.Replace(" ", "").Replace("-", "");
            if (!ValidateCardNumber(normalizedCardNumber))
                return false;

            if (!ValidateExpiryDate(expiryDate))
                return false;

            Console.WriteLine("[INFO] Simulating payment authorization; no real gateway or tokenization is used.");
            Thread.Sleep(1000); // Simulate network delay

            string transactionId = GenerateTransactionId();
            
            var paymentInfo = new PaymentInfo
            {
                Method = PaymentMethod.CreditCard,
                CardLastFourDigits = normalizedCardNumber[^4..],
                CardType = IdentifyCardType(normalizedCardNumber),
                Amount = amount,
                ProcessedDate = DateTime.UtcNow,
                Status = PaymentStatus.Approved,
                TransactionId = transactionId
            };

            Console.WriteLine("[SUCCESS] Simulated payment approved.");

            return true;
        }

        private bool ValidateCardNumber(string cardNumber)
        {
            return cardNumber.Length >= 13 &&
                   cardNumber.Length <= 19 &&
                   cardNumber.All(character => character >= '0' && character <= '9');
        }

        private PaymentCardType IdentifyCardType(string cardNumber)
        {
            if (cardNumber.StartsWith('4'))
                return PaymentCardType.Visa;

            if (cardNumber.Length >= 2 &&
                int.TryParse(cardNumber.Substring(0, 2), out int firstTwoDigits) &&
                firstTwoDigits >= 51 && firstTwoDigits <= 55)
                return PaymentCardType.Mastercard;

            if (cardNumber.Length >= 4 &&
                int.TryParse(cardNumber.Substring(0, 4), out int firstFourDigits) &&
                firstFourDigits >= 2221 && firstFourDigits <= 2720)
                return PaymentCardType.Mastercard;

            return PaymentCardType.Unknown;
        }

        private bool ValidateExpiryDate(string expiryDate)
        {
            // Security vulnerability: Basic validation only
            if (string.IsNullOrEmpty(expiryDate) || !expiryDate.Contains("/"))
                return false;

            var parts = expiryDate.Split('/');
            if (parts.Length != 2)
                return false;

            if (int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
            {
                if (year < 100) year += 2000; // Convert YY to YYYY
                var expiry = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
                return expiry >= DateTime.Now;
            }

            return false;
        }

        private string GenerateTransactionId()
        {
            return $"SIM-TXN-{Guid.NewGuid():N}";
        }

        public bool RefundPayment(string transactionId, decimal amount)
        {
            Console.WriteLine("[INFO] Simulating refund processing.");
            Thread.Sleep(500);

            Console.WriteLine("[SUCCESS] Simulated refund processed.");
            return true;
        }

        public List<PaymentInfo> GetPaymentHistory(int userId)
        {
            // In a real app, this would query the database
            // For demo purposes, we'll return empty list
            return new List<PaymentInfo>();
        }
    }
}