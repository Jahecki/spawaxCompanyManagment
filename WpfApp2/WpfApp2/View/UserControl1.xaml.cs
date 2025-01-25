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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.CLAS;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace WpfApp2.View
{
  

    public partial class UserControl1 : UserControl
    {
        private DatabaseHelper _dbHelper;
        public UserControl1()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            LoadData();
        }


        public void LoadData()
        {
            string query = "SELECT * FROM Klienci";
            var data = _dbHelper.GetData(query);
            Klienci.ItemsSource = data.DefaultView;



        }

    
        private void Dodaj_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pobranie danych z pól formularza
                string imie = Imie.Text.Trim();
                string nazwisko = Nazwisko.Text.Trim();
                string telefon = Telefon.Text.Trim();
                string email = Email.Text.Trim();
                string adres = Adres.Text.Trim();
                DateTime? dataRejestracji = DataRejestracji.SelectedDate; // Obsługuje brak wyboru daty

                // Walidacja danych
                if (string.IsNullOrEmpty(imie) || string.IsNullOrEmpty(nazwisko) || string.IsNullOrEmpty(telefon) ||
                    string.IsNullOrEmpty(email) || string.IsNullOrEmpty(adres) || !dataRejestracji.HasValue)
                {
                    MessageBox.Show("Wszystkie pola muszą zostać wypełnione.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Konwersja danych
                imie = imie.Trim();
                nazwisko = nazwisko.Trim();
                telefon = telefon.Trim();
                email = email.Trim();
                adres = adres.Trim();

                // Sprawdzanie poprawności numeru telefonu (przykład walidacji)
                if (!Regex.IsMatch(telefon, @"^\+?[0-9]{9,15}$"))  // Akceptuje numery telefonów w formacie międzynarodowym
                {
                    MessageBox.Show("Numer telefonu jest nieprawidłowy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Sprawdzanie poprawności adresu email (przykład walidacji)
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Adres email jest nieprawidłowy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Data rejestracji
                DateTime dataRejestracjiFinal = dataRejestracji.Value;  // Używamy wartości, jeśli jest dostępna

                // Przygotowanie zapytania SQL
                string query = "INSERT INTO Klienci (Imie, Nazwisko, Telefon, Email, Data_rejestracji, Adres) " +
                               "VALUES (@Imie, @Nazwisko, @Telefon, @Email, @Data_rejestracji, @Adres)";

                // Wstawienie danych do bazy
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Zamiast AddWithValue, dodajemy parametry jawnie określając ich typ
                        command.Parameters.Add("@Imie", SqlDbType.NVarChar).Value = imie;
                        command.Parameters.Add("@Nazwisko", SqlDbType.NVarChar).Value = nazwisko;
                        command.Parameters.Add("@Telefon", SqlDbType.NVarChar).Value = telefon;
                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;
                        command.Parameters.Add("@Data_rejestracji", SqlDbType.DateTime).Value = dataRejestracjiFinal;
                        command.Parameters.Add("@Adres", SqlDbType.NVarChar).Value = adres;

                        command.ExecuteNonQuery();
                    }
                }

                // Odświeżenie DataGrid
                LoadData();

                // Wyczyszczenie pól formularza
                Imie.Clear();
                Nazwisko.Clear();
                Telefon.Clear();
                Email.Clear();
                Adres.Clear();
                DataRejestracji.SelectedDate = null;

                MessageBox.Show("Klient został pomyślnie dodany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Usun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Klienci.SelectedItem is DataRowView selectedRow)
                {
                    // Pobierz ID klienta z wybranego wiersza
                    if (selectedRow["KlientID"] != DBNull.Value)
                    {
                        int klientID = Convert.ToInt32(selectedRow["KlientID"]);

                        // Wyświetl potwierdzenie usunięcia
                        if (MessageBox.Show("Czy na pewno chcesz usunąć wybranego klienta?", "Potwierdzenie",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            // Zapytanie SQL do usunięcia rekordu
                            string query = "DELETE FROM Klienci WHERE KlientID = @KlientID";

                            using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                            {
                                connection.Open();
                                using (SqlCommand command = new SqlCommand(query, connection))
                                {
                                    command.Parameters.Add("@KlientID", SqlDbType.Int).Value = klientID;
                                    command.ExecuteNonQuery();
                                }
                            }

                            // Odśwież dane w DataGridzie
                            LoadData();

                            // Informacja o sukcesie
                            MessageBox.Show("Klient został pomyślnie usunięty!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nieprawidłowy KlientID w wybranym wierszu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano klienta do usunięcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas usuwania klienta: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private void ZlozZamowienie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Klienci.SelectedItem is DataRowView selectedRow)
                {
                    // Pobierz ID klienta
                    if (selectedRow["KlientID"] != DBNull.Value)
                    {
                        int klientID = Convert.ToInt32(selectedRow["KlientID"]);

                        // Przekaz ID klienta do nowego okna zamówienia
                        NewOrderWindow newOrderWindow = new NewOrderWindow(klientID);
                        newOrderWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Nieprawidłowy KlientID w wybranym wierszu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano klienta z listy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas składania zamówienia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Klienci_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Klienci.SelectedItem is DataRowView selectedRow)
                {
                    // Wypełnij pola formularza danymi wybranego wiersza
                    Imie.Text = selectedRow["Imie"]?.ToString() ?? string.Empty;
                    Nazwisko.Text = selectedRow["Nazwisko"]?.ToString() ?? string.Empty;
                    Telefon.Text = selectedRow["Telefon"]?.ToString() ?? string.Empty;
                    Email.Text = selectedRow["Email"]?.ToString() ?? string.Empty;
                    Adres.Text = selectedRow["Adres"]?.ToString() ?? string.Empty;

                    // Obsługa daty rejestracji (z uwzględnieniem możliwych błędów parsowania)
                    if (DateTime.TryParse(selectedRow["Data_rejestracji"]?.ToString(), out DateTime dataRejestracji))
                    {
                        DataRejestracji.SelectedDate = dataRejestracji;
                    }
                    else
                    {
                        DataRejestracji.SelectedDate = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas wczytywania danych z wiersza: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Wyczyść wszystkie pola formularza
                Imie.Clear();
                Nazwisko.Clear();
                Telefon.Clear();
                Email.Clear();
                Adres.Clear();
                DataRejestracji.SelectedDate = null;

                MessageBox.Show("Formularz został wyczyszczony.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas czyszczenia formularza: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

