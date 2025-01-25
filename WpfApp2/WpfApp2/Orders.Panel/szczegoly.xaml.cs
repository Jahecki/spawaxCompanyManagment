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
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    /// 

    
    public partial class Window1 : Window
    {

        private int _numerZamowienia;
        public Window1(int numerZamowienia)
        {
            InitializeComponent();

            _numerZamowienia = numerZamowienia;

            // Załaduj szczegóły zamówienia
            LoadOrderDetails();

        }
        private void SzczegolyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Sprawdzanie, czy jest wybrany jakiś wiersz
                if (SzczegolyDataGrid.SelectedItem != null)
                {
                    // Pobierz wybrany element
                    var selectedRow = (DataRowView)SzczegolyDataGrid.SelectedItem;

                    // Przykład: Możesz uzyskać dane z wybranego wiersza
                    int numerZamowienia = Convert.ToInt32(selectedRow["Numer_zamowienia"]);
                    string nazwaProduktu = selectedRow["Nazwa_produktu"].ToString();
                    string kolor = selectedRow["Kolor"].ToString();
                    int iloscMaterialow = Convert.ToInt32(selectedRow["Ilosc_materialow"]);
                    decimal cenaZaMaterialy = Convert.ToDecimal(selectedRow["Cena_za_materialy"]);
                    decimal cenaKoncowa = Convert.ToDecimal(selectedRow["Cena_koncowa"]);

                    // Tutaj możesz wykorzystać te dane, np. wyświetlić je w innych kontrolkach,
                    // przypisać do innych zmiennych lub wykonać inne operacje.

                    MessageBox.Show($"Wybrano zamówienie nr {numerZamowienia}: {nazwaProduktu}, Kolor: {kolor}, Ilość: {iloscMaterialow}, Cena: {cenaKoncowa} zł.");
                }
                else
                {
                    // Jeśli nie wybrano żadnego wiersza, możesz obsłużyć ten przypadek
                    MessageBox.Show("Nie wybrano żadnego wiersza.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas zmiany wyboru: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LoadOrderDetails()
        {
            try
            {
                // Połączenie z bazą danych
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();

                    // Zapytanie SQL do pobrania szczegółów zamówienia
                    string query = "SELECT Numer_zamowienia, Nazwa_produktu, Kolor, Ilosc_materialow,Cena_za_materialy, Cena_koncowa FROM Szczegoly_zamowienia WHERE Numer_zamowienia = @NumerZamowienia";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NumerZamowienia", _numerZamowienia);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable detailsTable = new DataTable();
                        adapter.Fill(detailsTable);

                        // Przypisanie danych do DataGrid
                        SzczegolyDataGrid.ItemsSource = detailsTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas ładowania szczegółów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }
    }
}

