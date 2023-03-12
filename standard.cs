/*****************************
 *   Лабораторная работа      *
 *             № 4            *
 ******************************/
 using System;
 using System.Text;
 using System.Runtime.Serialization.Formatters.Binary;
 using System.Xml.Serialization;
 using System.IO;
 
 namespace ConsoleApp1 {
   class Memento {
     public string text { get; set; }
   }
   public interface IOriginator {
     object GetMemento();
     void SetMemento(object memento);
   }

   [Serializable]
   public class txtFile: IOriginator {
     public string text;
     public string tags;

     public txtFile() { }

     public txtFile(string text, string tags) {
       this.text = text;
       this.tags = tags;
     }

     public string BinarySerialize() {
       string FileName = "Данные файла";
       FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
       BinaryFormatter bf = new BinaryFormatter();
       bf.Serialize(fs, this);
       fs.Flush();
       fs.Close();
       return FileName;
     }

     public void BinaryDeserialize(string FileName) {
       FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read);
       BinaryFormatter bf = new BinaryFormatter();
       txtFile deserialized = (txtFile)bf.Deserialize(fs);
       text = deserialized.text;
       fs.Close();
     }

     static public string XMLSerialize(txtFile details) {
       string FileName = "Данные XML";
       XmlSerializer serializer = new XmlSerializer(typeof(txtFile));
       using (TextWriter writer = new StreamWriter(FileName)) {
         serializer.Serialize(writer, details);
       }
       return FileName;
     }

     static public txtFile XMLDeserialize(string FileName) {
       XmlSerializer deserializer = new XmlSerializer(typeof(txtFile));
       TextReader reader = new StreamReader(FileName);
       object obj = deserializer.Deserialize(reader);
       txtFile XmlData = (txtFile)obj;
       reader.Close();
       return XmlData;
     }

     public void PrintText() {
       Console.WriteLine(text);
     }

     object IOriginator.GetMemento() {
       return new Memento { text = this.text };
     }
     void IOriginator.SetMemento(object memento) {
       if (memento is Memento) {
         var mem = memento as Memento;
         text = mem.text;
       }
     }
   }
   public class Caretaker {
     private object memento;
     public void SaveState(IOriginator originator) {
       memento = originator.GetMemento();
     }

     public void RestoreState(IOriginator originator) {
       originator.SetMemento(memento);
     }
   }

   class FileSearch {
     public string FoundFiles = "";
     public void Search(txtFile[] library, string Request, int numberOfFiles) {
       for (int FileNumber = 0; FileNumber < numberOfFiles; ++FileNumber) {
         if (library[FileNumber].tags == Request) {
           FoundFiles += FileNumber + " ";
         }
       }

       if (FoundFiles == "") {
         Console.WriteLine("Файлы не обнаружены");
       } else {
         Console.WriteLine("\nРезультат: ");
       }
     }
   }

   class Program {
     static void Main(string[] args) {

       const int NumberOfFiles = 10;
       txtFile[] Library = new txtFile[NumberOfFiles];
       txtFile file;

       file = new txtFile("Insidious, evil and formidable animal", "Wolf");
       Library[0] = file;
       file = new txtFile("Cunning, cunning and smart", "Fox");
       Library[1] = file;
       file = new txtFile("Little black-eyed animals, love nuts", "Squirrel");
       Library[2] = file;
       file = new txtFile("Yellow-red forest cat with the famous bones on the ears", "Lynx");
       Library[3] = file;
       file = new txtFile("An animal with a long narrow muzzle and dark stripes stretching from eyes to ears", "Badger");
       Library[4] = file;
       file = new txtFile("Flexible, agile and strong. Can swim and climb trees.", "Lynx");
       Library[5] = file;
       file = new txtFile("They bear cubs for a little over a month", "Squirrel");
       Library[6] = file;
       file = new txtFile("The tail is fluffy and red, sometimes black at the tip", "Fox");
       Library[7] = file;
       file = new txtFile("When you see this animal, you should not look him in the eye.", "Wolf");
       Library[8] = file;
       file = new txtFile("They get very fat by the cold - sometimes twice", "Badger");
       Library[9] = file;

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
       Library[FileNumber].text = NewText;
       Console.WriteLine("\nСохранить новый текст? " +
                         "\n1 Да" +
                         "\n2 Нет");

       string SaveChoice = Convert.ToString(Console.ReadLine());
       if (SaveChoice == "1") {
         Console.WriteLine("\nФайл сохранен. ");
         Library[FileNumber].PrintText();
       } else {
         ct.RestoreState(Library[FileNumber]);
         Console.WriteLine("\nФайл не удалось сохранить. ");
         Library[FileNumber].PrintText();
       }

       Console.ReadKey();
     }
   }
 }
