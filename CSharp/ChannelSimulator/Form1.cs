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
        private MWArray medsGains;
        private MWArray medsFrequencies;
        private MWArray medsPhases;

        private double jmGains;
        private double jmFrequencies;
        private double jmPhases;

        public double[] profileEPA = {1.0000, 0.8913, 0.7943, 0.7079, 0.3981, 0.1380, 0.0912};
        public double[] profileEVA = { 0, -1.5, -1.4, -3.6, -0.6, -9.1, -7.0, -12.0, -16, 9 };
        public double[] profileETU = { -1, -1, -1, 0, 0, 0, -3, -5, -7 };

        public MSimulator simulator;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            updateSinusoidsNumber();          
            simulator = new MSimulator();
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

        
        public MWArray getGains()
        {
            if (radioButtonMEDS.Checked) {
                if (medsGains == null) {

                    MWArray outData = new MWNumericArray(profileEPA);
                    medsGains = simulator.createGainsByMEDS(outData, Convert.ToDouble(textBox3.Text));
                }  
                return medsGains;    
            } else {
                if (jmGains == null) {
                   // jmGains = simulator.createGainsByJM();
                }
                return jmGains;
            }
        }
        
        public MWArray getFrequencies()
        {
            if (radioButtonMEDS.Checked)
            {
                if (medsFrequencies == null)
                {
                    medsFrequencies = simulator.createFrequenciesByMEDS(
                            Convert.ToDouble(textBox1.Text),
                            Convert.ToDouble(textBox3.Text)
                    );
                }
                return medsFrequencies;
            } else {
                if (jmGains == null)
                {
                    // jmGains = simulator.createGainsByJM();
                }
                return jmGains;
            }
        }

        public MWArray getPhases()
        {
            if (radioButtonMEDS.Checked)
            {
                if (medsPhases == null)
                {
                    medsPhases = simulator.createPhasesByMEDS(Convert.ToDouble(textBox3.Text));
                }
                return medsPhases;
            } else {
                if (jmGains == null)
                {
                    // jmGains = simulator.createGainsByJM();
                }
                return jmGains;
            }
        }
        
        
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";

            
            MWNumericArray c = (MWNumericArray)getGains();


            double[,] c2 = (double[,])c.ToArray(MWArrayComponent.Real);
            с2[,1];
            double[] d = {0.0171, 0.0169};
            MWArray outData = new MWNumericArray(d);

            MWArray sinudoidsCount = new MWNumericArray(
                    new double[2] {
                        Convert.ToDouble(textBox3.Text),
                        Convert.ToDouble(textBox5.Text) 
                    }
            );

            MWArray res = simulator.createRayCoeff(
                    outData,
                    getFrequencies(),
                    getPhases(),
                    Convert.ToDouble(textBox2.Text),
                    Convert.ToDouble(textBox4.Text),
                    sinudoidsCount
            );


            MWNumericArray descriptor = (MWNumericArray)res; //выбор первого элемента из массива MWArray и преобразование в числовой тип MWNumericArray
            double[,] d_descriptor = (double[,])descriptor.ToArray(MWArrayComponent.Real);//преобразование массива MWNUmericArray  к масииву типа double  

            for (int i = 0; i < 50; i++)//вывод массива d_descriptor в RichBox
            {
                richTextBox1.Text += i.ToString() + '\t';
                richTextBox1.Text += d_descriptor[0, i].ToString("0.000") + '\n';//преобразование элеметна массива double в string
            }

            for (int i = 0; i < 50; i++)//вывод массива d_descriptor в RichBox
            {
                richTextBox2.Text += i.ToString() + '\t';
                richTextBox2.Text += d_descriptor[1, i].ToString("0.000") + '\n';//преобразование элеметна массива double в string
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}