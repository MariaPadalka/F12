namespace Presentation
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using BLL;

    /// <summary>
    /// Interaction logic for CreateCategoryWindow.xaml.
    /// </summary>
    public partial class CreateCategoryWindow : Window
    {
        private CategoryRepository categoryRepository = new CategoryRepository();
        private Settings settingsPage;

        public CreateCategoryWindow(Settings page)
        {
            this.InitializeComponent();
            this.settingsPage = page;
        }

        private void CreateCategory(object sender, RoutedEventArgs e)
        {
            if (this.errormessage.Text != string.Empty)
            {
                return;
            }

            string title = this.titleTextBox.Text.Trim();
            string percentage = this.percentageTextBox.Text.Trim();
            string type = this.TypeComboBox.Text.Trim();
            if (title == string.Empty || percentage == string.Empty)
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

                this.categoryRepository.CreateCategory(title, false, dec_percentage, type, App.GetCurrentUser()?.Id);
                this.settingsPage.UpdateItems();
                this.settingsPage.AddEmptyCategory();
                this.titleTextBox.Text = string.Empty;
                this.percentageTextBox.Text = string.Empty;
                this.TypeComboBox.Text = "Витрати";

                this.errormessage.Text = string.Empty;
            }
        }

        private void ValidatePercentage(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string percentage = textBox.Text;
            var userCategories = this.settingsPage.Categories.Where(categ => categ.UserId != null && categ.Type == "Витрати");
            var sum = userCategories.Sum(category => category.PercentageAmount);

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

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)this.TypeComboBox.SelectedItem;

            if (selectedItem.Content.ToString() == "Витрати")
            {
                this.percentageTextBox.Visibility = Visibility.Visible;
                this.PercentageTextBlock.Visibility = Visibility.Visible;
                this.percentageBorder.Visibility = Visibility.Visible;
            }
            else
            {
                this.percentageTextBox.Visibility = Visibility.Collapsed;
                this.PercentageTextBlock.Visibility = Visibility.Collapsed;
                this.percentageBorder.Visibility = Visibility.Collapsed;
            }
        }
    }
}