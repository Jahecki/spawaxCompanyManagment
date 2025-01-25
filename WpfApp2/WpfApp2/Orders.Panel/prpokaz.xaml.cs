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
    /// Logika interakcji dla klasy prpokaz.xaml
    /// </summary>
    public partial class prpokaz : Window
    {
        private int _numerZamowienia;
        public prpokaz(int numerZamowienia)
        {
            InitializeComponent();
            _numerZamowienia = numerZamowienia;
            LoadEmployeeDetails();
        }

        private void LoadEmployeeDetails()
        {

          
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\KOMP\\Downloads\\WpfApp2\\WpfApp2\\WpfApp2\\Database1.mdf;Integrated Security=True"))
                    {
                        connection.Open();
                        string query = @"
                SELECT 
                    P.Imie + ' ' + P.Nazwisko AS Pracownik,
                    CASE WHEN PDZ.Kierownik = 'tak' THEN 'Tak' ELSE 'Nie' END AS Kierownik,
                    PDZ.Numer_zamowienia
                FROM 
                    Pracujacy PDZ
                JOIN 
                    Pracownicy P ON PDZ.PracownikID = P.PracownikID
                WHERE 
                    PDZ.Numer_zamowienia = @NumerZamowienia";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@NumerZamowienia", _numerZamowienia);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable employeeTable = new DataTable();
                        adapter.Fill(employeeTable);

                        // Wyświetl dane w DataGridzie
                        EmployeeDetailsDataGrid.ItemsSource = employeeTable.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas ładowania danych o pracownikach: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
    
