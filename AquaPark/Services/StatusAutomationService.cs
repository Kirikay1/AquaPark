using AquaPark.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AquaPark.Services
{
    public static class StatusAutomationService
    {
        public static void UpdateOperationalStatuses()
        {
            UpdateTicketStatuses();
            UpdatePaymentStatuses();
        }

        public static void UpdateTicketStatuses()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            var expiredTickets = AppData.db.Tickets
                .Where(t => t.Status == "Активен" && t.VisitDate < today)
                .ToList();

            foreach (var ticket in expiredTickets)
            {
                ticket.Status = "Истек";
            }

            var usedTicketIds = AppData.db.Visits
                .Where(v => v.ExitTime != null)
                .Select(v => v.TicketId)
                .Distinct()
                .ToList();

            var usedTickets = AppData.db.Tickets
                .Where(t => usedTicketIds.Contains(t.TicketId) && t.Status != "Отменен")
                .ToList();

            foreach (var ticket in usedTickets)
            {
                ticket.Status = "Использован";
            }

            SaveAutomationChanges();
        }

        public static string GetSalePaymentStatus(int saleId, decimal saleTotalAmount, int? ignoredPaymentId = null, decimal? editedAmount = null)
        {
            decimal paidAmount = AppData.db.Payments
                .Where(p => p.SaleId == saleId
                         && p.PaymentStatus != "Отменено"
                         && (!ignoredPaymentId.HasValue || p.PaymentId != ignoredPaymentId.Value))
                .Sum(p => (decimal?)p.Amount) ?? 0;

            if (editedAmount.HasValue)
            {
                paidAmount += editedAmount.Value;
            }

            if (paidAmount <= 0)
            {
                return "Ожидает оплаты";
            }

            return paidAmount >= saleTotalAmount ? "Оплачено" : "Частично оплачено";
        }

        public static void UpdatePaymentStatuses()
        {
            var saleIds = AppData.db.Payments
                .Select(p => p.SaleId)
                .Distinct()
                .ToList();

            foreach (int saleId in saleIds)
            {
                var sale = AppData.db.Sales.FirstOrDefault(s => s.SaleId == saleId);

                if (sale == null)
                {
                    continue;
                }

                string paymentStatus = GetSalePaymentStatus(saleId, sale.TotalAmount);

                var payments = AppData.db.Payments
                    .Where(p => p.SaleId == saleId && p.PaymentStatus != "Отменено")
                    .ToList();

                foreach (var payment in payments)
                {
                    payment.PaymentStatus = paymentStatus;
                }
            }

            SaveAutomationChanges();
        }

        private static void SaveAutomationChanges()
        {
            try
            {
                AppData.db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                foreach (var entry in AppData.db.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
                {
                    entry.State = EntityState.Unchanged;
                }
            }
        }
    }
}
