﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace LunaparkGame
{
    public interface IUpdatable {
        void MyUpdate();
    }
    public class InputFileFormatException : Exception {
        public InputFileFormatException() { }
        public InputFileFormatException(string message) : base(message) { }
        public InputFileFormatException(string message, Exception inner) : base(message, inner) { }
    }

    public class Data {

        List<Image> images;
        public List<AmusementsFactory> initialAmus = new List<AmusementsFactory>();
        public List<PathFactory> initialPaths = new List<PathFactory>();
        public List<MapObjectsFactory> initialOthers = new List<MapObjectsFactory>();
        public Queue<AmusementsFactory> otherAmus = new Queue<AmusementsFactory>();
        public Queue<PathFactory> otherPaths = new Queue<PathFactory>();
        public Queue<MapObjectsFactory> otherOthers = new Queue<MapObjectsFactory>();
       
        public Data(Image[] otherLoadedImages) {
            images = new List<Image>(otherLoadedImages);
        }
        public void LoadAmus(System.IO.StreamReader r) {
            string line;
            int count = 1;
            while ((line = r.ReadLine()) != null) {
                ProcessLine<AmusementsFactory>(line, initialAmus, otherAmus, count);
                count++;
            }
        }
        public void LoadPaths(System.IO.StreamReader r) {
            string line;
            int count = 1;
            while ((line = r.ReadLine()) != null) {
                ProcessLine<PathFactory>(line, initialPaths, otherPaths, count);
                count++;
            }
        }

        public void LoadOthers(System.IO.StreamReader r) {
            string line;
            int count = 1;
            while ((line = r.ReadLine()) != null) {
                ProcessLine<MapObjectsFactory>(line, initialOthers, otherOthers, count);
                count++;
            }
        }

        private void ProcessLine<T>(string line, List<T> list, Queue<T> queue, int lineCount) {
            line=line.Trim();
            if (line == "" || line[0] == '#') return;
            string[] parts = line.Split('|');
            if (parts.Length != 5) throw new InputFileFormatException("Wrong count of columns, line: " + lineCount);
            // parts[0] is not important, parts[1]...type of Factory, parts[2]...args, parts[3]...image, parts[4]...visible at start

            Image im = (Image)Properties.Images.ResourceManager.GetObject(parts[3].Trim());
            
            object[] args = GetArgs(parts[2], lineCount);
                     
            bool visible;
            if (!Boolean.TryParse(parts[4], out visible)) throw new InputFileFormatException("Wrong format of the column Visible, line: " + lineCount);
         
            try {
                System.Runtime.Remoting.ObjectHandle item = Activator.CreateInstance("LunaparkGame", "LunaparkGame." + parts[1].Trim(), false, BindingFlags.Default, null, args, null, null);
                object o = item.Unwrap();
                int typeId = images.Count;
                ((MapObjectsFactory)o).internTypeId = typeId;
                images.Add(im);
                if (visible) list.Add((T)o);
                else queue.Enqueue((T)o);
            }
           /* catch (TypeLoadException) {
                throw new InputFileFormatException("The given type is not valid, line: " + lineCount);
            }*/
            catch (MissingMethodException) {
                throw new InputFileFormatException("Wrong args, line: " + lineCount);
            }
            catch (MethodAccessException) {
                throw new InputFileFormatException("Wrong args, line: " + lineCount);
            }

        }
        private object[] GetArgs(string argsLine, int lineCount) {
            string[] args = argsLine.Split(',');
            List<object> list = new List<object>();
            try {
                foreach (var arg in args) {
                    char c = arg[0];
                    int i = 1;
                    while (char.IsWhiteSpace(c)) { c = arg[i]; i++; }
                    if (c != '[') throw new InputFileFormatException("Wrong format of arguments, line: " + lineCount);
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    c = arg[i];
                    while (c != ']') { sb.Append(c); i++; c = arg[i]; }
                    i++; c = arg[i];
                    string type = sb.ToString();
                    while (char.IsWhiteSpace(c)) { i++; c = arg[i]; }
                    sb.Clear();
                    i++;
                    while (!char.IsWhiteSpace(c) && c != '(' && i < arg.Length) { sb.Append(c); c = arg[i]; i++; }
                    if (i == arg.Length && !char.IsWhiteSpace(c) && c != '(') sb.Append(c);
                    if (type == "int" || type == "Int32" || type == "System.Int32") list.Add(Int32.Parse(sb.ToString()));
                    else if (type == "string" || type == "String" || type == "System.String") list.Add(sb.ToString());
                    else if (type == "byte" || type == "Byte") list.Add(Byte.Parse(sb.ToString()));
                    else if (type == "bool" || type == "Boolean") list.Add(Boolean.Parse(sb.ToString()));
                    else if (type == "resx") {
                        string s, r;
                        s = sb.ToString().Replace('_', ' ');
                        if ((r = Properties.Images.ResourceManager.GetString(s)) == null) {
                            throw new InputFileFormatException("Given resources do not exist, line: " + lineCount);
                        }
                        else list.Add(r);
                    }
                    else throw new InputFileFormatException("Uncompatible type of an argument in Arguments, line:" + lineCount);
                }
                
                return list.ToArray();
            }
            catch (IndexOutOfRangeException) {
                throw new InputFileFormatException("Wrong format of arguments, line: " + lineCount);
            }
        }
        public Image[] GetImages() {
            Image[] ia = images.ToArray();
            return ia;
        }
        public int GetItemsCount() { 
            return images.Count;
        }
    }
    public class ExponentialRandom {
        public double lambda { get; set; }
        Random rand;
        public ExponentialRandom() {
            rand = new Random();
        }
        public ExponentialRandom(int seed) {
            rand = new Random(seed);
        }

        public double NextDouble() {
            return (Math.Log(1 - rand.NextDouble()) * (-lambda));
        }
        public int NextInt() {
            return (int)(0.5 + Math.Log(1 - rand.NextDouble()) * (-lambda));//0.5 due to rounding (zaokrouhlovani)
        }
    }
    public class ProbabilityGenerationPeople { //predpoklada, ze se brana pta jednou za 1s, mozna pridat do konstruktoru
        private ExponentialRandom expRnd;
        private Random rnd;
        private int waitingTime = 0;
        private Model model;
        System.IO.StreamWriter DEBUGwriter = new System.IO.StreamWriter("exponential.txt");

        public ProbabilityGenerationPeople(Model model) {
            expRnd = new ExponentialRandom();
            rnd = new Random();
            this.model = model;

        }
       
        public bool ShouldCreateNewPerson() { 
            waitingTime--;
            if (waitingTime > 0) return false;
            int variousItems = 0;
            foreach (var i in model.currBuildedItems) if (i > 0) variousItems++;
            
            int propagation = Math.Min(model.propagation, 100);
            int minCount = (int)(model.persList.contenment / 2 + variousItems * 4 + propagation / 2);
            if (model.CurrPeopleCount < minCount) {
                waitingTime = rnd.Next((int)(model.gate.entranceFee/Gate.originalFee * 2));
            }
            else
            {
                double variety = variousItems / model.currBuildedItems.Length * 100;
                double contenment = model.persList.contenment;
                double fee = Gate.originalFee / (model.gate.entranceFee + 1) * 100;
                double awards = model.effects.awardsCount / SpecialEffects.maxAwardsCount * 100;
                double peopleCount = (1000 - Math.Min(model.CurrPeopleCount, 1000)) / 10;
                               
                expRnd.lambda = -((2 * contenment + 4 * Math.Max(propagation, variety) + 2 * fee + propagation + variety + awards + 5 * peopleCount) / 16 - 100) / 4;      
                waitingTime = expRnd.NextInt();
                DEBUGwriter.WriteLine("lambda: " + expRnd.lambda + "  time: " +waitingTime);               
            }
            return true;
        }
       /* private double CalculateLambda(int variousItems, int propagation) {
                     
            double variety = variousItems / model.currBuildedItems.Length * 100;
            propagation = Math.Min(model.propagation, 100);
            double contenment = model.persList.contenment;
            double fee = Gate.originalFee / (model.gate.entranceFee + 1) * 100;
            double awards = model.effects.awardsCount / SpecialEffects.maxAwardsCount * 100;
            double peopleCount = (1000 - Math.Min(model.CurrPeopleCount, 1000)) / 10;

            return -((2 * contenment + 4 * Math.Max(propagation, variety) + 2 * fee + propagation + variety + awards + 5 * peopleCount) / 16 - 100) / 4;
        }*/
    }
      


    static class Program
    {
        public static void SaveToFile(Model model, View2 view, System.IO.FileStream file) {

           BinaryFormatter binF = new BinaryFormatter();
           object[] args = { model, view };
            //binF.Serialize(file, args);
           binF.Serialize(file, model);
        
        }      
      
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           //  System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("cs-CZ");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            StartForm s=new StartForm();
            Application.Run(s);
           /* Data data = new Data(null);
            System.IO.StreamReader sr = new System.IO.StreamReader("amusements.txt"); 
           // System.IO.StreamReader sr = new System.IO.StreamReader("amusementsInitial.txt");   
            data.LoadAmus(sr);
            sr.Close();
            sr = new System.IO.StreamReader("paths.txt");
            data.LoadPaths(sr);
            sr.Close();*/
            

           // Application.Run(new MainForm(s.width,s.height,data));
            Application.Run(new MainForm(s.width,s.height));
           // Application.Run(new MainForm());
            
        }
    }
}
