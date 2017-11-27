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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Labb1_Db_iago
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Labb1_Db;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        string defaultQuery ="select VehiclesL.Name, VehiclesL.Year, VehiclesR.Engine, VehiclesR.[0 - 100], VehiclesR.Type from VehiclesL join VehiclesR on VehiclesL.Id = VehiclesR.CarId";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void B_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenLeft();
        }

        private void ListBox_Right_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Right.SelectedIndex >= 0)
            {
                RightBox(ListBox_Right.SelectedIndex);


            }
        }

        private void ListBox_Left_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Left.SelectedIndex >= 0)
            {
                LeftBox(ListBox_Left.SelectedIndex);
            }
        }

        private async void OpenLeft()
        {
            SqlConnection thisConnection = new SqlConnection(connectionString);

            try
            {
                string leftQuery = $"select count(Name) from VehiclesL";
                thisConnection.Open();

                B_Open.Content = "Updating..";
                B_Open.IsEnabled = false;
                await Task.Delay(1000);

                SqlCommand command = new SqlCommand(leftQuery, thisConnection);

                int i = (int)command.ExecuteScalar();

                ListBox_Left.Items.Clear();
                ListBox_Right.Items.Clear();

                for (int x = 0; x < i; x++)
                {
                    ListBox_Left.Items.Add("Car : " + (x + 1));
                    ListBox_Right.Items.Add("Car : " + (x + 1));
                    await Task.Delay(50);
                }

                B_Open.Content = "Update";
                B_Open.IsEnabled = true;

            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }

        }

        private async void OpenRight()
        {
            SqlConnection thisConnection = new SqlConnection(connectionString);

            try
            {
                string rightQuery = $"select count(Name) from VehiclesL";
                thisConnection.Open();

                B_Open.Content = "Updating..";
                B_Open.IsEnabled = false;
                await Task.Delay(1000);

                SqlCommand command = new SqlCommand(rightQuery, thisConnection);

                int i = (int)command.ExecuteScalar();
                
                ListBox_Right.Items.Clear();

                for (int x = 0; x < i; x++)
                {
                    ListBox_Right.Items.Add("Car : " + (x + 1));
                    await Task.Delay(50);
                }

                B_Open.Content = "Update";
                B_Open.IsEnabled = true;

            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }

        }

        string l1, l2, l3;

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection leftConnection = new SqlConnection(connectionString);

            try
            {
                string leftBoxQuery = $"delete From Vehicles where id = {ListBox_Left.SelectedIndex + 1}";

                await Task.Delay(1000);

                //SqlCommand command = new SqlCommand(leftBoxQuery, leftConnection);

                //int i = (int)command.ExecuteScalar();                
                
            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }
        }

        private void Add_Left_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection LeftBoxConnection = new SqlConnection(connectionString);

            try
            {
                int count;

                string leftAddQuery = $"insert into ";

            }
            catch (Exception)
            {

            }
        }

        public void LeftBox(int i)
        {
            SqlConnection LeftBoxConnection = new SqlConnection(connectionString);

            try
            {
                string leftBoxQuery = $"select VehiclesL.Name, VehiclesL.Year from VehiclesL join VehiclesR on VehiclesL.Id = VehiclesR.CarId where id = {i + 1}";

                LeftBoxConnection.Open();

                SqlCommand command = new SqlCommand(leftBoxQuery, LeftBoxConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {

                    while (dataReader.Read())
                    {
                        TextBox_L1.Text = (dataReader["Name"].ToString());
                        TextBox_L2.Text = (dataReader["Year"].ToString());
                    }                    
                }
            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }
        }

        public void RightBox(int i)
        {
            SqlConnection thisConnection = new SqlConnection(connectionString);

            try
            {
                string Query = $"select VehiclesR.Engine, VehiclesR.[0-100], VehiclesR.Type from VehiclesL join VehiclesR on VehiclesL.Id = VehiclesR.CarId where id = {i + 1}";

                thisConnection.Open();

                SqlCommand command = new SqlCommand(Query, thisConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {

                    while (dataReader.Read())
                    {
                        TextBox_R1.Text = (dataReader["Engine"].ToString());
                        TextBox_R2.Text = (dataReader["0-100"].ToString());
                        TextBox_R3.Text = (dataReader["Type"].ToString());
                    }

                    while (dataReader.Read())
                    {
                        l1 = (dataReader["Name"].ToString());
                        l2 = (dataReader["Engine"].ToString());
                        l3 = (dataReader["Year"].ToString());
                    }
                }
            }
            catch (Exception)
            {

                ListBox_Right.Items.Insert(0, "SQL Error.");
            }
        }

    }
}
