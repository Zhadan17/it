﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Lab4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label_time.Text = "";
            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(this.button_fileKEYopen, "Відкрити");
            toolTip1.SetToolTip(this.button_fileKEYsave, "Зберегти");
        }

        private byte[] myCipherXOR(string fileINpath, string fileKEYpath)
        {
            byte[] arrIN = File.ReadAllBytes(fileINpath);
            byte[] arrKEY = File.ReadAllBytes(fileKEYpath);
            int lenIN = arrIN.Length;
            int lenKEY = arrKEY.Length;
            byte[] arrCipher = new byte[lenIN];

            for (int i = 0; i < lenIN; i++)
            {

                byte p = arrIN[i];
                byte k = arrKEY[i % lenKEY]; // mod
                byte c = (byte)(p ^ k); // XOR

                arrCipher[i] = c;

            }
            return arrCipher;
        }
        static String BytesToString(long byteCount)
        {
            string[] suf = { "Byt", "KB", "MB", "GB", "TB", "PB", "EB" }; //
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];


        }

        private void mySaveToFileOUT(byte[] arrCipher, string fileOUTpath)
        {
            using (FileStream fs = File.Create(fileOUTpath))
            {
                fs.Write(arrCipher, 0, arrCipher.Length);
                fs.Close();
            }
        }



        private void button_fileIN_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileINpath = openFileDialog.FileName;
                    textBox_fileIN.Text = fileINpath;
                }
            }

        }

        private void button_fileOUT_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileOUTpath = saveFileDialog.FileName;
                textBox_fileOUT.Text = fileOUTpath;
            }

        }

        private void button_fileKEYopen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileKEYPath = openFileDialog.FileName;
                    textBox_fileKEY.Text = fileKEYPath;
                }
            }

        }

        private void button_fileKEYsave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileKEYPath = saveFileDialog.FileName;
                textBox_fileKEY.Text = fileKEYPath;
            }

        }

        private void button_KEYgenerator_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Створити новий ключ?", "Увага", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;

                string fileINpath = textBox_fileIN.Text;
                if (File.Exists(fileINpath))
                {
                    string fileKEYpath = textBox_fileKEY.Text;
                    string dirKEYpath = Path.GetDirectoryName(fileKEYpath);
                    if (Directory.Exists(dirKEYpath))
                    {
                        long lengthINfile = new FileInfo(fileINpath).Length;
                        byte[] arrKEY = new byte[lengthINfile / 2];
                        RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
                        rngCsp.GetBytes(arrKEY);
                        using (FileStream fs = File.Create(fileKEYpath))
                        {
                            fs.Write(arrKEY, 0, arrKEY.Length);
                            fs.Close();

                            long byteCount = new System.IO.FileInfo(fileKEYpath).Length;
                            string bytesize = BytesToString(byteCount);
                            labelsize.Text = "Розмір ключа:" + '\n' + bytesize;

                        }
                    }
                    else
                    {
                        MessageBox.Show("Шлях до ключа не вказаний\nабо такий шлях не існує");
                        textBox_fileKEY.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Вхідний файл не існує");
                    button_fileIN.Focus();
                }

                Cursor.Current = Cursors.Default;
            }

        }


        private void button_start_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string fileINpath = textBox_fileIN.Text;
            string fileOUTpath = textBox_fileOUT.Text;
            string fileKEYpath = textBox_fileKEY.Text;
            if (File.Exists(fileINpath))
            {
                string dirOUTpath = Path.GetDirectoryName(fileOUTpath);
                if (Directory.Exists(dirOUTpath))
                {
                    if (File.Exists(fileKEYpath))
                    {
                        long lengthINfile = new FileInfo(fileINpath).Length;
                        long lengthKEYfile = new FileInfo(fileKEYpath).Length;
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        byte[] arrCipher = myCipherXOR(fileINpath, fileKEYpath);
                        mySaveToFileOUT(arrCipher, fileOUTpath);
                        stopwatch.Stop();
                        label_time.Text = stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
                    }
                    else
                    {
                        MessageBox.Show("Шлях до файлу ключа не вказаний\nабо такий файл не існує");
                        textBox_fileKEY.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Шлях до вихідного файлу не вказаний\nабо такий шлях не існує");
                    textBox_fileOUT.Focus();
                }
            }
            else
            {
                MessageBox.Show("Вхідний файл не існує");
                button_fileIN.Focus();
            }

            Cursor.Current = Cursors.Default;



        }
    }
}
