using System;
using System.Linq;
using System.Net.Mail;

namespace AquaPark.Services
{
    public static class ValidationService
    {
        public static bool ValidateClient(string fullName, DateTime? birthDate, string phone, string email, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                errorMessage = "Введите ФИО клиента";
                return false;
            }

            if (birthDate.HasValue && birthDate.Value.Date > DateTime.Today)
            {
                errorMessage = "Дата рождения не может быть позже сегодняшней даты";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(phone) && !IsValidPhone(phone))
            {
                errorMessage = "Введите корректный телефон";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
            {
                errorMessage = "Введите корректный email";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public static bool ValidateVisitDate(DateTime? visitDate, out string errorMessage)
        {
            if (visitDate == null)
            {
                errorMessage = "Выберите дату посещения";
                return false;
            }

            if (visitDate.Value.Date < DateTime.Today)
            {
                errorMessage = "Дата посещения не может быть раньше сегодняшней даты";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public static bool ValidatePositiveAmount(decimal amount, out string errorMessage)
        {
            if (amount <= 0)
            {
                errorMessage = "Введите корректную сумму";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public static bool ValidateAttraction(string attractionName, int ageLimit, int? heightLimit, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(attractionName))
            {
                errorMessage = "Введите название аттракциона";
                return false;
            }

            if (ageLimit < 0 || ageLimit > 120)
            {
                errorMessage = "Возрастное ограничение должно быть от 0 до 120";
                return false;
            }

            if (heightLimit.HasValue && (heightLimit.Value < 0 || heightLimit.Value > 250))
            {
                errorMessage = "Ограничение по росту должно быть от 0 до 250";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidPhone(string phone)
        {
            if (!phone.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' ' || c == '(' || c == ')'))
            {
                return false;
            }

            int digitsCount = phone.Count(char.IsDigit);
            return digitsCount >= 7 && digitsCount <= 15;
        }
    }
}
