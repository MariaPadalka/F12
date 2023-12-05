using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ThinkTwice_Context;
using BLL;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for CreateCategoryWindow.xaml
    /// </summary>
    public partial class CreateCategoryWindow : Window
    {
        private CategoryRepository categoryRepository = new CategoryRepository();
        public CreateCategoryWindow()
        {
            InitializeComponent();
        }
        private void CreateCategory(object sender, RoutedEventArgs e)
        {
            string title = titleTextBox.Text.Trim();
            string percentage = percentageTextBox.Text.Trim();
            string type = TypeComboBox.Text.Trim();

            if (errormessage.Text == "")
            {
                decimal.TryParse(percentage, out decimal dec_percentage);
              
                categoryRepository.CreateCategory(title, false, dec_percentage, type, App.GetCurrentUser()?.Id);
                //тут треба закрити вікно

                this.Close();
            }

        }
        private void ValidatePercentage(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string percentage = textBox.Text;

            if (!decimal.TryParse(percentage, out decimal result))
            {
                errormessage.Text = "Некоректний формат відсотку.";
            }
            else if (result < 1 || result > 100)
            {
                errormessage.Text = "Відсоток повинен бути від 1 до 100";
            }
            else
            {
                errormessage.Text = ""; // Очищаємо повідомлення про помилки, якщо все валідно
            }
        }

    }
}
