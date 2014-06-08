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
using System.Text.RegularExpressions;

using System.Windows.Forms.DataVisualization.Charting;

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
        public double[][,] frecResponse = new double[2][,];
        public List<double> signal;
        public double[][] complexSignal = new double[2][];

        public string sinusoidsNumberPred;
        public string kPred;
        public string fdPred;
        public string tsPred;

        public MSimulator simulator;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            sinusoidsNumberPred = textBox3.Text;
            kPred = textBox4.Text;
            fdPred = textBox1.Text;
            tsPred = textBox2.Text;
            updateSinusoidsNumber();          
            simulator = new MSimulator();
        }

        private void textBox3_TextChanged(object sender, EventArgs e) 
        {
            updateSinusoidsNumber();
        }

        private void updateSinusoidsNumber()
        {
            try {
                int realSinusoidNumber = Convert.ToInt32(textBox3.Text);
                if (textBox3.Text.Length > 3 || realSinusoidNumber < 0) {
                    throw new Exception();
                }
                if (radioButtonMEDS.Checked) {
                    realSinusoidNumber++;
                }
                textBox5.Text = Convert.ToString(realSinusoidNumber);
                button1.Enabled = true;
                sinusoidsNumberPred = textBox3.Text;
            } catch (Exception e) {
                if (textBox3.Text == "") {
                    button1.Enabled = false;
                    sinusoidsNumberPred = "";
                }
                else {
                    textBox3.Text = sinusoidsNumberPred;
                }
            }
        }

        private void radioButtonMEDS_CheckedChanged(object sender, EventArgs e)
        {
            updateSinusoidsNumber();
            if (radioButtonMEDS.Checked) {
                numericUpDown1.Enabled = true;
                label7.Enabled = true;
            } else{
                numericUpDown1.Enabled = false;
                label7.Enabled = false;
            }
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
                result = simulator.createPhasesByMEDS(Convert.ToDouble(textBox3.Text), Convert.ToInt32(numericUpDown1.Value));
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
            //button8.Enabled = true;

            updateChart(SeriesChartType.Column);

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
                chart1.ChartAreas.First().AxisX.Title = "Лучи";
                chart1.ChartAreas.First().AxisY.Title = "Мощность луча";
            }

        }

        private void updateChart(SeriesChartType chartType)
        {
            chart1.Legends.Clear();

            chart1.Series[0].Points.Clear();
            chart1.Series[0].Name = "XXX";
            chart1.Series[0].ChartType = chartType;

            
            if (chart1.Series.Count == 2) {
                chart1.Series[1].Points.Clear();
                chart1.Series.Remove(chart1.Series[1]);
                chart1.ChartAreas.Remove(chart1.ChartAreas[1]);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            updateChart(SeriesChartType.Spline);
            chart1.ChartAreas.First().AxisX.Title = "Частота";
            chart1.ChartAreas.First().AxisY.Title = "Значение";

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
            updateChart(SeriesChartType.Spline);
            chart1.ChartAreas.First().AxisX.Title = "Частота";
            chart1.ChartAreas.First().AxisY.Title = "Мощность";

            int correlationLength = 90;
            int rayLength = rays[0].Length;
            double[] ray = new double[rayLength];
            for (int i = 0; i < rays[0].Length / 2; i++) {
                ray[i] = rays[0][0, i];
            }
            MWNumericArray m_correlation = (MWNumericArray)simulator.psdd(new MWNumericArray(ray), correlationLength);
            double[,] correlation = (double[,])m_correlation.ToArray(MWArrayComponent.Real);

            double fd = 1/Convert.ToDouble(textBox2.Text);
            double step = fd / correlationLength;
            double tempStep = 0;
            for (int i = 0; i < correlationLength; i++) {
                chart1.Series[0].Points.AddXY(Math.Round(tempStep), correlation[i, 0]);
                tempStep = tempStep + step;
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {

            int frecCount = 999;
            int raysCount = getCurrentProfile().Length;
            int timeCount = Convert.ToInt32(textBox4.Text);

            // Почему у нас в начале идёт время- ??? 
            double[, ,] threeDimensionsRays = new double[timeCount, raysCount, 2];

            for (int rayIndex = 0; rayIndex < raysCount; rayIndex++) {
                for (int numberPiece = 0; numberPiece < 2; numberPiece++) {
                    for (int timeIndex = 0; timeIndex < timeCount; timeIndex++) {
                        threeDimensionsRays[timeIndex,rayIndex, numberPiece] = rays[rayIndex][numberPiece, timeIndex];
                    }
                }
            }
            double[] time = getCurrentProfileTime();
            MWNumericArray m_frecResponse = (MWNumericArray)simulator.getFrequencyResponse(
                    new MWNumericArray(threeDimensionsRays),
                    frecCount,
                    new MWNumericArray(time),
                    Convert.ToDouble(textBox2.Text)
            );
            


            frecResponse[0] = (double[,])m_frecResponse.ToArray(MWArrayComponent.Real);
            frecResponse[1] = (double[,])m_frecResponse.ToArray(MWArrayComponent.Real);
           // double[,] realFR = (double[,])m_frecResponse.ToArray(MWArrayComponent.Real);
           // double[,] imFR = (double[,])m_frecResponse.ToArray(MWArrayComponent.Imaginary);

            updateChart(SeriesChartType.Column);

            /*
            
            chart1.Series[0].IsXValueIndexed = true;

            chart1.ChartAreas[0].Area3DStyle.Enable3D = true;

            for (int i = 0; i < 100; i=i+20) {
                for (int j = 0; j < 100; j+=20) {

                    double numb = Math.Pow(Complex.Abs(new Complex(realFR[i, j], imFR[i, j])), 2);


                    chart1.Series[i].Points.AddY(numb);
                }
                chart1.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series());
                chart1.Series[0].ChartType = SeriesChartType.Column;
                chart1.Series[0].Color = Color.FromArgb(123, 50, 8);
                chart1.Series[0].IsXValueIndexed = true;
            }

             */
  
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


                int a = frecResponse[0].GetLength(0)-500;
                int b = frecResponse[0].GetLength(1)-500;



                string sep = "";

                for (int i = 0; i < a; i++) {
                    string line = "";
                    for (int j = 0; j < b; j++) {

                        if (frecResponse[1][i, j] > 0) {
                            sep = "+";
                        }
                        else {
                            sep = "";
                        }

                        line = line
                            + frecResponse[0][i, j].ToString(CultureInfo.CreateSpecificCulture("en-GB"))
                            + sep
                            + frecResponse[1][i, j].ToString(CultureInfo.CreateSpecificCulture("en-GB"))
                            + "i";
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
            updateChart(SeriesChartType.Spline);

            signal = new List<double>();
            List<double> signalR = new List<double>();
            List<double> signalI = new List<double>();

            string[] stringSignal = File.ReadAllLines(openFileDialog1.FileName);
            string[] stringArraySignal = stringSignal[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in stringArraySignal) {

                Regex rgx = new Regex("(.+)([\\|+-].*)i", RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(str);

                signalR.Add(Convert.ToDouble(matches[0].Groups[1].Value));
                signalI.Add(Convert.ToDouble(matches[0].Groups[2].Value));
                           
               // signal.Add(Convert.ToDouble(str, CultureInfo.CreateSpecificCulture("en-GB"))); 
            }

            //Вывод данных
            chart1.Series[0].Points.Clear();
            chart1.Series[0].ChartType = SeriesChartType.Spline;

            chart1.ChartAreas.Add("fff");

            chart1.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series());
            chart1.Series[1].ChartType = SeriesChartType.Spline;
            chart1.Series[1].ChartArea = "fff";


            chart1.Series[0].Name = "Входной сигнал";
            chart1.Series[1].Name = "Выходной сигнал";

            chart1.Legends.Add(new Legend("L1"));
            chart1.Legends.Add(new Legend("L2"));
            chart1.Series[0].Legend = "L1";
            chart1.Series[1].Legend = "L2";

            chart1.Legends[0].Alignment = System.Drawing.StringAlignment.Center;
            chart1.Legends[0].DockedToChartArea = "ChartArea1";
            chart1.Legends[0].Docking = Docking.Bottom;
            chart1.Legends[0].IsDockedInsideChartArea = false;


            chart1.Legends[1].Alignment = System.Drawing.StringAlignment.Center;
            chart1.Legends[1].DockedToChartArea = "fff";
            chart1.Legends[1].Docking = Docking.Bottom;
            chart1.Legends[1].IsDockedInsideChartArea = false;

            for (int i = 0; i < signal.Count; i++) {
                chart1.Series[0].Points.AddY(signalR[i]);
            }

            chart1.ChartAreas[0].AxisX.Title = "Время";
            chart1.ChartAreas[0].AxisY.Title = "Сила";

            chart1.ChartAreas[1].AxisX.Title = "Время";
            chart1.ChartAreas[0].AxisY.Title = "Сила";

            /*
            for (int i = 0; i < signal.Count; i++) {
                signal[i] = signal[i] / 2;
            }
            */

  
            int raysCount = getCurrentProfile().Length;
            int timeCount = Convert.ToInt32(textBox4.Text);

            // Почему у нас в начале идёт время- ??? 
            double[, ,] threeDimensionsRays = new double[timeCount, raysCount, 2];

            for (int rayIndex = 0; rayIndex < raysCount; rayIndex++) {
                for (int numberPiece = 0; numberPiece < 2; numberPiece++) {
                    for (int timeIndex = 0; timeIndex < timeCount; timeIndex++) {
                        threeDimensionsRays[timeIndex, rayIndex, numberPiece] = rays[rayIndex][numberPiece, timeIndex];
                    }
                }
            }

            MWNumericArray m_outputSignal = (MWNumericArray)simulator.processSignal(new MWNumericArray(signal.ToArray()), new MWNumericArray(threeDimensionsRays));
            double[,] outputSignal = (double[,])m_outputSignal.ToArray(MWArrayComponent.Real);

            for (int i = 0; i < signal.Count; i++) {
                chart1.Series[1].Points.AddY(outputSignal[0,i]);
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

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

            try
            {
                if (Convert.ToInt32(textBox4.Text) > 4 || Convert.ToInt32(textBox4.Text) < 0)
                {
                    throw new Exception();
                }
                button1.Enabled = true;
                kPred = textBox4.Text;
            }
            catch
            {
                if (textBox4.Text == "")
                {
                    button1.Enabled = false;
                    kPred = "";
                }
                else
                {
                    textBox4.Text = kPred;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Length > 3 || Convert.ToInt32(textBox1.Text) < 0)
                {
                    throw new Exception();
                }
                button1.Enabled = true;
                fdPred = textBox1.Text;
            }
            catch
            {
                if (textBox1.Text == "")
                {
                    button1.Enabled = false;
                    fdPred = "";
                }
                else
                {
                    textBox1.Text = fdPred;
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBox2.Text) < 0 || Convert.ToInt32(textBox2.Text) > 500)
                {
                    textBox2.Text = tsPred;
                }
                tsPred = textBox2.Text;
            }
            catch
            {
                textBox2.Text = tsPred;
            }
        }
    }
}