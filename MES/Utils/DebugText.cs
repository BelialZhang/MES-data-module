using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace MES.Utils
{
    class DebugText
    {
        //异常信息文件输出存储
        public void WriteToText(Exception ex)
        {
            string FileURL = @"./数据库异常输出/" + DateTime.Now.ToString("yyyy年MM月dd日").ToString() + ".txt";
            string mesData = DateTime.Now.ToString() + ":\t数据库连接异常，重新连接 ";
            mesData += ex;
            Console.WriteLine("**********出现异常**********");
            if (!File.Exists(FileURL))//文件不存在则创建新文件
            {

                mesData += Environment.NewLine;
                File.WriteAllText(FileURL, mesData);
            }
            else
            {
                mesData += Environment.NewLine;
                File.AppendAllText(FileURL, mesData);
            }
            
        }
    }
}
