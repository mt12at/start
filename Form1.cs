using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Threading;

namespace INFO_PC
{
    public partial class Form1 : Form
    {
        string user = "";
        string serialnumber = "";
        string hostname = "";
        string cdprodukt = "";
        string monitor = "";
        string disk = "";
        string info = "";
        private const string V = "\\";
        bool checkChekBox;
        Thread potok1;
        public Form1()
        {
            InitializeComponent();

        }
        private void compInfo()
        {
            if (checkBox1.Checked | checkBox2.Checked | checkBox3.Checked | checkBox4.Checked | checkBox5.Checked | checkBox6.Checked) checkChekBox = true;
            else checkChekBox = false;
            if (checkBox1.Checked) user = " QUERY USER;"; else user = "";
            if (checkBox2.Checked) serialnumber = " wmic bios get serialnumber;"; else serialnumber = "";
            if (checkBox3.Checked) hostname = " HOSTNAME;"; else hostname = "";
            if (checkBox4.Checked) cdprodukt = " wmic CSPRODUCT get Name;"; else cdprodukt = "";
            if (checkBox5.Checked) monitor = " wmic DESKTOPMONITOR get PNPDeviceID;"; else monitor = "";
            if (checkBox6.Checked) disk = " Get-PSDrive;"; else disk = "";
        
            info = user + serialnumber + hostname + cdprodukt + monitor + disk;
        }
        private string read_Info(string ipconf)
        {

            string ip = Convert.ToString(V + V + ipconf);
            Process psi = Process.Start(new ProcessStartInfo
            {
                StandardOutputEncoding = Encoding.GetEncoding(866),
                
                FileName = "cmd.exe",
                Arguments = "/k " + "psexec.exe " + ip + " powershell.exe" + info + " exit & exit",
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden

            });

            return psi.StandardOutput.ReadToEnd();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            compInfo();
            if (checkChekBox)
            {
                potok1 = new Thread(go);
                potok1.Start();
            }
            else MessageBox.Show("Будьте так любезны, поставте хоть одну галочку в поле Информация", "ЯЯЯ!!! ТЕБЕ!!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
        private void go()
        {
            progressBar1.Visible = true;
            string[] listIp = textBox1.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            progressBar1.Maximum = listIp.Length;
            for (int i = 0; i < listIp.Length; i++)
            {
                progressBar1.Value = i;
                printResult(listIp[i].ToString());
                
            }
            progressBar1.Visible = false;
        }
        private void printResult(string ipws)
        {
            if (CheckIp(ipws))
            {
                if (Testping(ipws))
                {
                    textBox2.Text += "\tОтвет от " + ipws + "\r\n" + read_Info(ipws) + "\r\n-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-\r\n";
                }
                else textBox2.Text += "\t" + ipws + " Недоступен!!! ОПЯТЬ СВЯЗИСТЫ ВЫНОВАТЫ!!!\r\n\r\n-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-\r\n";
            }
            else
            {
                string newip = "";
                if(DNS_show(ipws, ref newip))
                {
                    if (Testping(ipws))
                    {
                        textBox2.Text += "\tОтвет от " + ipws + "\r\n" + read_Info(newip) + "\r\n-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-\r\n";
                    }
                    else textBox2.Text += "\t" + ipws + " Недоступен!!! ОПЯТЬ СВЯЗИСТЫ ВЫНОВАТЫ!!!\r\n\r\n-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-\r\n";
                }               
                else textBox2.Text += "\t" + ipws + " Не могу найти ip!!! ПИШИ ПРАВИЛЬНО!!!\r\n\r\n-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-\r\n";
            }
        }
        private bool Testping(string ip)
        {
            Ping png = new Ping();
            PingReply pingReply = png.Send(ip);
            if (pingReply.Status == IPStatus.Success)
                return true;
            else return false;
        }
        static bool CheckIp(string address)
        {
            var nums = address.Split('.');
            int useless;
            return nums.Length == 4 && nums.All(n => int.TryParse(n, out useless)) &&
                   nums.Select(int.Parse).All(n => n < 256);
        }
        private bool DNS_show(string nameps, ref string result)
        {
            try
            {
                IPHostEntry host2 = Dns.GetHostEntry(nameps);
                foreach (IPAddress ip in host2.AddressList)
                    result = ip.ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.SelectionStart = textBox2.Text.Length;
            textBox2.ScrollToCaret();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(potok1 != null)
            {
                potok1.Abort();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (potok1 != null)
            {
                potok1.Abort();
                textBox2.Text += "\r\n\r\n\r\n\t ПРОЦЕСС ОСТАНОВЛЕН";
                progressBar1.Visible = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label3.Text = textBox1.Lines.Length.ToString();
        }

        private void выгрузитьРезультатToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Текстовый документ (*.txt)|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter streamWriter = new StreamWriter(saveFileDialog1.FileName);
                streamWriter.WriteLine(textBox2.Text);
                streamWriter.Close();
            }
            else
                return;
        }
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Приложение используеться для сбора информации с рабочих станций в сети.\r\n"+
                "\r\n\r\nПриложение будет работать только если ты админ на удаленном ПК, иначе все втанет колом!!!\r\n\r\nВерсия 1.1. от 19.01.2022", "О бо мне");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
