using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
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
using SzereloWPF.Models;
using System.Data;
using Microsoft.Win32;
using ClosedXML.Excel;

namespace SzereloWPF.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        const string _connectionString = "server=localhost;port=3306;uid=root;pwd=;database=szerelo";
        MySqlConnection _conn;
        List<Ugyfel> _ugyfelek;
        List<Dolgozo> _dolgozok;
        List<Idopont> _idopontok;
        List<Jarmu> _jarmuvek;
        List<Javitas> _javitasok;

        private bool _isAddingNewUgyfel = false;
        private bool _isAddingNewJarmu = false;
        private bool _isAddingNewIdopont = false;
        private bool _isAddingNewDolgozo = false;
        private bool _isAddingNewJavitas = false;


        public Home()
        {
            InitializeComponent();
            InitializeDatabase();
            if (_conn != null && _conn.State == ConnectionState.Open)
            {
                LoadUgyfelek();
                LoadDolgozok();
                LoadIdopontok();
                LoadAllJarmuvek();

                ComboBoxJarmuUgyfel.ItemsSource = _ugyfelek;
                ComboBoxIdopontUgyfel.ItemsSource = _ugyfelek;
                ComboBoxJavitasJarmu.ItemsSource = _jarmuvek;
                ComboBoxJavitasDolgozo.ItemsSource = _dolgozok;
            }
            else
            {
                DisableUI();
            }

            HideAllViews();
            CustomersView.Visibility = Visibility.Visible;
        }

        private void DisableUI()
        {
            ButtonNavigateCustomers.IsEnabled = false;
            ButtonNavigateVehicles.IsEnabled = false;
            ButtonNavigateAppointments.IsEnabled = false;
            ButtonNavigateEmployees.IsEnabled = false;
            ButtonNavigateRepairs.IsEnabled = false;

            SetUgyfelDetailPanelState(false);
            SetJarmuDetailPanelState(false);
            SetIdopontDetailPanelState(false);
            SetDolgozoDetailPanelState(false);
            SetJavitasDetailPanelState(false);
        }


        private void InitializeDatabase()
        {
            try
            {
                _conn = new MySqlConnection(_connectionString);
                _conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Adatbázis kapcsolódási hiba: {ex.Message}\nEllenőrizze a kapcsolati sztringet és a MySQL szerver állapotát.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                _conn = null;
            }
        }

        private void LoadUgyfelek()
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat az ügyfelek betöltéséhez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Ugyfel_id, Nev, Email_cim, Telefonszam, Lakcim FROM ugyfelek ORDER BY Nev;";
                _ugyfelek = new List<Ugyfel>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _ugyfelek.Add(
                            new Ugyfel
                            {
                                UgyfelId = reader.GetInt32(0),
                                Nev = reader.GetString(1),
                                EmailCim = reader.GetString(2),
                                Telefonszam = reader.GetString(3),
                                Lakcim = reader.GetString(4)
                            });
                    }
                }

                UgyfelListView.ItemsSource = _ugyfelek;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ügyfelek betöltési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDolgozok()
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a dolgozók betöltéséhez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Dolgozo_id, Nev, Beosztas, Lakcim, Telefonszam, Email_cim, Szemelyazonosito_igazolvany_szam FROM dolgozo ORDER BY Nev;";
                _dolgozok = new List<Dolgozo>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _dolgozok.Add(
                            new Dolgozo
                            {
                                DolgozoId = reader.GetInt32(0),
                                Nev = reader.GetString(1),
                                Beosztas = reader.GetString(2),
                                Lakcim = reader.GetString(3),
                                Telefonszam = reader.GetString(4),
                                EmailCim = reader.GetString(5),
                                SzemelyazonositoIgazolvanySzam = reader.GetString(6)
                            });
                    }
                }

                DolgozoListView.ItemsSource = _dolgozok;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dolgozók betöltési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadIdopontok()
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat az időpontok betöltéséhez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Idopont_id, Datum, Nap, Statusz FROM idopont ORDER BY Datum;";
                _idopontok = new List<Idopont>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _idopontok.Add(
                            new Idopont
                            {
                                IdopontId = reader.GetInt32(0),
                                Datum = reader.GetDateTime(1),
                                Nap = reader.GetString(2),
                                Statusz = reader.GetString(3)
                            });
                    }
                }

                IdopontokGrid.ItemsSource = _idopontok;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Időpontok betöltési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAllJarmuvek()
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a járművek betöltéséhez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Jarmu_id, Rendszam, Alvazszam, Marka, Modell, Gyartasi_ev, Km_ora_allas, Ugyfel_id FROM jarmuvek ORDER BY Rendszam;";
                _jarmuvek = new List<Jarmu>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _jarmuvek.Add(
                            new Jarmu
                            {
                                JarmuId = reader.GetInt32(0),
                                Rendszam = reader.GetString(1),
                                Alvazszam = reader.GetString(2),
                                Marka = reader.GetString(3),
                                Modell = reader.GetString(4),
                                GyartasiEv = reader.GetInt32(5),
                                KmOraAllas = reader.GetInt32(6),
                                UgyfelId = reader.GetInt32(7)
                            });
                    }
                }

                JarmuListView.ItemsSource = _jarmuvek;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Összes jármű betöltési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LoadJavitasok()
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a javítások betöltéséhez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Javitas_id, Megnevezes, Leiras, Koltseg, Datum FROM javitasok ORDER BY Datum DESC;";
                _javitasok = new List<Javitas>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _javitasok.Add(
                            new Javitas
                            {
                                JavitasId = reader.GetInt32(0),
                                Megnevezes = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Leiras = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Koltseg = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                Datum = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4)
                    });
                    }
                }
                JavitasokGrid.ItemsSource = _javitasok;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Javítások betöltési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetJavitasDetailPanelState(bool enable)
        {
            TextBoxJavitasMegnevezes.IsReadOnly = !enable;
            TextBoxJavitasLeiras.IsReadOnly = !enable;
            TextBoxJavitasKoltseg.IsReadOnly = !enable;
            DatePickerJavitasDatum.IsEnabled = enable;
            ComboBoxJavitasJarmu.IsEnabled = enable;
            ComboBoxJavitasDolgozo.IsEnabled = enable;

            ButtonMentJavitas.IsEnabled = enable;
            ButtonTorolJavitas.IsEnabled = enable && !_isAddingNewJavitas;
            ButtonUjJavitas.IsEnabled = !enable;
            JavitasDetailPanel.IsEnabled = enable;
        }

        private void JavitasokGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var javitas = ((DataGrid)sender).SelectedItem as Javitas;
            if (javitas != null)
            {
                _isAddingNewJavitas = false;
                SetJavitasDetailPanelState(true);

                TextBoxJavitasId.Text = javitas.JavitasId.ToString();
                TextBoxJavitasMegnevezes.Text = javitas.Megnevezes;
                TextBoxJavitasLeiras.Text = javitas.Leiras;
                TextBoxJavitasKoltseg.Text = javitas.Koltseg.ToString();
                DatePickerJavitasDatum.SelectedDate = javitas.Datum;

                LoadJavitasKapcsolatok(javitas.JavitasId);
            }
            else
            {
                _isAddingNewJavitas = false;
                SetJavitasDetailPanelState(false);

                TextBoxJavitasId.Text = "";
                TextBoxJavitasMegnevezes.Text = "";
                TextBoxJavitasLeiras.Text = "";
                TextBoxJavitasKoltseg.Text = "";
                DatePickerJavitasDatum.SelectedDate = null;
                ComboBoxJavitasJarmu.SelectedItem = null;
                ComboBoxJavitasDolgozo.SelectedItem = null;
            }
        }

        private void LoadJavitasKapcsolatok(int javitasId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open) return;

            int? jarmuId = null;
            int? dolgozoId = null;

            try
            {
                var cmdVegez = _conn.CreateCommand();
                cmdVegez.CommandText = "SELECT Jarmu_id FROM vegez WHERE Javitas_id = @JavitasId LIMIT 1;";
                cmdVegez.Parameters.AddWithValue("@JavitasId", javitasId);
                var jarmuIdObj = cmdVegez.ExecuteScalar();
                if (jarmuIdObj != null && jarmuIdObj != DBNull.Value)
                {
                    jarmuId = Convert.ToInt32(jarmuIdObj);
                }

                var cmdElvegzi = _conn.CreateCommand();
                cmdElvegzi.CommandText = "SELECT Dolgozo_id FROM elvegzi WHERE Javitas_id = @JavitasId LIMIT 1;";
                cmdElvegzi.Parameters.AddWithValue("@JavitasId", javitasId);
                var dolgozoIdObj = cmdElvegzi.ExecuteScalar();
                if (dolgozoIdObj != null && dolgozoIdObj != DBNull.Value)
                {
                    dolgozoId = Convert.ToInt32(dolgozoIdObj);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a javítás kapcsolatainak betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ComboBoxJavitasJarmu.SelectedValue = jarmuId;
            ComboBoxJavitasDolgozo.SelectedValue = dolgozoId;

        }

        private void ButtonUjJavitas_Click(object sender, RoutedEventArgs e)
        {
            _isAddingNewJavitas = true;
            JavitasokGrid.SelectedItem = null;

            TextBoxJavitasId.Text = "";
            TextBoxJavitasMegnevezes.Text = "";
            TextBoxJavitasLeiras.Text = "";
            TextBoxJavitasKoltseg.Text = "";
            DatePickerJavitasDatum.SelectedDate = DateTime.Now;
            ComboBoxJavitasJarmu.SelectedItem = null;
            ComboBoxJavitasDolgozo.SelectedItem = null;

            SetJavitasDetailPanelState(true);
            ButtonTorolJavitas.IsEnabled = false;
            ButtonUjJavitas.IsEnabled = false;

            MessageBox.Show("Új javítás adatok megadásához kérem töltse ki a mezőket.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
            TextBoxJavitasMegnevezes.Focus();
        }

        private void ButtonMentJavitas_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a mentéshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TextBoxJavitasMegnevezes.Text))
            {
                MessageBox.Show("A javítás megnevezése kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!DatePickerJavitasDatum.SelectedDate.HasValue)
            {
                MessageBox.Show("A javítás dátuma kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(TextBoxJavitasKoltseg.Text, out int koltseg) || koltseg < 0)
            {
                MessageBox.Show("Érvénytelen költség!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (ComboBoxJavitasJarmu.SelectedValue == null)
            {
                MessageBox.Show("Ki kell választani egy járművet!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? selectedJarmuId = (int?)ComboBoxJavitasJarmu.SelectedValue;
            int? selectedDolgozoId = (int?)ComboBoxJavitasDolgozo.SelectedValue;

            try
            {
                using (var transaction = _conn.BeginTransaction())
                {
                    var cmd = _conn.CreateCommand();
                    cmd.Transaction = transaction;

                    int javitasId;

                    if (_isAddingNewJavitas)
                    {
                        cmd.CommandText = @"
                     INSERT INTO javitasok (Megnevezes, Leiras, Koltseg, Datum)
                     VALUES (@Megnevezes, @Leiras, @Koltseg, @Datum);
                     SELECT LAST_INSERT_ID();";
                        cmd.Parameters.AddWithValue("@Megnevezes", TextBoxJavitasMegnevezes.Text);
                        cmd.Parameters.AddWithValue("@Leiras", TextBoxJavitasLeiras.Text);
                        cmd.Parameters.AddWithValue("@Koltseg", koltseg);
                        cmd.Parameters.AddWithValue("@Datum", DatePickerJavitasDatum.SelectedDate.Value);

                        javitasId = Convert.ToInt32(cmd.ExecuteScalar());

                        cmd.Parameters.Clear();
                        cmd.CommandText = "INSERT INTO vegez (Jarmu_id, Javitas_id) VALUES (@JarmuId, @JavitasId);";
                        cmd.Parameters.AddWithValue("@JarmuId", selectedJarmuId.Value);
                        cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                        cmd.ExecuteNonQuery();

                        if (selectedDolgozoId.HasValue)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "INSERT INTO elvegzi (Dolgozo_id, Javitas_id) VALUES (@DolgozoId, @JavitasId);";
                            cmd.Parameters.AddWithValue("@DolgozoId", selectedDolgozoId.Value);
                            cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show($"Új javítás sikeresen hozzáadva! ID: {javitasId}", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        if (!int.TryParse(TextBoxJavitasId.Text, out javitasId) || javitasId <= 0)
                        {
                            MessageBox.Show("Érvénytelen javítás azonosító a módosításhoz.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                            transaction.Rollback();
                            return;
                        }

                        cmd.CommandText = @"UPDATE javitasok SET Megnevezes=@Megnevezes, Leiras=@Leiras, Koltseg=@Koltseg, Datum=@Datum WHERE Javitas_id=@JavitasId;";
                        cmd.Parameters.AddWithValue("@Megnevezes", TextBoxJavitasMegnevezes.Text);
                        cmd.Parameters.AddWithValue("@Leiras", TextBoxJavitasLeiras.Text);
                        cmd.Parameters.AddWithValue("@Koltseg", koltseg);
                        cmd.Parameters.AddWithValue("@Datum", DatePickerJavitasDatum.SelectedDate.Value);
                        cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        cmd.Parameters.Clear();
                        cmd.CommandText = "DELETE FROM vegez WHERE Javitas_id=@JavitasId;";
                        cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                        cmd.ExecuteNonQuery();

                        cmd.Parameters.Clear();
                        cmd.CommandText = "INSERT INTO vegez (Jarmu_id, Javitas_id) VALUES (@JarmuId, @JavitasId);";
                        cmd.Parameters.AddWithValue("@JarmuId", selectedJarmuId.Value);
                        cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                        cmd.ExecuteNonQuery();

                        cmd.Parameters.Clear();
                        cmd.CommandText = "DELETE FROM elvegzi WHERE Javitas_id=@JavitasId;";
                        cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                        cmd.ExecuteNonQuery();

                        if (selectedDolgozoId.HasValue)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "INSERT INTO elvegzi (Dolgozo_id, Javitas_id) VALUES (@DolgozoId, @JavitasId);";
                            cmd.Parameters.AddWithValue("@DolgozoId", selectedDolgozoId.Value);
                            cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                            cmd.ExecuteNonQuery();
                        }


                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Javítás adatok sikeresen módosítva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Nem történt módosítás.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }

                    transaction.Commit();

                    LoadJavitasok();
                    _isAddingNewJavitas = false;
                    SetJavitasDetailPanelState(false);
                    JavitasokGrid.SelectedItem = null;
                }
            }
            catch (MySqlException mySqlEx)
            {
                MessageBox.Show($"Adatbázis hiba a mentés során: {mySqlEx.Message}\nError Number: {mySqlEx.Number}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a mentés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonTorolJavitas_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a törléshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TextBoxJavitasId.Text, out int javitasId) || javitasId <= 0)
            {
                MessageBox.Show("Nincs kiválasztott javítás a törléshez.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Biztosan törölni szeretné a javítást (ID: {javitasId})?\nFIGYELEM: Ez törli a kapcsolódó 'vegez' és 'elvegzi' bejegyzéseket is!", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var transaction = _conn.BeginTransaction())
                    {
                        var cmd = _conn.CreateCommand();
                        cmd.Transaction = transaction;

                        cmd.CommandText = "DELETE FROM vegez WHERE Javitas_id=@JavitasId;";
                        cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "DELETE FROM elvegzi WHERE Javitas_id=@JavitasId;";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "DELETE FROM javitasok WHERE Javitas_id=@JavitasId;";
                        int rowsAffected = cmd.ExecuteNonQuery();

                        transaction.Commit();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Javítás sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadJavitasok();
                            JavitasokGrid.SelectedItem = null;
                        }
                        else
                        {
                            MessageBox.Show("Nem történt törlés. Lehet, hogy a javítás már nem létezik?", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (MySqlException mySqlEx)
                {
                    MessageBox.Show($"Adatbázis hiba a törlés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonExportExcel_Repairs_Click(object sender, RoutedEventArgs e)
        {
            var lista = _javitasok;
            if (lista == null || !lista.Any()) { MessageBox.Show("Nincs exportálható adat.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            var dlg = new SaveFileDialog { Filter = "Excel fájl (*.xlsx)|*.xlsx", FileName = "Javitasok.xlsx" };
            if (dlg.ShowDialog() != true) return;

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Javítások");

                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Megnevezés";
                ws.Cell(1, 3).Value = "Leírás";
                ws.Cell(1, 4).Value = "Költség";
                ws.Cell(1, 5).Value = "Dátum";

                int row = 2;
                foreach (var j in lista)
                {
                    ws.Cell(row, 1).Value = j.JavitasId;
                    ws.Cell(row, 2).Value = j.Megnevezes;
                    ws.Cell(row, 3).Value = j.Leiras;
                    ws.Cell(row, 4).Value = j.Koltseg;
                    ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0 Ft";
                    ws.Cell(row, 5).Value = j.Datum;
                    ws.Cell(row, 5).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";

                    row++;
                }
                ws.Columns().AdjustToContents();
                try
                {
                    wb.SaveAs(dlg.FileName);
                    MessageBox.Show("Exportálás sikeres!", "Kész", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba az exportálás során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void HideAllViews()
        {
            CustomersView.Visibility = Visibility.Collapsed;
            VehiclesView.Visibility = Visibility.Collapsed;
            AppointmentsView.Visibility = Visibility.Collapsed;
            EmployeesView.Visibility = Visibility.Collapsed;
            RepairsView.Visibility = Visibility.Collapsed;
        }

        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            _isAddingNewUgyfel = false;
            _isAddingNewJarmu = false;
            _isAddingNewIdopont = false;
            _isAddingNewDolgozo = false;


            HideAllViews();

            switch (button.Name)
            {
                case "ButtonNavigateCustomers":
                    CustomersView.Visibility = Visibility.Visible;
                    if (_ugyfelek == null || _ugyfelek.Count == 0) LoadUgyfelek();
                    else UgyfelListView.ItemsSource = _ugyfelek;
                    UgyfelListView.SelectedItem = null;
                    SetUgyfelDetailPanelState(false);
                    break;
                case "ButtonNavigateVehicles":
                    VehiclesView.Visibility = Visibility.Visible;
                    if (_jarmuvek == null || _jarmuvek.Count == 0) LoadAllJarmuvek();
                    else JarmuListView.ItemsSource = _jarmuvek;
                    JarmuListView.SelectedItem = null;
                    SetJarmuDetailPanelState(false);

                    if (ComboBoxJarmuUgyfel.ItemsSource == null && (_ugyfelek != null && _ugyfelek.Count > 0))
                    {
                        ComboBoxJarmuUgyfel.ItemsSource = _ugyfelek;
                    }

                    break;
                case "ButtonNavigateAppointments":
                    AppointmentsView.Visibility = Visibility.Visible;
                    if (_idopontok == null || _idopontok.Count == 0) LoadIdopontok();
                    else IdopontokGrid.ItemsSource = _idopontok;
                    IdopontokGrid.SelectedItem = null;
                    SetIdopontDetailPanelState(false);

                    if (ComboBoxIdopontUgyfel.ItemsSource == null && (_ugyfelek != null && _ugyfelek.Count > 0))
                    {
                        ComboBoxIdopontUgyfel.ItemsSource = _ugyfelek;
                    }
                    break;
                case "ButtonNavigateEmployees":
                    EmployeesView.Visibility = Visibility.Visible;
                    if (_dolgozok == null || _dolgozok.Count == 0) LoadDolgozok();
                    else DolgozoListView.ItemsSource = _dolgozok;
                    DolgozoListView.SelectedItem = null;
                    SetDolgozoDetailPanelState(false);
                    break;
                case "ButtonNavigateRepairs":
                    RepairsView.Visibility = Visibility.Visible;
                    if (_javitasok == null || _javitasok.Count == 0) LoadJavitasok();
                    else JavitasokGrid.ItemsSource = _javitasok;
                    JavitasokGrid.SelectedItem = null;
                    SetJavitasDetailPanelState(false);
                    break;
            }
        }

        private void SetUgyfelDetailPanelState(bool enable)
        {
            TextBoxUgyfelNev.IsReadOnly = !enable;
            TextBoxUgyfelEmail.IsReadOnly = !enable;
            TextBoxUgyfelTelefon.IsReadOnly = !enable;
            TextBoxUgyfelLakcim.IsReadOnly = !enable;

            ButtonMentUgyfel.IsEnabled = enable;
            ButtonTorolUgyfel.IsEnabled = enable && !_isAddingNewUgyfel;
            ButtonUjUgyfel.IsEnabled = !enable;
        }

        private void UgyfelListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ugyfel = ((ListView)sender).SelectedItem as Ugyfel;
            if (ugyfel != null)
            {
                _isAddingNewUgyfel = false;

                SetUgyfelDetailPanelState(true);

                TextBoxUgyfelId.Text = ugyfel.UgyfelId.ToString();
                TextBoxUgyfelNev.Text = ugyfel.Nev;
                TextBoxUgyfelEmail.Text = ugyfel.EmailCim;
                TextBoxUgyfelTelefon.Text = ugyfel.Telefonszam;
                TextBoxUgyfelLakcim.Text = ugyfel.Lakcim;

                LoadJarmuvekForUgyfel(ugyfel.UgyfelId);

            }
            else
            {
                _isAddingNewUgyfel = false;

                SetUgyfelDetailPanelState(false);

                TextBoxUgyfelId.Text = "";
                TextBoxUgyfelNev.Text = "";
                TextBoxUgyfelEmail.Text = "";
                TextBoxUgyfelTelefon.Text = "";
                TextBoxUgyfelLakcim.Text = "";
                JarmuvekGrid.ItemsSource = null;
                LabelJarmuvekSzama.Content = "";
            }
        }

        private void LoadJarmuvekForUgyfel(int ugyfelId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a járművek betöltéséhez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                JarmuvekGrid.ItemsSource = null;
                LabelJarmuvekSzama.Content = "Hiba";
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Jarmu_id, Rendszam, Alvazszam, Marka, Modell, Gyartasi_ev, Km_ora_allas, Ugyfel_id FROM jarmuvek WHERE Ugyfel_id=@UgyfelId;";
                cmd.Parameters.AddWithValue("@UgyfelId", ugyfelId);

                List<Jarmu> jarmuvek = new List<Jarmu>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        jarmuvek.Add(
                            new Jarmu
                            {
                                JarmuId = reader.GetInt32(0),
                                Rendszam = reader.GetString(1),
                                Alvazszam = reader.GetString(2),
                                Marka = reader.GetString(3),
                                Modell = reader.GetString(4),
                                GyartasiEv = reader.GetInt32(5),
                                KmOraAllas = reader.GetInt32(6),
                                UgyfelId = reader.GetInt32(7)
                            });
                    }
                }

                JarmuvekGrid.ItemsSource = jarmuvek;
                LabelJarmuvekSzama.Content = jarmuvek.Count.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Járművek betöltési hiba az ügyfélhez: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                JarmuvekGrid.ItemsSource = null;
                LabelJarmuvekSzama.Content = "Hiba";
            }
        }

        private void ButtonUjUgyfel_Click(object sender, RoutedEventArgs e)
        {
            _isAddingNewUgyfel = true;
            UgyfelListView.SelectedItem = null;

            TextBoxUgyfelId.Text = "";
            TextBoxUgyfelNev.Text = "";
            TextBoxUgyfelEmail.Text = "";
            TextBoxUgyfelTelefon.Text = "";
            TextBoxUgyfelLakcim.Text = "";
            JarmuvekGrid.ItemsSource = null;
            LabelJarmuvekSzama.Content = "";

            SetUgyfelDetailPanelState(true);
            ButtonTorolUgyfel.IsEnabled = false;
            ButtonUjUgyfel.IsEnabled = false;

            MessageBox.Show("Új ügyfél adatok megadásához kérem töltse ki a mezőket.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonMentUgyfel_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a mentéshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TextBoxUgyfelNev.Text))
            {
                MessageBox.Show("Ügyfél neve kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();

                if (_isAddingNewUgyfel)
                {
                    cmd.CommandText = @"INSERT INTO ugyfelek (Nev, Email_cim, Telefonszam, Lakcim) VALUES (@Nev, @EmailCim, @Telefonszam, @Lakcim); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.AddWithValue("@Nev", TextBoxUgyfelNev.Text);
                    cmd.Parameters.AddWithValue("@EmailCim", TextBoxUgyfelEmail.Text);
                    cmd.Parameters.AddWithValue("@Telefonszam", TextBoxUgyfelTelefon.Text);
                    cmd.Parameters.AddWithValue("@Lakcim", TextBoxUgyfelLakcim.Text);

                    int newUgyfelId = Convert.ToInt32(cmd.ExecuteScalar());

                    MessageBox.Show($"Új ügyfél sikeresen hozzáadva! ID: {newUgyfelId}", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    if (!int.TryParse(TextBoxUgyfelId.Text, out int ugyfelId) || ugyfelId <= 0)
                    {
                        MessageBox.Show("Érvénytelen ügyfél azonosító a módosításhoz.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    cmd.CommandText = @"UPDATE ugyfelek SET Nev=@Nev, Email_cim=@EmailCim, Telefonszam=@Telefonszam, Lakcim=@Lakcim WHERE Ugyfel_id=@UgyfelId;";

                    cmd.Parameters.AddWithValue("@Nev", TextBoxUgyfelNev.Text);
                    cmd.Parameters.AddWithValue("@EmailCim", TextBoxUgyfelEmail.Text);
                    cmd.Parameters.AddWithValue("@Telefonszam", TextBoxUgyfelTelefon.Text);
                    cmd.Parameters.AddWithValue("@Lakcim", TextBoxUgyfelLakcim.Text);
                    cmd.Parameters.AddWithValue("@UgyfelId", ugyfelId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Ügyfél adatok sikeresen módosítva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nem történt módosítás.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                LoadUgyfelek();
                _isAddingNewUgyfel = false;
                SetUgyfelDetailPanelState(false);
                UgyfelListView.SelectedItem = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a mentés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonTorolUgyfel_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a törléshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TextBoxUgyfelId.Text, out int ugyfelId) || ugyfelId <= 0)
            {
                MessageBox.Show("Nincs kiválasztott ügyfél a törléshez.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Biztosan törölni szeretné az ügyfelet (ID: {ugyfelId})?\nFIGYELEM: Ez törölheti a kapcsolódó járműveket, javításokat és időpontokat is a CASCADE törlési szabályoktól függően!", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var cmd = _conn.CreateCommand();
                    cmd.CommandText = "DELETE FROM ugyfelek WHERE Ugyfel_id=@UgyfelId;";
                    cmd.Parameters.AddWithValue("@UgyfelId", ugyfelId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Ügyfél sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadUgyfelek();

                        UgyfelListView.SelectedItem = null;
                    }
                    else
                    {
                        MessageBox.Show("Nem történt törlés. Lehet, hogy az ügyfél már nem létezik?", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (MySqlException mySqlEx)
                {
                    if (mySqlEx.Number == 1451)
                    {
                        MessageBox.Show("Az ügyfél nem törölhető, mert kapcsolódó járművek, javítások vagy időpontok tartoznak hozzá.\nElőször törölje a kapcsolódó bejegyzéseket.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Hiba a törlés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SetJarmuDetailPanelState(bool enable)
        {
            TextBoxJarmuId.IsReadOnly = true;
            TextBoxJarmuRendszam.IsReadOnly = !enable;
            TextBoxJarmuAlvazszam.IsReadOnly = !enable;
            TextBoxJarmuMarka.IsReadOnly = !enable;
            TextBoxJarmuModell.IsReadOnly = !enable;
            TextBoxJarmuGyartasiEv.IsReadOnly = !enable;
            TextBoxJarmuKmOraAllas.IsReadOnly = !enable;
            ComboBoxJarmuUgyfel.IsEnabled = enable;
            ButtonMentJarmu.IsEnabled = enable;
            ButtonTorolJarmu.IsEnabled = enable && !_isAddingNewJarmu;
            ButtonUjJarmu.IsEnabled = !enable;
        }

        private void SetIdopontDetailPanelState(bool enable)
        {
            TextBoxIdopontId.IsReadOnly = true;
            DatePickerIdopontDatum.IsEnabled = enable;
            TextBoxIdopontNap.IsReadOnly = !enable;
            TextBoxIdopontStatusz.IsReadOnly = !enable;
            ComboBoxIdopontUgyfel.IsEnabled = enable;

            ButtonMentIdopont.IsEnabled = enable;
            ButtonTorolIdopont.IsEnabled = enable && !_isAddingNewIdopont;
            ButtonUjIdopont.IsEnabled = !enable;
        }

        private void SetDolgozoDetailPanelState(bool enable)
        {
            TextBoxDolgozoId.IsReadOnly = true;
            TextBoxDolgozoNev.IsReadOnly = !enable;
            TextBoxDolgozoBeosztas.IsReadOnly = !enable;
            TextBoxDolgozoEmail.IsReadOnly = !enable;
            TextBoxDolgozoTelefon.IsReadOnly = !enable;
            TextBoxDolgozoLakcim.IsReadOnly = !enable;
            TextBoxDolgozoSzig.IsReadOnly = !enable;
            ButtonMentDolgozo.IsEnabled = enable;
            ButtonTorolDolgozo.IsEnabled = enable && !_isAddingNewDolgozo;
            ButtonUjDolgozo.IsEnabled = !enable;
        }

        private void JarmuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var jarmu = ((ListView)sender).SelectedItem as Jarmu;
            if (jarmu != null)
            {
                _isAddingNewJarmu = false;

                SetJarmuDetailPanelState(true);
                TextBoxJarmuId.Text = jarmu.JarmuId.ToString();
                TextBoxJarmuRendszam.Text = jarmu.Rendszam;
                TextBoxJarmuAlvazszam.Text = jarmu.Alvazszam;
                TextBoxJarmuMarka.Text = jarmu.Marka;
                TextBoxJarmuModell.Text = jarmu.Modell;
                TextBoxJarmuGyartasiEv.Text = jarmu.GyartasiEv.ToString();
                TextBoxJarmuKmOraAllas.Text = jarmu.KmOraAllas.ToString();
                ComboBoxJarmuUgyfel.SelectedValue = jarmu.UgyfelId;

                LoadJarmuJavitasokForJarmu(jarmu.JarmuId);
            }
            else
            {
                _isAddingNewJarmu = false;

                SetJarmuDetailPanelState(false);

                TextBoxJarmuId.Text = "";
                TextBoxJarmuRendszam.Text = "";
                TextBoxJarmuAlvazszam.Text = "";
                TextBoxJarmuMarka.Text = "";
                TextBoxJarmuModell.Text = "";
                TextBoxJarmuGyartasiEv.Text = "";
                TextBoxJarmuKmOraAllas.Text = "";
                ComboBoxJarmuUgyfel.SelectedItem = null;
                JarmuJavitasokGrid.ItemsSource = null;
            }
        }

        private void LoadJarmuJavitasokForJarmu(int jarmuId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a javítások betöltéséhez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                JarmuJavitasokGrid.ItemsSource = null;
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = @"SELECT j.Javitas_id, j.Megnevezes, j.Leiras, j.Koltseg, j.Datum FROM javitasok j JOIN vegez v ON j.Javitas_id = v.Javitas_id WHERE v.Jarmu_id = @JarmuId;";
                cmd.Parameters.AddWithValue("@JarmuId", jarmuId);

                List<Javitas> javitasok = new List<Javitas>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        javitasok.Add(
                            new Javitas
                            {
                                JavitasId = reader.GetInt32(0),
                                Megnevezes = reader.GetString(1),
                                Leiras = reader.GetString(2),
                                Koltseg = reader.GetInt32(3),
                                Datum = reader.GetDateTime(4)
                            });
                    }
                }

                JarmuJavitasokGrid.ItemsSource = javitasok;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Jármű javítások betöltési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                JarmuJavitasokGrid.ItemsSource = null;
            }
        }

        private void ButtonUjJarmu_Click(object sender, RoutedEventArgs e)
        {
            _isAddingNewJarmu = true;
            JarmuListView.SelectedItem = null;

            TextBoxJarmuId.Text = "";
            TextBoxJarmuRendszam.Text = "";
            TextBoxJarmuAlvazszam.Text = "";
            TextBoxJarmuMarka.Text = "";
            TextBoxJarmuModell.Text = "";
            TextBoxJarmuGyartasiEv.Text = "";
            TextBoxJarmuKmOraAllas.Text = "";
            ComboBoxJarmuUgyfel.SelectedItem = null;
            JarmuJavitasokGrid.ItemsSource = null;

            SetJarmuDetailPanelState(true);
            ButtonTorolJarmu.IsEnabled = false;
            ButtonUjJarmu.IsEnabled = false;

            MessageBox.Show("Új jármű adatok megadásához kérem töltse ki a mezőket.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonMentJarmu_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a mentéshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TextBoxJarmuRendszam.Text) || ComboBoxJarmuUgyfel.SelectedItem == null)
            {
                MessageBox.Show("Rendszám és Ügyfél kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TextBoxJarmuGyartasiEv.Text, out int gyartasiEv))
            {
                MessageBox.Show("Érvénytelen gyártási év!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TextBoxJarmuKmOraAllas.Text, out int kmOraAllas))
            {
                MessageBox.Show("Érvénytelen km óra állás!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedUgyfelId = (int)ComboBoxJarmuUgyfel.SelectedValue;


            try
            {
                var cmd = _conn.CreateCommand();

                if (_isAddingNewJarmu)
                {
                    cmd.CommandText = @"
                         INSERT INTO jarmuvek (Rendszam, Alvazszam, Marka, Modell, Gyartasi_ev, Km_ora_allas, Ugyfel_id)
                         VALUES (@Rendszam, @Alvazszam, @Marka, @Modell, @GyartasiEv, @KmOraAllas, @UgyfelId);
                         SELECT LAST_INSERT_ID();";

                    cmd.Parameters.AddWithValue("@Rendszam", TextBoxJarmuRendszam.Text);
                    cmd.Parameters.AddWithValue("@Alvazszam", TextBoxJarmuAlvazszam.Text);
                    cmd.Parameters.AddWithValue("@Marka", TextBoxJarmuMarka.Text);
                    cmd.Parameters.AddWithValue("@Modell", TextBoxJarmuModell.Text);
                    cmd.Parameters.AddWithValue("@GyartasiEv", gyartasiEv);
                    cmd.Parameters.AddWithValue("@KmOraAllas", kmOraAllas);
                    cmd.Parameters.AddWithValue("@UgyfelId", selectedUgyfelId);


                    int newJarmuId = Convert.ToInt32(cmd.ExecuteScalar());

                    MessageBox.Show($"Új jármű sikeresen hozzáadva! ID: {newJarmuId}", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    if (!int.TryParse(TextBoxJarmuId.Text, out int jarmuId) || jarmuId <= 0)
                    {
                        MessageBox.Show("Érvénytelen jármű azonosító a módosításhoz.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    cmd.CommandText = @" UPDATE jarmuvek SET Rendszam=@Rendszam, Alvazszam=@Alvazszam, Marka=@Marka, Modell=@Modell, Gyartasi_ev=@GyartasiEv, Km_ora_allas=@KmOraAllas, Ugyfel_id=@UgyfelId WHERE Jarmu_id=@JarmuId;";

                    cmd.Parameters.AddWithValue("@Rendszam", TextBoxJarmuRendszam.Text);
                    cmd.Parameters.AddWithValue("@Alvazszam", TextBoxJarmuAlvazszam.Text);
                    cmd.Parameters.AddWithValue("@Marka", TextBoxJarmuMarka.Text);
                    cmd.Parameters.AddWithValue("@Modell", TextBoxJarmuModell.Text);
                    cmd.Parameters.AddWithValue("@GyartasiEv", gyartasiEv);
                    cmd.Parameters.AddWithValue("@KmOraAllas", kmOraAllas);
                    cmd.Parameters.AddWithValue("@UgyfelId", selectedUgyfelId);
                    cmd.Parameters.AddWithValue("@JarmuId", jarmuId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Jármű adatok sikeresen módosítva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nem történt módosítás.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                LoadAllJarmuvek();
                _isAddingNewJarmu = false;
                SetJarmuDetailPanelState(false);
                JarmuListView.SelectedItem = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a jármű mentése során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonTorolJarmu_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a törléshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TextBoxJarmuId.Text, out int jarmuId) || jarmuId <= 0)
            {
                MessageBox.Show("Nincs kiválasztott jármű a törléshez.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Biztosan törölni szeretné a járművet (ID: {jarmuId}, Rendszám: {TextBoxJarmuRendszam.Text})?\nFIGYELEM: Ez törölheti a kapcsolódó javításokat is!", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var cmd = _conn.CreateCommand();
                    cmd.CommandText = "DELETE FROM jarmuvek WHERE Jarmu_id=@JarmuId;";
                    cmd.Parameters.AddWithValue("@JarmuId", jarmuId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Jármű sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadAllJarmuvek();
                        JarmuListView.SelectedItem = null;
                    }
                    else
                    {
                        MessageBox.Show("Nem történt törlés. Lehet, hogy a jármű már nem létezik?", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (MySqlException mySqlEx)
                {
                    if (mySqlEx.Number == 1451)
                    {
                        MessageBox.Show("A jármű nem törölhető, mert kapcsolódó javítások tartoznak hozzá.\nElőször törölje a kapcsolódó javításokat.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Hiba a törlés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void IdopontokGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var idopont = ((DataGrid)sender).SelectedItem as Idopont;
            if (idopont != null)
            {
                _isAddingNewIdopont = false;

                SetIdopontDetailPanelState(true);

                TextBoxIdopontId.Text = idopont.IdopontId.ToString();
                DatePickerIdopontDatum.SelectedDate = idopont.Datum;
                TextBoxIdopontNap.Text = idopont.Nap;
                TextBoxIdopontStatusz.Text = idopont.Statusz;

                LoadIdopontUgyfel(idopont.IdopontId);
            }
            else
            {
                _isAddingNewIdopont = false;

                SetIdopontDetailPanelState(false);

                TextBoxIdopontId.Text = "";
                DatePickerIdopontDatum.SelectedDate = null;
                TextBoxIdopontNap.Text = "";
                TextBoxIdopontStatusz.Text = "";
                ComboBoxIdopontUgyfel.SelectedItem = null;
            }
        }

        private void LoadIdopontUgyfel(int idopontId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open) return;

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Ugyfel_id FROM foglal WHERE Idopont_id = @IdopontId;";
                cmd.Parameters.AddWithValue("@IdopontId", idopontId);

                var ugyfelIdObj = cmd.ExecuteScalar();

                if (ugyfelIdObj != null && ugyfelIdObj != DBNull.Value)
                {
                    int ugyfelId = Convert.ToInt32(ugyfelIdObj);
                    var ugyfel = _ugyfelek?.FirstOrDefault(u => u.UgyfelId == ugyfelId);
                    if (ugyfel != null)
                    {
                        ComboBoxIdopontUgyfel.SelectedItem = ugyfel;
                    }
                    else
                    {
                        ComboBoxIdopontUgyfel.SelectedItem = null;
                    }
                }
                else
                {
                    ComboBoxIdopontUgyfel.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az időponthoz tartozó ügyfél betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                ComboBoxIdopontUgyfel.SelectedItem = null;
            }
        }

        private void ButtonFilterDates_Click(object sender, RoutedEventArgs e)
        {
            if (DpFilterDate.SelectedDate.HasValue)
            {
                if (_idopontok == null || _idopontok.Count == 0)
                {
                    MessageBox.Show("Az időpontok listája nincs betöltve.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime selectedDate = DpFilterDate.SelectedDate.Value;
                var filteredDates = _idopontok.Where(i => i.Datum.Date == selectedDate.Date).ToList();
                IdopontokGrid.ItemsSource = filteredDates;
            }
            else
            {
                MessageBox.Show("Kérem válasszon egy dátumot a szűréshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonShowAllDates_Click(object sender, RoutedEventArgs e)
        {
            if (_idopontok == null || _idopontok.Count == 0)
            {
                MessageBox.Show("Az időpontok listája nincs betöltve.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IdopontokGrid.ItemsSource = _idopontok;
        }

        private void ButtonUjIdopont_Click(object sender, RoutedEventArgs e)
        {
            _isAddingNewIdopont = true;
            IdopontokGrid.SelectedItem = null;

            TextBoxIdopontId.Text = "";
            DatePickerIdopontDatum.SelectedDate = null;
            TextBoxIdopontNap.Text = "";
            TextBoxIdopontStatusz.Text = "";
            ComboBoxIdopontUgyfel.SelectedItem = null;

            SetIdopontDetailPanelState(true);
            ButtonTorolIdopont.IsEnabled = false;
            ButtonUjIdopont.IsEnabled = false;

            MessageBox.Show("Új időpont adatok megadásához kérem töltse ki a mezőket.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonMentIdopont_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a mentéshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!DatePickerIdopontDatum.SelectedDate.HasValue || string.IsNullOrWhiteSpace(TextBoxIdopontNap.Text) || string.IsNullOrWhiteSpace(TextBoxIdopontStatusz.Text))
            {
                MessageBox.Show("Dátum, Nap és Státusz kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? selectedUgyfelId = null;
            if (ComboBoxIdopontUgyfel.SelectedValue != null)
            {
                selectedUgyfelId = (int)ComboBoxIdopontUgyfel.SelectedValue;
            }


            try
            {
                var cmd = _conn.CreateCommand();

                if (_isAddingNewIdopont)
                {
                    using (var transaction = _conn.BeginTransaction())
                    {
                        cmd.Transaction = transaction;

                        cmd.CommandText = @"INSERT INTO idopont (Datum, Nap, Statusz) VALUES (@Datum, @Nap, @Statusz); SELECT LAST_INSERT_ID();";

                        cmd.Parameters.AddWithValue("@Datum", DatePickerIdopontDatum.SelectedDate.Value);
                        cmd.Parameters.AddWithValue("@Nap", TextBoxIdopontNap.Text);
                        cmd.Parameters.AddWithValue("@Statusz", TextBoxIdopontStatusz.Text);

                        int newIdopontId = Convert.ToInt32(cmd.ExecuteScalar());

                        if (selectedUgyfelId.HasValue)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = @"
                                    INSERT INTO foglal (Idopont_id, Ugyfel_id)
                                    VALUES (@IdopontId, @UgyfelId);";
                            cmd.Parameters.AddWithValue("@IdopontId", newIdopontId);
                            cmd.Parameters.AddWithValue("@UgyfelId", selectedUgyfelId.Value);
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();

                        MessageBox.Show($"Új időpont sikeresen hozzáadva! ID: {newIdopontId}", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    }


                }
                else
                {
                    if (!int.TryParse(TextBoxIdopontId.Text, out int idopontId) || idopontId <= 0)
                    {
                        MessageBox.Show("Érvénytelen időpont azonosító a módosításhoz.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (var transaction = _conn.BeginTransaction())
                    {
                        cmd.Transaction = transaction;

                        cmd.CommandText = @"UPDATE idopont SET Datum=@Datum, Nap=@Nap, Statusz=@Statusz WHERE Idopont_id=@IdopontId;";

                        cmd.Parameters.AddWithValue("@Datum", DatePickerIdopontDatum.SelectedDate.Value);
                        cmd.Parameters.AddWithValue("@Nap", TextBoxIdopontNap.Text);
                        cmd.Parameters.AddWithValue("@Statusz", TextBoxIdopontStatusz.Text);
                        cmd.Parameters.AddWithValue("@IdopontId", idopontId);

                        cmd.ExecuteNonQuery();

                        cmd.Parameters.Clear();
                        cmd.CommandText = "DELETE FROM foglal WHERE Idopont_id = @IdopontId;";
                        cmd.Parameters.AddWithValue("@IdopontId", idopontId);
                        cmd.ExecuteNonQuery();

                        if (selectedUgyfelId.HasValue)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = @"
                                    INSERT INTO foglal (Idopont_id, Ugyfel_id)
                                    VALUES (@IdopontId, @UgyfelId);";
                            cmd.Parameters.AddWithValue("@IdopontId", idopontId);
                            cmd.Parameters.AddWithValue("@UgyfelId", selectedUgyfelId.Value);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        MessageBox.Show("Időpont adatok sikeresen módosítva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                LoadIdopontok();

                _isAddingNewIdopont = false;
                SetIdopontDetailPanelState(false);
                IdopontokGrid.SelectedItem = null;

            }
            catch (MySqlException mySqlEx)
            {
                MessageBox.Show($"Adatbázis hiba a mentés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a mentés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonTorolIdopont_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a törléshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TextBoxIdopontId.Text, out int idopontId) || idopontId <= 0)
            {
                MessageBox.Show("Nincs kiválasztott időpont a törléshez.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Biztosan törölni szeretné az időpontot (ID: {idopontId})?", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var cmd = _conn.CreateCommand();

                    using (var transaction = _conn.BeginTransaction())
                    {
                        cmd.Transaction = transaction;

                        cmd.CommandText = "DELETE FROM foglal WHERE Idopont_id=@IdopontId;";
                        cmd.Parameters.AddWithValue("@IdopontId", idopontId);
                        cmd.ExecuteNonQuery();

                        cmd.Parameters.Clear();
                        cmd.CommandText = "DELETE FROM idopont WHERE Idopont_id=@IdopontId;";
                        cmd.Parameters.AddWithValue("@IdopontId", idopontId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        transaction.Commit();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Időpont sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadIdopontok();
                            IdopontokGrid.SelectedItem = null;
                        }
                        else
                        {
                            MessageBox.Show("Nem történt törlés. Lehet, hogy az időpont már nem létezik?", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (MySqlException mySqlEx)
                {
                    MessageBox.Show($"Adatbázis hiba a törlés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DolgozoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dolgozo = ((ListView)sender).SelectedItem as Dolgozo;
            if (dolgozo != null)
            {
                _isAddingNewDolgozo = false;

                SetDolgozoDetailPanelState(true);

                TextBoxDolgozoId.Text = dolgozo.DolgozoId.ToString();
                TextBoxDolgozoNev.Text = dolgozo.Nev;
                TextBoxDolgozoBeosztas.Text = dolgozo.Beosztas;
                TextBoxDolgozoEmail.Text = dolgozo.EmailCim;
                TextBoxDolgozoTelefon.Text = dolgozo.Telefonszam;
                TextBoxDolgozoLakcim.Text = dolgozo.Lakcim;
                TextBoxDolgozoSzig.Text = dolgozo.SzemelyazonositoIgazolvanySzam;

                LoadDolgozoJavitasokCountForDolgozo(dolgozo.DolgozoId);
            }
            else
            {
                _isAddingNewDolgozo = false;

                SetDolgozoDetailPanelState(false);

                TextBoxDolgozoId.Text = "";
                TextBoxDolgozoNev.Text = "";
                TextBoxDolgozoBeosztas.Text = "";
                TextBoxDolgozoEmail.Text = "";
                TextBoxDolgozoTelefon.Text = "";
                TextBoxDolgozoLakcim.Text = "";
                TextBoxDolgozoSzig.Text = "";

                LabelJavitasokSzama.Content = "";
            }
        }

        private void LoadDolgozoJavitasokCountForDolgozo(int dolgozoId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a javítások számának lekérdezéséhez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                LabelJavitasokSzama.Content = "Hiba";
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM elvegzi WHERE Dolgozo_id=@DolgozoId;";
                cmd.Parameters.AddWithValue("@DolgozoId", dolgozoId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        LabelJavitasokSzama.Content = reader.GetInt32(0).ToString();
                    }
                    else
                    {
                        LabelJavitasokSzama.Content = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Javítások számának lekérdezési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                LabelJavitasokSzama.Content = "Hiba";
            }
        }

        private void ButtonUjDolgozo_Click(object sender, RoutedEventArgs e)
        {
            _isAddingNewDolgozo = true;
            DolgozoListView.SelectedItem = null;
            TextBoxDolgozoId.Text = "";
            TextBoxDolgozoNev.Text = "";
            TextBoxDolgozoBeosztas.Text = "";
            TextBoxDolgozoEmail.Text = "";
            TextBoxDolgozoTelefon.Text = "";
            TextBoxDolgozoLakcim.Text = "";
            TextBoxDolgozoSzig.Text = "";

            LabelJavitasokSzama.Content = "";

            SetDolgozoDetailPanelState(true);
            ButtonTorolDolgozo.IsEnabled = false;
            ButtonUjDolgozo.IsEnabled = false;

            MessageBox.Show("Új dolgozó adatok megadásához kérem töltse ki a mezőket.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonMentDolgozo_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a mentéshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TextBoxDolgozoNev.Text) || string.IsNullOrWhiteSpace(TextBoxDolgozoBeosztas.Text))
            {
                MessageBox.Show("Dolgozó neve és beosztása kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            try
            {
                var cmd = _conn.CreateCommand();

                if (_isAddingNewDolgozo)
                {
                    cmd.CommandText = @"INSERT INTO dolgozo (Nev, Beosztas, Lakcim, Telefonszam, Email_cim, Szemelyazonosito_igazolvany_szam) VALUES (@Nev, @Beosztas, @Lakcim, @Telefonszam, @EmailCim, @Szig); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.AddWithValue("@Nev", TextBoxDolgozoNev.Text);
                    cmd.Parameters.AddWithValue("@Beosztas", TextBoxDolgozoBeosztas.Text);
                    cmd.Parameters.AddWithValue("@Lakcim", TextBoxDolgozoLakcim.Text);
                    cmd.Parameters.AddWithValue("@Telefonszam", TextBoxDolgozoTelefon.Text);
                    cmd.Parameters.AddWithValue("@EmailCim", TextBoxDolgozoEmail.Text);
                    cmd.Parameters.AddWithValue("@Szig", TextBoxDolgozoSzig.Text);

                    int newDolgozoId = Convert.ToInt32(cmd.ExecuteScalar());

                    MessageBox.Show($"Új dolgozó sikeresen hozzáadva! ID: {newDolgozoId}", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    if (!int.TryParse(TextBoxDolgozoId.Text, out int dolgozoId) || dolgozoId <= 0)
                    {
                        MessageBox.Show("Érvénytelen dolgozó azonosító a módosításhoz.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    cmd.CommandText = @"UPDATE dolgozo SET Nev=@Nev, Beosztas=@Beosztas, Lakcim=@Lakcim, Telefonszam=@Telefonszam, Email_cim=@EmailCim, Szemelyazonosito_igazolvany_szam=@Szig WHERE Dolgozo_id=@DolgozoId;";

                    cmd.Parameters.AddWithValue("@Nev", TextBoxDolgozoNev.Text);
                    cmd.Parameters.AddWithValue("@Beosztas", TextBoxDolgozoBeosztas.Text);
                    cmd.Parameters.AddWithValue("@Lakcim", TextBoxDolgozoLakcim.Text);
                    cmd.Parameters.AddWithValue("@Telefonszam", TextBoxDolgozoTelefon.Text);
                    cmd.Parameters.AddWithValue("@EmailCim", TextBoxDolgozoEmail.Text);
                    cmd.Parameters.AddWithValue("@Szig", TextBoxDolgozoSzig.Text);
                    cmd.Parameters.AddWithValue("@DolgozoId", dolgozoId);


                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Dolgozó adatok sikeresen módosítva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nem történt módosítás.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                LoadDolgozok();

                _isAddingNewDolgozo = false;
                SetDolgozoDetailPanelState(false);
                DolgozoListView.SelectedItem = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a dolgozó mentése során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonTorolDolgozo_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a törléshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TextBoxDolgozoId.Text, out int dolgozoId) || dolgozoId <= 0)
            {
                MessageBox.Show("Nincs kiválasztott dolgozó a törléshez.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Biztosan törölni szeretné a dolgozót (ID: {dolgozoId}, Név: {TextBoxDolgozoNev.Text})?\nFIGYELEM: Ez törölheti a kapcsolódó 'elvegzi' bejegyzéseket is!", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var cmd = _conn.CreateCommand();
                    cmd.CommandText = "DELETE FROM dolgozo WHERE Dolgozo_id=@DolgozoId;";
                    cmd.Parameters.AddWithValue("@DolgozoId", dolgozoId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Dolgozó sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadDolgozok();
                        DolgozoListView.SelectedItem = null;
                    }
                    else
                    {
                        MessageBox.Show("Nem történt törlés. Lehet, hogy a dolgozó már nem létezik?", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (MySqlException mySqlEx)
                {
                    if (mySqlEx.Number == 1451)
                    {
                        MessageBox.Show("A dolgozó nem törölhető, mert kapcsolódó javítások tartoznak hozzá ('elvegzi' tábla).\nElőször törölje a kapcsolódó bejegyzéseket.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Hiba a törlés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonExportExcel_Customers_Click(object sender, RoutedEventArgs e)
        {
            var lista = _ugyfelek;
            if (lista == null || !lista.Any()) { MessageBox.Show("Nincs exportálható adat.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            var dlg = new SaveFileDialog { Filter = "Excel fájl (*.xlsx)|*.xlsx", FileName = "Ügyfelek.xlsx" };
            if (dlg.ShowDialog() != true) return;
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Ügyfelek");
                ws.Cell(1, 1).Value = "ID"; ws.Cell(1, 2).Value = "Név";
                ws.Cell(1, 3).Value = "Email"; ws.Cell(1, 4).Value = "Telefon"; ws.Cell(1, 5).Value = "Lakcím";
                int row = 2;
                foreach (var u in lista)
                {
                    ws.Cell(row, 1).Value = u.UgyfelId;
                    ws.Cell(row, 2).Value = u.Nev;
                    ws.Cell(row, 3).Value = u.EmailCim;
                    ws.Cell(row, 4).Value = u.Telefonszam;
                    ws.Cell(row, 5).Value = u.Lakcim;
                    row++;
                }
                ws.Columns().AdjustToContents();
                wb.SaveAs(dlg.FileName);
            }
            MessageBox.Show("Exportálás sikeres!", "Kész", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonExportExcel_Vehicles_Click(object sender, RoutedEventArgs e)
        {
            var lista = _jarmuvek;
            if (lista == null || !lista.Any()) { MessageBox.Show("Nincs exportálható adat.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            var dlg = new SaveFileDialog { Filter = "Excel fájl (*.xlsx)|*.xlsx", FileName = "Jarmuvek.xlsx" };
            if (dlg.ShowDialog() != true) return;
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Járművek");
                ws.Cell(1, 1).Value = "ID"; ws.Cell(1, 2).Value = "Rendszám";
                ws.Cell(1, 3).Value = "Márka"; ws.Cell(1, 4).Value = "Modell";
                ws.Cell(1, 5).Value = "Év"; ws.Cell(1, 6).Value = "Km állás";
                int row = 2;
                foreach (var j in lista)
                {
                    ws.Cell(row, 1).Value = j.JarmuId;
                    ws.Cell(row, 2).Value = j.Rendszam;
                    ws.Cell(row, 3).Value = j.Marka;
                    ws.Cell(row, 4).Value = j.Modell;
                    ws.Cell(row, 5).Value = j.GyartasiEv;
                    ws.Cell(row, 6).Value = j.KmOraAllas;
                    row++;
                }
                ws.Columns().AdjustToContents();
                wb.SaveAs(dlg.FileName);
            }
            MessageBox.Show("Exportálás sikeres!", "Kész", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonExportExcel_Appointments_Click(object sender, RoutedEventArgs e)
        {
            var lista = _idopontok;
            if (lista == null || !lista.Any()) { MessageBox.Show("Nincs exportálható adat.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            var dlg = new SaveFileDialog { Filter = "Excel fájl (*.xlsx)|*.xlsx", FileName = "Idopontok.xlsx" };
            if (dlg.ShowDialog() != true) return;
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Időpontok");
                ws.Cell(1, 1).Value = "ID"; ws.Cell(1, 2).Value = "Dátum";
                ws.Cell(1, 3).Value = "Nap"; ws.Cell(1, 4).Value = "Státusz";
                int row = 2;
                foreach (var i in lista)
                {
                    ws.Cell(row, 1).Value = i.IdopontId;
                    ws.Cell(row, 2).Value = i.Datum;
                    ws.Cell(row, 3).Value = i.Nap;
                    ws.Cell(row, 4).Value = i.Statusz;
                    row++;
                }
                ws.Columns().AdjustToContents();
                wb.SaveAs(dlg.FileName);
            }
            MessageBox.Show("Exportálás sikeres!", "Kész", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonExportExcel_Employees_Click(object sender, RoutedEventArgs e)
        {
            var lista = _dolgozok;
            if (lista == null || !lista.Any()) { MessageBox.Show("Nincs exportálható adat.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            var dlg = new SaveFileDialog { Filter = "Excel fájl (*.xlsx)|*.xlsx", FileName = "Dolgozok.xlsx" };
            if (dlg.ShowDialog() != true) return;
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Dolgozók");
                ws.Cell(1, 1).Value = "ID"; ws.Cell(1, 2).Value = "Név";
                ws.Cell(1, 3).Value = "Beosztás"; ws.Cell(1, 4).Value = "Email"; ws.Cell(1, 5).Value = "Telefon";
                int row = 2;
                foreach (var d in lista)
                {
                    ws.Cell(row, 1).Value = d.DolgozoId;
                    ws.Cell(row, 2).Value = d.Nev;
                    ws.Cell(row, 3).Value = d.Beosztas;
                    ws.Cell(row, 4).Value = d.EmailCim;
                    ws.Cell(row, 5).Value = d.Telefonszam;
                    row++;
                }
                ws.Columns().AdjustToContents();
                wb.SaveAs(dlg.FileName);
            }
            MessageBox.Show("Exportálás sikeres!", "Kész", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}