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

namespace WpfApp2.Orders.Panel
{
    /// <summary>
    /// Logika interakcji dla klasy Pracujacy.xaml
    /// </summary>
    public partial class Pracujacy : Window
    {
        private int _numerZamowienia;
        public Pracujacy(int numerZamowienia)
        {
            InitializeComponent();
            _numerZamowienia = numerZamowienia;
            LoadEmployees();
        }
        private void LoadEmployees()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    string query = "SELECT PracownikID, Imie + ' ' + Nazwisko AS PelneImie FROM Pracownicy";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable employeesTable = new DataTable();
                    adapter.Fill(employeesTable);

                    EmployeeComboBox.DisplayMemberPath = "PelneImie"; // Co wyświetlać
                    EmployeeComboBox.SelectedValuePath = "PracownikID"; // Wartość zwracana
                    EmployeeComboBox.ItemsSource = employeesTable.DefaultView;

                    if (employeesTable.Rows.Count == 0)
                    {
                        MessageBox.Show("Brak dostępnych pracowników do przypisania.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania pracowników: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmployeeComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Wybierz pracownika z listy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int numerPracownika = (int)EmployeeComboBox.SelectedValue;
                bool isManager = IsManagerCheckBox.IsChecked == true;

                AssignEmployeeToOrder(numerPracownika, isManager);

                MessageBox.Show("Pracownik został pomyślnie przypisany do zamówienia.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Zamknięcie okna
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AssignEmployeeToOrder(int numerPracownika, bool isManager)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                {
                    connection.Open();

                    // Dodanie pracownika do zamówienia
                    AddEmployeeToDatabase(connection, numerPracownika, isManager);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas przypisywania pracownika do zamówienia: {ex.Message}");
            }
        }
        private void AddEmployeeToDatabase(SqlConnection connection, int numerPracownika, bool isManager)
        {
            // Przypisz "tak" jeśli pracownik jest kierownikiem, w przeciwnym razie "nie"
            string kierownikValue = isManager ? "tak" : "nie";

            string query = "INSERT INTO Pracujacy (Numer_zamowienia, PracownikID, kierownik) " +
                           "VALUES (@NumerZamowienia, @NumerPracownika, @Kierownik)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NumerZamowienia", _numerZamowienia);
                command.Parameters.AddWithValue("@NumerPracownika", numerPracownika);
                command.Parameters.AddWithValue("@Kierownik", kierownikValue); // Wstawiamy 'tak' lub 'nie'
                command.ExecuteNonQuery();
            }


        }
    }
}