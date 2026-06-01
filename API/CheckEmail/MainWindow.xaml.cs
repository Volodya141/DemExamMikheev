using System;
using System.Linq;
using System.Net.Http;
using System.Windows;

namespace CheckEmail
{
    public partial class MainWindow : Window
    {
        private string email = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetDataButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                using (HttpClient client = new HttpClient())
                {

                    string url = "http://localhost:4444/TransferSimulator/email";

                    string jsonAnswer = await client.GetStringAsync(url);

                    jsonAnswer = jsonAnswer.Replace("{", "");
                    jsonAnswer = jsonAnswer.Replace("}", "");
                    jsonAnswer = jsonAnswer.Replace("\"", "");
                    jsonAnswer = jsonAnswer.Replace("value :", "");

                    email = jsonAnswer.Trim();

                    txtBoxEmailText.Text = email;

                    txtBoxResultText.Text = "";
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка API", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(email))
            {
                txtBoxResultText.Text = "Сначала получите данные";


                return;
            }

            string forbiddenSymbols = "!#$%^&*():;_-+=[]{}<>?/|\\&";

            if (email.Intersect(forbiddenSymbols).Count() > 0)
            {
                txtBoxResultText.Text = "Не корректная электронная почта";
                return;
            }
            if (email.Contains("mail"))
            {
                txtBoxResultText.Text = "Все верно";
                return;
            }
            else
            {
                txtBoxResultText.Text = "Не корректная электронная почта";
                return;
            }

          
        }
    }
}