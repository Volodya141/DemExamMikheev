using BurgerPlusManagementSystem.DataBase;
using System.Linq;
using System.Windows;

namespace BurgerPlusManagementSystem.Windows
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();

            LoadUsers();
        }

        private void LoadUsers()
        {
            dgUsers.ItemsSource =
                BurgerPlusEntities
                .GetContext()
                .Users
                .ToList();
        }

        private void btnRefresh_Click(
            object sender,
            RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void btnUnblock_Click(
            object sender,
            RoutedEventArgs e)
        {
            Users selectedUser =
                dgUsers.SelectedItem as Users;

            if (selectedUser == null)
            {
                MessageBox.Show(
                    "Выберите пользователя",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            selectedUser.IsBlocked = false;

            selectedUser.FailedAttemps = 0;

            BurgerPlusEntities
                .GetContext()
                .SaveChanges();

            MessageBox.Show(
                "Пользователь успешно разблокирован",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            LoadUsers();
        }

        private void btnAdd_Click(
            object sender,
            RoutedEventArgs e)
        {
            UserEditWindow window =
                new UserEditWindow(null);

            window.ShowDialog();

            LoadUsers();
        }

        private void btnEdit_Click(
            object sender,
            RoutedEventArgs e)
        {
            Users selectedUser =
                dgUsers.SelectedItem as Users;

            if (selectedUser == null)
            {
                MessageBox.Show(
                    "Выберите пользователя",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            UserEditWindow window =
                new UserEditWindow(selectedUser);

            window.ShowDialog();

            LoadUsers();
        }
    }
}