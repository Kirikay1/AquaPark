using AquaPark.Data;
using System.Linq;

namespace AquaPark.Services
{
    public static class DuplicateCheckService
    {
        public static bool ClientPhoneExists(string phone, int? ignoredClientId = null)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            string normalizedPhone = Normalize(phone);

            return AppData.db.Clients.Any(c =>
                c.Phone != null
                && c.Phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "") == normalizedPhone
                && (!ignoredClientId.HasValue || c.ClientId != ignoredClientId.Value));
        }

        public static bool ClientEmailExists(string email, int? ignoredClientId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string normalizedEmail = email.Trim().ToLower();

            return AppData.db.Clients.Any(c =>
                c.Email != null
                && c.Email.ToLower() == normalizedEmail
                && (!ignoredClientId.HasValue || c.ClientId != ignoredClientId.Value));
        }

        public static bool TicketHasSale(int ticketId, int? ignoredSaleId = null)
        {
            return AppData.db.Sales.Any(s =>
                s.TicketId == ticketId
                && (!ignoredSaleId.HasValue || s.SaleId != ignoredSaleId.Value));
        }

        public static bool SaleIsFullyPaid(int saleId, decimal? ignoredPaymentAmount = null)
        {
            var sale = AppData.db.Sales.FirstOrDefault(s => s.SaleId == saleId);

            if (sale == null)
            {
                return false;
            }

            decimal paymentsTotal = AppData.db.Payments
                .Where(p => p.SaleId == saleId)
                .Sum(p => (decimal?)p.Amount) ?? 0;

            if (ignoredPaymentAmount.HasValue)
            {
                paymentsTotal -= ignoredPaymentAmount.Value;
            }

            return paymentsTotal >= sale.TotalAmount;
        }

        public static bool PaymentExceedsSaleAmount(int saleId, decimal paymentAmount, int? ignoredPaymentId = null)
        {
            var sale = AppData.db.Sales.FirstOrDefault(s => s.SaleId == saleId);

            if (sale == null)
            {
                return false;
            }

            decimal paymentsTotal = AppData.db.Payments
                .Where(p => p.SaleId == saleId
                         && (!ignoredPaymentId.HasValue || p.PaymentId != ignoredPaymentId.Value))
                .Sum(p => (decimal?)p.Amount) ?? 0;

            return paymentsTotal + paymentAmount > sale.TotalAmount;
        }

        private static string Normalize(string value)
        {
            return value
                .Trim()
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "");
        }
    }
}
