using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        string connectionStringLaptop = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=LabDb1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

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

        private void B_Open_Update_Click(object sender, RoutedEventArgs e)
        {
            OpenUpdateDb();

            Debugger = false;
            Debug.IsEnabled = true;
            ListBox_Left.IsEnabled = true;
            ListBox_Right.IsEnabled = true;
        }

        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            if (rightIsSelected) DeleteRight(GetItemIdRight());

            if (leftIsSelected) DeleteLeft(GetItemIdLeft());
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (leftIsSelected) SaveChangesLeft(GetItemIdLeft());
            if (rightIsSelected) SaveChangesRight(GetItemIdRight());
        }

        public void ConnectionError(char LR, string methodName)
        {
            if (LR == 'l' || LR == 'L') ListBox_Left.Items.Add("SQL Excep L: " + $"{methodName}");
            if (LR == 'r' || LR == 'R') ListBox_Right.Items.Add("SQL Excep R: " + $"{methodName}");
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
                Save.IsEnabled = true;
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
                Save.IsEnabled = true;
            }

        }

        private void TextBox_LEngine_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsNotText(e.Text);
        }

        private void TextBox_LSeconds_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsNotText(e.Text);
        }

        private void TextBox_LType_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsNotText(e.Text);
        }

        private void TextBox_RName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsNotText(e.Text);
        }

        private void TextBox_RYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsNotText(e.Text);
        }

        public int CountLeft()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string countLeft = "select max(id) from VehiclesL";

                thisConnection.Open();

                SqlCommand countCommand = new SqlCommand(countLeft, thisConnection);

                ListCountLeft = (int)countCommand.ExecuteScalar();

            }
            catch (SqlException)
            {
                ConnectionError('L', "@TCount L");
            }

            if (CountRight() > ListCountLeft) return ListCountLeft + 1; else return ListCountLeft;
        }

        public int CountRight()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string countRight = "select max(CarId) from VehiclesR";

                thisConnection.Open();

                SqlCommand countCommand = new SqlCommand(countRight, thisConnection);

                ListCountRight = (int)countCommand.ExecuteScalar();

            }
            catch (SqlException)
            {
                ConnectionError('R', "@TCount R");
            }

            return ListCountRight;
        }

        private void DeleteLeft(int id)
        {
            SqlConnection leftConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string leftBoxQuery = $"delete From VehiclesL where Id = {(id)}";

                leftConnection.Open();

                SqlCommand command = new SqlCommand(leftBoxQuery, leftConnection);

                command.ExecuteNonQuery();

                leftConnection.Close();

                DeleteRight(id);

            }
            catch (SqlException)
            {
                ConnectionError('L', "@Delete L");
            }
        }

        private void DeleteRight(int id)
        {
            SqlConnection rightConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string rightBoxQuery = $"delete From VehiclesR where CarId = {(id)}";

                rightConnection.Open();

                SqlCommand command = new SqlCommand(rightBoxQuery, rightConnection);

                command.ExecuteNonQuery();

                rightConnection.Close();

                OpenUpdateDb();

            }
            catch (SqlException)
            {
                ConnectionError('R', "@Delete R");
            }
        }

        private int GetItemIdLeft()
        {
            SqlConnection leftConnection = new SqlConnection(connectionStringLaptop);

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
            catch (SqlException)
            {
                ConnectionError('L', "@GetId L");
            }

            return ItemIdLeft;
        }

        private int GetItemIdRight()
        {
            SqlConnection rightConnection = new SqlConnection(connectionStringLaptop);

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
            catch (SqlException)
            {
                ConnectionError('R', "@GetId R");
            }

            return ItemIdRight;
        }

        public void InsertLeft(int id)
        {
            InsertRight(id);

            try
            {
                SqlConnection leftInsertConnection = new SqlConnection(connectionStringLaptop);

                string insertLeftQuery = $"insert into VehiclesL values ('Engine', 'to100s', 'Type', {(id + 1)})";

                leftInsertConnection.Open();

                SqlCommand command = new SqlCommand(insertLeftQuery, leftInsertConnection);

                command.ExecuteNonQuery();

                leftInsertConnection.Close();

            }
            catch (SqlException)
            {
                ConnectionError('L', "@Insert L");
            }

            OpenUpdateDb();
        }

        public void InsertRight(int id)
        {
            SqlConnection rightInsertConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string insertRightQuery = $"insert into VehiclesR values ({(id + 1)}, 'Car Name', 1234)";

                rightInsertConnection.Open();

                SqlCommand command = new SqlCommand(insertRightQuery, rightInsertConnection);

                command.ExecuteNonQuery();

                rightInsertConnection.Close();

            }
            catch (SqlException)
            {
                ConnectionError('R', "@Insert R");
            }

            OpenUpdateDb();

        }

        private bool IsNotText(string s)
        {
            return !Regex.Match(s, @"^\w*$").Success;
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

        public void LeftBoxPrintSelectedItem(int id)
        {
            SqlConnection LeftBoxConnect = new SqlConnection(connectionStringLaptop);

            try
            {   // Jag googlade denna lösning

                string rightQuery = $"Select * From (Select Row_Number() Over (Order By id) As RwNr, * From VehiclesL) t2 Where RwNr = {id + 1}";

                LeftBoxConnect.Open();

                SqlCommand command = new SqlCommand(rightQuery, LeftBoxConnect);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        TextBox_LEngine.Text = (dataReader["Engine"].ToString());
                        TextBox_LSeconds.Text = (dataReader["to100"].ToString());
                        TextBox_LType.Text = (dataReader["Type"].ToString());
                    }

                }

                LeftBoxConnect.Close();

                RightBoxPrintSelectedItem(id);

            }
            catch (SqlException)
            {
                ConnectionError('L', "@Print L");
            }
        }

        public void RightBoxPrintSelectedItem(int id)
        {
            SqlConnection RightBoxConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string rightBoxQuery = $"Select * From (Select Row_Number() Over (Order By Carid) As RwNr, * From VehiclesR) t2 Where RwNr = {id + 1}";

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
            catch (SqlException)
            {
                ConnectionError('R', "@Print R");
            }
        }

        public void SaveChangesLeft(int id)
        {
            try
            {
                SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

                string updateLeft = $"update VehiclesL set Engine = '{TextBox_LEngine.Text}', to100 = '{TextBox_LSeconds.Text}',  Type = '{TextBox_LType.Text}' where id = {id}";

                thisConnection.Open();

                SqlCommand command = new SqlCommand(updateLeft, thisConnection);

                command.ExecuteNonQuery();

                thisConnection.Close();

                SaveChangesRight(id);

            }
            catch (SqlException)
            {
                ConnectionError('L', "@Save L");
            }
        }

        public void SaveChangesRight(int id)
        {
            try
            {
                SqlConnection rightSaveConnection = new SqlConnection(connectionStringLaptop);

                string updateLeft = $"update VehiclesR set Name = '{TextBox_RName.Text}', Year = '{TextBox_RYear.Text}' where CarId = {id}";

                rightSaveConnection.Open();

                SqlCommand command = new SqlCommand(updateLeft, rightSaveConnection);

                command.ExecuteNonQuery();

                rightSaveConnection.Close();

                OpenUpdateDb();

            }
            catch (SqlException)
            {
                ConnectionError('L', "@Save L");
            }
        }

        public void UpdateLeft()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                UpdateRight();

                ListBox_Left.Items.Clear();

                string updateLeft = $"select Id from VehiclesL";

                thisConnection.Open();

                SqlCommand command = new SqlCommand(updateLeft, thisConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ListBox_Left.Items.Add(dataReader["Id"].ToString());
                    }
                }

                B_Open_Update.Content = "Update";
                B_Open_Update.IsEnabled = true;

                ListBox_Left.SelectedIndex = 0;

            }
            catch (SqlException)
            {
                ConnectionError('L', "@Update L");
            }
        }

        public void UpdateRight()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                ListBox_Right.Items.Clear();

                string updateRight = $"select CarId from VehiclesR";

                thisConnection.Open();

                SqlCommand command = new SqlCommand(updateRight, thisConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ListBox_Right.Items.Add(dataReader["CarId"].ToString());
                    }
                }

                ListBox_Right.SelectedIndex = 0;

            }
            catch (SqlException)
            {
                ConnectionError('R', "@Update R");
            }

        }

    }

}
