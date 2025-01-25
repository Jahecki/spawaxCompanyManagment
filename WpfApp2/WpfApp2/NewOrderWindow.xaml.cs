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
using WpfApp2.CLAS; 
using WpfApp2.View;

namespace WpfApp2
{
    /// <summary>
    /// Logika interakcji dla klasy NewOrderWindow.xaml
    /// </summary>
    public partial class NewOrderWindow : Window
    {
        private int _klientId;
        public NewOrderWindow(int KlientID)
        {
            InitializeComponent();

            _klientId = KlientID;
            klientID.Text = _klientId.ToString();

        }

        private void SubmitOrder(object sender, RoutedEventArgs e)
        {

            try
            {
                // Pobierz dane z formularza
                DateTime dataRealizacji = DataRealizacji.SelectedDate ?? DateTime.Now;
                string status = ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString();
                string nazwaProduktu = NazwaProduktu.Text.Trim();
                string kolor = Kolor.Text.Trim();

                // Walidacja
                if (string.IsNullOrEmpty(nazwaProduktu) || string.IsNullOrEmpty(kolor))
                {
                    MessageBox.Show("Proszę uzupełnić nazwę produktu i kolor.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Zapytanie SQL do dodania zamówienia
                string insertOrderQuery = "INSERT INTO Zamowienia (KlientID, Data_realizacji, status) " +
                                          "VALUES (@KlientID, @Data_realizacji, @status); SELECT SCOPE_IDENTITY();";  // Pobierz Numer_zamowienia po wstawieniu

                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();

                    // Wstawienie zamówienia i pobranie Numer_zamowienia
                    using (SqlCommand command = new SqlCommand(insertOrderQuery, connection))
                    {
                        command.Parameters.AddWithValue("@KlientID", _klientId);
                        command.Parameters.AddWithValue("@Data_realizacji", dataRealizacji);
                        command.Parameters.AddWithValue("@status", status);

                        // Pobierz Numer_zamowienia, który właśnie został wstawiony
                        int numerZamowienia = Convert.ToInt32(command.ExecuteScalar()); // Numer_zamowienia

                        // Zapytanie do dodania szczegółów zamówienia
                        string insertDetailsQuery = "INSERT INTO Szczegoly_zamowienia (Numer_zamowienia, Nazwa_produktu, Kolor) " +
                                                    "VALUES (@Numer_zamowienia, @NazwaProduktu, @Kolor)";

                        using (SqlCommand detailsCommand = new SqlCommand(insertDetailsQuery, connection))
                        {
                            detailsCommand.Parameters.AddWithValue("@Numer_zamowienia", numerZamowienia); // Numer zamówienia z poprzedniego zapytania
                            detailsCommand.Parameters.AddWithValue("@NazwaProduktu", nazwaProduktu);
                            detailsCommand.Parameters.AddWithValue("@Kolor", kolor);

                            // Wstawienie szczegółów zamówienia
                            detailsCommand.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Zamówienie zostało pomyślnie złożone!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Zamknij okno po złożeniu zamówienia
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    } 
}

