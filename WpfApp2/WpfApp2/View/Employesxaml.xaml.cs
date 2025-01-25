using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.CLAS;

namespace WpfApp2.View
{
    /// <summary>
    /// Logika interakcji dla klasy Employesxaml.xaml
    /// </summary>
    public partial class Employesxaml : UserControl
    {
        private DatabaseHelper _dbHelper;
        public Employesxaml()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            LoadData();
        }
        public void LoadData()
        {
            string query = "SELECT * FROM Pracownicy";
            var data = _dbHelper.GetData(query);
            Pracownicy.ItemsSource = data.DefaultView;



        }
        private void Pracownicy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Pracownicy.SelectedItem is DataRowView selectedRow)
            {
                // Pobierz dane z wybranego wiersza i wypełnij pola formularza
                Imie.Text = selectedRow["Imie"].ToString();
                Naziwsko.Text = selectedRow["Nazwisko"].ToString();
            }
        }

        private void Usun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Pracownicy.SelectedItem is DataRowView selectedRow)
                {
                    // Pobierz ID pracownika do usunięcia
                    int pracownikID = Convert.ToInt32(selectedRow["PracownikID"]);

                    // Potwierdzenie usunięcia
                    if (MessageBox.Show("Czy na pewno chcesz usunąć wybranego pracownika?", "Potwierdzenie",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        // Zapytanie SQL do usunięcia rekordu
                        string query = "DELETE FROM Pracownicy WHERE PracownikID = @PracownikID";

                        using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@PracownikID", pracownikID);
                                command.ExecuteNonQuery();
                            }
                        }

                        // Odśwież dane w DataGrid
                        LoadData();

                        MessageBox.Show("Pracownik został pomyślnie usunięty!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano pracownika do usunięcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas usuwania pracownika: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Dodaj_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pobranie danych z pól formularza
                string imie = Imie.Text.Trim();
                string nazwisko = Naziwsko.Text.Trim();

                // Walidacja danych
                if (string.IsNullOrEmpty(imie) || string.IsNullOrEmpty(nazwisko))
                {
                    MessageBox.Show("Imię i Nazwisko są wymagane.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Przygotowanie zapytania SQL
                string query = "INSERT INTO Pracownicy (Imie, Nazwisko) VALUES (@Imie, @Nazwisko)";

                // Wstawienie danych do bazy
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@Imie", SqlDbType.NVarChar).Value = imie;
                        command.Parameters.Add("@Nazwisko", SqlDbType.NVarChar).Value = nazwisko;

                        command.ExecuteNonQuery();
                    }
                }

                // Odświeżenie DataGrid
                LoadData();

                // Wyczyszczenie pól formularza
                Imie.Clear();
                Naziwsko.Clear();

                MessageBox.Show("Pracownik został pomyślnie dodany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
