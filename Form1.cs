using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace Gyro_Calibrate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SerialPort port;
        String str_port = "COM1";
        Boolean isOpen = false;

        private void Open_Com_button_Click(object sender, EventArgs e)
        {
            if (!isOpen)
            {
                str_port = COM_Port_list_comboBox.SelectedItem.ToString();
                port = new SerialPort(str_port);
                // port.WriteTimeout = 1000;
                port.ReadTimeout = 10000;
                port.BaudRate = 9600;
                port.Parity = Parity.None;
                port.DataBits = 8;
                try
                {
                    port.Open();
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Не найдено устройство. Проверьте питание и подключение");
                    Open_Com_button.BackColor = System.Drawing.Color.Red;
                    toolStripStatusLabel1.Text = "Соединение не установлено";
                    return;
                }
                port.BaudRate = 9600;
                port.Parity = Parity.None;
                port.DataBits = 8;
                port.StopBits = StopBits.One;

                Open_Com_button.BackColor = System.Drawing.Color.Green;
                toolStripStatusLabel1.Text = "Соединение установлено. Порт " + str_port;
                Load_button.Enabled = true;
                numericUpDown1.Enabled = true;
                Read_button.Enabled = true;
                isOpen = true;
            }
            else
            {
                port.Close();
                Open_Com_button.BackColor = SystemColors.Control;
                toolStripStatusLabel1.Text = "Соединение не установлено." ;
                Load_button.Enabled = false;
                numericUpDown1.Enabled = false;
                Read_button.Enabled = false;
                isOpen = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
            }
            catch (System.NullReferenceException)
            { }
            catch (System.IO.IOException)
            { }
        }

        private void Load_button_Click(object sender, EventArgs e)
        {
            Byte[] Command = new Byte[6];
            
            UInt16 Data = (UInt16)numericUpDown1.Value;
            Command[0] = 0x8A;
            Command[1] = 0xAA;
            Command[4] = 0x85;
            Command[5] = 0x00;

            Command[2] = (Byte)(Data >> 8);
            Command[3] = (Byte)(Data);
           // Thread.Sleep(1000);

            port.Write(Command, 0, 6);

         //   Thread.Sleep(1000);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String[] ports = SerialPort.GetPortNames();
            try
            {
                foreach (string port in ports)
                {
                    COM_Port_list_comboBox.Items.Add(port);
                }
                COM_Port_list_comboBox.SelectedIndex = 0;
                Open_Com_button.Select();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Не найден последовательный порт");
                this.Close();
            }
        }

        private void Read_button_Click(object sender, EventArgs e)
        {
            Byte[] Read_data = new Byte[6];
            
            Byte[] Command = new Byte[6];

            Command[0] = 0x8A;
            Command[1] = 0xAB;
            Command[2] = 0x00;
            Command[3] = 0x00;
            Command[4] = 0x85;
            Command[5] = 0x00;

            
            Command[3] = 0x45; // Read calibration value
           
           // Thread.Sleep(1000);

            port.Write(Command, 0, 6);

            Thread.Sleep(10);

            try
                {

                for (int i=0; i<6; i++)
                    {
                      Read_data[i] = (Byte)port.ReadByte();
                    }
                }
                catch (System.TimeoutException)
                {
                    MessageBox.Show("Устройство не отвечает. Чтение не выполнено");
                    return;
                }
            
            String Test = Read_data[0].ToString() + Read_data[1].ToString() + Read_data[2].ToString() + Read_data[3].ToString() + Read_data[4].ToString() + Read_data[5].ToString();
            textBox1.Text = Test;
           if ((Read_data[0] == 0x8A) && (Read_data[1] == 0xAB) && (Read_data[4] == 0x85))
            {
               // numericUpDown1.Value = (Decimal)((int)(Read_data[3]) + ((int)(Read_data[2]) << 8));
                Read_textBox.Text = (((int)(Read_data[3]) + ((int)(Read_data[2]) << 8))).ToString();
           }
            else
            {
                MessageBox.Show("Некорректный пакет");
                    return;
            }
           textBox2.Text = Read_data[2].ToString();
           textBox3.Text = Read_data[3].ToString();
           
        }

        private void Corr_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Corr_checkBox.Checked == true)
            {
                Byte[] Command = new Byte[6];

                UInt16 Data = (UInt16)numericUpDown1.Value;
                Command[0] = 0x8A;
                Command[1] = 0xAC;
                Command[4] = 0x85;
                Command[5] = 0x00;

                Command[2] = 0x02;
                Command[3] = 0x00;
                
                port.Write(Command, 0, 6);

                //Thread.Sleep(1000);
            }
            else
            {
                Byte[] Command = new Byte[6];

                UInt16 Data = (UInt16)numericUpDown1.Value;
                Command[0] = 0x8A;
                Command[1] = 0xAC;
                Command[4] = 0x85;
                Command[5] = 0x00;

                Command[2] = 0x01;
                Command[3] = 0x00;

                port.Write(Command, 0, 6);

               // Thread.Sleep(1000);            
            }
        }

        private void Corr_Read_button_Click(object sender, EventArgs e)
        {
            Byte[] Read_data = new Byte[6];

            Byte[] Command = new Byte[6];

            Command[0] = 0x8A;
            Command[1] = 0xAD;
            Command[2] = 0x00;
            Command[3] = 0x00;
            Command[4] = 0x85;
            Command[5] = 0x00;


            Command[3] = 0x45; // Read calibration value

            // Thread.Sleep(1000);

            port.Write(Command, 0, 6);

            Thread.Sleep(10);

            try
            {

                for (int i = 0; i < 6; i++)
                {
                    Read_data[i] = (Byte)port.ReadByte();
                }
            }
            catch (System.TimeoutException)
            {
                MessageBox.Show("Устройство не отвечает. Чтение не выполнено");
                return;
            }

            if ((Read_data[0] == 0x8A) && (Read_data[1] == 0xAD) && (Read_data[4] == 0x85))
            {
                if (Read_data[2] == 2)
                {
                    Corr_Read_textBox.Text = "Yes";
                }
                else if (Read_data[2] == 1)
                {
                    Corr_Read_textBox.Text = "No";
                }

            }
            else
            {
                MessageBox.Show("Некорректный пакет");
                return;
            }
        }
    }
}
