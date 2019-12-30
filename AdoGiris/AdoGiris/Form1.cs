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
using System.Configuration;


namespace AdoGiris
{
    /* ADO.NET = Active Data Object 
       Veritabanı ile c# dilini konuşturmak için kullanılan teknolojilerden birisidir. 
       1) Connected (Bağlantılı model)
       2)Disconnected( Bağlantısız model) (bu ikisini araştır!!)
                
        DataSource--> DataProvider--> DataSet (Bu üç yapı ADO.NETi oluşturur)
        -->ASP.NET | -->WindowsForm |--> Web services |-->Security etc. yerlere bilgi yollar.
    */
    public partial class Form1 : Form
    {
        SqlCommand uygula;
        SqlDataReader oku;
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CON"].ToString());
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ComboDoldur();
          Listele();
        }

        private void Listele()
        {
            Liste.Rows.Clear();
            int i = 0;
            conn.Open();
            uygula = new SqlCommand("select*from tblDoktors",conn);
            SqlDataReader oku = null;
            oku = uygula.ExecuteReader();
            foreach (var k in oku)
            {
                Liste.Rows.Add();
                Liste.Rows[i].Cells[0].Value = oku["Id"].ToString();
                Liste.Rows[i].Cells[1].Value = oku["Unvan"].ToString();
                Liste.Rows[i].Cells[2].Value = oku["DoktorAdi"].ToString();
                Liste.Rows[i].Cells[3].Value = oku["SehirId"].ToString();
                i++;
            }
            Liste.AllowUserToAddRows = false;
            oku.Close();
            conn.Close();
        }

        private void Labelistele()
        {
            //hatalı
        }

        private void ComboDoldur()
        {
            txtSehir.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection veri = new AutoCompleteStringCollection();
            string query = "select*from iller";
            uygula = new SqlCommand(query, conn);
            uygula.Connection.Open();
            oku = uygula.ExecuteReader(CommandBehavior.CloseConnection);
            while(oku.Read())
            {
                if(txtSehir.Items.Contains(oku.GetString(1).ToString())!=true)
                {
                    veri.Add(oku.GetString(1));
                    txtSehir.Items.Add(oku.GetString(1));
                }
            }
            conn.Close();
            oku.Close();
            txtSehir.AutoCompleteCustomSource = veri;
        }

        private void btnKayit_Click(object sender, EventArgs e)
        {
            YeniKaydet();
        }

        private void YeniKaydet()
        {
            uygula = new SqlCommand();
            uygula.CommandType = CommandType.StoredProcedure;
            uygula.CommandText="[SE_Doktor]";
            uygula.Connection = conn;
            uygula.Connection.Open();

            uygula.Parameters.Add("@DoktorAdi", SqlDbType.NVarChar, 50).Value = txtAd.Text;
            uygula.Parameters.Add("@SehirId", SqlDbType.Int).Value = txtSehirId.Text;
            uygula.Parameters.Add("@Unvan", SqlDbType.NVarChar, 50).Value = txtUnvan.Text;
            uygula.ExecuteNonQuery();
            uygula.Connection.Close();
            conn.Close();
            MessageBox.Show("Kayıt Başarılı");
            Listele();
        }

        private void txtSehir_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = $"select il_no from iller where isim='{txtSehir.Text}'";
            uygula = new SqlCommand(query);
            uygula.Connection = conn;
            uygula.Connection.Open();
            txtSehirId.Text = uygula.ExecuteScalar().ToString();
            uygula.Connection.Close();

            txtilceler.Items.Clear();
            txtilceler.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection veri = new AutoCompleteStringCollection();
            string query1 =$"select*from ilceler where il_no='{txtSehirId.Text}'";
            uygula = new SqlCommand(query1, conn);
            uygula.Connection.Open();
            oku = uygula.ExecuteReader(CommandBehavior.CloseConnection);
            while (oku.Read())
            {
                if (txtilceler.Items.Contains(oku.GetString(1).ToString()) != true)
                {
                    veri.Add(oku.GetString(1));
                    txtilceler.Items.Add(oku.GetString(1));
                }
            }
            conn.Close();
            oku.Close();
            txtSehir.AutoCompleteCustomSource = veri;
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            DialogResult mesaj;
            mesaj = MessageBox.Show("Bu kaydı silmek üzeresiniz..", "Silme işlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (mesaj==DialogResult.Yes)
            {
                Sil();
            }
        }
        private void Sil()
        {
            if (secimId>0)
            {
                uygula = new SqlCommand();
                uygula.CommandType = CommandType.StoredProcedure;
                uygula.CommandText = "[D_Doktor]";
                uygula.Connection = conn;
                uygula.Connection.Open();

                uygula.Parameters.Add("@Id", SqlDbType.Int).Value = secimId;
                uygula.ExecuteNonQuery();
                uygula.Connection.Close();
                conn.Close();
                MessageBox.Show("Kayıt Silinmiştir!!");
                    Temizle();
                Listele();


            }
        }

        private void Temizle()
        {
            txtAd.Text = "";
            txtSehirId.Text = "";
            txtSehir.Text = "";
            txtUnvan.Text = "";
            secimId = -1;
        }

        public int secimId = -1;

        private void Liste_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                secimId = int.Parse(Liste.CurrentRow.Cells[0].Value.ToString());
            }
            catch (Exception)
            {

                secimId = -1;
            }
            if (secimId>0)
            {
                Ac();
            }
        }

        private void Ac()
        {
            txtAd.Text = Liste.CurrentRow.Cells[2].Value.ToString();
            txtSehirId.Text = Liste.CurrentRow.Cells[3].Value.ToString();
            txtUnvan.Text = Liste.CurrentRow.Cells[1].Value.ToString();
        }

        private void txtilceler_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            uygula = new SqlCommand();
            uygula.CommandType = CommandType.StoredProcedure;
            uygula.CommandText = "[G_Doktor]";
            uygula.Connection = conn;
            uygula.Connection.Open();


            uygula.Parameters.Add("@Id", SqlDbType.Int).Value = secimId;
            uygula.Parameters.Add("@DoktorAdi", SqlDbType.NVarChar, 50).Value = txtAd.Text;
            uygula.Parameters.Add("@SehirId", SqlDbType.Int).Value = txtSehirId.Text;
            uygula.Parameters.Add("@Unvan", SqlDbType.NVarChar, 50).Value = txtUnvan.Text;
            uygula.ExecuteNonQuery();
            uygula.Connection.Close();
            conn.Close();
            MessageBox.Show("Kayıt Başarılı");
            Listele();
        }
    }
}