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
using System.Text.RegularExpressions;
using System.Net.Mail;

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
                LoadJavitasok();

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

        #region Validation Helper Methods

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return true;
            return Regex.IsMatch(phone, @"^\+?[0-9]{7,}$");
        }

        private bool IsValidLicensePlate(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate)) return false;
            return Regex.IsMatch(plate.ToUpper(), @"^[A-Z]{3}-[0-9]{3}$") || Regex.IsMatch(plate.ToUpper(), @"^[A-Z]{4}-[0-9]{3}$");
        }

        private bool IsValidVinLike(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin)) return true;
            return vin.Length == 17 && Regex.IsMatch(vin, @"^[a-zA-Z0-9]+$");
        }

        private bool IsValidYear(int year)
        {
            return year >= 1900 && year <= DateTime.Now.Year + 1;
        }

        private bool IsNonNegativeInt(string value, out int result)
        {
            return int.TryParse(value, out result) && result >= 0;
        }

        private bool IsPositiveInt(string value, out int result)
        {
            return int.TryParse(value, out result) && result > 0;
        }

        private bool IsDateInPast(DateTime? date)
        {
            return date.HasValue && date.Value.Date < DateTime.Now.Date;
        }

        private bool IsDateNotInFuture(DateTime? date)
        {
            return date.HasValue && date.Value.Date <= DateTime.Now.Date;
        }

        private bool IsValidIdNumber(string idNumber)
        {
            if (string.IsNullOrWhiteSpace(idNumber)) return true;

            return Regex.IsMatch(idNumber, @"^[0-9]{6}[A-Za-z]{2}$");
        }


        #endregion Validation Helper Methods

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

        #region Data Loading Methods
        private void LoadUgyfelek()
        {
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
                                 EmailCim = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                 Telefonszam = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                 Lakcim = reader.IsDBNull(4) ? "" : reader.GetString(4)
                             });
                    }
                }

                UgyfelListView.ItemsSource = _ugyfelek;
                ComboBoxJarmuUgyfel.ItemsSource = _ugyfelek;
                ComboBoxIdopontUgyfel.ItemsSource = _ugyfelek;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ügyfelek betöltési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDolgozok()
        {
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
                                 Lakcim = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                 Telefonszam = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                 EmailCim = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                 SzemelyazonositoIgazolvanySzam = reader.IsDBNull(6) ? "" : reader.GetString(6)
                             });
                    }
                }

                DolgozoListView.ItemsSource = _dolgozok;
                ComboBoxJavitasDolgozo.ItemsSource = _dolgozok;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dolgozók betöltési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadIdopontok()
        {
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
                                 Alvazszam = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                 Marka = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                 Modell = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                 GyartasiEv = reader.GetInt32(5),
                                 KmOraAllas = reader.GetInt32(6),
                                 UgyfelId = reader.GetInt32(7)
                             });
                    }
                }

                JarmuListView.ItemsSource = _jarmuvek;
                ComboBoxJavitasJarmu.ItemsSource = _jarmuvek;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Összes jármű betöltési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LoadJavitasok()
        {
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

        private void LoadJarmuvekForUgyfel(int ugyfelId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
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
                                 Alvazszam = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                 Marka = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                 Modell = reader.IsDBNull(4) ? "" : reader.GetString(4),
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

        private void LoadJarmuJavitasokForJarmu(int jarmuId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                JarmuJavitasokGrid.ItemsSource = null;
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = @"SELECT j.Javitas_id, j.Megnevezes, j.Leiras, j.Koltseg, j.Datum FROM javitasok j JOIN vegez v ON j.Javitas_id = v.Javitas_id WHERE v.Jarmu_id = @JarmuId ORDER BY j.Datum DESC;";
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
                                 Megnevezes = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                 Leiras = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                 Koltseg = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                 Datum = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4)
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

        private void LoadIdopontUgyfel(int idopontId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open) return;
            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Ugyfel_id FROM foglal WHERE Idopont_id = @IdopontId LIMIT 1;";
                cmd.Parameters.AddWithValue("@IdopontId", idopontId);

                var ugyfelIdObj = cmd.ExecuteScalar();
                if (ugyfelIdObj != null && ugyfelIdObj != DBNull.Value)
                {
                    int ugyfelId = Convert.ToInt32(ugyfelIdObj);
                    ComboBoxIdopontUgyfel.SelectedValue = ugyfelId;
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

        private void LoadDolgozoJavitasokCountForDolgozo(int dolgozoId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                LabelJavitasokSzama.Content = "Hiba";
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM elvegzi WHERE Dolgozo_id=@DolgozoId;";
                cmd.Parameters.AddWithValue("@DolgozoId", dolgozoId);
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    LabelJavitasokSzama.Content = Convert.ToInt32(result).ToString();
                }
                else
                {
                    LabelJavitasokSzama.Content = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Javítások számának lekérdezési hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                LabelJavitasokSzama.Content = "Hiba";
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

        #endregion Data Loading Methods

        #region UI Navigation and State Management
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
            _isAddingNewJavitas = false;

            HideAllViews();
            switch (button.Name)
            {
                case "ButtonNavigateCustomers":
                    CustomersView.Visibility = Visibility.Visible;

                    if (_ugyfelek == null) LoadUgyfelek();
                    UgyfelListView.SelectedItem = null;
                    SetUgyfelDetailPanelState(false);
                    break;
                case "ButtonNavigateVehicles":
                    VehiclesView.Visibility = Visibility.Visible;
                    if (_jarmuvek == null) LoadAllJarmuvek();

                    if (ComboBoxJarmuUgyfel.ItemsSource == null && _ugyfelek != null) ComboBoxJarmuUgyfel.ItemsSource = _ugyfelek;
                    JarmuListView.SelectedItem = null;
                    SetJarmuDetailPanelState(false);
                    break;
                case "ButtonNavigateAppointments":
                    AppointmentsView.Visibility = Visibility.Visible;
                    if (_idopontok == null) LoadIdopontok();

                    if (ComboBoxIdopontUgyfel.ItemsSource == null && _ugyfelek != null) ComboBoxIdopontUgyfel.ItemsSource = _ugyfelek;
                    IdopontokGrid.SelectedItem = null;
                    SetIdopontDetailPanelState(false);
                    break;
                case "ButtonNavigateEmployees":
                    EmployeesView.Visibility = Visibility.Visible;
                    if (_dolgozok == null) LoadDolgozok();
                    DolgozoListView.SelectedItem = null;
                    SetDolgozoDetailPanelState(false);
                    break;
                case "ButtonNavigateRepairs":
                    RepairsView.Visibility = Visibility.Visible;
                    if (_javitasok == null) LoadJavitasok();

                    if (ComboBoxJavitasJarmu.ItemsSource == null && _jarmuvek != null) ComboBoxJavitasJarmu.ItemsSource = _jarmuvek;
                    if (ComboBoxJavitasDolgozo.ItemsSource == null && _dolgozok != null) ComboBoxJavitasDolgozo.ItemsSource = _dolgozok;
                    JavitasokGrid.SelectedItem = null;
                    SetJavitasDetailPanelState(false);
                    break;
            }
        }

        private void SetUgyfelDetailPanelState(bool enable)
        {
            UgyfelDetailPanel.IsEnabled = enable;
            TextBoxUgyfelNev.IsReadOnly = !enable;
            TextBoxUgyfelEmail.IsReadOnly = !enable;
            TextBoxUgyfelTelefon.IsReadOnly = !enable;
            TextBoxUgyfelLakcim.IsReadOnly = !enable;
            ButtonMentUgyfel.IsEnabled = enable;
            ButtonTorolUgyfel.IsEnabled = enable && !_isAddingNewUgyfel;
            ButtonUjUgyfel.IsEnabled = !enable;
        }

        private void SetJarmuDetailPanelState(bool enable)
        {
            JarmuDetailPanel.IsEnabled = enable;
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
            DolgozoDetailPanel.IsEnabled = enable;
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

        private void SetJavitasDetailPanelState(bool enable)
        {
            JavitasDetailPanel.IsEnabled = enable;
            TextBoxJavitasMegnevezes.IsReadOnly = !enable;
            TextBoxJavitasLeiras.IsReadOnly = !enable;
            TextBoxJavitasKoltseg.IsReadOnly = !enable;
            DatePickerJavitasDatum.IsEnabled = enable;
            ComboBoxJavitasJarmu.IsEnabled = enable;
            ComboBoxJavitasDolgozo.IsEnabled = enable;
            ButtonMentJavitas.IsEnabled = enable;
            ButtonTorolJavitas.IsEnabled = enable && !_isAddingNewJavitas;
            ButtonUjJavitas.IsEnabled = !enable;
        }

        #endregion UI Navigation and State Management

        #region CRUD Operations - Ugyfel (Customer)

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
                ClearUgyfelDetailFields();
                SetUgyfelDetailPanelState(false);
            }
        }

        private void ButtonUjUgyfel_Click(object sender, RoutedEventArgs e)
        {
            _isAddingNewUgyfel = true;
            UgyfelListView.SelectedItem = null;
            ClearUgyfelDetailFields();
            SetUgyfelDetailPanelState(true);
            ButtonTorolUgyfel.IsEnabled = false;
            ButtonUjUgyfel.IsEnabled = false;
            TextBoxUgyfelNev.Focus();
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
                TextBoxUgyfelNev.Focus();
                return;
            }
            if (!IsValidEmail(TextBoxUgyfelEmail.Text))
            {
                MessageBox.Show("Érvénytelen email cím formátum!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxUgyfelEmail.Focus();
                return;
            }
            if (!IsValidPhoneNumber(TextBoxUgyfelTelefon.Text))
            {
                MessageBox.Show("Érvénytelen telefonszám formátum! (Csak számok és '+', min. 7 karakter)", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxUgyfelTelefon.Focus();
                return;
            }

            try
            {
                var cmd = _conn.CreateCommand();
                string nev = TextBoxUgyfelNev.Text;
                string email = TextBoxUgyfelEmail.Text;
                string telefon = TextBoxUgyfelTelefon.Text;
                string lakcim = TextBoxUgyfelLakcim.Text;

                if (_isAddingNewUgyfel)
                {
                    cmd.CommandText = @"INSERT INTO ugyfelek (Nev, Email_cim, Telefonszam, Lakcim) VALUES (@Nev, @EmailCim, @Telefonszam, @Lakcim); SELECT LAST_INSERT_ID();";
                    cmd.Parameters.AddWithValue("@Nev", nev);
                    cmd.Parameters.AddWithValue("@EmailCim", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);
                    cmd.Parameters.AddWithValue("@Telefonszam", string.IsNullOrWhiteSpace(telefon) ? (object)DBNull.Value : telefon);
                    cmd.Parameters.AddWithValue("@Lakcim", string.IsNullOrWhiteSpace(lakcim) ? (object)DBNull.Value : lakcim);

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
                    cmd.Parameters.AddWithValue("@Nev", nev);
                    cmd.Parameters.AddWithValue("@EmailCim", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);
                    cmd.Parameters.AddWithValue("@Telefonszam", string.IsNullOrWhiteSpace(telefon) ? (object)DBNull.Value : telefon);
                    cmd.Parameters.AddWithValue("@Lakcim", string.IsNullOrWhiteSpace(lakcim) ? (object)DBNull.Value : lakcim);
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
                UgyfelListView.SelectedItem = null;
            }
            catch (MySqlException mySqlEx)
            {
                MessageBox.Show($"Adatbázis hiba mentés közben: {mySqlEx.Message}\nHibakód: {mySqlEx.Number}", "Adatbázis Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Általános hiba a mentés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
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

            var result = MessageBox.Show($"Biztosan törölni szeretné az ügyfelet (ID: {ugyfelId}, Név: {TextBoxUgyfelNev.Text})?\nFIGYELEM: Ez törölheti a kapcsolódó járműveket és egyéb adatokat is!", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);
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
                        MessageBox.Show("Az ügyfél nem törölhető, mert kapcsolódó adatok (pl. jármű, javítás) tartoznak hozzá.\nElőször törölje a kapcsolódó bejegyzéseket.", "Törlési Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Adatbázis hiba a törlés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearUgyfelDetailFields()
        {
            TextBoxUgyfelId.Text = "";
            TextBoxUgyfelNev.Text = "";
            TextBoxUgyfelEmail.Text = "";
            TextBoxUgyfelTelefon.Text = "";
            TextBoxUgyfelLakcim.Text = "";
            JarmuvekGrid.ItemsSource = null;
            LabelJarmuvekSzama.Content = "";
        }

        #endregion CRUD Operations - Ugyfel (Customer)

        #region CRUD Operations - Jarmu (Vehicle)

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
                ClearJarmuDetailFields();
                SetJarmuDetailPanelState(false);
            }
        }

        private void ButtonUjJarmu_Click(object sender, RoutedEventArgs e)
        {
            _isAddingNewJarmu = true;
            JarmuListView.SelectedItem = null;
            ClearJarmuDetailFields();
            SetJarmuDetailPanelState(true);
            ButtonTorolJarmu.IsEnabled = false;
            ButtonUjJarmu.IsEnabled = false;

            if (ComboBoxJarmuUgyfel.ItemsSource == null && _ugyfelek != null) ComboBoxJarmuUgyfel.ItemsSource = _ugyfelek;
            ComboBoxJarmuUgyfel.Focus();
        }

        private void ButtonMentJarmu_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a mentéshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidLicensePlate(TextBoxJarmuRendszam.Text))
            {
                MessageBox.Show("Érvénytelen vagy hiányzó rendszám! (Formátum: AAA-123 vagy AAAA-123)", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxJarmuRendszam.Focus();
                return;
            }
            if (ComboBoxJarmuUgyfel.SelectedItem == null)
            {
                MessageBox.Show("Ki kell választani a jármű tulajdonosát!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                ComboBoxJarmuUgyfel.Focus();
                return;
            }
            if (!int.TryParse(TextBoxJarmuGyartasiEv.Text, out int gyartasiEv) || !IsValidYear(gyartasiEv))
            {
                MessageBox.Show("Érvénytelen gyártási év! (Ésszerű évszámot adjon meg, pl. 1900-" + (DateTime.Now.Year + 1) + ")", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxJarmuGyartasiEv.Focus();
                return;
            }
            if (!IsNonNegativeInt(TextBoxJarmuKmOraAllas.Text, out int kmOraAllas))
            {
                MessageBox.Show("Érvénytelen km óra állás! (Nem negatív egész számot adjon meg)", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxJarmuKmOraAllas.Focus();
                return;
            }
            if (!IsValidVinLike(TextBoxJarmuAlvazszam.Text))
            {
                MessageBox.Show("Érvénytelen alvázszám formátum! (Ha megadja, 17 karakter hosszú legyen, csak betű és szám)", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxJarmuAlvazszam.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(TextBoxJarmuMarka.Text))
            {
                MessageBox.Show("A Márka megadása kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxJarmuMarka.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(TextBoxJarmuModell.Text))
            {
                MessageBox.Show("A Modell megadása kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxJarmuModell.Focus();
                return;
            }

            int selectedUgyfelId = (int)ComboBoxJarmuUgyfel.SelectedValue;
            string rendszam = TextBoxJarmuRendszam.Text.ToUpper();
            string alvazszam = TextBoxJarmuAlvazszam.Text;
            string marka = TextBoxJarmuMarka.Text;
            string modell = TextBoxJarmuModell.Text;

            try
            {
                var cmd = _conn.CreateCommand();
                if (_isAddingNewJarmu)
                {
                    cmd.CommandText = @"INSERT INTO jarmuvek (Rendszam, Alvazszam, Marka, Modell, Gyartasi_ev, Km_ora_allas, Ugyfel_id) VALUES (@Rendszam, @Alvazszam, @Marka, @Modell, @GyartasiEv, @KmOraAllas, @UgyfelId); SELECT LAST_INSERT_ID();";
                    cmd.Parameters.AddWithValue("@Rendszam", rendszam);
                    cmd.Parameters.AddWithValue("@Alvazszam", string.IsNullOrWhiteSpace(alvazszam) ? (object)DBNull.Value : alvazszam);
                    cmd.Parameters.AddWithValue("@Marka", marka);
                    cmd.Parameters.AddWithValue("@Modell", modell);
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
                    cmd.Parameters.AddWithValue("@Rendszam", rendszam);
                    cmd.Parameters.AddWithValue("@Alvazszam", string.IsNullOrWhiteSpace(alvazszam) ? (object)DBNull.Value : alvazszam);
                    cmd.Parameters.AddWithValue("@Marka", marka);
                    cmd.Parameters.AddWithValue("@Modell", modell);
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
                JarmuListView.SelectedItem = null;
            }
            catch (MySqlException mySqlEx)
            {
                if (mySqlEx.Number == 1062)
                {
                    MessageBox.Show($"Adatbázis hiba: A '{rendszam}' rendszám már létezik!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Adatbázis hiba jármű mentése közben: {mySqlEx.Message}", "Adatbázis Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                        MessageBox.Show("A jármű nem törölhető, mert kapcsolódó javítások tartoznak hozzá.\nElőször törölje a kapcsolódó javításokat.", "Törlési Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Adatbázis hiba a törlés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearJarmuDetailFields()
        {
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

        #endregion CRUD Operations - Jarmu (Vehicle)

        #region CRUD Operations - Idopont (Appointment)

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
                ClearIdopontDetailFields();
                SetIdopontDetailPanelState(false);
            }
        }

        private void ButtonUjIdopont_Click(object sender, RoutedEventArgs e)
        {
            _isAddingNewIdopont = true;
            IdopontokGrid.SelectedItem = null;
            ClearIdopontDetailFields();
            SetIdopontDetailPanelState(true);
            ButtonTorolIdopont.IsEnabled = false;
            ButtonUjIdopont.IsEnabled = false;

            if (ComboBoxIdopontUgyfel.ItemsSource == null && _ugyfelek != null) ComboBoxIdopontUgyfel.ItemsSource = _ugyfelek;
            DatePickerIdopontDatum.Focus();
        }

        private void ButtonMentIdopont_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a mentéshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!DatePickerIdopontDatum.SelectedDate.HasValue)
            {
                MessageBox.Show("A dátum megadása kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                DatePickerIdopontDatum.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TextBoxIdopontNap.Text))
            {
                MessageBox.Show("A Nap megadása kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxIdopontNap.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TextBoxIdopontStatusz.Text))
            {
                MessageBox.Show("A Státusz megadása kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxIdopontStatusz.Focus();
                return;
            }

            int? selectedUgyfelId = null;
            if (ComboBoxIdopontUgyfel.SelectedValue != null)
            {
                selectedUgyfelId = (int)ComboBoxIdopontUgyfel.SelectedValue;
            }

            DateTime datum = DatePickerIdopontDatum.SelectedDate.Value;
            string nap = TextBoxIdopontNap.Text;
            string statusz = TextBoxIdopontStatusz.Text;

            MySqlTransaction transaction = null;
            try
            {
                transaction = _conn.BeginTransaction();
                var cmd = _conn.CreateCommand();
                cmd.Transaction = transaction;

                int idopontId;

                if (_isAddingNewIdopont)
                {
                    cmd.CommandText = @"INSERT INTO idopont (Datum, Nap, Statusz) VALUES (@Datum, @Nap, @Statusz); SELECT LAST_INSERT_ID();";
                    cmd.Parameters.AddWithValue("@Datum", datum);
                    cmd.Parameters.AddWithValue("@Nap", nap);
                    cmd.Parameters.AddWithValue("@Statusz", statusz);
                    idopontId = Convert.ToInt32(cmd.ExecuteScalar());

                    if (selectedUgyfelId.HasValue)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = @"INSERT INTO foglal (Idopont_id, Ugyfel_id) VALUES (@IdopontId, @UgyfelId);";
                        cmd.Parameters.AddWithValue("@IdopontId", idopontId);
                        cmd.Parameters.AddWithValue("@UgyfelId", selectedUgyfelId.Value);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show($"Új időpont sikeresen hozzáadva! ID: {idopontId}", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (!int.TryParse(TextBoxIdopontId.Text, out idopontId) || idopontId <= 0)
                    {
                        MessageBox.Show("Érvénytelen időpont azonosító a módosításhoz.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        transaction.Rollback();
                        return;
                    }

                    cmd.CommandText = @"UPDATE idopont SET Datum=@Datum, Nap=@Nap, Statusz=@Statusz WHERE Idopont_id=@IdopontId;";
                    cmd.Parameters.AddWithValue("@Datum", datum);
                    cmd.Parameters.AddWithValue("@Nap", nap);
                    cmd.Parameters.AddWithValue("@Statusz", statusz);
                    cmd.Parameters.AddWithValue("@IdopontId", idopontId);
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM foglal WHERE Idopont_id = @IdopontId;";
                    cmd.Parameters.AddWithValue("@IdopontId", idopontId);
                    cmd.ExecuteNonQuery();

                    if (selectedUgyfelId.HasValue)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = @"INSERT INTO foglal (Idopont_id, Ugyfel_id) VALUES (@IdopontId, @UgyfelId);";
                        cmd.Parameters.AddWithValue("@IdopontId", idopontId);
                        cmd.Parameters.AddWithValue("@UgyfelId", selectedUgyfelId.Value);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Időpont adatok sikeresen módosítva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                transaction.Commit();

                LoadIdopontok();
                _isAddingNewIdopont = false;
                IdopontokGrid.SelectedItem = null;
            }
            catch (MySqlException mySqlEx)
            {
                if (transaction != null) transaction.Rollback();
                MessageBox.Show($"Adatbázis hiba a mentés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
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

            string datumStr = DatePickerIdopontDatum.SelectedDate.HasValue
                               ? DatePickerIdopontDatum.SelectedDate.Value.ToString("yyyy-MM-dd")
                               : "ismeretlen dátum";
            var result = MessageBox.Show($"Biztosan törölni szeretné az időpontot (ID: {idopontId}, Dátum: {datumStr})?\nEz a foglalást is törli, ha van.", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                MySqlTransaction transaction = null;
                try
                {
                    transaction = _conn.BeginTransaction();
                    var cmd = _conn.CreateCommand();
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
                        MessageBox.Show("Nem történt törlés az 'idopont' táblában. Lehet, hogy már nem létezik?", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (MySqlException mySqlEx)
                {
                    if (transaction != null) transaction.Rollback();
                    MessageBox.Show($"Adatbázis hiba a törlés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    if (transaction != null) transaction.Rollback();
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearIdopontDetailFields()
        {
            TextBoxIdopontId.Text = "";
            DatePickerIdopontDatum.SelectedDate = null;
            TextBoxIdopontNap.Text = "";
            TextBoxIdopontStatusz.Text = "";
            ComboBoxIdopontUgyfel.SelectedItem = null;
        }

        private void ButtonFilterDates_Click(object sender, RoutedEventArgs e)
        {
            if (DpFilterDate.SelectedDate.HasValue)
            {
                if (_idopontok == null) LoadIdopontok();
                if (_idopontok == null) return;

                DateTime selectedDate = DpFilterDate.SelectedDate.Value.Date;
                var filteredDates = _idopontok.Where(i => i.Datum.Date == selectedDate).ToList();
                IdopontokGrid.ItemsSource = filteredDates;
            }
            else
            {
                MessageBox.Show("Kérem válasszon egy dátumot a szűréshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonShowAllDates_Click(object sender, RoutedEventArgs e)
        {
            if (_idopontok == null) LoadIdopontok();
            if (_idopontok == null) return;

            IdopontokGrid.ItemsSource = _idopontok;
            DpFilterDate.SelectedDate = null;
        }

        #endregion CRUD Operations - Idopont (Appointment)

        #region CRUD Operations - Dolgozo (Employee)

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
                ClearDolgozoDetailFields();
                SetDolgozoDetailPanelState(false);
            }
        }

        private void ButtonUjDolgozo_Click(object sender, RoutedEventArgs e)
        {
            _isAddingNewDolgozo = true;
            DolgozoListView.SelectedItem = null;
            ClearDolgozoDetailFields();
            SetDolgozoDetailPanelState(true);
            ButtonTorolDolgozo.IsEnabled = false;
            ButtonUjDolgozo.IsEnabled = false;
            TextBoxDolgozoNev.Focus();
        }

        private void ButtonMentDolgozo_Click(object sender, RoutedEventArgs e)
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat a mentéshez.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TextBoxDolgozoNev.Text))
            {
                MessageBox.Show("Dolgozó neve kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxDolgozoNev.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(TextBoxDolgozoBeosztas.Text))
            {
                MessageBox.Show("Dolgozó beosztása kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxDolgozoBeosztas.Focus();
                return;
            }
            if (!IsValidEmail(TextBoxDolgozoEmail.Text))
            {
                MessageBox.Show("Érvénytelen email cím formátum!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxDolgozoEmail.Focus();
                return;
            }
            if (!IsValidPhoneNumber(TextBoxDolgozoTelefon.Text))
            {
                MessageBox.Show("Érvénytelen telefonszám formátum! (Csak számok és '+', min. 7 karakter)", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxDolgozoTelefon.Focus();
                return;
            }
            if (!IsValidIdNumber(TextBoxDolgozoSzig.Text))
            {
                MessageBox.Show("Érvénytelen személyi igazolvány szám formátum! (Ha megadja, pl. 123456AB)", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxDolgozoSzig.Focus();
                return;
            }

            string nev = TextBoxDolgozoNev.Text;
            string beosztas = TextBoxDolgozoBeosztas.Text;
            string email = TextBoxDolgozoEmail.Text;
            string telefon = TextBoxDolgozoTelefon.Text;
            string lakcim = TextBoxDolgozoLakcim.Text;
            string szig = TextBoxDolgozoSzig.Text;

            try
            {
                var cmd = _conn.CreateCommand();
                if (_isAddingNewDolgozo)
                {
                    cmd.CommandText = @"INSERT INTO dolgozo (Nev, Beosztas, Lakcim, Telefonszam, Email_cim, Szemelyazonosito_igazolvany_szam) VALUES (@Nev, @Beosztas, @Lakcim, @Telefonszam, @EmailCim, @Szig); SELECT LAST_INSERT_ID();";
                    cmd.Parameters.AddWithValue("@Nev", nev);
                    cmd.Parameters.AddWithValue("@Beosztas", beosztas);
                    cmd.Parameters.AddWithValue("@Lakcim", string.IsNullOrWhiteSpace(lakcim) ? (object)DBNull.Value : lakcim);
                    cmd.Parameters.AddWithValue("@Telefonszam", string.IsNullOrWhiteSpace(telefon) ? (object)DBNull.Value : telefon);
                    cmd.Parameters.AddWithValue("@EmailCim", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);
                    cmd.Parameters.AddWithValue("@Szig", string.IsNullOrWhiteSpace(szig) ? (object)DBNull.Value : szig);

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
                    cmd.Parameters.AddWithValue("@Nev", nev);
                    cmd.Parameters.AddWithValue("@Beosztas", beosztas);
                    cmd.Parameters.AddWithValue("@Lakcim", string.IsNullOrWhiteSpace(lakcim) ? (object)DBNull.Value : lakcim);
                    cmd.Parameters.AddWithValue("@Telefonszam", string.IsNullOrWhiteSpace(telefon) ? (object)DBNull.Value : telefon);
                    cmd.Parameters.AddWithValue("@EmailCim", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);
                    cmd.Parameters.AddWithValue("@Szig", string.IsNullOrWhiteSpace(szig) ? (object)DBNull.Value : szig);
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
                DolgozoListView.SelectedItem = null;
            }
            catch (MySqlException mySqlEx)
            {
                MessageBox.Show($"Adatbázis hiba dolgozó mentése közben: {mySqlEx.Message}", "Adatbázis Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
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

            var result = MessageBox.Show($"Biztosan törölni szeretné a dolgozót (ID: {dolgozoId}, Név: {TextBoxDolgozoNev.Text})?\nFIGYELEM: Ez törölheti a hozzárendelt javítási kapcsolatokat is!", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);
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
                        MessageBox.Show("A dolgozó nem törölhető, mert kapcsolódó javítások ('elvegzi') tartoznak hozzá.\nElőször módosítsa/törölje a kapcsolódó javításokat.", "Törlési Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Adatbázis hiba a törlés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearDolgozoDetailFields()
        {
            TextBoxDolgozoId.Text = "";
            TextBoxDolgozoNev.Text = "";
            TextBoxDolgozoBeosztas.Text = "";
            TextBoxDolgozoEmail.Text = "";
            TextBoxDolgozoTelefon.Text = "";
            TextBoxDolgozoLakcim.Text = "";
            TextBoxDolgozoSzig.Text = "";
            LabelJavitasokSzama.Content = "";
        }

        #endregion CRUD Operations - Dolgozo (Employee)

        #region CRUD Operations - Javitas (Repair)

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
                DatePickerJavitasDatum.SelectedDate = javitas.Datum == DateTime.MinValue ? (DateTime?)null : javitas.Datum;

                LoadJavitasKapcsolatok(javitas.JavitasId);
            }
            else
            {
                _isAddingNewJavitas = false;
                ClearJavitasDetailFields();
                SetJavitasDetailPanelState(false);
            }
        }

        private void ButtonUjJavitas_Click(object sender, RoutedEventArgs e)
        {
            _isAddingNewJavitas = true;
            JavitasokGrid.SelectedItem = null;
            ClearJavitasDetailFields();
            SetJavitasDetailPanelState(true);
            ButtonTorolJavitas.IsEnabled = false;
            ButtonUjJavitas.IsEnabled = false;

            if (ComboBoxJavitasJarmu.ItemsSource == null && _jarmuvek != null) ComboBoxJavitasJarmu.ItemsSource = _jarmuvek;
            if (ComboBoxJavitasDolgozo.ItemsSource == null && _dolgozok != null) ComboBoxJavitasDolgozo.ItemsSource = _dolgozok;

            DatePickerJavitasDatum.SelectedDate = DateTime.Now;
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
                TextBoxJavitasMegnevezes.Focus();
                return;
            }
            if (!DatePickerJavitasDatum.SelectedDate.HasValue)
            {
                MessageBox.Show("A javítás dátuma kötelező!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                DatePickerJavitasDatum.Focus();
                return;
            }

            if (!IsNonNegativeInt(TextBoxJavitasKoltseg.Text, out int koltseg))
            {
                MessageBox.Show("Érvénytelen költség! (Nem negatív egész számot adjon meg)", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                TextBoxJavitasKoltseg.Focus();
                return;
            }
            if (ComboBoxJavitasJarmu.SelectedValue == null)
            {
                MessageBox.Show("Ki kell választani egy járművet a javításhoz!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                ComboBoxJavitasJarmu.Focus();
                return;
            }

            int? selectedDolgozoId = null;
            if (ComboBoxJavitasDolgozo.SelectedValue != null)
            {
                selectedDolgozoId = (int)ComboBoxJavitasDolgozo.SelectedValue;
            }

            int selectedJarmuId = (int)ComboBoxJavitasJarmu.SelectedValue;
            string megnevezes = TextBoxJavitasMegnevezes.Text;
            string leiras = TextBoxJavitasLeiras.Text;
            DateTime datum = DatePickerJavitasDatum.SelectedDate.Value;

            MySqlTransaction transaction = null;
            try
            {
                transaction = _conn.BeginTransaction();
                var cmd = _conn.CreateCommand();
                cmd.Transaction = transaction;

                int javitasId;

                if (_isAddingNewJavitas)
                {
                    cmd.CommandText = @"INSERT INTO javitasok (Megnevezes, Leiras, Koltseg, Datum) VALUES (@Megnevezes, @Leiras, @Koltseg, @Datum); SELECT LAST_INSERT_ID();";
                    cmd.Parameters.AddWithValue("@Megnevezes", megnevezes);
                    cmd.Parameters.AddWithValue("@Leiras", string.IsNullOrWhiteSpace(leiras) ? (object)DBNull.Value : leiras);
                    cmd.Parameters.AddWithValue("@Koltseg", koltseg);
                    cmd.Parameters.AddWithValue("@Datum", datum);
                    javitasId = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.Parameters.Clear();
                    cmd.CommandText = "INSERT INTO vegez (Jarmu_id, Javitas_id) VALUES (@JarmuId, @JavitasId);";
                    cmd.Parameters.AddWithValue("@JarmuId", selectedJarmuId);
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
                    cmd.Parameters.AddWithValue("@Megnevezes", megnevezes); 
                    cmd.Parameters.AddWithValue("@Leiras", string.IsNullOrWhiteSpace(leiras) ? (object)DBNull.Value : leiras);
                    cmd.Parameters.AddWithValue("@Koltseg", koltseg);
                    cmd.Parameters.AddWithValue("@Datum", datum);
                    cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM vegez WHERE Javitas_id=@JavitasId;";
                    cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.CommandText = "INSERT INTO vegez (Jarmu_id, Javitas_id) VALUES (@JarmuId, @JavitasId);";
                    cmd.Parameters.AddWithValue("@JarmuId", selectedJarmuId);
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
                        MessageBox.Show("Nem történt módosítás a javítás fő adataiban (de a kapcsolatok frissülhettek).", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                transaction.Commit();

                LoadJavitasok();
                _isAddingNewJavitas = false;
                JavitasokGrid.SelectedItem = null;
            }
            catch (MySqlException mySqlEx)
            {
                if (transaction != null) transaction.Rollback();
                MessageBox.Show($"Adatbázis hiba a mentés során: {mySqlEx.Message}\nError Number: {mySqlEx.Number}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
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

            var result = MessageBox.Show($"Biztosan törölni szeretné a javítást (ID: {javitasId}, Megnevezés: {TextBoxJavitasMegnevezes.Text})?\nFIGYELEM: Ez törli a kapcsolódó 'vegez' és 'elvegzi' bejegyzéseket is!", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                MySqlTransaction transaction = null;
                try
                {
                    transaction = _conn.BeginTransaction();
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
                        MessageBox.Show("Nem történt törlés a 'javitasok' táblában. Lehet, hogy már nem létezett?", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (MySqlException mySqlEx)
                {
                    if (transaction != null) transaction.Rollback();
                    MessageBox.Show($"Adatbázis hiba a törlés során: {mySqlEx.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    if (transaction != null) transaction.Rollback();
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearJavitasDetailFields()
        {
            TextBoxJavitasId.Text = "";
            TextBoxJavitasMegnevezes.Text = "";
            TextBoxJavitasLeiras.Text = "";
            TextBoxJavitasKoltseg.Text = "";
            DatePickerJavitasDatum.SelectedDate = null;
            ComboBoxJavitasJarmu.SelectedItem = null;
            ComboBoxJavitasDolgozo.SelectedItem = null;
        }

        #endregion CRUD Operations - Javitas (Repair)

        #region Export Operations

        private void ExportToExcel<T>(IEnumerable<T> data, string defaultFileName, Action<IXLWorksheet, IEnumerable<T>> populateWorksheet)
        {
            if (data == null || !data.Any())
            {
                MessageBox.Show("Nincs exportálható adat.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Filter = "Excel fájl (*.xlsx)|*.xlsx",
                FileName = defaultFileName
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                using (var wb = new XLWorkbook())
                {
                    var wsName = System.IO.Path.GetFileNameWithoutExtension(defaultFileName);
                    wsName = Regex.Replace(wsName, @"[\\/*?:\[\]]", "");
                    if (wsName.Length > 30) wsName = wsName.Substring(0, 30);

                    var ws = wb.Worksheets.Add(string.IsNullOrEmpty(wsName) ? "Export" : wsName);

                    populateWorksheet(ws, data);

                    ws.Columns().AdjustToContents();
                    wb.SaveAs(dlg.FileName);
                }
                MessageBox.Show("Exportálás sikeres!", "Kész", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az exportálás során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonExportExcel_Customers_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel(_ugyfelek, "Ügyfelek.xlsx", (ws, lista) =>
            {
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Név";
                ws.Cell(1, 3).Value = "Email";
                ws.Cell(1, 4).Value = "Telefon";
                ws.Cell(1, 5).Value = "Lakcím";
                ws.Row(1).Style.Font.Bold = true;

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
            });
        }

        private void ButtonExportExcel_Vehicles_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel(_jarmuvek, "Jarmuvek.xlsx", (ws, lista) =>
            {
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Rendszám";
                ws.Cell(1, 3).Value = "Alvázszám";
                ws.Cell(1, 4).Value = "Márka";
                ws.Cell(1, 5).Value = "Modell";
                ws.Cell(1, 6).Value = "Gyártási év";
                ws.Cell(1, 7).Value = "Km állás";
                ws.Cell(1, 8).Value = "Ügyfél ID";
                ws.Row(1).Style.Font.Bold = true;

                int row = 2;
                foreach (var j in lista)
                {
                    ws.Cell(row, 1).Value = j.JarmuId;
                    ws.Cell(row, 2).Value = j.Rendszam;
                    ws.Cell(row, 3).Value = j.Alvazszam;
                    ws.Cell(row, 4).Value = j.Marka;
                    ws.Cell(row, 5).Value = j.Modell;
                    ws.Cell(row, 6).Value = j.GyartasiEv;
                    ws.Cell(row, 7).Value = j.KmOraAllas;
                    ws.Cell(row, 8).Value = j.UgyfelId;
                    row++;
                }

                ws.Column(6).Style.NumberFormat.Format = "0";
                ws.Column(7).Style.NumberFormat.Format = "#,##0";
            });
        }

        private void ButtonExportExcel_Appointments_Click(object sender, RoutedEventArgs e)
        {
            var lista = IdopontokGrid.ItemsSource as IEnumerable<Idopont>;
            if (lista == null) lista = _idopontok;

            ExportToExcel(lista, "Idopontok.xlsx", (ws, data) =>
            {
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Dátum";
                ws.Cell(1, 3).Value = "Nap";
                ws.Cell(1, 4).Value = "Státusz";
                ws.Cell(1, 5).Value = "Ügyfél ID";
                ws.Cell(1, 6).Value = "Ügyfél Név";
                ws.Row(1).Style.Font.Bold = true;

                int row = 2;
                var ugyfelLookup = _ugyfelek?.ToDictionary(u => u.UgyfelId, u => u.Nev);

                foreach (var i in data)
                {
                    ws.Cell(row, 1).Value = i.IdopontId;
                    ws.Cell(row, 2).Value = i.Datum;
                    ws.Cell(row, 3).Value = i.Nap;
                    ws.Cell(row, 4).Value = i.Statusz;

                    int? foglaloUgyfelId = GetUgyfelIdForIdopont(i.IdopontId);
                    ws.Cell(row, 5).Value = foglaloUgyfelId;
                    if (foglaloUgyfelId.HasValue && ugyfelLookup != null && ugyfelLookup.TryGetValue(foglaloUgyfelId.Value, out string ugyfelNev))
                    {
                        ws.Cell(row, 6).Value = ugyfelNev;
                    }
                    else
                    {
                        ws.Cell(row, 6).Value = "-";
                    }

                    row++;
                }
                ws.Column(2).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
            });
        }

        private int? GetUgyfelIdForIdopont(int idopontId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open) return null;
            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Ugyfel_id FROM foglal WHERE Idopont_id = @IdopontId LIMIT 1;";
                cmd.Parameters.AddWithValue("@IdopontId", idopontId);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
            catch {}
            return null;
        }

        private void ButtonExportExcel_Employees_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel(_dolgozok, "Dolgozok.xlsx", (ws, lista) =>
            {
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Név";
                ws.Cell(1, 3).Value = "Beosztás";
                ws.Cell(1, 4).Value = "Email";
                ws.Cell(1, 5).Value = "Telefon";
                ws.Cell(1, 6).Value = "Lakcím";
                ws.Cell(1, 7).Value = "Szig. szám";
                ws.Row(1).Style.Font.Bold = true;

                int row = 2;
                foreach (var d in lista)
                {
                    ws.Cell(row, 1).Value = d.DolgozoId;
                    ws.Cell(row, 2).Value = d.Nev;
                    ws.Cell(row, 3).Value = d.Beosztas;
                    ws.Cell(row, 4).Value = d.EmailCim;
                    ws.Cell(row, 5).Value = d.Telefonszam;
                    ws.Cell(row, 6).Value = d.Lakcim;
                    ws.Cell(row, 7).Value = d.SzemelyazonositoIgazolvanySzam;
                    row++;
                }
            });
        }

        private void ButtonExportExcel_Repairs_Click(object sender, RoutedEventArgs e)
        {
            var lista = JavitasokGrid.ItemsSource as IEnumerable<Javitas>;
            if (lista == null) lista = _javitasok;

            var jarmuLookup = _jarmuvek?.ToDictionary(j => j.JarmuId, j => j.Rendszam);
            var dolgozoLookup = _dolgozok?.ToDictionary(d => d.DolgozoId, d => d.Nev);


            ExportToExcel(lista, "Javitasok.xlsx", (ws, data) =>
            {
                ws.Cell(1, 1).Value = "Javítás ID";
                ws.Cell(1, 2).Value = "Megnevezés";
                ws.Cell(1, 3).Value = "Leírás";
                ws.Cell(1, 4).Value = "Költség";
                ws.Cell(1, 5).Value = "Dátum";
                ws.Cell(1, 6).Value = "Jármű ID";
                ws.Cell(1, 7).Value = "Jármű Rendszám";
                ws.Cell(1, 8).Value = "Dolgozó ID";
                ws.Cell(1, 9).Value = "Dolgozó Név";
                ws.Row(1).Style.Font.Bold = true;

                int row = 2;
                foreach (var j in data)
                {
                    ws.Cell(row, 1).Value = j.JavitasId;
                    ws.Cell(row, 2).Value = j.Megnevezes;
                    ws.Cell(row, 3).Value = j.Leiras;
                    ws.Cell(row, 4).Value = j.Koltseg;
                    ws.Cell(row, 5).Value = j.Datum;
                    ws.Cell(row, 5).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";

                    int? kapcsJarmuId = GetJarmuIdForJavitas(j.JavitasId);
                    int? kapcsDolgozoId = GetDolgozoIdForJavitas(j.JavitasId);

                    ws.Cell(row, 6).Value = kapcsJarmuId;
                    if (kapcsJarmuId.HasValue && jarmuLookup != null && jarmuLookup.TryGetValue(kapcsJarmuId.Value, out string rendszam))
                    {
                        ws.Cell(row, 7).Value = rendszam;
                    }
                    else { ws.Cell(row, 7).Value = "-"; }

                    ws.Cell(row, 8).Value = kapcsDolgozoId;
                    if (kapcsDolgozoId.HasValue && dolgozoLookup != null && dolgozoLookup.TryGetValue(kapcsDolgozoId.Value, out string dolgozoNev))
                    {
                        ws.Cell(row, 9).Value = dolgozoNev;
                    }
                    else { ws.Cell(row, 9).Value = "-"; }


                    row++;
                }
                ws.Column(4).Style.NumberFormat.Format = "#,##0 Ft";
                ws.Column(5).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
            });
        }

        private int? GetJarmuIdForJavitas(int javitasId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open) return null;
            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Jarmu_id FROM vegez WHERE Javitas_id = @JavitasId LIMIT 1;";
                cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
            catch { }
            return null;
        }
        private int? GetDolgozoIdForJavitas(int javitasId)
        {
            if (_conn == null || _conn.State != ConnectionState.Open) return null;
            try
            {
                var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT Dolgozo_id FROM elvegzi WHERE Javitas_id = @JavitasId LIMIT 1;";
                cmd.Parameters.AddWithValue("@JavitasId", javitasId);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
            catch { }
            return null;
        }

        #endregion Export Operations
    }
}