using AquaPark.Models;
using AquaPark.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AquaPark.Views
{
    /// <summary>
    /// Логика взаимодействия для EditClientPage.xaml
    /// </summary>
    public partial class EditClientPage : Page
    {
        public EditClientPage(int clientId)
        {
            InitializeComponent();
            DataContext = new EditClientViewModel(clientId);
        }
    }
}
