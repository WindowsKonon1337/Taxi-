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
using System.Configuration;
using order.Models;

namespace order
{
    public partial class LoginForm : Form
    {
        private SqlConnection sqlConnection;

        private UserShowFormModel user = null;
        public LoginForm()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            SqlDataReader reader = null;

            try
            {
                reader = await new SqlCommand($"SELECT * FROM [Users] WHERE Login LIKE '{textBox1.Text}' AND Password LIKE '{textBox2.Text.GetHashCode()}'", sqlConnection).
                    ExecuteReaderAsync();

                if (!reader.ReadAsync().GetAwaiter().GetResult())
                    throw new Exception("Пользователь не найден!");
                else
                {
                    user = new UserShowFormModel
                    {
                        Id = Convert.ToString(reader["Id"]),
                        login = Convert.ToString(reader["Login"])
                    };
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            if (user != null)
            {
                var mainForm = new WorkingForm(user);
                mainForm.Show();
                Hide();
            }

        }

        private async void LoginForm_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(
                ConfigurationManager.ConnectionStrings["ProjectDbConnectionString"].ConnectionString
                );

            await sqlConnection.OpenAsync();

        }

        public void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                sqlConnection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var regForm = new RegForm();
            regForm.Show();
        }
    }
}
