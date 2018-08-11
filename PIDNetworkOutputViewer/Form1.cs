using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PIDNetworkOutputViewer
{
    //learning_rate P I D train_lossf val_lossf train_accf val_accf
    public partial class Form1 : Form
    {
        private class data_block
        {
            public double learning_rate, P, I, D, p1, p2;
            public List<double> train_lossf, val_lossf, train_accf, val_accf;
            public data_block()
            {
                learning_rate = 0.0;
                P = 0.0;
                I = 0.0;
                D = 0.0;
                train_lossf = new List<double>();
                val_lossf = new List<double>();
                train_accf = new List<double>();
                val_accf = new List<double>();
            }
            //public System.Windows.Forms.DataVisualization.Charting.Series this[int key]
            //{
            //    get
            //    {
            //        System.Windows.Forms.DataVisualization.Charting.Series sr = new System.Windows.Forms.DataVisualization.Charting.Series();
                    
            //    }
            //}
        }
        /// <summary>
        /// 字符串中多个连续空格转为一个空格，去除收尾空格
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>合并空格后的字符串</returns>
        private static string MergeSpace(string str)
        {
            if (str != string.Empty &&
                str != null &&
                str.Length > 0
                )
            {
                str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(str, " ");
            }
            return str.Trim();
        }

        private string get_list(string str, List<double> ret_list)
        {
            int start = str.IndexOf('[');
            int end = str.IndexOf(']');
            string use = str.Substring(start + 1, end - start - 1);
            string ret = str.Substring(end + 1);
            string[] nums = use.Split(',');
            for(int i = 0;i < nums.Length;i++)
            {
                ret_list.Add(double.Parse(nums[i].Trim()));
            }

            return ret;
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> data_string_raw = new List<string>(textBox1.Lines);
            List<data_block> datas = new List<data_block>();
            for (int i = data_string_raw.Count - 1; i >= 0; i--)
            {
                string str = data_string_raw[i];
                str = str.Replace(" ", "");
                str = str.Replace("\r", "");
                str = str.Replace("\n", "");
                str = str.Replace("\t", "");
                if (str == "")
                    data_string_raw.RemoveAt(i);
            }
            for (int i = data_string_raw.Count - 1; i >= 0; i--)
            {
                string str = data_string_raw[i];
                str = MergeSpace(str);
                string[] split_space = str.Split(' ');
                data_block db = new data_block();
                db.learning_rate = double.Parse(split_space[0]);
                db.P = double.Parse(split_space[1]);
                db.I = double.Parse(split_space[2]);
                db.D = double.Parse(split_space[3]);
                db.p1 = double.Parse(split_space[4]);
                db.p2 = double.Parse(split_space[5]);
                str = get_list(str, db.train_lossf);
                str = get_list(str, db.val_lossf);
                str = get_list(str, db.train_accf);
                str = get_list(str, db.val_accf);
                datas.Add(db);
            }
            if(File.Exists("excel.csv"))
            {
                int k = 0;
                while (File.Exists("excel.csv." + k + ".bak")) k++;
                File.Move("excel.csv", "excel.csv." + k + ".bak");
            }
            StreamWriter sw = new StreamWriter("./excel.csv");
            
            for (int i = 0; i < datas.Count; i++)
            {
                sw.Write(datas[i].learning_rate + "," + datas[i].P + "," + datas[i].I + "," + datas[i].D
                    +"," + datas[i].p1 +"," + datas[i].p2 + ", ,");
                for(int j = 0;j < datas[i].train_lossf.Count;j++)
                {
                    sw.Write(datas[i].train_lossf[j] + ",");
                }
                sw.Write(" ,");
                for (int j = 0; j < datas[i].val_lossf.Count; j++)
                {
                    sw.Write(datas[i].val_lossf[j] + ",");
                }
                sw.Write(" ,");
                for (int j = 0; j < datas[i].val_lossf.Count; j++)
                {
                    sw.Write(datas[i].val_lossf[j] + ",");
                }
                sw.Write(" ,");
                for (int j = 0; j < datas[i].val_accf.Count; j++)
                {
                    sw.Write(datas[i].val_accf[j] + ",");
                }
                sw.Write(" \r\n");
            }
            sw.Close();
            if (checkBox1.Checked)
            {
                for (int i = 0; i < datas.Count; i++)
                {
                    Table tb = new Table();
                    tb.label1.Text = "learning_rate = " + datas[i].learning_rate.ToString("G10");
                    tb.label2.Text = "P = " + datas[i].P.ToString("G10");
                    tb.label3.Text = "I = " + datas[i].I.ToString("G10");
                    tb.label4.Text = "D = " + datas[i].D.ToString("G10");

                    tb.textBox1.Text = datas[i].learning_rate.ToString("G10") + ", " +
                        datas[i].P.ToString("G10") + ", " +
                        datas[i].I.ToString("G10") + ", " +
                        datas[i].D.ToString("G10");

                    tb.chart1.Titles[0].Text = "train_lossf";
                    for (int j = 0; j < datas[i].train_lossf.Count; j++)
                    {
                        tb.chart1.Series[0].Points.AddXY(j, datas[i].train_lossf[j]);
                    }
                    tb.chart1.Series[0].Points[0].Label = datas[i].train_lossf[0].ToString("G6");
                    tb.chart1.Series[0].Points[datas[i].train_lossf.Count - 1].Label = datas[i].train_lossf[datas[i].train_lossf.Count - 1].ToString("G6");

                    tb.chart2.Titles[0].Text = "val_lossf";
                    for (int j = 0; j < datas[i].val_lossf.Count; j++)
                    {
                        tb.chart2.Series[0].Points.AddXY(j, datas[i].val_lossf[j]);
                    }
                    tb.chart2.Series[0].Points[0].Label = datas[i].val_lossf[0].ToString("G6");
                    tb.chart2.Series[0].Points[datas[i].val_lossf.Count - 1].Label = datas[i].val_lossf[datas[i].val_lossf.Count - 1].ToString("G6");

                    tb.chart3.Titles[0].Text = "train_accf";
                    for (int j = 0; j < datas[i].train_accf.Count; j++)
                    {
                        tb.chart3.Series[0].Points.AddXY(j, datas[i].train_accf[j]);
                    }
                    tb.chart3.Series[0].Points[0].Label = datas[i].train_accf[0].ToString("G6");
                    tb.chart3.Series[0].Points[datas[i].train_accf.Count - 1].Label = datas[i].train_accf[datas[i].train_accf.Count - 1].ToString("G6");

                    tb.chart4.Titles[0].Text = "val_accf";
                    for (int j = 0; j < datas[i].val_accf.Count; j++)
                    {
                        tb.chart4.Series[0].Points.AddXY(j, datas[i].val_accf[j]);
                    }
                    tb.chart4.Series[0].Points[0].Label = datas[i].val_accf[0].ToString("G6");
                    tb.chart4.Series[0].Points[datas[i].val_accf.Count - 1].Label = datas[i].val_accf[datas[i].val_accf.Count - 1].ToString("G6");

                    //tb.chart1.Series.Add()
                    tb.Show();
                }
            }
            else
            {
                MessageBox.Show("Done!");
            }
            return;
        }
    }
}
