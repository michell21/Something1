using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = "Server=stud-mssql.sttec.yar.ru,38325;Database=user43_db;User Id=user43_db;Password=user43;";
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            // Вызываем фильтрацию при создании окна
        }
        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Запрос на выборку всех данных из таблицы
                    string query = "SELECT * FROM [09Pacient]";
                    dataAdapter = new SqlDataAdapter(query, connection);
                    dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // Привязка данных к DataGrid
                    dataGrid.ItemsSource = dataTable.DefaultView;

                    // Вызываем фильтрацию при загрузке данных

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Ваши TextBox'ы должны иметь имена, например: textBox1, textBox2, ..., textBox5
                    string F = textBox1.Text;
                    string I = textBox2.Text;
                    string O = textBox3.Text;
                    string bolezn = textBox4.Text;
                    string id_vracha = combobox1.Text;

                    // Вставка новой записи в базу данных
                    string insertQuery = "INSERT INTO [09Pacient] (F, I, O, bolezn, id_vracha) VALUES (@F, @I, @O, @bolezn, @id_vracha)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@F", F);
                    insertCommand.Parameters.AddWithValue("@I", I);
                    insertCommand.Parameters.AddWithValue("@O", O);
                    insertCommand.Parameters.AddWithValue("@bolezn", bolezn);
                    insertCommand.Parameters.AddWithValue("@id_vracha", id_vracha);
                    // Здесь вы можете добавить параметр для изображения, если это необходимо

                    insertCommand.ExecuteNonQuery();

                    // Загрузка обновленных данных
                    LoadData();

                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    MessageBox.Show("Запись добавлена.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string strConn = @"data source=stud-mssql.sttec.yar.ru,38325;initial catalog=user43_db;persist security info=True;user id=user43_db;password=user43;MultipleActiveResultSets=True;App=EntityFramework";
            SqlConnection sqlConnect = new SqlConnection(strConn);
            sqlConnect.Open();
            string strAll1 = "update [09Pacient] set F='" + this.textBox1.Text + "',I='" + this.textBox2.Text + "',O='" + this.textBox3.Text + "',bolezn='" + this.textBox4.Text + "',id_vracha='" + this.combobox1.Text + "'";

            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0 || textBox3.Text.Length == 0 || textBox4.Text.Length == 0 || combobox1.Text.Length == 0)
                MessageBox.Show("Строка успешно изменена");
            else
            {
                string id_pacienta = textBox5.Text;
                string F = textBox1.Text;
                string I = textBox2.Text;
                string O = textBox3.Text;
                string bolezn = textBox4.Text;
                string id_vracha = combobox1.Text;
                SqlCommand com2 = new SqlCommand(strAll1, sqlConnect);
                com2.ExecuteNonQuery();
                MessageBox.Show("Строка успешно изменена!");


                // Загрузка обновленных данных
                LoadData();

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                MessageBox.Show("Запись изменена!.");
            }
        }

        private void UpdateDataGrid()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Здесь предполагается, что у вас есть dataAdapter и dataTable,
                // которые связаны с вашей таблицей данных
                SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM [09Pacient]", connection);
                DataTable dataTable = new DataTable();

                // Заполняем dataTable данными из базы данных
                dataAdapter.Fill(dataTable);

                // Обновляем источник данных для dataGrid
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                string artikulToDelete = selectedRow["id_pacienta"].ToString();

                // Просим пользователя подтвердить операцию
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    string deleteQuery = "DELETE FROM [09Pacient] WHERE id_pacienta = @id_pacienta";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@id_pacienta", artikulToDelete);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Обновляем отображение данных в dataGrid
                    UpdateDataGrid();
                    MessageBox.Show("Запись успешно удалена!");
                }
            }
        }

        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Только буквы
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void textBox5_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Только числа
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
