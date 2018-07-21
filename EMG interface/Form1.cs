using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace EMG_interface
{
    public partial class Form1 : Form
    {
        Thread ThreadAquisicao;
        Thread ThreadPlotagem;
        Mutex Mutexbuffer;
        Mutex MutexPlotagem;
        string []sinal = new string[1000];
        int contador = 0;
        string valorlido;
        Boolean rw;
        Boolean plot;

        public void Aquisicao()
        {
            while (true)
            {
                Mutexbuffer.WaitOne();

                if (rw == true)
                {
                    sinal[contador] = serialPort1.ReadLine();
                    //sinal[contador] = contador;
                    rw = false;
                }

                Mutexbuffer.ReleaseMutex();
                Thread.Sleep(200);
            }
        }

        public void Plotagem()
        {
            while (true)
            {
                Mutexbuffer.WaitOne();
                if (rw == false)
                {
                    if (contador < 1000)
                    {
                        valorlido = sinal[contador];
                        contador++;
                    }
                    else if(contador >= 1000)
                    {
                        contador = 0;
                    }
                    
                    rw = true;
                    Mutexbuffer.ReleaseMutex();
                    Thread.Sleep(200);

                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen == false)
            serialPort1.Open();
            ThreadAquisicao = new Thread(Aquisicao);
            ThreadPlotagem = new Thread(Plotagem);
            Mutexbuffer = new Mutex();
            MutexPlotagem = new Mutex();
            plot = true;
            if (ThreadAquisicao.IsAlive != true)
            {
                ThreadAquisicao.Start();
                ThreadPlotagem.Start();
                timer1.Start();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ThreadAquisicao.IsAlive == true)
            {
                ThreadAquisicao.Abort();
                ThreadPlotagem.Abort();
                plot = false;  
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MutexPlotagem.WaitOne();
            if (plot != false && valorlido != null)
            {
                listBox1.Items.Add( valorlido );
            }
            MutexPlotagem.ReleaseMutex();
            timer1.Stop();
            timer1.Start();
        }
    }
}