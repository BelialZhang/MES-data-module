using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace MES.Utils
{
    class MessageSave
    {
        //接收报文存储
        Utils.DTConversion conversion = new DTConversion();
        public void WriteToText(byte[] M_receive)
        {
            //文件名以日期小时为文件名
            string FileURL = @"./过程报文存储/" + DateTime.Now.ToString("yyyy年MM月dd日 HH时").ToString() + ".txt";
            string mesData = DateTime.Now.ToString()+":\tFA FB ";
            Console.WriteLine("越界数值"+M_receive.GetLength(0));
            //StreamWriter sw;
            //将接收的报文存储到mesData字符串中，便于文件写入
            for(int i =2;i<M_receive.GetLength(0);i++)
            {
                
                mesData +=  conversion.BcdToIntString(M_receive[i]) + " ";
            }
            if (!File.Exists(FileURL))//文件不存在则创建新文件
            {

                //File.Create(FileURL);
                string Example = "时间\t\t\t报文"+Environment.NewLine;
                File.WriteAllText(FileURL, Example);
            }
            mesData += Environment.NewLine;
            File.AppendAllText(FileURL, mesData);
             //sw.Flush();
             //sw.Close();
        }
    }
}
