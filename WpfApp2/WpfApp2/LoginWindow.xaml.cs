using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace WpfApp2
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "Nazwa użytkownika")
            {
                UsernameTextBox.Text = string.Empty; // Czyści placeholder
                UsernameTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black); // Zmienia kolor tekstu
            }
        }

        // Obsługuje zdarzenie, gdy użytkownik opuszcza pole tekstowe dla nazwy użytkownika
        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                UsernameTextBox.Text = "Nazwa użytkownika"; // Ustawia placeholder
                UsernameTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray); // Ustawia kolor placeholdera
            }
        }

        // Obsługuje zdarzenie, gdy użytkownik kliknie w pole tekstowe dla hasła
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == "Hasło")
            {
                PasswordBox.Password = string.Empty; // Czyści placeholder
            }
        }

        // Obsługuje zdarzenie, gdy użytkownik opuszcza pole tekstowe dla hasła
        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                PasswordBox.Password = "Hasło"; // Ustawia placeholder
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (AuthenticateUser(username, password))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close(); // Zamknij okno logowania
            }
            else
            {
                MessageBox.Show("Niepoprawna nazwa użytkownika lub hasło.", "Błąd logowania", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Uzytkownicy WHERE Nazwa_uzytkownika = @Username AND Haslo = @Password";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    int result = (int)command.ExecuteScalar();

                    return result > 0; // Jeśli wynik jest większy niż 0, użytkownik jest uwierzytelniony
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd logowania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
    

  
