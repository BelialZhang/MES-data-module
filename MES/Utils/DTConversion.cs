using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Utils
{
    class DTConversion
    {
        //Bcd码转Int型字符串
        public string BcdToIntString(Byte M_Bcd)
        {
            string IntString;
            //转换算法：高位除以16，低位对16取余
            IntString = (Convert.ToInt32(M_Bcd) / 16 ).ToString()
                + (Convert.ToInt32(M_Bcd) % 16).ToString();
            return IntString;
        }
        //Int型转成16进制Bcd码
        public byte IntToBCDByte(int M_num)
        {
            //转换算法：取高位，乘以16后加上低位
            M_num = M_num / 10 * 16 + M_num % 10;
            byte TempByte = Convert.ToByte(M_num.ToString());
            return TempByte;
        }
    }
}
