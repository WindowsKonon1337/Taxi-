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

namespace order
{
    public partial class RegForm : Form
    {
        private SqlConnection insertConnection;
        public RegForm()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var insertCommand = new SqlCommand("INSERT INTO [Users] (Login, Password) VALUES (@Login, @Password)", insertConnection);

            if (textBox1.Text != null && textBox2.Text != null)
            {
                try
                {
                    using (var reader = await new SqlCommand($"SELECT Login FROM [Users] WHERE Login LIKE '{textBox1.Text}'", insertConnection).
                        ExecuteReaderAsync())
                    {
                        if (reader.ReadAsync().GetAwaiter().GetResult())
                        {
                            throw new Exception("Пользователь уже существует!");
                        }

                        reader.Close();
                    }

                    insertCommand.Parameters.AddWithValue("@Login", textBox1.Text);
                    insertCommand.Parameters.AddWithValue("@Password", textBox2.Text.GetHashCode());
                    await insertCommand.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else MessageBox.Show("Нельзя оставлять поля пустыми!");

        }

        private async void RegForm_Load(object sender, EventArgs e)
        {
            insertConnection = new SqlConnection(
                ConfigurationManager.ConnectionStrings["ProjectDbConnectionString"].ConnectionString
                );

            await insertConnection.OpenAsync();
        }

        private void RegForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (insertConnection != null && insertConnection.State != ConnectionState.Closed)
                insertConnection.Close();
        }
    }
}
