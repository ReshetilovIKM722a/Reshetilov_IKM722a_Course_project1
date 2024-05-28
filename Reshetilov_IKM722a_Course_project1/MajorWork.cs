using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Collections;

namespace Reshetilov_IKM722a_Course_project1
{
    class MajorWork
    {

        public bool Modify;
        private string SaveFileName;// ім’я файлу для запису
        private string OpenFileName;// ім’я файлу для читання
        public Stack myStack = new Stack();
        public string[] myArr = new string[100];
        public Queue myQueue = new Queue();
        public string[] smyQueue = new string[100];
        public void WriteSaveFileName(string S)// метод запису даних в об'єкт
        {
            this.SaveFileName = S;// запам'ятати ім’я файлу для запису
        }
        public void WriteOpenFileName(string S)
        {
            this.OpenFileName = S;// запам'ятати ім’я файлу для відкриття
        }

        private System.DateTime TimeBegin; // час початку роботи програми

        private string Data; //вхідні дані

        private string Result; // Поле результату

        private int Key;// поле ключа

        public void SetTime() // метод запису часу початку роботи програми
        {
            this.TimeBegin = System.DateTime.Now;
        }

        public System.DateTime GetTime() // Метод отримання часу завершення програми
        {
            return this.TimeBegin;
        }

        public void Write(string D, string D1, string D2)
        {
            this.Data = D + ' ' + D1 + ' ' + D2;
        }
        public string Read()
        {
            return this.Result;// метод відображення результату
        }

        public void Task()
        {
            try
            {
                    string[] dataParts = Data.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    List<int[]> numberLists = new List<int[]>();
                    List<double> averages = new List<double>();

                    foreach (var str in dataParts)
                    {
                        int[] numbers = str.Select(c => int.Parse(c.ToString())).ToArray();
                        numberLists.Add(numbers);
                        var average = numbers.Average();
                        averages.Add(average);
                    }

                    double maxAverage = averages.Max();
                    this.Result = maxAverage.ToString();
                    this.Modify = true; // Дозвіл запису
            }
            catch
            {
                MessageBox.Show("Порожній рядок", "Помилка!");
            }

        }


        public void SaveToFile() // Запис даних до файлу    
        {
            if (!this.Modify)
                return;
            try
            {
                Stream S;
                if (File.Exists(this.SaveFileName))
                {
                    S = File.Open(this.SaveFileName, FileMode.Append);
                }
                else
                {
                    S = File.Open(this.SaveFileName, FileMode.Create);
                }

                
                string[] dataArray = this.Data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                Buffer D = new Buffer();
                D.Data = dataArray;
                D.Result = Convert.ToString(this.Result);
                D.Key = Key;
                Key++;
                BinaryFormatter BF = new BinaryFormatter();
                BF.Serialize(S, D);
                S.Flush();
                S.Close();
                this.Modify = false;
            }
            catch
            {
                MessageBox.Show("Помилка роботи з файлом");
            }
        }

