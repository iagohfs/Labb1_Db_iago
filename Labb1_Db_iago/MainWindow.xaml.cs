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
        string connectionStringPC = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LabDb1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        /* string connectionStringLaptop = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=TestDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";*/

        // Default Query "select VehiclesL.Name, VehiclesL.Year, VehiclesR.Engine, VehiclesR.[0 - 100], VehiclesR.Type from VehiclesL join VehiclesR on VehiclesL.Id = VehiclesR.CarId";

        string l1, l2, l3;

        public string update = "update";
        private bool leftIsSelected;
        private bool rightIsSelected;
        private bool Debugger;
        int ListCountRight { get; set; }
        int ListCountLeft { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void B_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenUpdateDb();

            Debugger = false;
            Debug.IsEnabled = true;
            ListBox_Left.IsEnabled = true;
            ListBox_Right.IsEnabled = true;
        }

        private void ListBox_Left_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ListBox_Left.SelectedIndex > -1)
            {
                LeftBoxPrintSelectedItem(ListBox_Left.SelectedIndex);
            }

            if (ListBox_Left.IsMouseOver && ListBox_Left.SelectedIndex > -1)
            {
                rightIsSelected = false;

                leftIsSelected = true;
                if (Debugger) Debug_Label.Content = "LB " + leftIsSelected.ToString() + "\nRB " + rightIsSelected.ToString();
                New_Table.IsEnabled = true;
                Delete.IsEnabled = true;
            }

        }

        private void ListBox_Right_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Right.SelectedIndex >= 0)
            {
                RightBoxPrintSelectedItem(ListBox_Right.SelectedIndex);
            }

            if (ListBox_Right.IsMouseOver && ListBox_Right.SelectedIndex > -1)
            {
                leftIsSelected = false;

                rightIsSelected = true;
                if (Debugger) Debug_Label.Content = "RB " + rightIsSelected.ToString() + "\nLB " + leftIsSelected.ToString();
                New_Table.IsEnabled = true;
                Delete.IsEnabled = true;
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
                InsertLeft(CountLeft());
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
                InsertRight(CountRight());
            }

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (rightIsSelected) DeleteRight(ListBox_Right.SelectedIndex);

            if (leftIsSelected) DeleteLeft(ListBox_Left.SelectedIndex);

        }

        private void DeleteLeft(int i)
        {
            SqlConnection leftConnection = new SqlConnection(connectionStringPC);

            try
            {
                string leftBoxQuery = $"delete From VehiclesL where Id = {(i + 1)}";

                leftConnection.Open();

                SqlCommand command = new SqlCommand(leftBoxQuery, leftConnection);

                command.ExecuteNonQuery();

                leftConnection.Close();

                DeleteRight(i);

            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }
        }

        private void DeleteRight(int i)
        {
            SqlConnection rightConnection = new SqlConnection(connectionStringPC);

            try
            {
                string rightBoxQuery = $"delete From VehiclesR where CarId = {(i + 1)}";

                rightConnection.Open();

                SqlCommand command = new SqlCommand(rightBoxQuery, rightConnection);

                command.ExecuteNonQuery();

                rightConnection.Close();

                OpenUpdateDb();

            }
            catch (Exception)
            {
                ListBox_Left.Items.Insert(0, "SQL Error.");
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }
        }

        private void New_Table_Click(object sender, RoutedEventArgs e)
        {

            if (leftIsSelected)
            {
                LeftTable("insert");
            }

            if (rightIsSelected)
            {
                RightTable("insert");
            }
        }

        public void RightBoxPrintSelectedItem(int i)
        {
            SqlConnection RightBoxConnection = new SqlConnection(connectionStringPC);

            try
            {
                string rightBoxQuery = $"Select * From (Select Row_Number() Over (Order By Carid) As RwNr, * From VehiclesR) t2 Where RwNr = {i + 1}";

                RightBoxConnection.Open();

                SqlCommand command = new SqlCommand(rightBoxQuery, RightBoxConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {

                    while (dataReader.Read())
                    {
                        TextBox_RName.Text = (dataReader["Name"].ToString());
                        TextBox_RYear.Text = (dataReader["Year"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                ListBox_Right.Items.Insert(0, "SQL Error.");
            }
        }

        public void LeftBoxPrintSelectedItem(int i)
        {
            SqlConnection LeftBoxConnect = new SqlConnection(connectionStringPC);

            try
            {
                string rightQuery = $"Select * From (Select Row_Number() Over (Order By id) As RwNr, * From VehiclesL) t2 Where RwNr = 1 {i + 1}";

                LeftBoxConnect.Open();

                SqlCommand command = new SqlCommand(rightQuery, LeftBoxConnect);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {

                    while (dataReader.Read())
                    {
                        TextBox_LEngine.Text = (dataReader["Engine"].ToString());
                        TextBox_LSeconds.Text = (dataReader["0-100"].ToString());
                        TextBox_LType.Text = (dataReader["Type"].ToString());                       
                    }

                }

            }
            catch (Exception)
            {

                ListBox_Left.Items.Insert(0, "SQL Error.");
            }
        }

        public void InsertRight(int i)
        {
            SqlConnection rightInsertConnection = new SqlConnection(connectionStringPC);

            try
            {
                string insertRightQuery = $"insert into VehiclesR values ({(i + 1)}, 'Name', 1234)";

                rightInsertConnection.Open();

                SqlCommand command = new SqlCommand(insertRightQuery, rightInsertConnection);

                command.ExecuteNonQuery();

                rightInsertConnection.Close();

                OpenUpdateDb();
            }
            catch (Exception)
            {
                ConnectionError('R');
            }

        }

        public void InsertLeft(int i)
        {
            try
            {

                SqlConnection leftInsertConnection = new SqlConnection(connectionStringPC);

                string insertLeftQuery = $"insert into VehiclesL values ('Engine', '0-100s', 'Type', {(i + 1)})";

                leftInsertConnection.Open();

                SqlCommand command = new SqlCommand(insertLeftQuery, leftInsertConnection);

                command.ExecuteNonQuery();

                leftInsertConnection.Close();

                InsertRight(i);

            }
            catch (SqlException)
            {
                ConnectionError('L');
            }
        }

        public int CountRight()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringPC);

            try
            {
                string countRight = "select count(CarId) from VehiclesR";

                thisConnection.Open();

                SqlCommand countCommand = new SqlCommand(countRight, thisConnection);

                ListCountRight = (int)countCommand.ExecuteScalar();

            }
            catch (Exception)
            {
                ConnectionError('R');
            }

            return ListCountRight;
        }

        public int CountLeft()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringPC);

            try
            {
                string countLeft = "select count(id) from VehiclesL ";

                thisConnection.Open();

                SqlCommand countCommand = new SqlCommand(countLeft, thisConnection);

                ListCountLeft = (int)countCommand.ExecuteScalar();

            }
            catch (Exception)
            {
                ConnectionError('L');
            }

            if (CountRight() > ListCountLeft) return ListCountLeft + 1; else return ListCountLeft;
        }

        private void Debug_Click(object sender, RoutedEventArgs e)
        {

            if (Debugger)
            {
                Debugger = false;
                Debug.Content = "Debug is Off";
                Debug_Label.Content = "";
            }
            else
            {
                Debugger = true;
                Debug.Content = "Debug is On";
            }

        }

        public void UpdateRight()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringPC);

            try
            {
                string updateRight = $"select CarId from VehiclesR";

                thisConnection.Open();

                SqlCommand command = new SqlCommand(updateRight, thisConnection);

                ListBox_Right.Items.Clear();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ListBox_Right.Items.Add(dataReader["CarId"].ToString());
                    }
                }

                ListBox_Right.SelectedIndex = 0;

            }
            catch (Exception)
            {
                ConnectionError('R');
            }

        }

        public void UpdateLeft()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringPC);

            try
            {
                string updateLeft = $"select Id from VehiclesL";

                thisConnection.Open();

                ListBox_Left.Items.Clear();                

                SqlCommand command = new SqlCommand(updateLeft, thisConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ListBox_Left.Items.Add(dataReader["Id"].ToString());
                    }
                }

                B_Open.Content = "Update";
                B_Open.IsEnabled = true;

                ListBox_Left.SelectedIndex = 0;

            }
            catch (Exception)
            {
                ConnectionError('L');
            }
        }

        public void ConnectionError(char LR)
        {
            if (LR == 'l' || LR == 'L') ListBox_Left.Items.Insert(0, "SQL Error.");
            if (LR == 'r' || LR == 'R') ListBox_Right.Items.Insert(0, "SQL Error.");
        }
    }

}
