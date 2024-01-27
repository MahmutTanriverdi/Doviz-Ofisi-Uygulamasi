using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Data.SqlClient;
using System.Net.Configuration;

namespace Doviz_Ofisi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection("server=localhost; Initial Catalog=DbDovizOfisi;Integrated Security=SSPI");
        string exchange = "";
        string[] turIsim = { "TRY", "USD", "EUR" };
        double[] tur = { 1, 2, 3 };
        private void Form1_Load(object sender, EventArgs e)
        {
            string bugün = "http://www.tcmb.gov.tr/kurlar/today.xml";
            var xmldosya = new XmlDocument();
            xmldosya.Load(bugün);

            string dolarAlis = xmldosya.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteBuying").InnerXml;
            LblDolarAlis.Text = dolarAlis;

            string dolarSatis = xmldosya.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteSelling").InnerXml;
            LblDolarSatis.Text = dolarSatis;

            string euroAlis = xmldosya.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteBuying").InnerXml;
            LblEuroAlis.Text = euroAlis;

            string euroSatis = xmldosya.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteSelling").InnerXml;
            LblEuroSatis.Text = euroSatis;

            listele();

        }

        private void BtnDolarAl_Click(object sender, EventArgs e)
        {
            TxtKur.Text = LblDolarAlis.Text;
            exchange = "TRYtoUSD";
        }

        private void BtnDolarSat_Click(object sender, EventArgs e)
        {
            TxtKur.Text = LblDolarSatis.Text;
            exchange = "USDtoTRY";
        }

        private void BtnEuroAl_Click(object sender, EventArgs e)
        {
            TxtKur.Text = LblEuroAlis.Text;
            exchange = "TRYtoEUR";
        }

        private void BtnEuroSat_Click(object sender, EventArgs e)
        {
            TxtKur.Text = LblEuroSatis.Text;
            exchange = "EURtoTRY";
        }

        private void BtnSatisYap_Click(object sender, EventArgs e)
        {
            double kur = double.Parse(TxtKur.Text);
            double miktar = double.Parse(TxtMiktar.Text);
            double tutar = kur * miktar;
            double eurMiktar = Convert.ToDouble(LblEuroMiktar.Text);
            double usdMiktar = Convert.ToDouble(LblDolarMiktar.Text);
            double tryMiktar = Convert.ToDouble(LblTlMiktar.Text);
            if (exchange == "USDtoTRY")
            {
                usdMiktar -= miktar;
                tryMiktar += tutar;
            }
            else if (exchange == "TRYtoUSD")
            {
                usdMiktar += miktar;
                tryMiktar -= tutar;
            }
            else if (exchange == "EURtoTRY")
            {
                eurMiktar -= miktar;
                tryMiktar += tutar;
            }
            else if (exchange == "TRYtoEUR")
            {
                eurMiktar += miktar;
                tryMiktar -= tutar;
            }
            tur[0] = tryMiktar;
            tur[1] = usdMiktar;
            tur[2] = eurMiktar;

            TxtTutar.Text = tutar.ToString();
            guncelle();
        }

        private void TxtKur_TextChanged(object sender, EventArgs e)
        {
            TxtKur.Text = TxtKur.Text.Replace(".", ",");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double miktar = Convert.ToInt16(TxtMiktar.Text);
            double kur = Convert.ToDouble(TxtKur.Text);
            double tutar = Convert.ToInt16(miktar * kur);
            TxtTutar.Text = tutar.ToString();

        }

        public void listele()
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select miktar from Tbl_Kasa", baglanti);
            SqlDataReader dr = komut.ExecuteReader();
            int i = 0;
            while (dr.Read())
            {
                tur[i] = Convert.ToDouble(dr[0]);
                i++;
            }
            dr.Close();
            baglanti.Close();
            LblTlMiktar.Text = tur[0].ToString();
            LblDolarMiktar.Text = tur[1].ToString();
            LblEuroMiktar.Text = tur[2].ToString();
        }

        void guncelle()
        {
            baglanti.Open();
            for(int i = 0;i<tur.Length;i++) 
            {
                SqlCommand komut = new SqlCommand("UPDATE Tbl_Kasa SET miktar='" + tur[i] + "' WHERE tur='" + turIsim[i] + "'", baglanti);
                komut.ExecuteNonQuery();
            }
            baglanti.Close();
            listele();
        }

    }
}
