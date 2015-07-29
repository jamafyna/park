using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LunaparkGame
{
    class Data //reprezentuje konfiguracni soubor, ktery se mi nepovedlo vytvorit
    {
        public System.Collections.Generic.Dictionary<Type, PictureBox> dict = new Dictionary<Type, PictureBox>();
       
        public Data()
        {
            
        }
        private void Initialize()
        {
            //asphalt path
            PictureBox pbox = new PictureBox();
            pbox.BackgroundImage = Properties.Images.path_asphalt; 
            pbox.Width = MainForm.sizeOfSquare - 1;
            pbox.Height = MainForm.sizeOfSquare - 1;
            pbox.Visible = true;
            dict.Add(typeof(AsphaltPath),pbox);

        }


    }
}
