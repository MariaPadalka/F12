namespace Presentation
{
    using System.Windows;
    using System.Windows.Controls;
    using BLL;

    /// <summary>
    /// Interaction logic for CreateCategoryWindow.xaml.
    /// </summary>
    public partial class CreateCategoryWindow : Window
    {
        private CategoryRepository categoryRepository = new CategoryRepository();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCategoryWindow"/> class.
        /// </summary>
        public CreateCategoryWindow()
        {
            this.InitializeComponent();
        }

        private void CreateCategory(object sender, RoutedEventArgs e)
        {
            string title = this.titleTextBox.Text.Trim();
            string percentage = this.percentageTextBox.Text.Trim();
            string type = this.TypeComboBox.Text.Trim();

            if (this.errormessage.Text == string.Empty)
            {
                decimal.TryParse(percentage, out decimal dec_percentage);

                this.categoryRepository.CreateCategory(title, false, dec_percentage, type, App.GetCurrentUser()?.Id);

                this.Close();
            }
        }

        private void ValidatePercentage(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string percentage = textBox.Text;

            if (!decimal.TryParse(percentage, out decimal result))
            {
                this.errormessage.Text = "Некоректний формат відсотку.";
            }
            else if (result < 1 || result > 100)
            {
                this.errormessage.Text = "Відсоток повинен бути від 1 до 100";
            }
            else
            {
                this.errormessage.Text = string.Empty; // Очищаємо повідомлення про помилки, якщо все валідно
            }
        }
    }
}