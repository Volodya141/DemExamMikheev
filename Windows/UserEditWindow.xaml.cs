using BurgerPlusManagementSystem.DataBase;
using System.Linq;
using System.Windows;

namespace BurgerPlusManagementSystem.Windows
{
    public partial class UserEditWindow : Window
    {
        private Users currentUser;

        public UserEditWindow(Users user)
        {
            InitializeComponent();

            cbRole.ItemsSource =
                BurgerPlusEntities
                .GetContext()
                .Roles
                .ToList();

            if (user == null)
            {
                currentUser = new Users();
            }
            else
            {
                currentUser = user;

                tbSecondName.Text =
                    currentUser.UserSecondName;

                tbFirstName.Text =
                    currentUser.UserFirstName;

                tbPatronymic.Text =
                    currentUser.UserPatronymic;

                tbLogin.Text =
                    currentUser.Login;

                pbPassword.Password =
                    currentUser.Password;

                chBlocked.IsChecked =
                    currentUser.IsBlocked;

                cbRole.SelectedValue =
                    currentUser.UserRoleKey;
            }
        }

        private void BtnSave_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbSecondName.Text) ||
                string.IsNullOrWhiteSpace(tbFirstName.Text) ||
                string.IsNullOrWhiteSpace(tbLogin.Text) ||
                string.IsNullOrWhiteSpace(pbPassword.Password) ||
                cbRole.SelectedItem == null)
            {
                MessageBox.Show(
                    "Заполните обязательные поля",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            bool loginExists =
                BurgerPlusEntities
                .GetContext()
                .Users
                .Any(x =>
                    x.Login == tbLogin.Text &&
                    x.UserID != currentUser.UserID);

            if (loginExists)
            {
                MessageBox.Show(
                    "Пользователь с таким логином уже существует",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            currentUser.UserSecondName =
                tbSecondName.Text;

            currentUser.UserFirstName =
                tbFirstName.Text;

            currentUser.UserPatronymic =
                tbPatronymic.Text;

            currentUser.Login =
                tbLogin.Text;

            currentUser.Password =
                pbPassword.Password;

            currentUser.UserRoleKey =
                (int)cbRole.SelectedValue;

            currentUser.IsBlocked =
                chBlocked.IsChecked ?? false;

            if (currentUser.UserID == 0)
            {
                BurgerPlusEntities
                    .GetContext()
                    .Users
                    .Add(currentUser);
            }

            BurgerPlusEntities
                .GetContext()
                .SaveChanges();

            MessageBox.Show(
                "Данные успешно сохранены",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;

            Close();
        }
    }

}