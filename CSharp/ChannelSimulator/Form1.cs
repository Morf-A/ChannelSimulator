using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using System.IO;
using System.Globalization;

using MathWorks.MATLAB.NET.Utility;
using MathWorks.MATLAB.NET.Arrays;
using MATLABchannelSimulator;

namespace ChannelSimulator
{
    public partial class Form1 : Form
    {

        //public double[] profileEPA = {1.0000, 0.8913, 0.7943, 0.7079, 0.3981, 0.1380, 0.0912};

        public double[] profileEPA = {0.7071, 0.6302, 0.5617, 0.5006, 0.2815, 0.0976, 0.0645};
        public double[] profileEVA = {0.7071, 0.5950, 0.6018, 0.4672, 0.6599, 0.2480, 0.3159, 0.1776, 0.1010};
        public double[] profileETU = {0.6302, 0.6302, 0.6302, 0.7071, 0.7071, 0.7071, 0.5006, 0.3976, 0.3159};

        double[] timeEPA = {0, 30, 70, 90, 110, 190, 410};
        double[] timeEVA = {0, 30, 150, 310, 370, 710, 1090, 1730, 2510};
        double[] timeETU =  { 0, 50, 120, 200, 230, 500, 1600, 2300, 5000 };

        public double[][,] rays;
        public double[,] frecResponse;
        public List<double> signal;

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

        public double[] getCurrentProfileTime()
        {
            double[] result = null;
            switch (comboBox1.SelectedIndex) {
                case 0:
                    result = timeEPA;
                    break;
                case 1:
                    result = timeEVA;
                    break;
                case 2:
                    result = timeETU;
                    break;
            }
            return result;
        }

        public double[] getCurrentProfile()
        {
            double[] result = null;
            switch (comboBox1.SelectedIndex) {
                case 0:
                    result = profileEPA;
                    break;
                case 1:
                    result = profileEVA;
                    break;
                case 2:
                    result = profileETU;
                    break;
            }
            return result;
        }

        public MWArray getGains()
        {
            MWArray result = null;
            MWArray currentProfile = new MWNumericArray(getCurrentProfile());
            if (radioButtonMEDS.Checked) {
                result = simulator.createGainsByMEDS(currentProfile, Convert.ToDouble(textBox3.Text));    
            } else {
                result = simulator.createGainsByJM(currentProfile, Convert.ToDouble(textBox3.Text));
            }
            return result;
        }
        
        public MWArray getFrequencies()
        {
            MWArray result = null;
            if (radioButtonMEDS.Checked){
                result = simulator.createFrequenciesByMEDS(
                        Convert.ToDouble(textBox1.Text),
                        Convert.ToDouble(textBox3.Text)
                );
            } else {
                result = simulator.createFrequenciesByJM(
                    Convert.ToDouble(textBox1.Text),
                    Convert.ToDouble(textBox3.Text)
                );
            }
            return result;
         }
        

        public MWArray getPhases()
        {
            MWArray result = null;
            if (radioButtonMEDS.Checked) {
                result = simulator.createPhasesByMEDS(Convert.ToDouble(textBox3.Text));
            } else {
                result = simulator.createPhasesByJM(Convert.ToDouble(textBox3.Text)); 
            }
            return result;
        }
        
        
        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button8.Enabled = true;

            chart1.Series[0].Points.Clear();
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

            MWNumericArray m_gains = (MWNumericArray)getGains();

            double[,,] gains = (double[,,])m_gains.ToArray(MWArrayComponent.Real);

            int raysCount = getCurrentProfile().Length;
            int maxSinusoidsNumber = Convert.ToInt32(textBox3.Text)+1;
            int timeCount          = Convert.ToInt32(textBox4.Text);

            // Формируем лучи
            rays = new double[raysCount][,];
            double[] averagePower = new double[raysCount];