        public void ReadFromFile(System.Windows.Forms.DataGridView DG) // зчитування з файлу
        {
            try
            {
                if (!File.Exists(this.OpenFileName))
                {
                    MessageBox.Show("Файлу немає"); 
                    return;
                }
                Stream S = File.Open(this.OpenFileName, FileMode.Open); 
                Buffer D;
                object O; 
                BinaryFormatter BF = new BinaryFormatter(); 

                System.Data.DataTable MT = new System.Data.DataTable();
                System.Data.DataColumn cKey = new System.Data.DataColumn("Ключ"); 
                System.Data.DataColumn cInput1 = new System.Data.DataColumn("Вхідні дані 1"); 
                System.Data.DataColumn cInput2 = new System.Data.DataColumn("Вхідні дані 2"); 
                System.Data.DataColumn cInput3 = new System.Data.DataColumn("Вхідні дані 3");
                System.Data.DataColumn cResult = new System.Data.DataColumn("Результат");
                MT.Columns.Add(cKey); 
                MT.Columns.Add(cInput1); 
                MT.Columns.Add(cInput2); 
                MT.Columns.Add(cInput3); 
                MT.Columns.Add(cResult); 

                while (S.Position < S.Length)
                {
                    O = BF.Deserialize(S); 
                    D = O as Buffer;
                    if (D == null) break;

                    System.Data.DataRow MR = MT.NewRow();
                    MR["Ключ"] = D.Key; 

                    string[] dataParts = D.Data[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    MR["Вхідні дані 1"] = dataParts.Length > 0 ? dataParts[0] : null;
                    MR["Вхідні дані 2"] = dataParts.Length > 1 ? dataParts[1] : null;
                    MR["Вхідні дані 3"] = dataParts.Length > 2 ? dataParts[2] : null;

                    MR["Результат"] = D.Result;

                    MT.Rows.Add(MR);
                }
                DG.DataSource = MT;
                S.Close();
            }
            catch
            {
                MessageBox.Show("Помилка файлу"); // Виведення на екран повідомлення "Помилка файлу"
            }

            //ReadFromFile закінчився
        }

        public void Generator() // метод формування ключового поля
        {
            try
            {
                if (!File.Exists(this.SaveFileName)) // існує файл?
                {
                    Key = 1;
                    return;
                }
                Stream S; // створення потоку
                S = File.Open(this.SaveFileName, FileMode.Open); // Відкриття файлу
                Buffer D;
                object O; // буферна змінна для контролю формату
                BinaryFormatter BF = new BinaryFormatter(); // створення елементу для форматування
                while (S.Position < S.Length)
                {
                    O = BF.Deserialize(S);
                    D = O as Buffer;
                    if (D == null) break;
                    Key = D.Key;
                }
                Key++;
                S.Close();
            }
            catch
            {
                MessageBox.Show("Помилка файлу"); // Виведення на екран повідомлення "Помилка файлу"
            }
        }

        public bool SaveFileNameExists()
        {
            if (this.SaveFileName == null)
                return false;
            else return true;
        }

        public void NewRec() // новий запис
        {
            this.Data = ""; // "" - ознака порожнього рядка
            this.Result = null; // для string- null
        }

        public void Find(string Num) // пошук
        {
            int N;
            try
            {
                N = Convert.ToInt16(Num); // перетворення номера рядка в int16 для відображення
            }
            catch
            {
                MessageBox.Show("помилка пошукового запиту"); // Виведення на екран повідомлення "помилка пошукового запиту
                return;
            }

            try
            {
                if (!File.Exists(this.OpenFileName))
                {
                    MessageBox.Show("Файлу немає"); // Виведення на екран повідомлення "файлу немає"

                    return;
                }
                Stream S; // створення потоку
                S = File.Open(this.OpenFileName, FileMode.Open); // відкриття файлу
                Buffer D;
                object O; // буферна змінна для контролю формату
                BinaryFormatter BF = new BinaryFormatter(); // створення об'єкта для форматування

                while (S.Position < S.Length)
                {
                    O = BF.Deserialize(S);
                    D = O as Buffer;
                    if (D == null) break;
                    if (D.Key == N) // перевірка дорівнює чи номер пошуку номеру рядка в таблиці
                    {
                        string ST;
                        ST = "Запис містить: " + (char)13 + "№" + Num + " Вхідні дані: " + D.Data + " Результат: " + D.Result;
                        MessageBox.Show(ST, "Запис знайдена"); // Виведення на екран повідомлення "запис містить", номер, вхідних даних і результат
                        S.Close();
                        return;
                    }
                }
                S.Close();
                MessageBox.Show("Запис не знайдена"); // Виведення на екран повідомлення "Запис не знайдена"
            }
            catch
            {
                MessageBox.Show("Помилка файлу"); // Виведення на екран повідомлення "Помилка файлу"
            }
        } // Find закінчився

        private string SaveTextFileName;// ім'я файлу для запису текстового файлу
        public void WriteSaveTextFileName(string S)
        {
            this.SaveTextFileName = S;
        }

        public bool SaveTextFileNameExists()
        {
            if (this.SaveTextFileName == null)
                return false;
            else return true;
        }

        public string ReadSaveTextFileName()
        {
            return SaveTextFileName;
        }

        public void SaveToTextFile(string name, System.Windows.Forms.DataGridView D)
        {
            try
            {
                System.IO.StreamWriter textFile;
                if (!File.Exists(name))
                {
                    textFile = new System.IO.StreamWriter(name);
                }
                else
                {
                    textFile = new System.IO.StreamWriter(name, true);
                }
                for (int i = 0; i < D.RowCount - 1; i++)
                {
                    textFile.WriteLine("{0};{1};{2}", D[0, i].Value.ToString(), D[1, i].Value.ToString(), D[2, i].Value.ToString());
                }
                textFile.Close();
            }
            catch
            {
                MessageBox.Show("Помилка роботи з файлом ");
            }
        }

        private string OpenTextFileName;
        public void WriteOpenTextFileName(string S)
        {
            this.OpenTextFileName = S;
        }

    }

}