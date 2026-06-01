using BurgerPlusManagementSystem.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BurgerPlusManagementSystem.Windows
{
    public partial class LoginWindow : Window
    {
        private List<int> puzzleOrder;


        private int selectedIndex = -1;

        private bool captchaPassed = false;

        private int captchaAttempts = 0;

        public LoginWindow()
        {
            InitializeComponent();

            GeneratePuzzle();
        }

        private void GeneratePuzzle()
        {
            puzzleOrder = new List<int>()
        {
            1,2,3,4
        };

            Random random = new Random();

            puzzleOrder = puzzleOrder
                .OrderBy(x => random.Next())
                .ToList();

            selectedIndex = -1;

            ResetBorders();

            tbPuzzleHint.Text =
                "Выберите первый фрагмент";

            LoadPuzzle();
        }

        private void LoadPuzzle()
        {
            img1.Source = LoadImage(puzzleOrder[0]);
            img2.Source = LoadImage(puzzleOrder[1]);
            img3.Source = LoadImage(puzzleOrder[2]);
            img4.Source = LoadImage(puzzleOrder[3]);
        }

        private BitmapImage LoadImage(int number)
        {
            return new BitmapImage(
                new Uri(
                    $"pack://application:,,,/Resources/{number}.png",
                    UriKind.Absolute));
        }

        private void ResetBorders()
        {
            border1.BorderBrush = Brushes.LightGray;
            border2.BorderBrush = Brushes.LightGray;
            border3.BorderBrush = Brushes.LightGray;
            border4.BorderBrush = Brushes.LightGray;

            border1.BorderThickness = new Thickness(2);
            border2.BorderThickness = new Thickness(2);
            border3.BorderThickness = new Thickness(2);
            border4.BorderThickness = new Thickness(2);
        }

        private void HighlightBorder(int index)
        {
            ResetBorders();

            switch (index)
            {
                case 0:
                    border1.BorderBrush = Brushes.LimeGreen;
                    border1.BorderThickness = new Thickness(4);
                    break;

                case 1:
                    border2.BorderBrush = Brushes.LimeGreen;
                    border2.BorderThickness = new Thickness(4);
                    break;

                case 2:
                    border3.BorderBrush = Brushes.LimeGreen;
                    border3.BorderThickness = new Thickness(4);
                    break;

                case 3:
                    border4.BorderBrush = Brushes.LimeGreen;
                    border4.BorderThickness = new Thickness(4);
                    break;
            }
        }

        private int GetBorderIndex(Border border)
        {
            if (border == border1)
                return 0;

            if (border == border2)
                return 1;

            if (border == border3)
                return 2;

            return 3;
        }

        private void PuzzlePart_Click(
            object sender,
            MouseButtonEventArgs e)
        {
            Border border =
                sender as Border;

            int currentIndex =
                GetBorderIndex(border);

            if (selectedIndex == -1)
            {
                selectedIndex = currentIndex;

                HighlightBorder(currentIndex);

                tbPuzzleHint.Text =
                    "Теперь выберите второй фрагмент";

                return;
            }

            int temp =
                puzzleOrder[selectedIndex];

            puzzleOrder[selectedIndex] =
                puzzleOrder[currentIndex];

            puzzleOrder[currentIndex] = temp;

            selectedIndex = -1;

            tbPuzzleHint.Text =
                "Выберите первый фрагмент";

            ResetBorders();

            LoadPuzzle();
        }

        private void BtnCheckPuzzle_Click(
            object sender,
            RoutedEventArgs e)
        {
            bool success =
                puzzleOrder.SequenceEqual(
                new List<int>()
                {
                1,2,3,4
                });

            if (success)
            {
                captchaPassed = true;

                MessageBox.Show(
                    "Капча успешно пройдена",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            captchaAttempts++;

            MessageBox.Show(
                $"Пазл собран неверно. Попытка {captchaAttempts} из 3.",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            if (captchaAttempts >= 3)
            {
                var user =
                    BurgerPlusEntities
                    .GetContext()
                    .Users
                    .FirstOrDefault(x =>
                        x.Login == tbLogin.Text);

                if (user != null)
                {
                    user.IsBlocked = true;

                    BurgerPlusEntities
                        .GetContext()
                        .SaveChanges();
                }

                MessageBox.Show(
                    "Вы заблокированы. Обратитесь к администратору",
                    "Блокировка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
            }

            GeneratePuzzle();
        }

        private void BtnLogin_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbLogin.Text))
                {
                    MessageBox.Show(
                        "Введите логин",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                if (string.IsNullOrWhiteSpace(pbPassword.Password))
                {
                    MessageBox.Show(
                        "Введите пароль",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                if (!captchaPassed)
                {
                    MessageBox.Show(
                        "Сначала необходимо собрать пазл",
                        "Проверка безопасности",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                Users user =
                    BurgerPlusEntities
                    .GetContext()
                    .Users
                    .FirstOrDefault(x =>
                        x.Login == tbLogin.Text);

                if (user == null)
                {
                    MessageBox.Show(
                        "Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные",
                        "Ошибка авторизации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                if (user.IsBlocked)
                {
                    MessageBox.Show(
                        "Вы заблокированы. Обратитесь к администратору",
                        "Блокировка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Stop);

                    return;
                }

                if (user.Password != pbPassword.Password)
                {
                    user.FailedAttemps++;

                    if (user.FailedAttemps >= 3)
                    {
                        user.IsBlocked = true;

                        MessageBox.Show(
                            "Вы заблокированы. Обратитесь к администратору",
                            "Блокировка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Stop);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные",
                            "Ошибка авторизации",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }

                    BurgerPlusEntities
                        .GetContext()
                        .SaveChanges();

                    return;
                }

                user.FailedAttemps = 0;

                BurgerPlusEntities
                    .GetContext()
                    .SaveChanges();

                MessageBox.Show(
                    "Вы успешно авторизовались",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                if (user.Roles.RoleName ==
                    "Администратор")
                {
                    AdminWindow adminWindow =
                        new AdminWindow();

                    adminWindow.Show();
                }
                else
                {
                    UserWindow userWindow =
                        new UserWindow();

                    userWindow.Show();
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

}
