using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.CLAS;

namespace WpfApp2.View
{
    /// <summary>
    /// Logika interakcji dla klasy Warehouse.xaml
    /// </summary>private DatabaseHelper _dbHelper;
      
    public partial class Warehouse : UserControl
    {
        private DatabaseHelper _dbHelper;
        public Warehouse()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            LoadData();
        }
        public void LoadData()
        {
            string query = "SELECT * FROM Materialy";
            var data = _dbHelper.GetData(query);
            Materialy.ItemsSource = data.DefaultView;



        }
        private void Materialy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Materialy.SelectedItem is DataRowView selectedRow)
            {
                // Pobierz dane z wybranego wiersza i wypełnij pola formularza
                Nazwa.Text = selectedRow["Nazwa_materialu"].ToString();
                Jednostka.Text = selectedRow["Jednostka_miary"].ToString();
                Ilosc.Text = selectedRow["Ilosc"].ToString();
                Cena.Text = selectedRow["Cena_za_jednostke"].ToString();
                Kolor.Text = selectedRow["Kolor"].ToString();
                Opis.Text = selectedRow["Opis"].ToString();
            }
        }
        private void Usun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Materialy.SelectedItem is DataRowView selectedRow)
                {
                    // Pobierz ID materiału do usunięcia
                    int materialID = Convert.ToInt32(selectedRow["Numer_materialu"]);

                    // Potwierdzenie usunięcia
                    if (MessageBox.Show("Czy na pewno chcesz usunąć wybrany materiał?", "Potwierdzenie",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        // Zapytanie SQL do usunięcia rekordu
                        string query = "DELETE FROM Materialy WHERE Numer_materialu = @MaterialID";

                        using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@MaterialID", materialID);
                                command.ExecuteNonQuery();
                            }
                        }

                        // Odśwież dane w DataGrid
                        LoadData();

                        MessageBox.Show("Materiał został pomyślnie usunięty!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano materiału do usunięcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas usuwania materiału: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Dodaj_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pobranie danych z pól formularza
                string nazwaMaterialu = Nazwa.Text.Trim();
                string jednostkaMiary = Jednostka.Text.Trim();
                string iloscText = Ilosc.Text.Trim();
                string cenaZaJednostkeText = Cena.Text.Trim();
                string kolor = Kolor.Text.Trim();
                string opis = Opis.Text.Trim();

                // Walidacja danych
                if (string.IsNullOrEmpty(nazwaMaterialu) || string.IsNullOrEmpty(jednostkaMiary) ||
                    string.IsNullOrEmpty(iloscText) || string.IsNullOrEmpty(cenaZaJednostkeText))
                {
                    MessageBox.Show("Wszystkie wymagane pola muszą zostać wypełnione.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(iloscText, out int ilosc) || ilosc < 0)
                {
                    MessageBox.Show("Ilość musi być liczbą całkowitą większą lub równą zero.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!decimal.TryParse(cenaZaJednostkeText, out decimal cenaZaJednostke) || cenaZaJednostke < 0)
                {
                    MessageBox.Show("Cena za jednostkę musi być liczbą większą lub równą zero.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Przygotowanie zapytania SQL
                string query = "INSERT INTO Materialy (Nazwa_materialu, Jednostka_miary, ilosc, cena_za_jednostke, Kolor, opis) " +
                               "VALUES (@Nazwa_materialu, @Jednostka_miary, @ilosc, @cena_za_jednostke, @Kolor, @opis)";

                // Wstawienie danych do bazy
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@Nazwa_materialu", SqlDbType.NVarChar).Value = nazwaMaterialu;
                        command.Parameters.Add("@Jednostka_miary", SqlDbType.NChar).Value = jednostkaMiary;
                        command.Parameters.Add("@ilosc", SqlDbType.Int).Value = ilosc;
                        command.Parameters.Add("@cena_za_jednostke", SqlDbType.Money).Value = cenaZaJednostke;
                        command.Parameters.Add("@Kolor", SqlDbType.NChar).Value = kolor ?? (object)DBNull.Value;
                        command.Parameters.Add("@opis", SqlDbType.NVarChar).Value = opis ?? (object)DBNull.Value;

                        command.ExecuteNonQuery();
                    }
                }

                // Odświeżenie DataGrid
                LoadData();

                // Wyczyszczenie pól formularza
                Nazwa.Clear();
                Jednostka.Clear();
                Ilosc.Clear();
                Cena.Clear();
                Kolor.Clear();
                Opis.Clear();

                MessageBox.Show("Materiał został pomyślnie dodany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
