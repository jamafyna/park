using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LunaparkGame
{
    static class Program
    {
        public enum Direction { N, S, W, E, no };//smer


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
    
        
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            StartForm s=new StartForm();
            Application.Run(s);
            Application.Run(new MainForm(s.width,s.height));
           // Application.Run(new MainForm());
            
        }
    }
}
