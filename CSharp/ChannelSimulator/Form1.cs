using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MathWorks.MATLAB.NET.Utility;
using MathWorks.MATLAB.NET.Arrays;
using MATLABchannelSimulator;

namespace ChannelSimulator
{
    public partial class Form1 : Form
    {
        private double medsGains;
        private double medsFrequencies;
        private double medsPhases;

        private double jmGains;
        private double jmFrequencies;
        private double jmPhases;

        public MSimulator simulator;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            updateSinusoidsNumber();


            simulator= new MSimulator();
        }

        private void textBox3_TextChanged(object sender, EventArgs e) 
        {
            updateSinusoidsNumber();
        }

        private void updateSinusoidsNumber()
        {
            int realSinusoidNumber = Convert.ToInt32(textBox3.Text);
            if (radioButtonMEDS.Checked) {
                realSinusoidNumber++;
            }
            textBox5.Text = Convert.ToString(realSinusoidNumber);
        }

        private void radioButtonMEDS_CheckedChanged(object sender, EventArgs e)
        {
            updateSinusoidsNumber();
        }

        /*
        public double getGains()
        {
            if (radioButtonMEDS.Checked) {
                if (medsGains == null) {
                    medsGains = simulator.createGainsByMEDS();
                }  
                return medsGains;    
            } else {
                if (jmGains == null) {
                    jmGains = simulator.createGainsByJM();
                }
                return jmGains;
            }
        }

        public double getFrequencies()
        {
            if (radioButtonMEDS.Checked) {
                return medsGains;
            } else {
                return jmGains;
            }
        }

        public double getPhases()
        {
            if (radioButtonMEDS.Checked) {
                return medsPhases;
            } else {
                return jmPhases;
            }
        }
         * */
        
        private void button1_Click(object sender, EventArgs e)
        {
           simulator.createRayCoeff();
        }
    }
}