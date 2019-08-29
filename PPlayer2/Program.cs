using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Accord.Controls;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Optimization.Losses;
using Accord.Statistics;
using Accord.Statistics.Kernels;
using Accord.MachineLearning.VectorMachines;
using System.Diagnostics;
using Accord.Imaging;
using Accord.MachineLearning;
using Accord.Math;
using AForge;
using Accord;
using Accord.Math.Distances;
using CNTK;
using Accord.Neuro;
using Accord.Neuro.Learning;

namespace PPlayer2
{
    class Program
    {
        public class Digit
        {
            public double[] Image;
            public int Label;
            public int Width;
            public int Height;
        }


        static void Main(string[] args)
        {
            string fn = @"C:\Users\semio\Desktop\nefile.csv";
            var f = File.ReadLines(fn);
            var data = from z in f.Skip(1)
                       let zz = z.Split(';')
                       select new Digit
                       {
                           Label = int.Parse(zz.Last()),
                           Width = int.Parse(zz[1]),
                           Height = int.Parse(zz[2]),
                           Image = zz.FirstOrDefault().Split(' ').Select(double.Parse).ToArray()
                       };
            Digit[] train = data.Take(9).ToArray();
            Digit[] test = data.Skip(9).ToArray();

            double prErr = 10000000;
            double error = 100;

            var network = new ActivationNetwork(new SigmoidFunction(), 50*50, 100, 100, 2);
            var learner = new BackPropagationLearning(network)
            {
                LearningRate = 0.005
            };
            Console.WriteLine("Start Learning");
            new GaussianWeights(network, 0.1).Randomize();
            double err = 1;
            int i = 1;
            while (err > 0.004)       
            {
                err = learner.RunEpoch((from x in train select x.Image.Select(t => (double)t).ToArray()).ToArray(),
                    (from x in train select ToOneHotEnc(x.Label, 2)).ToArray());
                Console.WriteLine($"EpochNum = {i} --- Error={err}");
                i++;
                if (Math.Abs(error - prErr) < 0.0001)
                {
                    // Уменьшаем коэффициент скорости обучения на 2
                    //learner.LearningRate /= 2;
                    //if (learner.LearningRate < 0.001)
                    //    learner.LearningRate = 0.001;
                }
                prErr = error;

            }


            foreach (Digit v in test)
            {
                var n = network.Compute(v.Image);
                //ImageBox.Show(v.Image.Select(x => (double)x / 2).ToArray(), 50, 50);
                var z = getMaxInex(n);
                
                Console.WriteLine("{0} => {1};    2 - {2}; 3 - {3}", v.Label, z+2, n[0], n[1]);
            }
            Console.ReadKey();

        }

        private static int getMaxInex(double[] mass)
        {
            int maxIndex = 0;
            for (int i =0; i<mass.Length; i++)
            {
                if (mass[i] > mass[maxIndex])
                {
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        private static double[] ToOneHotEnc(int target, int count)
        {
            double[] result = new double[count];
            result = result.Select(d => d = 0).ToArray();
            result[target-2] = 1;
            return result;
        }


        //private static Digit[] makeTheSameSize(Digit[] images)
        //{
        //    int maxWidth = 0, maxHeight = 0;
        //    foreach (Digit digit in images)
        //    {
        //        if (digit.Width > maxWidth)
        //    }
        //}


    }
}
