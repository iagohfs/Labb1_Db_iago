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
        /* string connectionStringPC = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Labb1_Db;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"; */

        string connectionStringLaptop = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=TestDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // Default Query "select VehiclesL.Name, VehiclesL.Year, VehiclesR.Engine, VehiclesR.[0 - 100], VehiclesR.Type from VehiclesL join VehiclesR on VehiclesL.Id = VehiclesR.CarId";

        string l1, l2, l3;

        public string update = "update";
        private bool leftIsSelected;
        private bool rightIsSelected;
        private bool Debugger;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void B_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenUpdateDb();
            Add_Left.IsEnabled = true;
            Debugger = false;
            Debug.IsEnabled = true;
        }

        private void ListBox_Right_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ListBox_Right.SelectedIndex >= 0)
            {
                RightBoxItemSelectedProperties(ListBox_Right.SelectedIndex);
            }

            if (ListBox_Right.IsMouseOver && ListBox_Right.SelectedIndex > -1)
            {
                leftIsSelected = false;


                rightIsSelected = true;
                if (Debugger) Debug_L.Content = "RBSlctd: " + rightIsSelected.ToString();
            }

        }

        private void ListBox_Left_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ListBox_Left.SelectedIndex >= 0)
            {
                LeftBoxItemSelectedProperties(ListBox_Left.SelectedIndex);
            }

            if (ListBox_Left.IsMouseOver && ListBox_Left.SelectedIndex > -1)
            {
                rightIsSelected = false;


                leftIsSelected = true;
                if (Debugger) Debug_L.Content = "LBSlctd: " + leftIsSelected.ToString();
            }

        }

        private void OpenUpdateDb()
        {
            LeftTable(update);
            RightTable(update);
        }

        private void LeftTable(string commandL)
        {
            if (commandL == "update")
            {
                UpdateLeft();

            }
            else if (commandL == "insert")
            {
                InserLeft(CountLeft());
            }

        }

        private void RightTable(string commandR)
        {

            if (commandR == "update")
            {
                UpdateRight();
            }
            else if (commandR == "insert")
            {
                InsertRight(CounterRight());
            }

        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection leftConnection = new SqlConnection(connectionStringLaptop);

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

            if (true)
            {
                //LeftTable("insert");
                //RightTable("insert");
                //ListBox_Left.Items.Add("testing add");

            }
        }

        public void LeftBoxItemSelectedProperties(int i)
        {
            SqlConnection LeftBoxConnection = new SqlConnection(connectionStringLaptop);

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

        public void RightBoxItemSelectedProperties(int i)
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

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

        public void InserLeft(int i)
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string insertLeft = $"insert into VehiclesL values({i + 1}, 'name', 404)";

                thisConnection.Open();

                SqlCommand command = new SqlCommand(insertLeft, thisConnection);

            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }

        }

        public void InsertRight(int i)
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string insertLeft = $"insert into VehiclesR values('Engine', '0-100s', 'Type', {i + 1})";

                thisConnection.Open();

                SqlCommand command = new SqlCommand(insertLeft, thisConnection);

            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }
        }

        public int CounterRight()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

            int toReturn = 0;

            try
            {
                string countLeft = "select count(id) from VehiclesR ";

                thisConnection.Open();

                SqlCommand countCommand = new SqlCommand(countLeft, thisConnection);

                int i = (int)countCommand.ExecuteScalar();

                toReturn = i;

            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }

            return toReturn;
        }

        public int CountLeft()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);
            int toReturn = 0;

            try
            {
                string countLeft = "select count(id) from VehiclesL ";

                thisConnection.Open();

                SqlCommand countCommand = new SqlCommand(countLeft, thisConnection);

                int i = (int)countCommand.ExecuteScalar();

                toReturn = i;

            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }

            return toReturn;
        }

        private void Debug_Click(object sender, RoutedEventArgs e)
        {

            if (Debugger)
            {
                Debugger = false;
                Debug.Content = "Debug is Off";
                Debug_L.Content = "";
            }
            else
            {
                Debugger = true;
                Debug.Content = "Debug is On";
            }

        }

        public async void UpdateLeft()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string updateLeft = $"select count(Name) from VehiclesL";
                thisConnection.Open();

                B_Open.Content = "Updating..";
                B_Open.IsEnabled = false;
                await Task.Delay(1000);

                SqlCommand command = new SqlCommand(updateLeft, thisConnection);

                int i = (int)command.ExecuteScalar();

                ListBox_Left.Items.Clear();

                for (int x = 0; x < i; x++)
                {
                    ListBox_Left.Items.Add("Car : " + (x + 1));
                    await Task.Delay(50);
                }

                B_Open.Content = "Update";
                B_Open.IsEnabled = true;

                ListBox_Left.SelectedIndex = 0;

            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }
        }

        public async void UpdateRight()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string updateRight = $"select count(Name) from VehiclesL";

                thisConnection.Open();

                await Task.Delay(1000);

                SqlCommand command = new SqlCommand(updateRight, thisConnection);

                int i = (int)command.ExecuteScalar();

                ListBox_Right.Items.Clear();

                for (int x = 0; x < i; x++)
                {
                    ListBox_Right.Items.Add("Car : " + (x + 1));
                    await Task.Delay(50);
                }

                ListBox_Right.SelectedIndex = 0;

            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }
        }
    }

}
