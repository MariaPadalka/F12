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
        private Settings settingsPage;
        public CreateCategoryWindow(Settings page)
        {
            InitializeComponent();
            settingsPage = page;
        }
        private void CreateCategory(object sender, RoutedEventArgs e)
        {
            if (errormessage.Text != "") { return; };
            string title = titleTextBox.Text.Trim();
            string percentage = percentageTextBox.Text.Trim();
            string type = TypeComboBox.Text.Trim();
            if( title == "" || percentage == "")
            {
                errormessage.Text = "Заповніть усі поля";
            }
            else
            {
                errormessage.Text = "";
            }

            if (errormessage.Text == "")
            {
                decimal.TryParse(percentage, out decimal dec_percentage);
              
                categoryRepository.CreateCategory(title, false, dec_percentage, type, App.GetCurrentUser()?.Id);
                settingsPage.updateItems();
                settingsPage.AddEmptyCategory();
                titleTextBox.Text = "";
                percentageTextBox.Text = "";
                TypeComboBox.Text = "Витрати";

                errormessage.Text = "";

                //this.Close();
            }

        }
        private void ValidatePercentage(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string percentage = textBox.Text;
            var userCategories = settingsPage.categories.Where(categ => categ.UserId != null && categ.Type == "Витрати");
            var sum = userCategories.Sum(category => category.PercentageAmount);


            if (!decimal.TryParse(percentage, out decimal result))
            {
                errormessage.Text = "Некоректний формат відсотку.";
            }
            else if (result < 1 || result > 100)
            {
                errormessage.Text = "Відсоток повинен бути від 1 до 100";
            }
            else if(result > (100 - sum))
            {
                errormessage.Text = $"У вас залишилось лише {100 - sum}% фінансів для витрат";

            }
            else
            {
                errormessage.Text = ""; // Очищаємо повідомлення про помилки, якщо все валідно
            }
        }
        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Отримуємо вибраний елемент ComboBox
            ComboBoxItem selectedItem = (ComboBoxItem)TypeComboBox.SelectedItem;

            if(selectedItem.Content.ToString() == "Витрати")
            {
                percentageTextBox.Visibility = Visibility.Visible;
                PercentageTextBlock.Visibility = Visibility.Visible;
                percentageBorder.Visibility = Visibility.Visible;
            }
            else
            {
                percentageTextBox.Visibility = Visibility.Collapsed;
                PercentageTextBlock.Visibility = Visibility.Collapsed;
                percentageBorder.Visibility = Visibility.Collapsed;
            }
        }


    }
}
