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
        public int ItemIdLeft { get; private set; }
        public int ItemIdRight { get; private set; }

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

                if (Debugger) ListBox_Left.Items.Add(GetItemIdLeft());
            }

            if (ListBox_Left.IsMouseOver && ListBox_Left.SelectedIndex > -1)
            {
                rightIsSelected = false;

                leftIsSelected = true;
                if (Debugger) Debug_Label.Content = "LB " + leftIsSelected.ToString() + "\nRB " + rightIsSelected.ToString();
                New_Table.IsEnabled = true;
                Sell.IsEnabled = true;
            }

        }

        private void ListBox_Right_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Right.SelectedIndex >= 0)
            {
                RightBoxPrintSelectedItem(ListBox_Right.SelectedIndex);

                if (Debugger) ListBox_Right.Items.Add(GetItemIdRight());
            }

            if (ListBox_Right.IsMouseOver && ListBox_Right.SelectedIndex > -1)
            {
                leftIsSelected = false;

                rightIsSelected = true;
                if (Debugger) Debug_Label.Content = "RB " + rightIsSelected.ToString() + "\nLB " + leftIsSelected.ToString();
                New_Table.IsEnabled = true;
                Sell.IsEnabled = true;
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

        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            if (rightIsSelected) DeleteRight(GetItemIdRight());

            if (leftIsSelected) DeleteLeft(GetItemIdLeft());
        }

        private int GetItemIdLeft()
        {
            SqlConnection leftConnection = new SqlConnection(connectionStringPC);

            try
            {
                string rightBoxQuery = $"Select * From (Select Row_Number() Over (Order By Id) As RwNr, * From VehiclesL) t2 Where RwNr = {ListBox_Left.SelectedIndex + 1}";

                leftConnection.Open();

                int tempIndexLeft = 0;

                SqlCommand command = new SqlCommand(rightBoxQuery, leftConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        tempIndexLeft = (int)(dataReader["Id"]);
                    }
                }

                leftConnection.Close();

                ItemIdLeft = tempIndexLeft;

            }
            catch (Exception)
            {
                ConnectionError('L', "Id L");
            }

            return ItemIdLeft;
        }

        private int GetItemIdRight()
        {
            SqlConnection rightConnection = new SqlConnection(connectionStringPC);

            try
            {
                string rightBoxQuery = $"Select * From (Select Row_Number() Over (Order By CarId) As RwNr, * From VehiclesR) t2 Where RwNr = {ListBox_Right.SelectedIndex + 1}";

                rightConnection.Open();

                int tempIndexRight = 0;

                SqlCommand command = new SqlCommand(rightBoxQuery, rightConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        tempIndexRight = (int)(dataReader["CarId"]);
                    }
                }

                rightConnection.Close();

                ItemIdRight = tempIndexRight;

            }
            catch (Exception)
            {
                ConnectionError('R', "Id R");
            }

            return ItemIdRight;
        }

        private void DeleteLeft(int i)
        {
            SqlConnection leftConnection = new SqlConnection(connectionStringPC);

            try
            {
                string leftBoxQuery = $"delete From VehiclesL where Id = {(i)}";

                leftConnection.Open();

                SqlCommand command = new SqlCommand(leftBoxQuery, leftConnection);

                command.ExecuteNonQuery();

                leftConnection.Close();

                DeleteRight(i);

            }
            catch (Exception)
            {
                ConnectionError('L', "Delete L");
            }
        }

        private void DeleteRight(int i)
        {
            SqlConnection rightConnection = new SqlConnection(connectionStringPC);

            try
            {
                string rightBoxQuery = $"delete From VehiclesR where CarId = {(i)}";

                rightConnection.Open();

                SqlCommand command = new SqlCommand(rightBoxQuery, rightConnection);

                command.ExecuteNonQuery();

                rightConnection.Close();

                OpenUpdateDb();

            }
            catch (Exception)
            {
                ConnectionError('R', "Delete R");
            }
        }

        private void New_Table_Click(object sender, RoutedEventArgs e)
        {

            if (leftIsSelected)
            {
                LeftTable("insert");
                OpenUpdateDb();
            }

            if (rightIsSelected)
            {
                RightTable("insert");
                OpenUpdateDb();
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
                ConnectionError('R', "Print R");
            }
        }

        public void LeftBoxPrintSelectedItem(int i)
        {
            SqlConnection LeftBoxConnect = new SqlConnection(connectionStringPC);

            try
            {   // Jag googlade denna lösning

                string rightQuery = $"Select * From (Select Row_Number() Over (Order By id) As RwNr, * From VehiclesL) t2 Where RwNr = {i + 1}";

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

                LeftBoxConnect.Close();

                RightBoxPrintSelectedItem(i);

            }
            catch (Exception)
            {
                ConnectionError('L', "Print L");
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

            }
            catch (Exception)
            {
                ConnectionError('R', "Insert R");
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
                ConnectionError('L', "Insert L");
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
                ConnectionError('R', "TCount R");
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
                ConnectionError('L', "TCount L");
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
                ConnectionError('R', "Update R");
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
                ConnectionError('L', "Update L");
            }
        }

        public void ConnectionError(char LR, string methodName)
        {
            if (LR == 'l' || LR == 'L') ListBox_Left.Items.Add("SQL Error L: " + $"{methodName}");
            if (LR == 'r' || LR == 'R') ListBox_Right.Items.Add("SQL Error R: " + $"{methodName}");
        }
    }

}
