using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChannelSimulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            updateSinusoidsNumber();
        }

        private void textBox3_TextChanged(object sender, EventArgs e) 
        {
            updateSinusoidsNumber();
        }

        private void updateSinusoidsNumber()
        {
            int realSinusoidNumber = Convert.ToInt32(textBox3.Text);
            if (radioButton1.Checked) {
                realSinusoidNumber++;
            }
            textBox5.Text = Convert.ToString(realSinusoidNumber);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            updateSinusoidsNumber();
        }
    }
}