            for (int rayIndex = 0; rayIndex < raysCount; rayIndex++) {

                double[,] rayGains = new double[2, maxSinusoidsNumber];
                for (int sinusoinIndex = 0; sinusoinIndex < maxSinusoidsNumber; sinusoinIndex++) {
                    for (int numberPiece = 0; numberPiece < 2; numberPiece++) {
                        rayGains[numberPiece, sinusoinIndex] = gains[rayIndex, numberPiece, sinusoinIndex];
                    }
                }

                MWArray m_rayGains = new MWNumericArray(rayGains);
                MWArray sinudoidsCount = new MWNumericArray(
                        new double[2] {
                            Convert.ToDouble(textBox3.Text),
                            Convert.ToDouble(textBox5.Text) 
                        }
                );

                MWNumericArray m_channelRay = (MWNumericArray)simulator.createRayCoeff(
                        m_rayGains,
                        getFrequencies(),
                        getPhases(),
                        Convert.ToDouble(textBox2.Text),
                        Convert.ToDouble(textBox4.Text),
                        sinudoidsCount
                );

                double[,] channelRay = (double[,])m_channelRay.ToArray(MWArrayComponent.Real);
                rays[rayIndex] = channelRay;

                // Средняя мощность луча
                averagePower[rayIndex] = 0;
                for (int k = 0; k < timeCount; k++) {
                    averagePower[rayIndex] = averagePower[rayIndex] + Math.Pow(Complex.Abs(new Complex(channelRay[0, k], channelRay[1, k])), 2);
                }
                averagePower[rayIndex] = averagePower[rayIndex] / timeCount;

                chart1.Series[0].Points.AddY(averagePower[rayIndex]);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;

            int correlationLength = 30;
            int rayLength = rays[0].Length;
            double[] ray = new double[rayLength];
            for(int i = 0; i < rays[0].Length/2; i++){
                ray[i] = rays[0][0,i];
            }
            MWNumericArray m_correlation = (MWNumericArray)simulator.correlation(new MWNumericArray(ray), correlationLength);
            double[,] correlation = (double[,])m_correlation.ToArray(MWArrayComponent.Real);

            for (int i = 0; i < correlationLength; i++) {
                chart1.Series[0].Points.AddY(correlation[i, 0]);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;

            int correlationLength = 90;
            int rayLength = rays[0].Length;
            double[] ray = new double[rayLength];
            for (int i = 0; i < rays[0].Length / 2; i++) {
                ray[i] = rays[0][0, i];
            }
            MWNumericArray m_correlation = (MWNumericArray)simulator.psdd(new MWNumericArray(ray), correlationLength);
            double[,] correlation = (double[,])m_correlation.ToArray(MWArrayComponent.Real);
            for (int i = 0; i < correlationLength; i++) {
                chart1.Series[0].Points.AddY(correlation[i, 0]);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            int frecCount = 999;

            int raysCount = getCurrentProfile().Length;
            int timeCount = Convert.ToInt32(textBox4.Text);

            // Почему у нас в начале идёт время, а в конце лучи - ??? Хотя метод принимает в начале лучи, а в конце время... 
            double[, ,] threeDimensionsRays = new double[timeCount, 2, raysCount];

            for (int rayIndex = 0; rayIndex < raysCount; rayIndex++) {
                for (int numberPiece = 0; numberPiece < 2; numberPiece++) {
                    for (int timeIndex = 0; timeIndex < timeCount; timeIndex++) {
                        threeDimensionsRays[timeIndex, numberPiece, rayIndex] = rays[rayIndex][numberPiece, timeIndex];
                    }
                }
            }

            MWNumericArray m_frecResponse = (MWNumericArray)simulator.getFrequencyResponse(
                    new MWNumericArray(threeDimensionsRays),
                    frecCount,
                    new MWNumericArray(getCurrentProfileTime())
            );
            frecResponse = (double[,])m_frecResponse.ToArray(MWArrayComponent.Real);

           // chart1.Series[0].ChartArea.Area
          //  for (int i = 0; i < correlationLength; i++) {
          //      chart1.Series[0].Points (correlation[i, 0]);
          //  }

            button6.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            button7.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                // Код по сохранению...
                this.Cursor = Cursors.WaitCursor;
                string fileName = saveFileDialog1.FileName;
                List<string> lines = new List<string>();
                int a = frecResponse.GetLength(0);
                int b = frecResponse.GetLength(1);

                for (int i = 0; i < a; i++) {
                    string line = "";
                    for (int j = 0; j < b; j++) {
                        line = line + frecResponse[i, j].ToString(CultureInfo.CreateSpecificCulture("en-GB"));
                        if (j != (b - 1)) {
                            line = line + ",";
                        }
                    }
                    lines.Add(line);
                }
                File.WriteAllLines(fileName, lines);
            }
            this.Cursor = Cursors.Arrow;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            signal = new List<double>();
            string[] stringSignal = File.ReadAllLines(openFileDialog1.FileName);
            string[] stringArraySignal = stringSignal[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in stringArraySignal) {
                signal.Add(Convert.ToDouble(str, CultureInfo.CreateSpecificCulture("en-GB")) / 2); //делим на 2. тест
            }
           
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (saveFileDialog2.ShowDialog() == DialogResult.OK) {
                this.Cursor = Cursors.WaitCursor;
                string fileName = saveFileDialog2.FileName;
                int a = signal.Count;
                string[] line = {""};
                for (int i = 0; i < a; i++) {
                    line[0] = line[0] + signal[i].ToString(CultureInfo.CreateSpecificCulture("en-GB"));
                    if (i != (a - 1)) {
                        line[0] = line[0] + ",";
                    }
                }
                File.WriteAllLines(fileName, line);
            }
            this.Cursor = Cursors.Arrow;
        }
    }
}