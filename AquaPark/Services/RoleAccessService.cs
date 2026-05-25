using AquaPark.Data;
using System.Windows;

namespace AquaPark.Services
{
    public static class RoleAccessService
    {
        private const string Clients = "Clients";
        private const string Tickets = "Tickets";
        private const string Attractions = "Attractions";
        private const string Sales = "Sales";
        private const string Payments = "Payments";
        private const string Schedules = "Schedules";
        private const string Reports = "Reports";
        private const string Users = "Users";
        private const string Logs = "Logs";

        public static bool CanAddOrEdit(string sectionName)
        {
            string roleName = AppData.CurrentUser?.Role?.RoleName ?? "";

            if (roleName == "Администратор" || roleName == "Менеджер")
            {
                return true;
            }

            if (roleName == "Кассир")
            {
                return sectionName == Tickets
                    || sectionName == Sales
                    || sectionName == Payments;
            }

            return false;
        }

        public static bool CanDelete()
        {
            return AppData.CurrentUser?.Role?.RoleName == "Администратор";
        }

        public static bool CanManageUsers()
        {
            return CanDelete();
        }

        public static bool CanOpenMenuSection(string sectionName)
        {
            string roleName = AppData.CurrentUser?.Role?.RoleName ?? "";

            if (sectionName == Users || sectionName == Logs)
            {
                return CanManageUsers();
            }

            if (roleName == "Администратор" || roleName == "Менеджер")
            {
                return true;
            }

            if (roleName == "Кассир")
            {
                return sectionName == Tickets
                    || sectionName == Sales
                    || sectionName == Payments;
            }

            if (roleName == "Сотрудник")
            {
                return sectionName == Attractions
                    || sectionName == Schedules;
            }

            if (sectionName == Reports)
            {
                return roleName == "Администратор"
                    || roleName == "Менеджер";
            }

            return false;
        }

        public static Visibility AddEditVisibility(string sectionName)
        {
            return CanAddOrEdit(sectionName) ? Visibility.Visible : Visibility.Collapsed;
        }

        public static Visibility DeleteVisibility()
        {
            return CanDelete() ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
