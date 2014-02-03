using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace RcMupper
{
    public partial class Form1 : Form
    {
        System.IO.Ports.SerialPort com;

        public Form1()
        {
            InitializeComponent();
            btnEnumeratePorts_Click(null, null);
        }

        private void SetText(string str)
        {
            this.textBox1.Text += str + Environment.NewLine; 
        }

        private void com_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            var bytes = new byte[com.BytesToRead];
            com.Read(bytes, 0, com.BytesToRead);

            //this.Invoke((MethodInvoker)delegate { this.textBox1.Text += received + Environment.NewLine; });

            /**
             * $ - 1 BYTE
             * M - 1 BYTE
             * > | ! - 1 BYTE
             * LENGTH - 1 BYTE
             */
            var header = System.Text.Encoding.ASCII.GetString( bytes.Take(3).ToArray() );
            if (header == "$M>")
            {
                // Message on the way
                int length = bytes[3];

                this.UpdateText(bytes);
                //var messageSize = bytes.ElementAt(3);
                //var message = "";
                //for (var i = 0; i < messageSize; ++i)
                //{
                //    message += bytes.ElementAt(0 + i).ToString() + " ";
                //}
                //this.Invoke((MethodInvoker)delegate { this.textBox1.Text += "IN: " + message + Environment.NewLine; });
            }
            else
            {
                var errMsg = System.Text.Encoding.ASCII.GetString( bytes.ToArray() );
                this.Invoke((MethodInvoker)delegate { this.textBox1.Text += "Err: " + errMsg + Environment.NewLine; });
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            com = new System.IO.Ports.SerialPort(cbPorts.SelectedValue.ToString());
            com.BaudRate = 115200;
            com.DataBits = 8;
            com.Parity = System.IO.Ports.Parity.None;
            com.StopBits = System.IO.Ports.StopBits.One;
            com.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(com_DataReceived);
            com.Open();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            com.Write(this.textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // $M<
            //byte[] data = new byte[] { 0x24, 0x4D, 0x3C, 00, 101, 101 };
            byte[] data= { 0x24, 0x4D , 0x3C , 0x00 , 0x74 , 0x74 , 0x24 , 0x4D , 0x3C , 0x00 , 0x75 , 0x75 , 0x24 , 0x4D , 0x3C , 0x00 , 0x6F , 0x6F , 0x24 , 0x4D , 0x3C , 0x00 , 0x70 , 0x70
, 0x24 , 0x4D , 0x3C , 0x00 , 0x71 , 0x71 , 0x24 , 0x4D , 0x3C , 0x00 , 0x72 , 0x72 };
            com.Write(data, 0, data.Length);

            //var received = com.ReadExisting();
            //this.Invoke((MethodInvoker)delegate { this.textBox1.Text += received; });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] data = { 0x24, 0x4D, 0x3C, 0x00, 100, 100};
            com.Write(data, 0, data.Length);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte[] data = { 0x24, 0x4D, 0x3C, 0x00, 101, 101 };
            com.Write(data, 0, data.Length);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            byte[] data = { 0x24, 0x4D, 0x3C, 0x00, 108, 108 };
            com.Write(data, 0, data.Length);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            byte[] data = { 0x24, 0x4D, 0x3C, 0x00, 105, 105 };
            com.Write(data, 0, data.Length);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            byte[] data = { 0x24, 0x4D, 0x3C, 0x10, 200, 200,
                           5, 221, 5, 221, 5, 244, 5, 223, 5, 222, 5, 221, 5, 212, 5, 214};

            //byte[] data = { 0x24, 0x4D, 0x3C, 0x10, 200, 200,
            //               5, 123, 5, 175, 5, 138, 5, 219, 5, 218, 0x7, 0x9E, 0x5, 0x39, 5, 215 };
            
            com.Write(data, 0, data.Length);

            System.Threading.Thread.Sleep(50);

            byte[] data2 = { 0x24, 0x4D, 0x3C, 0x00, 105, 105 };
            com.Write(data2, 0, data2.Length);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.SendMotorData();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SendMotorData(1450, 1450, 1450, 1850);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            SendMotorData(1450, 1450, 1450, 1150);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            SendMotorData(1450, 1450, 1450, 0);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            SendMessage(BitConverter.GetBytes(205));
        }

        private void SendMotorData()
        {
            SendMotorData((short)this.trackBar1.Value, (short)this.trackBar2.Value, (short)this.trackBar3.Value, (short)this.trackBar4.Value, (short)this.trackBar5.Value, (short)this.trackBar6.Value );
        }

        private void SendMotorData(short roll, short pitch, short yaw, short throttle, short aux1 = 0, short aux2 = 0)
        {
            byte[] bRoll = BitConverter.GetBytes(roll);
            byte[] bPitch = BitConverter.GetBytes(pitch);
            byte[] bYaw = BitConverter.GetBytes(yaw);
            byte[] bThrottle = BitConverter.GetBytes(throttle);
            byte[] bAux1 = BitConverter.GetBytes(aux1);
            byte[] bAux2 = BitConverter.GetBytes(aux2);

            byte[] data = { bRoll[0], bRoll[1], bPitch[0], bPitch[1], bYaw[0], bYaw[1], bThrottle[0], bThrottle[1], bAux1[0], bAux1[1], bAux2[0], bAux2[1], 0, 0, 0, 0 }; // aux1 - aux4

            this.SendMessage(data);
            //Thread.Sleep(100);
            //this.SendMessage(data);
        }

        private void SendMessage(byte[] payload)
        {
            // Message format
            // $M<XXXXXXXXXX
            //    1233333333
            //
            // 1. Payload size
            // 2. Header command
            // 3. Data
            // If offset > payload size = valid message
            byte[] exampleData = { 0x24, 0x4D, 0x3C, 0x10, 200, 200,
                5, 222, 5, 221, 5, 244, 5, 223, 5, 222, 5, 221, 5, 212, 5, 214};

            // 8 channel message
            byte[] result = { 0x24, 0x4D, 0x3C, 22, 200, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff };

            // 5 channel message
            //byte[] result = { 0x24, 0x4D, 0x3C, 22, 200, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff };
            System.Buffer.BlockCopy(payload, 0, result, 5, payload.Length);

            // Set payload length
            // +2 for the 200 200 after payload size in the header?
            //result[3] = (byte)(payload.Length); 
            
            // Write result
            com.Write(result, 0, result.Length);
            this.UpdateText(payload);

            //System.Threading.Thread.Sleep(50);

            com.Write(new byte[] { 0x00 }, 0, 1);
            //byte[] data2 = { 0x24, 0x4D, 0x3C, 0x00, 105, 105 };
            //com.Write(data2, 0, data2.Length);
            //this.UpdateText(data2);
        }

        private void UpdateText(byte[] data)
        {
            if (data.Length < 1)
                return;

            var rc = data.ElementAt(4).Equals(105);
            var messageSize = data.ElementAt(3);
            var message = (data[2] == '<') ? "OUT: " : "IN: ";
            var dataStart = 5;
            for (var i = 0; i < messageSize; ++i)
            {
                message += data.ElementAt(dataStart + i).ToString() + " ";
            }
            this.Invoke((MethodInvoker)delegate { this.textBox1.Text += message + " " /*+ Encoding.ASCII.GetString(data)*/ + Environment.NewLine; this.textBox1.Invalidate(); });

            if (rc)
            {
                short roll = (short)((data[6] << 8) + data[5]);
                short pitch = (short)((data[8] << 8) + data[7]);
                short yaw = (short)((data[10] << 8) + data[9]);
                short throttle = (short)((data[12] << 8) + data[11]);
                short aux1 = (short)((data[14] << 8) + data[13]);
                short aux2 = (short)((data[16] << 8) + data[15]);

                this.Invoke((MethodInvoker)delegate { this.lblStatus1.Text = string.Format("Roll: {0}, Pitch: {1}, Yaw: {2}, Throttle: {3}, Aux1: {4}, Aux2: {5}", roll, pitch, yaw, throttle, aux1, aux2); this.textBox1.Invalidate(); });
            }
        }

        private void btnEnumeratePorts_Click(object sender, EventArgs e)
        {
            cbPorts.DataSource = System.IO.Ports.SerialPort.GetPortNames();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
             if (e.KeyCode == Keys.Q)
                SendMotorData(1450, 1450, 1450, 0);

        }

        private void button15_Click(object sender, EventArgs e)
        {
            byte[] data = { 0x24, 0x4D, 0x3C, 0x00, 12, 12 };
            com.Write(data, 0, data.Length);
        }

    }
}
