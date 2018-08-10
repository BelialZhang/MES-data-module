using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.RegularExpressions;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Threading;

namespace MES.Driver
{
    public partial class ComPort : Form
    {
        Utils.DBManager DataControl = new Utils.DBManager();
        List<byte> buffer = new List<byte>();
        Utils.DTConversion IntToBcd = new Utils.DTConversion();
        Utils.DTConversion BcdToInt = new Utils.DTConversion();
        Utils.MessageSave DataSave = new Utils.MessageSave();
        Utils.DebugText WriteDebug = new Utils.DebugText();
        int[] correlation = new int[4];
        int[] counts = new int[4];
        public ComPort()
        {
            InitializeComponent();
        }
        public void delay()//开机隐藏窗口
        {
            this.Hide();
        }
        //初始化程序加载
        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("程序加载");
            MySqlConnection myCon = Utils.DBManager.DataConn();
            //调用界面隐藏
            this.BeginInvoke(new ThreadStart(delay));
            //初始化串口数据
            comm.PortName = "COM3";
            comm.BaudRate =int.Parse("115200");
            comm.ReceivedBytesThreshold = 1;
            comm.StopBits = System.IO.Ports.StopBits.One;
            try
            {
                comm.Open();
            }
            catch(Exception ex)
            {
                MessageBox.Show("串口异常" + ex.Message);
            }
            //计时器启动
            timerHour.Enabled = true;
            timerSet.Enabled = true;
            timerCollect.Enabled = true;
            //初始化数据列表
            Console.WriteLine("初始化数据列表");
            DataControl.Initial(myCon);
        }
        //定时汇总
        private void HourCollect(object sender, EventArgs e)
        {
            DataControl.HourCollect();
        }
        //定时检测机器是否在运行
        private void TimeRise(object sender, EventArgs e)
        {
            DataControl.Timing();
        }
        //机器工作时间累加
        private void TimeWorking(object sender, EventArgs e)
        {
            MySqlConnection myCon = Utils.DBManager.DataConn();
            DataControl.TimeWork(myCon);
            //DataControl.TimeWork();
        }
        //报文接收处理
        void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(100);//延时200毫秒
            //报文头       命令行  关联号       机器号    校验位
            //0xfa 0xfb    0x01    0x23 0x10    0x12 0x34   0x00
            //报文头       命令行  回复   状态号   返还时间    校验位
            //0xfa 0xfb    0x02    0x23   0x10     0x12 0x34   0x00
            byte[] M_sendbuf = new byte[15];//返还关联报文
            //接收报文长度
            int num = comm.BytesToRead;
            //接收报文并写入M_buf数组
            byte[] M_buf = new byte[num];//接收报文
            for (int i = 0; i < num; i++)
            {
                //M_receive[i] = (byte)comm.ReadByte();
                M_buf[i] = (byte)comm.ReadByte();
            }
            DataSave.WriteToText(M_buf);
            /********************************************************/
            //将当前接收到的报文存入处理数组buffer中
            buffer.AddRange(M_buf);
            //释放串口接收数据
            comm.DiscardInBuffer();
            //校验位判别
            byte check = 0x00;
            while(buffer.Count >= 13)//完整报文长度为13
            {
                List<byte> M_receive = new List<byte>();
                //MySqlConnection myCon = Utils.DBManager.DataConn();
                if(buffer[0]==0xFA&&buffer[1]==0xFB)//报文头一致
                {
                    //累加校验
                    for(int i=0;i<12;i++)
                    {
                        check += buffer[i];
                        M_receive.Add(buffer[i]);
                    }
                    //检验位判断通过
                    if(check == buffer[12])
                    {
                        M_receive.Add(buffer[12]);
                        /****************************/
                        Console.WriteLine("校验通过");
                        Console.WriteLine();
                        //校验通过
                        switch (M_receive[2])
                        {
                            case 0x01://收到刷卡报文
                                //报文处理
                                Console.WriteLine("*****收到刷卡报文******");
                                string correlation_no = DataControl.DelData(M_receive);
                                //回复报文
                                M_sendbuf[0] = 0xFA;
                                M_sendbuf[1] = 0xFB;
                                //命令行
                                M_sendbuf[2] = 0x01;
                                //机器号
                                M_sendbuf[3] = M_receive[3];
                                M_sendbuf[4] = M_receive[4];
                                //关联号转换
                                correlation[0] = Convert.ToInt32(correlation_no) / 1000000;
                                correlation[1] = Convert.ToInt32(correlation_no) / 10000 - correlation[0] * 100;
                                correlation[2] = Convert.ToInt32(correlation_no) / 100 - (correlation[0] * 10000 + correlation[1] * 100);
                                correlation[3] = Convert.ToInt32(correlation_no) % 100;
                                M_sendbuf[5] = IntToBcd.IntToBCDByte(correlation[0]);
                                M_sendbuf[6] = IntToBcd.IntToBCDByte(correlation[1]);
                                M_sendbuf[7] = IntToBcd.IntToBCDByte(correlation[2]);
                                M_sendbuf[8] = IntToBcd.IntToBCDByte(correlation[3]);
                                //穴数
                                M_sendbuf[9] = IntToBcd.IntToBCDByte(DataControl.GetHole(M_receive));
                                //校验位累加
                                M_sendbuf[10] = 0x00;//初始化
                                for (int i = 0; i < 10; i++)
                                {
                                    M_sendbuf[10] += M_sendbuf[i];
                                }

                                comm.Write(M_sendbuf, 0, 11);
                                Console.WriteLine("报文发送");
                                break;

                            case 0x02://收到生产报文
                                //报文处理,生产报文不需要返回数据库信息，因此不需要设置接收对象
                                Console.WriteLine("-----收到生产报文-----");
                                DataControl.DelData(M_receive);
                                //回复报文
                                M_sendbuf[0] = 0xFA;
                                M_sendbuf[1] = 0xFB;
                                //命令行
                                M_sendbuf[2] = 0x02;
                                //关联号
                                M_sendbuf[3] = M_receive[3];
                                M_sendbuf[4] = M_receive[4];
                                M_sendbuf[5] = M_receive[5];
                                M_sendbuf[6] = M_receive[6];
                                //回复ok
                                M_sendbuf[7] = 0x01;
                                //状态号
                                M_sendbuf[8] = M_receive[7];
                                //穴数
                                M_sendbuf[9] = IntToBcd.IntToBCDByte(DataControl.GetHole(M_receive));
                                M_sendbuf[10] = 0x00;
                                //校验位累加
                                for (int i = 0; i < 10; i++)
                                {
                                    M_sendbuf[10] += M_sendbuf[i];
                                }
                                comm.Write(M_sendbuf, 0, 11);
                                Console.WriteLine("生产报文发送");
                                break;
                            case 0x03://收到掉电报文
                                //获取生产数
                                Console.WriteLine("=====收到掉电报文=====");
                                string get_count = "0";
                                get_count = DataControl.DelData(M_receive);
                                //回复报文
                                M_sendbuf[0] = 0xFA;
                                M_sendbuf[1] = 0xFB;
                                //命令行
                                M_sendbuf[2] = 0x03;
                                //关联号
                                M_sendbuf[3] = M_receive[3];
                                M_sendbuf[4] = M_receive[4];
                                M_sendbuf[5] = M_receive[5];
                                M_sendbuf[6] = M_receive[6];
                                //回复ok
                                M_sendbuf[7] = 0x01;
                                //状态号
                                M_sendbuf[8] = M_receive[7];
                                //生产数转换
                                try
                                {
                                    counts[0] = Convert.ToInt32(get_count) / 1000000;
                                    counts[1] = Convert.ToInt32(get_count) / 10000 - counts[0] * 100;
                                    counts[2] = Convert.ToInt32(get_count) / 100 - (counts[0] * 10000 + counts[1] * 100);
                                    counts[3] = Convert.ToInt32(get_count) % 100;
                                }
                                catch(Exception ex)
                                {
                                    WriteDebug.WriteToText(ex);
                                    Console.WriteLine(ex.Message);
                                }
                                M_sendbuf[9] = IntToBcd.IntToBCDByte(counts[0]);
                                M_sendbuf[10] = IntToBcd.IntToBCDByte(counts[1]);
                                M_sendbuf[11] = IntToBcd.IntToBCDByte(counts[2]);
                                M_sendbuf[12] = IntToBcd.IntToBCDByte(counts[3]);
                                //穴数
                                M_sendbuf[13] = IntToBcd.IntToBCDByte(DataControl.GetHole(M_receive));
                                Console.Write("穴数：" + M_sendbuf[13]);
                                M_sendbuf[14] = 0x00;
                                //校验位累加
                                for (int i = 0; i < 14; i++)
                                {
                                    M_sendbuf[14] += M_sendbuf[i];
                                }
                                Console.WriteLine("校验位：" + M_sendbuf[14].ToString());
                                comm.Write(M_sendbuf, 0, 15);
                                Console.WriteLine("掉电报文发送");
                                break;

                        }
                        check = 0x00;
                        comm.DiscardOutBuffer();
                        buffer.RemoveRange(0, 13);//移除已处理报文
                        /****************************/
                    }
                    else//校验失败，寻找下一个报文头
                    {
                        Console.WriteLine("报文校验失败");
                        int length =2;
                        int i = 2;
                        do
                        {
                            i++;
                            length++;
                        }
                        while (buffer[i] == 0xFA && buffer[i + 1] == 0xFB);
                        buffer.RemoveRange(0, length);
                    }
                }
                else//报文头有误，删除首位
                {
                    buffer.RemoveAt(0);
                }
            }

        }
    }
}
