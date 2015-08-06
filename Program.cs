using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

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
    
    static class Program
    {
       // public enum Direction { N, S, W, E, no };//smer


        public class ExponentialRandom
        {
            public double lambda { get; set; }
            Random rand;
            public ExponentialRandom()
            {
                rand = new Random();
            }
            public ExponentialRandom(int seed)
            {
                rand = new Random(seed);
            }

            public double NextDouble()
            {
                return (Math.Log(1 - rand.NextDouble()) * (-lambda));
            }
            public int NextInt()
            {
                return (int)(0.5 + Math.Log(1 - rand.NextDouble()) * (-lambda));//0.5 due to rounding (zaokrouhlovani)
            }
        }

        public class ProbabilityGenerationPeople
        { //predpoklada, ze se brana pta jednou za 1s, mozna pridat do konstruktoru
            private ExponentialRandom expRnd;
            private int waitingTime = 0;

            public ProbabilityGenerationPeople()
            {
                expRnd = new ExponentialRandom();
            }
            public bool ShouldGenerateNewPerson()
            { //todo: sem patri vsechny faktory, ktere ovlivnuji tvorbu lidi
                waitingTime--;
                if (waitingTime < 0)
                {
                    expRnd.lambda = CalculateLambda();
                    waitingTime = expRnd.NextInt();
                    return true;
                }
                return false;
            }
            private double CalculateLambda()
            {//todo: stejne parametry jako ShouldGenerateNewPerson
                throw new NotImplementedException();
            }
        }
    
        class Data{

            Dictionary<string, Image> images = new Dictionary<string, Image>();
            List<AmusementsFactory> initialAmus = new List<AmusementsFactory>();
            List<PathFactory> initialPaths = new List<PathFactory>();
            List<MapObjectsFactory> initialOthers = new List<MapObjectsFactory>();
            Queue<AmusementsFactory> otherAmus = new Queue<AmusementsFactory>();
            Queue<PathFactory> otherPaths = new Queue<PathFactory>();
            Queue<MapObjectsFactory> otherOthers = new Queue<MapObjectsFactory>();

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
                string[] parts = line.Split('#');
                if (parts.Length != 5) throw new InputFileFormatException("Wrong count of columns, line: "+lineCount);
                object[] args = GetArgs(parts[2], lineCount);

                bool visible;
                if (!Boolean.TryParse(parts[4], out visible)) throw new InputFileFormatException("Wrong format of the column Visible, line: "+lineCount);

                Image im = (Image)Properties.Images.ResourceManager.GetObject(parts[3].Trim()); 
                string name = parts[0].Trim();
                try {
                    System.Runtime.Remoting.ObjectHandle item = Activator.CreateInstance("LunaparkGame", "LunaparkGame." + parts[1].Trim(), false, BindingFlags.Default, null, args, null, null);
                    object o = item.Unwrap();
                    if (visible) list.Add((T)o);
                    else queue.Enqueue((T)o);
                }
                catch (TypeLoadException) {
                    throw new InputFileFormatException("The given type is not valid, line: " + lineCount);
                }
                catch (MissingMethodException) {
                    throw new InputFileFormatException("Wrong args, line: " + lineCount);
                }
                catch (MethodAccessException) {
                    throw new InputFileFormatException("Wrong args, line: " + lineCount);
                }
            
            }
            private object[] GetArgs(string argsLine, int lineCount) {
                string[] args = argsLine.Split(',');
                List<object> list=new List<object>();
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
                        if (i == arg.Length) sb.Append(c);
                        if (type == "int" || type == "Int32" || type == "System.Int32") list.Add(Int32.Parse(sb.ToString()));
                        else if (type == "string" || type == "String" || type == "System.String") list.Add(sb.ToString());
                        else throw new InputFileFormatException("Uncompatible type of an argument in Arguments, line:" + lineCount);
                    }
                    return list.ToArray();
                }
                catch (IndexOutOfRangeException) {
                    throw new InputFileFormatException("Wrong format of arguments, line: " + lineCount);
                }
            }
           
        }
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
             System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("cs-CZ");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            StartForm s=new StartForm();
            Application.Run(s);
            System.IO.StreamReader sr = new System.IO.StreamReader("amusements.txt");
            Data d = new Data();
            d.LoadAmus(sr);
            sr.Close();
            Application.Run(new MainForm(s.width,s.height));
            
           // Application.Run(new MainForm());
            
        }
    }
}
