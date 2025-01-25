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
using System.Windows.Shapes;
using WpfApp2.CLAS;

namespace WpfApp2.Orders.Panel
{
    /// <summary>
    /// Logika interakcji dla klasy Materialy.xaml
    /// </summary>
    public partial class Materialy : Window
    {

        private int _numerZamowienia;
        public Materialy(int numerZamowienia)
        {
            InitializeComponent();
            _numerZamowienia = numerZamowienia;
            LoadMaterials();
        }


        private void LoadMaterials()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    string query = "SELECT Numer_materialu, Nazwa_materialu FROM Materialy";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable materialsTable = new DataTable();
                    adapter.Fill(materialsTable);

                    MaterialComboBox.DisplayMemberPath = "Nazwa_materialu";
                    MaterialComboBox.SelectedValuePath = "Numer_materialu";
                    MaterialComboBox.ItemsSource = materialsTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania materiałów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Obsługa kliknięcia przycisku "Dodaj materiał"
        private void AddMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pobierz dane z formularza
                int numerMaterialu = (int)MaterialComboBox.SelectedValue;
                int ilosc = int.Parse(QuantityTextBox.Text);

                // Sprawdzanie dostępności materiału w magazynie
                int availableQuantity = GetMaterialStock(numerMaterialu);
                if (ilosc > availableQuantity)
                {
                    MessageBox.Show("Nie ma wystarczającej ilości materiału w magazynie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Dodaj materiał do zamówienia
                AddMaterialToOrder(numerMaterialu, ilosc);
                UpdateTotalMaterialQuantity();

                // Uruchom procedury
                ExecuteProcedure("ObliczCeneZaMaterialy");
                ExecuteProcedure("ObliczCeneOstatecznaZamoweinia");

                // Zaktualizuj stan magazynu
                UpdateMaterialStock(numerMaterialu, ilosc);

                MessageBox.Show("Materiał został pomyślnie dodany do zamówienia.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Zamknij okno
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Funkcja sprawdzająca dostępność materiału w magazynie
        private int GetMaterialStock(int numerMaterialu)
        {
            int stock = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    string query = "SELECT ilosc FROM Materialy WHERE Numer_materialu = @NumerMaterialu";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NumerMaterialu", numerMaterialu);
                    stock = (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas pobierania stanu magazynowego: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return stock;
        }

        // Funkcja dodająca materiał do zamówienia
        private void AddMaterialToOrder(int numerMaterialu, int ilosc)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    string query = "INSERT INTO Materialy_dla_zamowienia (Numer_zamowienia, Numer_materialu, Ilosc_materialu) " +
                                   "VALUES (@NumerZamowienia, @NumerMaterialu, @IloscMaterialu)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NumerZamowienia", _numerZamowienia);
                    command.Parameters.AddWithValue("@NumerMaterialu", numerMaterialu);
                    command.Parameters.AddWithValue("@IloscMaterialu", ilosc);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania materiału do zamówienia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Funkcja uruchamiająca procedury
        private void ExecuteProcedure(string procedureName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(procedureName, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas uruchamiania procedury: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Funkcja aktualizująca stan magazynowy
        private void UpdateMaterialStock(int numerMaterialu, int ilosc)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    string query = "UPDATE Materialy SET ilosc = ilosc - @Ilosc WHERE Numer_materialu = @NumerMaterialu";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NumerMaterialu", numerMaterialu);
                    command.Parameters.AddWithValue("@Ilosc", ilosc);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas aktualizacji stanu magazynowego: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTotalMaterialQuantity()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    // Obliczamy łączną ilość materiałów dla zamówienia
                    string query = @"
                UPDATE Szczegoly_Zamowienia
                SET Ilosc_materialow = (
                    SELECT SUM(Ilosc_materialu)
                    FROM Materialy_dla_Zamowienia
                    WHERE Numer_zamowienia = @NumerZamowienia
                )
                WHERE Numer_zamowienia = @NumerZamowienia";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NumerZamowienia", _numerZamowienia);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas aktualizacji łącznej ilości materiałów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}

