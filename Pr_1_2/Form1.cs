using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Pr_1_2
{
    public partial class Form1 : Form
    {
        private string dictionaryPath = "signatures.txt";

        public Form1()
        {
            InitializeComponent();
            LoadDictionary();
            InitializeContextMenu();
        }

        private void LoadDictionary()
        {
            listBox1.Items.Clear(); // Очистка списка перед загрузкой

            if (File.Exists(dictionaryPath))
            {
                var lines = File.ReadAllLines(dictionaryPath);
                listBox1.Items.AddRange(lines); // Добавление всех строк из файла в listBox1
            }
        }

        private ulong CalculateSignature(string filePath)
        {
            ulong sum = 0;
            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                foreach (byte b in fileBytes)
                {
                    sum = (sum + b) % ulong.MaxValue; // Предотвращение переполнения
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка обробки файлу {filePath}: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sum;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Multiselect = true };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    ulong signature = CalculateSignature(file);
                    string entry = $"{file}:{signature}";

                    // Проверяем, есть ли уже такая запись
                    if (!listBox1.Items.Contains(entry))
                    {
                        listBox1.Items.Add(entry);
                        File.AppendAllText(dictionaryPath, entry + Environment.NewLine);
                    }
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string searchSignature = Microsoft.VisualBasic.Interaction.InputBox("Введіть сигнатуру для пошуку:", "Пошук сигнатури");

            if (ulong.TryParse(searchSignature, out ulong sig))
            {
                if (File.Exists(dictionaryPath))
                {
                    var matches = File.ReadLines(dictionaryPath)
                                      .Where(line => line.EndsWith($":{searchSignature}")) // Ищем точное совпадение сигнатуры
                                      .ToList();

                    if (matches.Count > 0)
                    {
                        string result = "🔍 Знайдені файли:\n\n";
                        foreach (var line in matches)
                        {
                            int lastColonIndex = line.LastIndexOf(':'); // Находим последнее вхождение ':'
                            if (lastColonIndex > 2) // Проверяем, чтобы не обрезать диск (C:)
                            {
                                string filePath = line.Substring(0, lastColonIndex);
                                string signature = line.Substring(lastColonIndex + 1);

                                result += $"📂 Назва файлу: {Path.GetFileName(filePath)}\n" +
                                          $"📍 Шлях: {filePath}\n" +
                                          $"🔑 Сигнатура: {signature}\n\n";
                            }
                        }
                        MessageBox.Show(result, "Результат пошуку", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Файл не знайдено!", "Результат пошуку", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Некоректне значення сигнатури!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            var deleteItem = new ToolStripMenuItem("Видалити запис");
            deleteItem.Click += DeleteSelectedItem;
            contextMenu.Items.Add(deleteItem);
            listBox1.ContextMenuStrip = contextMenu;
        }

        private void DeleteSelectedItem(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedEntry = listBox1.SelectedItem.ToString();
                listBox1.Items.Remove(selectedEntry);

                // Удаление строки из файла
                var lines = File.ReadAllLines(dictionaryPath).Where(line => line != selectedEntry);
                File.WriteAllLines(dictionaryPath, lines);

                MessageBox.Show("Запис успішно видалено!", "Видалення", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
