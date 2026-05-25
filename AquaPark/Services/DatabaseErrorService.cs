using AquaPark.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;

namespace AquaPark.Services
{
    public static class DatabaseErrorService
    {
        public static bool TrySaveChanges(string successMessage)
        {
            try
            {
                AppData.db.SaveChanges();

                MessageBox.Show(successMessage,
                                "Сохранение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return true;
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Не удалось сохранить данные. Проверьте заполнение полей и связанные записи.",
                                "Ошибка базы данных",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось выполнить операцию. Проверьте подключение к базе данных.",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }
        }
    }
}
