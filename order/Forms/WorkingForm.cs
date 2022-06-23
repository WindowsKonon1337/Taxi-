using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using order.Models;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace order
{
    public partial class WorkingForm : Form
    {
        private UserShowFormModel user;

        private readonly decimal MiddleSalary = 65000m;

        private static List<CarListModel> carListModel = new List<CarListModel>();

        private SqlConnection sqlConnection;
        public WorkingForm(UserShowFormModel user)
        {
            this.user = user;
            InitializeComponent();
        }

        private async void WorkingForm_Load(object sender, EventArgs e)
        {
            carsTableAdapter.Fill(carsDataSet.Cars);
            textBox1.Text = $"{user.Id} | {user.login}";

            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectDbConnectionString"].ConnectionString);

            await sqlConnection.OpenAsync();

            using (var reader = await new SqlCommand("SELECT * FROM [Cars]", sqlConnection).ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    carListModel.Add(new CarListModel()
                    {
                        Model = reader.GetString(0),
                        Cost = reader.GetDecimal(1),
                        CosPerMounth = reader.GetDecimal(2),
                        Fuel = reader.GetDecimal(3),
                    });
                }

                reader.Close();
            }
        }

        private void WorkingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                sqlConnection.Close();

            Process.GetCurrentProcess().Kill(); // это подстраховка, чтобы hide-окно точно закрыллось
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var instant = 0m;

            var mounthly = 0m;

            int profit = 0;

            int val = 0;

            textBox4.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                if (dataGridView2.Rows[i].Cells[1].Value != null && Convert.ToInt32(dataGridView2.Rows[i].Cells[1].Value) > 0)
                {
                    instant += carListModel.FirstOrDefault(x => x.Model == (string)dataGridView2.Rows[i].Cells[0].Value).Cost
                        * Convert.ToInt32(dataGridView2.Rows[i].Cells[1].Value);

                    mounthly += carListModel.FirstOrDefault(x => x.Model == (string)dataGridView2.Rows[i].Cells[0].Value).CosPerMounth
                        * Convert.ToInt32(dataGridView2.Rows[i].Cells[1].Value)
                            + carListModel.FirstOrDefault(x => x.Model == (string)dataGridView2.Rows[i].Cells[0].Value).Fuel * 10;

                    val += Convert.ToInt32(dataGridView2.Rows[i].Cells[1].Value);
                }
            }

            profit = instant != 0 ? (int)Math.Round((instant + mounthly) / (MiddleSalary * val)) + 3 : 0;

            textBox4.Text = profit.ToString();
            textBox2.Text = instant.ToString();
            textBox3.Text = mounthly.ToString();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
           var carList = new List<CarCSVModel>();

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                if (dataGridView2.Rows[i].Cells[1].Value != null && Convert.ToInt32(dataGridView2.Rows[i].Cells[1].Value) > 0)
                {
                    carList.Add(
                        new CarCSVModel()
                        {
                            Model = (string)dataGridView2.Rows[i].Cells[0].Value,
                            value = Convert.ToInt32(dataGridView2.Rows[i].Cells[1].Value)
                        });
                }
            }

            var configPersons = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            using (var writer = new StreamWriter("data.csv"))
            using (var csv = new CsvWriter(writer, configPersons))
            {
                await csv.WriteRecordsAsync(carList);
                var info = new List<CostCsvDataModel> {new CostCsvDataModel(textBox2.Text, textBox3.Text, textBox4.Text)};
                await csv.WriteRecordsAsync((System.Collections.IEnumerable)info);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
