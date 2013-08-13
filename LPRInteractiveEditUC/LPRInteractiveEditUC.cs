using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ApplicationDataClass;
using System.Threading;
using Utilities;

namespace LPRInteractiveEditUC
{
    public partial class LPRInteractiveEditUC : UserControl
    {
        public LPRInteractiveEditUC(APPLICATION_DATA appData)
        {
            InitializeComponent();
            m_AppData = appData;
            m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);

            InitCharPictureBoxes();
            m_CommandsQ = new ThreadSafeQueue<COMMAND_DATA>(60);
            m_ProcessCommandsThread = new Thread(ProcessCommandsLoop);
            m_ProcessCommandsThread.Start();
        }

        ThreadSafeQueue<COMMAND_DATA> m_CommandsQ;
        APPLICATION_DATA m_AppData;
        PictureBox[] characterPBs;
        TextBox[] charResultTextBoxes;
        bool m_Stop = false;
        void Stop()
        {
            m_Stop = true;
        }

        Thread m_ProcessCommandsThread;

        enum COMMANDS {POST_PICUTRE, POST_CHAR_IMAGE, POST_HISTOGRAM }
        class COMMAND_DATA
        {
            public COMMANDS command;
            public string label;
            public Bitmap bmp;
            public int cIndex;
            public string charString;
            public string logString;
        }

        void ProcessCommandsLoop()
        {
            while (!m_Stop)
            {
                Thread.Sleep(1);

                try
                {
                    COMMAND_DATA cmd = m_CommandsQ.Dequeue();
                    if (cmd == null) continue;

                    switch (cmd.command)
                    {
                        case COMMANDS.POST_CHAR_IMAGE:

                            this.BeginInvoke((MethodInvoker)delegate { characterPBs[cmd.cIndex].Image = cmd.bmp; });
                            this.BeginInvoke((MethodInvoker)delegate { charResultTextBoxes[cmd.cIndex].Text = cmd.charString; });

                            break;

                        case COMMANDS.POST_PICUTRE:

                            this.BeginInvoke((MethodInvoker)delegate { pictureBoxPlateDisplay.Image = cmd.bmp; });
                            this.BeginInvoke((MethodInvoker)delegate { labelPlateNumbers.Text = cmd.label; });

                            break;

                        case COMMANDS.POST_HISTOGRAM:

                            this.BeginInvoke((MethodInvoker)delegate { pictureBoxHistogram.Image = cmd.bmp; });
                            this.BeginInvoke((MethodInvoker)delegate { labelHistoString.Text = cmd.label; });
                            break;

                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message+",  "+ex.StackTrace); }
            }
        }

        public void PostPicture (Bitmap bmp, string label)
        {
            COMMAND_DATA cmd = new COMMAND_DATA();
            cmd.command = COMMANDS.POST_PICUTRE;
            cmd.bmp = bmp;
            cmd.label = label;
            m_CommandsQ.Enqueue(cmd);
        }

        public void PostHistogram(Bitmap bmp, string label)
        {
            COMMAND_DATA cmd = new COMMAND_DATA();
            cmd.command = COMMANDS.POST_HISTOGRAM;
            cmd.bmp = bmp;
            cmd.label = label;
            m_CommandsQ.Enqueue(cmd);
        }

    

        public void PostCharImage(Bitmap bmp, int cindex, string c)
        {
            if (cindex < 0 || cindex > m_AppData.MAX_DISPLAY_CHARS) return;
            COMMAND_DATA cmd = new COMMAND_DATA();
            cmd.command = COMMANDS.POST_CHAR_IMAGE;
            cmd.bmp = bmp;
            cmd.charString = c;
            cmd.cIndex = cindex;
            m_CommandsQ.Enqueue(cmd);
        }


        void InitCharPictureBoxes()
        {
            int pbXOffset = 80;

           
            charResultTextBoxes = new TextBox[m_AppData.MAX_DISPLAY_CHARS];
            characterPBs = new PictureBox[m_AppData.MAX_DISPLAY_CHARS];
            for (int i = 0; i < m_AppData.MAX_DISPLAY_CHARS; i++)
            {
                characterPBs[i] = new PictureBox();

                characterPBs[i].Size = new Size(60, 80);
            //    characterPBs[i].Size = new Size(20, 40);
                characterPBs[i].BackColor = Color.Black;
                characterPBs[i].SizeMode = PictureBoxSizeMode.StretchImage;
                characterPBs[i].Location = new Point(150+(i * pbXOffset), 200);
                
                
                charResultTextBoxes[i] = new TextBox();
                charResultTextBoxes[i].TabIndex = i;
                charResultTextBoxes[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                charResultTextBoxes[i].Location = new Point(150 + (i * pbXOffset), 300);
                charResultTextBoxes[i].Size = new Size(40, 40);
                charResultTextBoxes[i].Text = "";
                charResultTextBoxes[i].TextChanged += new EventHandler(LPRInteractiveEditUC_TextChanged);
             
                this.Controls.Add(characterPBs[i]);
                this.Controls.Add(charResultTextBoxes[i]);

            }
        }

        public string GetCurrentPlateString()
        {
            return labelPlateNumbers.Text;
        }
     

        void LPRInteractiveEditUC_TextChanged(object sender, EventArgs e)
        {
           

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < m_AppData.MAX_DISPLAY_CHARS; i++)
            {
                charResultTextBoxes[i].Text = charResultTextBoxes[i].Text.ToUpper();

                sb.Append(charResultTextBoxes[i].Text);
            }

            labelPlateNumbers.Text = sb.ToString();
        }

        bool makePBFullSize = false;
        Size PlateDisplaySize;
        Point PlateDisplayLocation;
        private void pictureBoxPlateDisplay_Click(object sender, EventArgs e)
        {
            if (!makePBFullSize)
            {
                // make it big

                PlateDisplayLocation = pictureBoxPlateDisplay.Location;
                PlateDisplaySize = pictureBoxPlateDisplay.Size;

                makePBFullSize = true;

                pictureBoxPlateDisplay.Size = this.Size;
                pictureBoxPlateDisplay.Location = new Point(0, 0);
                pictureBoxPlateDisplay.Invalidate();
            }
            else
            {
                // make it small

                pictureBoxPlateDisplay.Location = PlateDisplayLocation;
                pictureBoxPlateDisplay.Size= PlateDisplaySize;

                makePBFullSize = false;

                pictureBoxPlateDisplay.Invalidate();
            }

        }

        bool makeHistoPBFullSize = false;
        Size HistogramSize;
        Point HistogramLocation;
        private void pictureBoxHistogram_Click(object sender, EventArgs e)
        {
            if (!makeHistoPBFullSize)
            {
                // make it big

                HistogramLocation = pictureBoxHistogram.Location;
                HistogramSize = pictureBoxHistogram.Size;

                makeHistoPBFullSize = true;

                pictureBoxHistogram.Size = this.Size;
                pictureBoxHistogram.Location = new Point(0, 0);
                pictureBoxHistogram.Invalidate();
            }
            else
            {
                // make it small

                pictureBoxHistogram.Location = HistogramLocation;
                pictureBoxHistogram.Size = HistogramSize;

                makeHistoPBFullSize = false;

                pictureBoxHistogram.Invalidate();
            }

        }

    }
}
