using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADOCRUD.ObjectClassGenerator
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            lblResults.Visible = false;
            txtOutput.Visible = false;
        }

        private void btnGenerateObjects_Click(object sender, EventArgs e)
        {
            try
            {
                string connString = String.Format(@"Data Source={0};Initial Catalog={1};uid={2};password={3}", txtDataSource.Text, txtDatabaseName.Text, txtUserId.Text, txtPassword.Text);

                Dictionary<string, string> allTableNames = new Dictionary<string, string>();
                DataSet ds = new DataSet();

                // Opens sql connection to grab all the tables and their information from the database
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    DataTable dbInfo = conn.GetSchema("tables");

                    for (int i = 0; i < dbInfo.Rows.Count; i++)
                        allTableNames.Add(dbInfo.Rows[i].ItemArray[2].ToString(), dbInfo.Rows[i].ItemArray[1].ToString());


                    foreach (KeyValuePair<string, string> pair in allTableNames)
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter("select top 1 * from " + pair.Value + ".[" + pair.Key + "]", conn);
                        DataTable dt = new DataTable();
                        adapter.FillSchema(dt, SchemaType.Source);
                        adapter.Fill(dt);
                        dt.Prefix = pair.Value;
                        dt.TableName = pair.Key;

                        ds.Tables.Add(dt);
                    }

                    conn.Close();
                }

                if (ds != null && ds.Tables != null)
                {
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        System.IO.File.WriteAllText(txtOutputPath.Text + "\\" + ds.Tables[i].TableName + ".cs", Generator.CreateClass(ds.Tables[i], txtNamespace.Text));
                    }
                }
            }
            catch (Exception ex)
            {
                lblResults.Visible = true;
                txtOutput.Visible = true;
                txtOutput.Text = ex.Message;
                return;
            }

            lblResults.Visible = true;
            txtOutput.Visible = true;
            txtOutput.Text = "Class generation successful. Please go to " + txtOutputPath.Text + " to view your objects.";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dialog = fBrowser.ShowDialog();

            if (dialog == DialogResult.OK)
                txtOutputPath.Text = fBrowser.SelectedPath;
        }
    }
}
