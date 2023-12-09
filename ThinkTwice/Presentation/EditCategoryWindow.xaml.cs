using BLL;
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

namespace Presentation
{
    /// <summary>
    /// Interaction logic for EditCategoryWindow.xaml
    /// </summary>
    public partial class EditCategoryWindow : Window
    {
        private Category? category;
        private Settings settingsPage;
        private readonly CategoryRepository categoryRepository = new CategoryRepository();

        public EditCategoryWindow(Settings settingsPage, Guid categoryId)
        {
            this.InitializeComponent();
            this.settingsPage = settingsPage;
            Category? category = this.categoryRepository.GetCategoryById(categoryId);
            this.category = category;
            this.setFields(category);
        }

        private void setFields(Category? category)
        {
            this.titleTextBox.Text = category.Title;
            this.textBoxType.Text = category.Type;
            if (this.category.IsGeneral)
            {
                this.titleTextBox.IsEnabled = false;
                this.submitButton.Visibility = Visibility.Collapsed;
                this.Title = "Деталі категорії";
            }
            else if (this.category.Type == "Витрати")
            {
                this.percentageTextBox.Visibility = Visibility.Visible;
                this.PercentageTextBlock.Visibility = Visibility.Visible;
                this.percentageBorder.Visibility = Visibility.Visible;
                this.percentageTextBox.Text = category.PercentageAmount.ToString();
            }
        }

        private void UpdateCategory(object sender, RoutedEventArgs e)
        {
            if (this.errormessage.Text != string.Empty)
            {
                return;
            }

            string title = this.titleTextBox.Text.Trim();
            string percentage = this.percentageTextBox.Text.Trim();
            if (title == string.Empty || (this.category.Type == "Витрати" && percentage == string.Empty))
            {
                this.errormessage.Text = "Заповніть усі поля";
            }
            else
            {
                this.errormessage.Text = string.Empty;
            }

            if (this.errormessage.Text == string.Empty)
            {
                decimal.TryParse(percentage, out decimal dec_percentage);
                this.category.Title = title;
                this.category.PercentageAmount = Math.Round(dec_percentage, 2);
                this.categoryRepository.Update(this.category);
                this.errormessage.Text = string.Empty;
                this.Close();

                this.settingsPage.UpdateCategory(this.category);
                //this.settingsPage.AddEmptyCategory();
            }
        }

        private void ValidatePercentage(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string percentage = textBox.Text;
            var userCategories = this.settingsPage.Categories.Where(categ => categ.Type == "Витрати");
            var sum = userCategories.Sum(category => category.PercentageAmount);
            sum -= this.category.PercentageAmount;

            if (!decimal.TryParse(percentage, out decimal result))
            {
                this.errormessage.Text = "Некоректний формат відсотку.";
            }
            else if (result < 1 || result > 100)
            {
                this.errormessage.Text = "Відсоток повинен бути від 1 до 100";
            }
            else if (result > (100 - sum))
            {
                this.errormessage.Text = $"У вас залишилось лише {100 - sum}% фінансів для витрат";
            }
            else
            {
                this.errormessage.Text = string.Empty;
            }
        }

        private void TitleChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string title = textBox.Text;
            if(title.Length > 0)
            {
                var found_category = this.categoryRepository.GetCategoryByName(App.GetCurrentUser().Id, title);
                if (found_category != null && found_category.Id != this.category.Id)
                {
                    this.errormessage.Text = "Категорія з такою назвою вже існує";
                }
            }
            else
            {
                this.errormessage.Text = "Заповніть всі поля";
            }
        }
    }

}
