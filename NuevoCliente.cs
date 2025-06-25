using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleDataApp
{
    public partial class NuevoCliente : Form
    {
        private int parsedCustomerID;
        private int orderID;
        
        private bool IsCustomerNameValid()
        {
            if (txtCustomerName.Text == "")
            {
                MessageBox.Show("Please enter a name.");
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool IsOrderDataValid()
        {
            // Verify that CustomerID is present.
            if (txtCustomerID.Text == "")
            {
                MessageBox.Show("Please create customer account before placing order.");
            return false;
            }
            // Verify that Amount isn't 0.
            else if ((numOrderAmount.Value < 1))
            {
                MessageBox.Show("Please specify an order amount.");
                return false;
            }
            else
            {
                // Order can be submitted.
                return true;
            }
        }
        private void ClearForm()
        {
            txtCustomerName.Clear();
            txtCustomerID.Clear();
            dtpOrderDate.Value = DateTime.Now;
            numOrderAmount.Value = 0;
            this.parsedCustomerID = 0;
        }
        public NuevoCliente()
        {
            InitializeComponent();
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (IsCustomerNameValid())
            {
                // Create the connection.
                using (SqlConnection connection = new
                SqlConnection(Properties.Settings.Default.connString))
                {
                    // Create a SqlCommand, and identify it as a stored procedure.
                    using (SqlCommand sqlCommand = new
                    SqlCommand("Sales.uspNewCustomer", connection))
                    {

                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerName",
                        SqlDbType.NVarChar, 40));
                        sqlCommand.Parameters["@CustomerName"].Value =
                        txtCustomerName.Text;
                        // Add the output parameter.
                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerID",
                        SqlDbType.Int));
                        sqlCommand.Parameters["@CustomerID"].Direction =
                        ParameterDirection.Output;
                        try
                        {
                            connection.Open();
                            // Run the stored procedure.
                            sqlCommand.ExecuteNonQuery();
                            // Customer ID is an IDENTITY value from the database.
                            this.parsedCustomerID =
                            (int)sqlCommand.Parameters["@CustomerID"].Value;
                            this.txtCustomerID.Text =
                            Convert.ToString(parsedCustomerID);
                        }
                        catch
                        {
                            MessageBox.Show("Customer ID was not returned. Account could not be created.");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderDataValid())
            {
                // Create the connection.
                using (SqlConnection connection = new
                SqlConnection(Properties.Settings.Default.connString))
                {
                    // Create SqlCommand and identify it as a stored procedure.
                    using (SqlCommand sqlCommand = new
                    SqlCommand("Sales.uspPlaceNewOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerID",SqlDbType.Int));
                        sqlCommand.Parameters["@CustomerID"].Value =
                        this.parsedCustomerID;
                        
                        sqlCommand.Parameters.Add(new SqlParameter("@OrderDate",
                        SqlDbType.DateTime, 8));
                        sqlCommand.Parameters["@OrderDate"].Value =
                        dtpOrderDate.Value;
                       
                        sqlCommand.Parameters.Add(new SqlParameter("@Amount",
                        SqlDbType.Int));
                        sqlCommand.Parameters["@Amount"].Value =
                        numOrderAmount.Value;
                        
                        sqlCommand.Parameters.Add(new SqlParameter("@Status",
                        SqlDbType.Char, 1));
                        sqlCommand.Parameters["@Status"].Value = "O";
                        sqlCommand.Parameters.Add(new SqlParameter("@RC",
                        SqlDbType.Int));
                        sqlCommand.Parameters["@RC"].Direction =
                        ParameterDirection.ReturnValue;
                        try
                        {
                            //Open connection.
                            connection.Open();
                            // Run the stored procedure.
                            sqlCommand.ExecuteNonQuery();
                            // Display the order number.
                            this.orderID = (int)sqlCommand.Parameters["@RC"].Value;
                            MessageBox.Show("Order number " + this.orderID + " has been submitted.");
                        } 
                        catch
                        {
                            MessageBox.Show("Order could not be placed.");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnAddAnotherAccount_Click(object sender, EventArgs e)
        {
            this.ClearForm();
        }

        private void btnAddFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
