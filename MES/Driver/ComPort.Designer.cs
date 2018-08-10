namespace MES.Driver
{
    partial class ComPort
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerHour = new System.Windows.Forms.Timer(this.components);
            this.timerSet = new System.Windows.Forms.Timer(this.components);
            this.timerCollect = new System.Windows.Forms.Timer(this.components);
            this.comm = new System.IO.Ports.SerialPort(this.components);
            this.SuspendLayout();
            // 
            // timerHour
            // 
            this.timerHour.Interval = 3600000;
            this.timerHour.Tick += new System.EventHandler(this.HourCollect);
            // 
            // timerSet
            // 
            this.timerSet.Interval = 30000;
            this.timerSet.Tick += new System.EventHandler(this.TimeRise);
            // 
            // timerCollect
            // 
            this.timerCollect.Interval = 300000;
            this.timerCollect.Tick += new System.EventHandler(this.TimeWorking);
            // 
            // comm
            // 
            this.comm.BaudRate = 115200;
            this.comm.PortName = "COM3";
            this.comm.ReceivedBytesThreshold = 16;
            this.comm.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.comm_DataReceived);
            // 
            // ComPort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "ComPort";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerHour;
        private System.Windows.Forms.Timer timerSet;
        private System.Windows.Forms.Timer timerCollect;
        private System.IO.Ports.SerialPort comm;


    }
}