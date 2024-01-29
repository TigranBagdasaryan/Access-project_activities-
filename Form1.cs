using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Npgsql;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;

namespace Dostup
{
    public partial class MyForm1 : Form
    {
        public MyForm1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл для перемещения",
                InitialDirectory = @"C:\\"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                string targetPath = @"C:\Users\Tigran\Desktop\Move";
                string destFile = Path.Combine(targetPath, Path.GetFileName(fileName));

                try
                {
                    string tempFile = Path.GetTempFileName();

                    using (Aes aes = Aes.Create())
                    {
                        string password = "a very long key..";
                        byte[] salt = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
                        const int iterations = 1000;
                        const int keySize = 256;

                        using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
                        {
                            byte[] key = rfc2898DeriveBytes.GetBytes(keySize / 8);
                            aes.Key = key;
                            aes.GenerateIV();

                            using (FileStream fsCrypt = new FileStream(tempFile, FileMode.Create))
                            {
                                fsCrypt.Write(aes.IV, 0, aes.IV.Length);

                                using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
                                {
                                    using (FileStream fsIn = new FileStream(fileName, FileMode.Open))
                                    {
                                        int data;
                                        while ((data = fsIn.ReadByte()) != -1)
                                            cs.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }

                    File.Delete(fileName);
                    File.Move(tempFile, destFile);

                    MessageBox.Show("Файл перемещён и зашифрован успешно!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при перемещении и шифровании файла: " + ex.Message);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        string connString = "Host=localhost;Username=postgres;Password=admin;Database=Dostup;port=5432";

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                label3.Visible = true;
            }
            else
            {
                label3.Visible = false;

                try
                {
                    using (var conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();

                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "INSERT INTO data (username, pass, encryption_key, file) VALUES (@user, @pass, @key, @file)";
                            cmd.Parameters.AddWithValue("@user", textBox2.Text);
                            cmd.Parameters.AddWithValue("@pass", textBox3.Text);
                            cmd.Parameters.AddWithValue("@key", textBox1.Text);
                            cmd.Parameters.AddWithValue("@file", " ");

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Data was inserted into the database.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while inserting data into the database: " + ex.Message);
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }



        private void DecryptFile(string filePath, string targetPath)
        {
            try
            {
                string tempFile = Path.GetTempFileName();

                using (Aes aes = Aes.Create())
                {
                    string password = "a very long key..";
                    byte[] salt = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
                    const int iterations = 1000;
                    const int keySize = 256;

                    using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
                    {
                        byte[] key = rfc2898DeriveBytes.GetBytes(keySize / 8);
                        aes.Key = key;

                        using (FileStream fsCrypt = new FileStream(filePath, FileMode.Open))
                        {
                            byte[] iv = new byte[16];
                            fsCrypt.Read(iv, 0, iv.Length);

                            aes.IV = iv;

                            using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read))
                            {
                                using (FileStream fsOut = new FileStream(tempFile, FileMode.Create))
                                {
                                    int data;
                                    while ((data = cs.ReadByte()) != -1)
                                        fsOut.WriteByte((byte)data);
                                }
                            }
                        }
                    }
                }

                string destFile = Path.Combine(targetPath, Path.GetFileName(filePath));
                File.Move(tempFile, destFile);
                MessageBox.Show("Файл успешно дешифрован!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при дешифровании файла: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл для дешифрования",
                InitialDirectory = @"C:\\Users\Tigran\Desktop\Move"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                string targetPath = @"C:\Users\Tigran\Desktop\Deshif";
                DecryptFile(fileName, targetPath);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox4.Text = GenerateRandomString(32);
        }

        private static string GenerateRandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[4];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = "SELECT * FROM data";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine(reader.GetString(0));
                            }
                        }
                    }
                }

                MessageBox.Show("Data was retrieved from the database. Check your console.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while retrieving data from the database: " + ex.Message);
            }
        }

        private void MyForm1_Load(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}  // Закрывающая фигурная скобка для класса MyForm1