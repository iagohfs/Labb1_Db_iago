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
        private const string connectionStringLaptop = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=LabDb1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private string update = "update";

        private bool leftIsSelected;
        private bool rightIsSelected;
        private bool Debugger;
        private static int firstCarNr = 1;

        private int ListCountRight { get; set; }
        private int ListCountLeft { get; set; }
        private int ItemIdLeft { get; set; }
        private int ItemIdRight { get; set; }
        private int TempCarNr { get; set; }
        private int TempIndexRight { get; set; }
        private int TempItemNrRight { get; set; }

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
            if (rightIsSelected) DeleteRight(GetItemNrRight());

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

            if (ListBox_Left.IsMouseOver || ListBox_Left.SelectedIndex > -1)
            {
                rightIsSelected = false;

                leftIsSelected = true;
                if (Debugger) Debug_Label.Content = "LB " + leftIsSelected.ToString() + "\nRB " + rightIsSelected.ToString();
                New_Table.IsEnabled = true;
                Sell.IsEnabled = true;
                Save.IsEnabled = true;

                Label_L_Help.Content = "Double click to create a linked car.";
            }

        }

        private void ListBox_Right_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Right.SelectedIndex >= 0)
            {
                RightBoxPrintSelectedItem(ListBox_Right.SelectedIndex);

                if (Debugger) ListBox_Right.Items.Add(GetItemIdRight());
            }

            if (ListBox_Right.IsMouseOver || ListBox_Right.SelectedIndex > -1)
            {
                leftIsSelected = false;

                rightIsSelected = true;
                if (Debugger) Debug_Label.Content = "RB " + rightIsSelected.ToString() + "\nLB " + leftIsSelected.ToString();
                New_Table.IsEnabled = false;
                Sell.IsEnabled = true;
                Save.IsEnabled = true;

                Label_L_Help.Content = "";
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
            SqlConnection countLeftConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string countLeft = "select max(id) from VehiclesL";

                countLeftConnection.Open();

                SqlCommand countCommand = new SqlCommand(countLeft, countLeftConnection);

                ListCountLeft = (int)countCommand.ExecuteScalar();

                countLeftConnection.Close();

            }
            catch (SqlException)
            {
                ConnectionError('L', "@CountLeft()");
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

                thisConnection.Close();

            }
            catch (SqlException)
            {
                ConnectionError('R', "@CountRight()");
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
                ConnectionError('L', "@DeleteLeft()");
            }
        }

        private void DeleteRight(int id)
        {
            SqlConnection rightConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string rightBoxQuery = $"delete From VehiclesR where CarNr = {(id)}";

                rightConnection.Open();

                SqlCommand command = new SqlCommand(rightBoxQuery, rightConnection);

                command.ExecuteNonQuery();

                rightConnection.Close();

                OpenUpdateDb();

            }
            catch (SqlException)
            {
                ConnectionError('R', "@DeleteRight()");
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
                ConnectionError('L', "@GetItemIdLeft()");
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

                SqlCommand command = new SqlCommand(rightBoxQuery, rightConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        TempIndexRight = (int)(dataReader["CarId"]);
                    }
                }

                rightConnection.Close();

                ItemIdRight = TempIndexRight;

            }
            catch (SqlException)
            {
                ConnectionError('R', "@GetItemIdRight()");
            }

            return ItemIdRight;
        }

        private int GetItemNrRight()
        {
            SqlConnection rightConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string rightBoxQuery = $"Select * From (Select Row_Number() Over (Order By CarId) As RwNr, * From VehiclesR) t2 Where RwNr = {ListBox_Right.SelectedIndex + 1}";

                rightConnection.Open();

                SqlCommand command = new SqlCommand(rightBoxQuery, rightConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        TempIndexRight = (int)(dataReader["CarNr"]);
                    }
                }

                rightConnection.Close();

                TempItemNrRight = TempIndexRight;

            }
            catch (SqlException)
            {
                ConnectionError('R', "@GetItemIdRight()");
            }

            return TempItemNrRight;
        }

        public int GetCarNr(int id)
        {
            SqlConnection GetRCarID = new SqlConnection(connectionStringLaptop);

            try
            {
                string countRight = $"select max(CarNr) from VehiclesR where CarId = {id + 1}";

                GetRCarID.Open();

                SqlCommand countRightCommand = new SqlCommand(countRight, GetRCarID);

                TempCarNr = (int)countRightCommand.ExecuteScalar();

                //command.Parameters.AddWithValue

                GetRCarID.Close();

            }
            catch (SqlException)
            {
                ConnectionError('L', "@GetCarNr()");
            }

            return TempCarNr;
        }

        public void InsertLeft(int id)
        {
            InsertRight(id, firstCarNr - 1);
            try
            {
                SqlConnection leftInsertConnection = new SqlConnection(connectionStringLaptop);

                string insertLeftQuery = $"insert into VehiclesL values ('Engine', 'to100s', 'Type', {(id + 1)})";

                leftInsertConnection.Open();

                SqlCommand command = new SqlCommand(insertLeftQuery, leftInsertConnection);

                //command.Parameters.AddWithValue
                command.ExecuteNonQuery();

                leftInsertConnection.Close();

            }
            catch (SqlException)
            {
                ConnectionError('L', "@InsertLeft()");
            }

            OpenUpdateDb();
        }

        public void InsertRight(int id, int carNr)
        {
            SqlConnection rightInsertConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                string insertRightQuery = $"insert into VehiclesR values ({(id + 1)}, 'Car Name', 1234, {(carNr + 1)})";

                rightInsertConnection.Open();

                SqlCommand command = new SqlCommand(insertRightQuery, rightInsertConnection);

                command.ExecuteNonQuery();

                rightInsertConnection.Close();

            }
            catch (SqlException)
            {
                ConnectionError('R', "@InsertRight()");
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
                InsertLeft(GetItemIdLeft());
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
                //InsertRight(CountRight());
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
                        TextBox_LSeconds.Text = (dataReader["to100"]).ToString();
                        TextBox_LType.Text = (dataReader["Type"].ToString());
                    }

                }

                LeftBoxConnect.Close();

                RightBoxPrintSelectedItem(id);

            }
            catch (SqlException)
            {
                ConnectionError('L', "@LeftBoxPrintSelectedItem()");
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

                RightBoxConnection.Close();
            }
            catch (SqlException)
            {
                ConnectionError('R', "@RightBoxPrintSelectedItem()");
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
                ConnectionError('L', "@SaveChangesLeft()");
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
                ConnectionError('L', "@SaveChangesRight()");
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
                        ListBox_Left.Items.Add("Engine " + dataReader["Id"].ToString() + " properties");
                    }
                }

                thisConnection.Close();

                B_Open_Update.Content = "Update";
                B_Open_Update.IsEnabled = true;

                //ListBox_Left.SelectedIndex = 0;

            }
            catch (SqlException)
            {
                ConnectionError('L', "@UpdateLeft()");
            }
        }

        public void UpdateRight()
        {
            SqlConnection thisConnection = new SqlConnection(connectionStringLaptop);

            try
            {
                ListBox_Right.Items.Clear();

                string updateRight = $"select CarId, CarNr from VehiclesR";

                thisConnection.Open();

                SqlCommand command = new SqlCommand(updateRight, thisConnection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ListBox_Right.Items.Add("Car : " + dataReader["CarNr"].ToString() + " Engine: " + dataReader["CarId"].ToString());
                    }
                }

                thisConnection.Close();

                //ListBox_Right.SelectedIndex = 0;

            }
            catch (SqlException)
            {
                ConnectionError('R', "@UpdateRight()");
            }

        }

        private void ListBox_Left_Double_Click(object sender, MouseButtonEventArgs e)
        {
            InsertRight((GetItemIdLeft() - 1), (GetCarNr(ListBox_Left.SelectedIndex)));
        }

    }

}
