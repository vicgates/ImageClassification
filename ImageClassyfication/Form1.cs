﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageRecognition;
using Telerik.Charting;
using MathNet.Numerics.Distributions;
using System.Windows.Forms.DataVisualization.Charting;
using PatternRecognition;
using ImageRecognition.ClassyfiAlgorithm;

namespace ImageClassyfication
{

    public partial class Form1 : Form
    {
        public double A1
        {
            get
            {
                return double.Parse(this.textBoxA1.Text);
            }
        }

        public double A2
        {
            get
            {
                return double.Parse(this.textBoxA2.Text);
            }
        }

        public double B1
        {
            get
            {
                return double.Parse(this.textBoxB1.Text);
            }
        }

        public double B2
        {
            get
            {
                return double.Parse(this.textBoxB2.Text);
            }
        }

        public double P1
        {
            get
            {
                return double.Parse(this.textBoxP1.Text);
            }
        }

        public double P2
        {
            get
            {
                return double.Parse(this.textBoxP2.Text);
            }
        }

        public int IloscWektorow
        {
            get
            {
                return int.Parse(this.textBoxIloscWekt.Text);
            }
        }

        public int IloscProb
        {
            get
            {
                return int.Parse(this.textBoxIloscProb.Text);
            }
        }

        public int IloscTestowychProbek
        {
            get
            {
                return int.Parse(textBoxIloscTestowychProbek.Text);
            }
        }

        public GeneratorType SelectedGenerator
        {
            get
            {
                if (comboBoxGeneratorType.SelectedItem.ToString().Contains("Normal"))
                    return GeneratorType.Normal;
                else if (comboBoxGeneratorType.SelectedItem.ToString().Contains("Uniform"))
                    return GeneratorType.Uniform;
                else
                    return GeneratorType.None;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnCorrelation_Click(object sender, EventArgs e)
        {

        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            chartDistribution.Series.Clear();
            chartDistribution.ResetAutoValues();

            IContinuousDistribution generator1 = null;
            IContinuousDistribution generator2 = null;

            if (SelectedGenerator == GeneratorType.Normal)
            {
                generator1 = new Normal(A1, A2);
                generator2 = new Normal(B1, B2);
            }
            else if (SelectedGenerator == GeneratorType.Uniform)
            {
                generator1 = new ContinuousUniform(A1, A2);
                generator2 = new ContinuousUniform(B1, B2);
            }

            double naiveBayesCounter = 0;
            double bayesRisk = 0.0;
            double averageCounter = 0;
            double nearestCounter = 0;

            for (int i = 0; i < IloscProb; i++)
            {

                List<PatternClass> wektoryUczace = Common.CreateWektoryUczaceUniform(P1, P2, generator1, generator2, IloscWektorow);

                List<PatternClass> obiektyTestowe = Common.CreateObject(generator1, IloscTestowychProbek/2, 1);
                obiektyTestowe.InsertRange(obiektyTestowe.Count,Common.CreateObject(generator2, IloscTestowychProbek/2, 2));



                NearestAverage averageAlg = new NearestAverage(new EuclideanDistance());

                foreach (PatternClass pClass in obiektyTestowe)
                {
                    if (averageAlg.Classify(wektoryUczace, pClass.WektorCech.wartosci) == pClass.NumerKlasy)
                    {
                        averageCounter++;
                    }
                }


                NearestNeighbour nearestNeighbour = new NearestNeighbour(1, new EuclideanDistance());

                foreach (PatternClass pClass in obiektyTestowe)
                {
                    if (nearestNeighbour.Classify(wektoryUczace, pClass.WektorCech.wartosci) == pClass.NumerKlasy)
                    {
                        nearestCounter++;
                    }
                }

                NaiveBayes naiveBayes = new NaiveBayes();

                foreach (PatternClass pClass in obiektyTestowe)
                {
                    if (naiveBayes.Classify(generator1, generator2, P1, P2, pClass.WektorCech.wartosci) == pClass.NumerKlasy)
                    {
                        naiveBayesCounter++;
                    }
                }
                bayesRisk = naiveBayes.CalculateBayesRisk(generator1, generator2,P1,P2);
            }

            Series series1 = new Series();
            series1.ChartType = SeriesChartType.SplineArea;
            Series series = new Series();
            series.ChartType = SeriesChartType.SplineArea;

            for (double i = generator1.Mean - (8 * generator1.StdDev); i < generator1.Mean + (8 * generator1.StdDev); i += 0.5)
            {
                double value = generator1.Density(i);
                series.Points.Add(new DataPoint(i, value));
            }

            for (double i = generator2.Mean - (8 * generator2.StdDev); i < generator2.Mean + (8 * generator2.StdDev); i += 0.5)
            {
                double value = generator2.Density(i);
                series1.Points.Add(new DataPoint(i, value));
            }

          


            chartDistribution.Series.Add(series1);
            chartDistribution.Series.Add(series);
            chartDistribution.Update();                

            double nearestAlgorithmError = 100-((nearestCounter *100.0 )/ (IloscTestowychProbek*IloscProb));
            double avgAlgorithmError = 100-((averageCounter*100.0 )/ (IloscTestowychProbek*IloscProb));
            double naiveBayesError = 100 - ((naiveBayesCounter * 100.0) / (IloscTestowychProbek * IloscProb));

            textBoxData.Text = String.Format("Neares : {0} \n Average {1} \n Bayes {2} \n Baies Risk {3}", nearestAlgorithmError, avgAlgorithmError, naiveBayesError, bayesRisk);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();

            pictureBox1.Image = Bitmap.FromFile(dialog.FileName);
        }
    }
}