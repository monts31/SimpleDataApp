using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SimpleDataApp
{
    public partial class LlenarOCancelar : Form
    {
        public LlenarOCancelar()
        {
            InitializeComponent();
        }
        private int parsedOrderID;
        /// <summary>
        /// Verifies that an order ID is present and contains valid characters.
        /// </summary>
        private bool IsOrderIDValid()
        {
            if (txtOrderID.Text == "")
            {
                MessageBox.Show("Please specify the Order ID.");
                return false;
            }
           
            else if (Regex.IsMatch(txtOrderID.Text, @"^\D*$"))  
            {
                // Show message and clear input.
                MessageBox.Show("Customer ID must contain only numbers.");
                txtOrderID.Clear();
                return false;
            }
            else
            { 
             parsedOrderID = Int32.Parse(txtOrderID.Text);
             return true;
            }
        }

        private void btnFindByOrderID_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                using (SqlConnection connection = new
                SqlConnection(Properties.Settings.Default.connString))
                {
                    // Define a t-SQL query string that has a parameter for orderID.
                    const string sql = "SELECT * FROM Sales.Orders WHERE orderID =@orderID";
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        // Define the @orderID parameter and set its value.
                        sqlCommand.Parameters.Add(new SqlParameter("@orderID",
                        SqlDbType.Int));
                        sqlCommand.Parameters["@orderID"].Value = parsedOrderID;
                        try
                        {
                            connection.Open();
                            // Run the query by calling ExecuteReader().
                            using (SqlDataReader dataReader =
                            sqlCommand.ExecuteReader())
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(dataReader);
                                this.dgvCustomerOrders.DataSource = dataTable;
                                // Close the SqlDataReader.
                                dataReader.Close();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("The requested order could not be loaded into the form.");
                        }
                        finally
                        {
                            // Close the connection.
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                // Create the connection.
                using (SqlConnection connection = new
                SqlConnection(Properties.Settings.Default.connString))
                {
                    
                using (SqlCommand sqlCommand = new

                SqlCommand("Sales.uspCancelOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                       
                        sqlCommand.Parameters.Add(new SqlParameter("@orderID",
                        SqlDbType.Int));
                        sqlCommand.Parameters["@orderID"].Value = parsedOrderID;
                        try
                        {
                            // Open the connection.
                            connection.Open();
                            // Run the command to execute the stored procedure.
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch
                        {
                            MessageBox.Show("The cancel operation was not completed.");
                        }
                        finally
                        {
                            // Close connection.
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnFillOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                // Create the connection.
                using (SqlConnection connection = new
                SqlConnection(Properties.Settings.Default.connString))
                {
                    // Create command and identify it as a stored procedure.
                    using (SqlCommand sqlCommand = new
                    SqlCommand("Sales.uspFillOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@orderID",SqlDbType.Int));
                        sqlCommand.Parameters["@orderID"].Value = parsedOrderID;
                        
                        sqlCommand.Parameters.Add(new SqlParameter("@FilledDate",
                        SqlDbType.DateTime, 8));
                        sqlCommand.Parameters["@FilledDate"].Value =
                        dtpFillDate.Value;
                        try
                        {
                            connection.Open();
                            // Execute the stored procedure.
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch
                        {
                            MessageBox.Show("The fill operation was not completed.");
                        }
                        finally
                        {
                            // Close the connection.
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
