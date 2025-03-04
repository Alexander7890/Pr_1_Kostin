using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace Pr_1
{
    public partial class Form1: Form
    {
        private string dictionaryPath = "signatures.txt";
        private Dictionary<string, ulong> signatures = new Dictionary<string, ulong>();

        public Form1()
        {
            InitializeComponent();
            LoadDictionary();
        }
        // Завантаження словника з файлу
        private void LoadDictionary()
        {
            if (File.Exists(dictionaryPath))
            {
                foreach (var line in File.ReadAllLines(dictionaryPath))
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2 && ulong.TryParse(parts[1], out ulong signature))
                    {
                        signatures[parts[0]] = signature;
                    }
                }
            }
            else
            {
                MessageBox.Show("Файл з сигнатурами не знайдений за вказаним шляхом.");
            }
        }

        // Обчислення сигнатури для файлу
        private ulong CalculateSignature(string filePath)
        {
            ulong sum = 0;
            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                foreach (byte b in fileBytes)
                {
                    sum = (sum + b) % ulong.MaxValue;  // Обробка переповнення
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка обробки файлу {filePath}: {ex.Message}");
            }
            return sum;
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string searchFilePath = Microsoft.VisualBasic.Interaction.InputBox("Введіть шлях до файлу для пошуку сигнатури:", "Пошук за файлом");

            if (File.Exists(searchFilePath))
            {
                ulong fileSignature = CalculateSignature(searchFilePath);
                var matchingFiles = signatures.Where(kvp => kvp.Value == fileSignature).Select(kvp => kvp.Key).ToList();

                if (matchingFiles.Count > 0)
                {
                    MessageBox.Show($"Знайдені файли з такою сигнатурою:\n{string.Join("\n", matchingFiles)}");
                }
                else
                {
                    MessageBox.Show("Не знайдено файлів з такою сигнатурою.");
                }
            }
            else
            {
                MessageBox.Show("Файл не існує.");
            }
        }
    }
}
