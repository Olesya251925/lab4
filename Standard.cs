/*****************************
 *   Лабораторная работа      *
 *           № 4              *
 ******************************/
using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;

namespace ConsoleApp1
{
    class Memento
    {
        public string Text { get; set; }
    }
    public interface IOriginator
    {
        object GetMemento();
        void SetMemento(object memento);
    }

    [Serializable]
    public class TxtFile : IOriginator
    {
        public string Text;
        public string Tags;

        public TxtFile() { }

        public TxtFile(string Text, string Tags)
        {
            this.Text = Text;
            this.Tags = Tags;
        }

        public string BinarySerialize()
        {
            string FileName = "Данные файла";
            FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, this);
            fs.Flush();
            fs.Close();
            return FileName;
        }

        public void BinaryDeserialize(string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            TxtFile deserialized = (TxtFile)bf.Deserialize(fs);
            Text = deserialized.Text;
            fs.Close();
        }

        static public string XMLSerialize(TxtFile details)
        {
            string FileName = "Данные XML";
            XmlSerializer serializer = new XmlSerializer(typeof(TxtFile));
            using (TextWriter writer = new StreamWriter(FileName))
            {
                serializer.Serialize(writer, details);
            }
            return FileName;
        }

        static public TxtFile XMLDeserialize(string FileName)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(TxtFile));
            TextReader reader = new StreamReader(FileName);
            object obj = deserializer.Deserialize(reader);
            TxtFile XmlData = (TxtFile)obj;
            reader.Close();
            return XmlData;
        }

        public void PrintText()
        {
            Console.WriteLine(Text);
        }

        object IOriginator.GetMemento()
        {
            return new Memento {Text = this.Text };
        }
        void IOriginator.SetMemento(object memento)
        {
            if (memento is Memento)
            {
                var mem = memento as Memento;
                Text = mem.Text;
            }
        }
    }
    public class Caretaker
    {
        private object memento;
        public void SaveState(IOriginator originator)
        {
            memento = originator.GetMemento();
        }

        public void RestoreState(IOriginator originator)
        {
            originator.SetMemento(memento);
        }
    }

    class FileSearch
    {
        public string FoundFiles = "";
        public void Search(TxtFile[] library, string Request, int numberOfFiles)
        {
            for (int FileNumber = 0; FileNumber < numberOfFiles; ++FileNumber)
            {
                if (library[FileNumber].Tags == Request)
                {
                    FoundFiles += FileNumber + " ";
                }
            }

            if (FoundFiles == "")
            {
                Console.WriteLine("Файлы не обнаружены");
            }
            else
            {
                Console.WriteLine("\nРезультат: ");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            const int NumberOfFiles = 10;
            TxtFile[] Library = new TxtFile[NumberOfFiles];
            TxtFile File;

            File = new TxtFile("Insidious, evil and formidable animal", "Wolf");
            Library[0] = File;
            File = new TxtFile("Cunning, cunning and smart", "Fox");
            Library[1] = File;
            File = new TxtFile("Little black-eyed animals, love nuts", "Squirrel");
            Library[2] = File;
            File = new TxtFile("Yellow-red forest cat with the famous bones on the ears", "Lynx");
            Library[3] = File;
            File = new TxtFile("An animal with a long narrow muzzle and dark stripes stretching from eyes to ears", "Badger");
            Library[4] = File;
            File = new TxtFile("Flexible, agile and strong. Can swim and climb trees.", "Lynx");
            Library[5] = File;
            File = new TxtFile("They bear cubs for a little over a month", "Squirrel");
            Library[6] = File;
            File = new TxtFile("The tail is fluffy and red, sometimes black at the tip", "Fox");
            Library[7] = File;
            File = new TxtFile("When you see this animal, you should not look him in the eye.", "Wolf");
            Library[8] = File;
            File = new TxtFile("They get very fat by the cold - sometimes twice", "Badger");
            Library[9] = File;

            Console.WriteLine("Поиск ключевых слов: ");
            string Request = Convert.ToString(Console.ReadLine());

            FileSearch filesearch = new FileSearch();
            filesearch.Search(Library, Request, NumberOfFiles);
            Console.WriteLine(filesearch.FoundFiles);

            Console.WriteLine("Выберите файл, который хотите отредактировать:");
            int FileNumber = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\nТекст файла:");
            Caretaker ct = new Caretaker();
            Library[FileNumber].PrintText();
            ct.SaveState(Library[FileNumber]);

            Console.WriteLine("\nВведите новый текст файла: ");
            string NewText = Convert.ToString(Console.ReadLine());
            Library[FileNumber].Text = NewText;
            Console.WriteLine("\nСохранить новый текст? " +
                              "\n1 Да" +
                              "\n2 Нет");

            string SaveChoice = Convert.ToString(Console.ReadLine());
            if (SaveChoice == "1")
            {
                Console.WriteLine("\nФайл сохранен. ");
                Library[FileNumber].PrintText();
            }
            else
            {
                ct.RestoreState(Library[FileNumber]);
                Console.WriteLine("\nФайл не удалось сохранить. ");
                Library[FileNumber].PrintText();
            }

            Console.ReadKey();
        }
    }
}
