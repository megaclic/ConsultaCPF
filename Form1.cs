using RetaguardaClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsultaCPF
{
    public partial class Form1 : Form
    {
        ConsultaCPFReceitaClass consulta = new ConsultaCPFReceitaClass();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = consulta.RecuperaCaptcha();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = consulta.Consulta(txtCPF.Text, txtTexto_captcha_serpro_gov_br.Text);
            if(result == null)
            {
                MessageBox.Show(consulta.ErroDetectado);
            }
            else
            {
                MessageBox.Show(result["Nome"]);
            }
        }
    }
}
