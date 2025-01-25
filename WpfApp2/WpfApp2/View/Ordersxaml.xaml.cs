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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.CLAS;
using WpfApp2.Orders.Panel;

namespace WpfApp2.View
{
    /// <summary>
    /// Logika interakcji dla klasy Ordersxaml.xaml
    /// </summary>
    public partial class Ordersxaml : UserControl
    {
        private DatabaseHelper _dbHelper;
        public Ordersxaml()
        {
            
        InitializeComponent();
         _dbHelper = new DatabaseHelper();
         LoadData();
        }
        public void LoadData()
        {
            string query = "SELECT Numer_zamowienia,KlientID FROM Zamowienia";
            var data = _dbHelper.GetData(query);
            Orders.ItemsSource = data.DefaultView;



        }
        private void Orders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Orders.SelectedItem is DataRowView selectedRow)
            {
                // Pobieramy dane z wybranego wiersza
                var numerZamowienia = selectedRow["Numer_zamowienia"];
                var klientId = selectedRow["KlientID"];

                // Wypełniamy pola tekstowe
               
            }
        }

        private void Usun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Orders.SelectedItem is DataRowView selectedRow)
                {
                    int numerZamowienia = Convert.ToInt32(selectedRow["Numer_zamowienia"]);

                    if (MessageBox.Show("Czy na pewno chcesz usunąć to zamówienie i wszystkie powiązane szczegóły?", "Potwierdzenie",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                        {
                            connection.Open();

                            using (SqlTransaction transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    // Usunięcie szczegółów zamówienia z Szczegoly_Zamowienia
                                    string deleteDetailsQuery = "DELETE FROM Szczegoly_Zamowienia WHERE Numer_zamowienia = @Numer_zamowienia";
                                    using (SqlCommand deleteDetailsCommand = new SqlCommand(deleteDetailsQuery, connection, transaction))
                                    {
                                        deleteDetailsCommand.Parameters.AddWithValue("@Numer_zamowienia", numerZamowienia);
                                        deleteDetailsCommand.ExecuteNonQuery();
                                    }

                                    // Usunięcie powiązanych materiałów z Materialy_dla_zamowienia
                                    string deleteMaterialsQuery = "DELETE FROM Materialy_dla_zamowienia WHERE Numer_zamowienia = @Numer_zamowienia";
                                    using (SqlCommand deleteMaterialsCommand = new SqlCommand(deleteMaterialsQuery, connection, transaction))
                                    {
                                        deleteMaterialsCommand.Parameters.AddWithValue("@Numer_zamowienia", numerZamowienia);
                                        deleteMaterialsCommand.ExecuteNonQuery();
                                    }

                                    // Usunięcie zamówienia z Zamowienia
                                    string deleteOrderQuery = "DELETE FROM Zamowienia WHERE Numer_zamowienia = @Numer_zamowienia";
                                    using (SqlCommand deleteOrderCommand = new SqlCommand(deleteOrderQuery, connection, transaction))
                                    {
                                        deleteOrderCommand.Parameters.AddWithValue("@Numer_zamowienia", numerZamowienia);
                                        deleteOrderCommand.ExecuteNonQuery();
                                    }

                                    // Zatwierdzenie transakcji
                                    transaction.Commit();
                                    MessageBox.Show("Zamówienie, powiązane szczegóły i materiały zostały usunięte.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                catch (Exception ex)
                                {
                                    // Wycofanie transakcji w razie błędu
                                    transaction.Rollback();
                                    throw new Exception("Wystąpił błąd podczas usuwania zamówienia: " + ex.Message);
                                }
                            }
                        }

                        // Odświeżenie danych w DataGrid
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void szczegol_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Sprawdź, czy wybrano zamówienie z DataGrid
                if (Orders.SelectedItem is DataRowView selectedRow)
                {
                    // Pobierz numer zamówienia
                    if (selectedRow["Numer_zamowienia"] != DBNull.Value)
                    {
                        int numerZamowienia = Convert.ToInt32(selectedRow["Numer_zamowienia"]);

                        // Otwórz nowe okno i przekaż numer zamówienia
                        Window1 szczegolyWindow = new Window1(numerZamowienia);
                        szczegolyWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Nieprawidłowy Numer_zamowienia w wybranym wierszu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano zamówienia z listy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas wyświetlania szczegółów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Materialy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Sprawdź, czy wybrano zamówienie z DataGrid
                if (Orders.SelectedItem is DataRowView selectedRow)
                {
                    // Pobierz numer zamówienia
                    if (selectedRow["Numer_zamowienia"] != DBNull.Value)
                    {
                        int numerZamowienia = Convert.ToInt32(selectedRow["Numer_zamowienia"]);

                        // Otwórz nowe okno i przekaż numer zamówienia
                        Materialy noweMaterialy = new Materialy(numerZamowienia);
                        noweMaterialy.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Nieprawidłowy Numer_zamowienia w wybranym wierszu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano zamówienia z listy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas wyświetlania szczegółów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void pracownik_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Sprawdź, czy wybrano zamówienie z DataGrid
                if (Orders.SelectedItem is DataRowView selectedRow)
                {
                    // Pobierz numer zamówienia
                    if (selectedRow["Numer_zamowienia"] != DBNull.Value)
                    {
                        int numerZamowienia = Convert.ToInt32(selectedRow["Numer_zamowienia"]);

                        // Otwórz nowe okno i przekaż numer zamówienia
                        Pracujacy nowiPracujacy = new Pracujacy(numerZamowienia);
                        nowiPracujacy.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Nieprawidłowy Numer_zamowienia w wybranym wierszu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano zamówienia z listy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas wyświetlania szczegółów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void pracownicy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Sprawdź, czy wybrano zamówienie z DataGrid
                if (Orders.SelectedItem is DataRowView selectedRow)
                {
                    // Pobierz numer zamówienia
                    if (selectedRow["Numer_zamowienia"] != DBNull.Value)
                    {
                        int numerZamowienia = Convert.ToInt32(selectedRow["Numer_zamowienia"]);

                        // Otwórz nowe okno i przekaż numer zamówienia
                        prpokaz nowiPracujacy = new prpokaz(numerZamowienia);
                        nowiPracujacy.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Nieprawidłowy Numer_zamowienia w wybranym wierszu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano zamówienia z listy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas wyświetlania szczegółów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    }


