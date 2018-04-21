using System;
using System.Collections.Generic;
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
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".txt";
            dlg.Filter = "TXT files|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                TextBox.Text = filename;
                listbox1.Items.Clear();
                listbox2.Items.Clear();

                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Cy\\source\\repos\\WpfApp1\\WpfApp1\\podatoci.mdf;Integrated Security=True";
                string insertString = "INSERT into info(datetime, filename) VALUES(@datetime, @filename)";
                string select = "SELECT * FROM info";


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(insertString, connection);

                    command.Parameters.Add("@filename", SqlDbType.VarChar);
                    command.Parameters["@filename"].Value = System.IO.Path.GetFileName(filename);

                    command.Parameters.Add("@datetime", SqlDbType.DateTime);
                    command.Parameters["@datetime"].Value = DateTime.UtcNow;

                    connection.Open();

                    command.ExecuteNonQuery();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(select, connectionString);
                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                    DataTable dt = new DataTable("info");
                    dataAdapter.Fill(dt);
                    grid.ItemsSource = dt.DefaultView;
                }

                using (StreamReader r = new StreamReader(filename))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        listbox1.Items.Add(line);

                        int[] unsortedarray = line.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

                        Quicksort(unsortedarray, 0, unsortedarray.Length - 1);

                        string sortedarray = string.Join(",", unsortedarray);

                        listbox2.Items.Add(sortedarray);
                    }
                }
            }   
        }

        public static void Quicksort(int[] elements, int left, int right)
        {
            int i = left, j = right;
            int pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i].CompareTo(pivot) < 0)
                {
                    i++;
                }

                while (elements[j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    int tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;
                    i++;
                    j--;
                }
            }

            if (left < j)
            {
                Quicksort(elements, left, j);
            }

            if (i < right)
            {
                Quicksort(elements, i, right);
            }
        } 
    }
}
