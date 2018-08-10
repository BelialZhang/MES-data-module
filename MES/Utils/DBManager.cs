using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Common;
using System.Data;

namespace MES.Utils
{
    public partial class DBManager
    {
        //数据库地址server=120.27.96.50
        static string url = "server=120.27.96.50;database=mesdatabase;Max Pool Size = 2048;User id=root;Password =root;Charset=utf8;Allow User Variables=True;";


        //进制转换
        DTConversion conversion = new DTConversion();
        //关联号、机器号、产品号、生产数量、机器状态
        string correlation_no, machine_no, products_no, situation_no, count, explain;
        //当前日期
        string Today;
        //插入关联号、更新数量、获取关联号、获取数量
        string Insert, Update, Select, Delete;
        //计数
        int NumSet = 0;
        //机器、产品、生产数、历史生产数、关联号、穴数、机器工作时间、计时器
        List<string> ListMachine = new List<string>();
        List<string> ListProducts = new List<string>();
        List<string> ListCounts = new List<string>();
        List<string> LastCount = new List<string>();
        List<string> ListCor = new List<string>();
        List<string> ListHole = new List<string>();
        List<int> ListTime = new List<int>();
        List<int> ListTiming = new List<int>();
        Utils.DebugText WriteDebug = new Utils.DebugText();
        //插入
        MySqlCommand myInsert;

        //读取
        MySqlCommand mySelect;
        MySqlCommand myUpdate;
        //
        MySqlCommand myDelete;
        //获取数据
        MySqlDataReader Reader;
        MySqlConnection myCon = new MySqlConnection(url);

        public static MySqlConnection DataConn()
        {
            //myCon = new MySqlConnection(url);
            MySqlConnection myConT = new MySqlConnection(url);
            try
            {
                Console.WriteLine("进入数据库连接");
                myConT.Open();
                Console.WriteLine("连接成功");
            }
            catch (Exception ex)
            {
                //Show("连接错误 " + ex.Message);
                Console.WriteLine("连接错误" + ex.Message);
            }

            return myConT;
        }
        //报文处理
        public string DelData(List<byte> M_receive)
        {
            try
            {
                //判断数据库是否断开连接
                while (myCon.State != ConnectionState.Open)
                {
                    myCon.Dispose();
                    myCon.Open();
                }
            }
            catch (Exception ex)
            {
                WriteDebug.WriteToText(ex);
            }
            //flag用于判断在接收到刷卡信息当前是否为新的机器
            bool flag = false;
            //获取数据
            MySqlDataReader Reader;

            //刷卡关联信息录入
            if (M_receive[2] == 0x01)
            {

                //机器号合并
                machine_no = conversion.BcdToIntString(M_receive[3])
                    + conversion.BcdToIntString(M_receive[4]);
                machine_no = machine_no.TrimStart(new char[] { '0' });
                Console.WriteLine("机器号为" + machine_no);
                //清除历史中介表信息
                DelAgency(machine_no);
                //获取新的产品号
                products_no = conversion.BcdToIntString(M_receive[5])
                    + conversion.BcdToIntString(M_receive[6])
                    + conversion.BcdToIntString(M_receive[7]);
                products_no = products_no.TrimStart(new char[] { '0' });
                //Reader.Close();
                try
                {
                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                    {
                        WriteDebug.WriteToText(null);
                        myCon.Dispose();
                        myCon.Open();
                    }
                    //插入语句：将收到的机器号及产品号插入关联表中
                    Insert = string.Format("insert into `mes_correlation`(`machine_no`,`products_no`,`createdate`) values('{0}','{1}','{2}')", machine_no, products_no, DateTime.Now.ToString());
                    myInsert = new MySqlCommand(Insert, myCon);
                    try
                    {
                        myInsert.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                }
                catch (Exception ex)
                {
                    WriteDebug.WriteToText(ex);
                    Console.WriteLine(ex.Message);
                }
                try
                {
                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                    {
                        WriteDebug.WriteToText(null);
                        myCon.Dispose();
                        myCon.Open();
                    }
                    //查询语句：从关联表中获取新插入的关联号
                    Select = string.Format("select `correlation_no` from `mes_correlation` where `machine_no`='{0}' and `products_no`='{1}' order by `createdate` desc", machine_no, products_no);
                    mySelect = new MySqlCommand(Select, myCon);
                    try
                    {
                        Reader = mySelect.ExecuteReader();
                        //获取关联号
                        if (Reader.Read())
                        {
                            //获取关联号
                            correlation_no = Reader[0].ToString();
                            Reader.Close();
                        }
                        else
                        {

                            Reader.Close();
                            //未获取到数据，终止程序
                            return "0";
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Reader.Close();
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                }
                catch (Exception ex)
                {
                    //this.Reader.Close();
                    WriteDebug.WriteToText(ex);
                    Console.WriteLine(ex.Message);
                }

                //修正新接收的机器产品信息，并对对应的List位修正
                for (int i = 0; i < ListMachine.Count; i++)
                {
                    //if (Convert.ToInt32(machine_no) == Convert.ToInt32(ListMachine[i]))
                    //判别当前接收到的机器号是否已经存在
                    if (string.Compare(machine_no, ListMachine[i]) == 0)
                    {
                        NumSet = i;
                        ListCor[i] = correlation_no;
                        ListCounts[i] = "0";
                        //收到的产品号与上一批次产品不同
                        if (string.Compare(products_no, ListProducts[i]) != 0)
                        {
                            //更新产品号
                            ListProducts[i] = products_no;
                            //更改产品穴数
                            try
                            {
                                while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                                {
                                    WriteDebug.WriteToText(null);
                                    myCon.Dispose();
                                    myCon.Open();
                                }
                                //查询语句：从产品信息表中获取到接收到的产品的穴数，并修正ListHole中穴数
                                Select = string.Format("select `hole` from `mes_products` where `products_no` = '{0}'", ListProducts[i]);
                                mySelect = new MySqlCommand(Select, myCon);
                                try
                                {
                                    Reader = mySelect.ExecuteReader();
                                    if (Reader.Read())
                                    {
                                        //添加产品穴数
                                        ListHole[i] = Reader[0].ToString();
                                        Reader.Close();
                                    }
                                    else
                                    {
                                        Reader.Close();
                                    }
                                    ListTime[i] = 0;
                                    ListTiming[i] = 0;
                                }
                                catch (Exception ex)
                                {
                                    this.Reader.Close();
                                    WriteDebug.WriteToText(ex);
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteDebug.WriteToText(ex);
                                Console.WriteLine(ex.Message);
                            }
                            //更改产品历史生产数
                            try
                            {
                                while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                                {
                                    WriteDebug.WriteToText(null);
                                    myCon.Dispose();
                                    myCon.Open();
                                }
                                //查询语句：获取历史同机器号，同产品号生产总数
                                Select = string.Format("select `last_count` from `mes_collect` where `machine_no` = '{0}' and `products_no` = '{1}'  order by `createdate` desc", ListMachine[i], ListProducts[i]);
                                mySelect = new MySqlCommand(Select, myCon);
                                try
                                {
                                    Reader = mySelect.ExecuteReader();
                                    if (Reader.Read())
                                    {
                                        //获取历史生产数
                                        LastCount[i] = Reader[0].ToString();
                                    }
                                    Reader.Close();
                                }
                                catch (Exception ex)
                                {
                                    this.Reader.Close();
                                    WriteDebug.WriteToText(ex);
                                    Console.WriteLine(ex.Message);
                                }

                            }

                            catch (Exception ex)
                            {
                                WriteDebug.WriteToText(ex);
                                Console.WriteLine(ex.Message);
                            }

                        }

                        flag = true;
                        break;
                    }
                }
                //当前接收到的机器为新的机器，添加新的机器关联
                if (flag == false)
                {
                    Console.WriteLine("无已存在机器信息，添加");
                    //统一增加关联List，避免数据丢失造成关联不准确
                    ListMachine.Add(machine_no);
                    ListProducts.Add(products_no);
                    ListCor.Add(correlation_no);
                    LastCount.Add("0");
                    ListHole.Add("1");
                    ListTime.Add(0);
                    ListTiming.Add(1);
                    ListCounts.Add("0");
                    //获取产品穴数
                    try
                    {
                        while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                        {
                            WriteDebug.WriteToText(null);
                            myCon.Dispose();
                            myCon.Open();
                        }
                        //查询语句：查询产品的穴数
                        Select = string.Format("select `hole` from `mes_products` where `products_no` = '{0}'", products_no);
                        mySelect = new MySqlCommand(Select, myCon);
                        try
                        {
                            Reader = mySelect.ExecuteReader();
                            if (Reader.Read())
                            {
                                //添加产品穴数
                                ListHole[ListMachine.Count-1]=Reader[0].ToString();
                                Reader.Close();
                            }
                            else
                            {
                                Reader.Close();
                            }
                            NumSet = ListMachine.Count - 1;
                            //新机器数据，添加新工作时间
                            Console.WriteLine("添加新机器运行");
                        }
                        catch (Exception ex)
                        {
                            this.Reader.Close();
                            WriteDebug.WriteToText(ex);
                            Console.WriteLine(ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }
                    try
                    {
                        while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                        {
                            WriteDebug.WriteToText(null);
                            myCon.Dispose();
                            myCon.Open();
                        }
                        //插入语句：当前机器为新的机器，从当前时刻插入机器的工作时间
                        Insert = string.Format("insert into `mes_machinetime`(`machine_no`,`workingtime`,`workinghour`,`createdate`) values('{0}','0','0.00','{1}')", ListMachine[NumSet], DateTime.Now.ToString("yyyy-MM-dd"));
                        myInsert = new MySqlCommand(Insert, myCon);
                        try
                        {
                            myInsert.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            WriteDebug.WriteToText(ex);
                            Console.WriteLine(ex.Message);
                        }

                        //添加新的报文中介信息，从零开始
                    }
                    catch (Exception ex)
                    {
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                }
                try
                {
                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                    {
                        WriteDebug.WriteToText(null);
                        myCon.Dispose();
                        myCon.Open();
                    }
                    //插入语句：插入新的中介报文
                    Insert = string.Format("insert into `mes_agency`(`correlation_no`,`count`,`createdate`) values('{0}','0','{1}')", ListCor[NumSet], DateTime.Now.ToString());
                    myInsert = new MySqlCommand(Insert, myCon);
                    try
                    {
                        myInsert.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        this.Reader.Close();
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                }
                catch (Exception ex)
                {
                    WriteDebug.WriteToText(ex);
                    Console.WriteLine(ex.Message);
                }

                return correlation_no;
            }

            //生产报文更新
            else if (M_receive[2] == 0x02)
            {
                //关联号合并
                correlation_no = conversion.BcdToIntString(M_receive[3])
                    + conversion.BcdToIntString(M_receive[4])
                    + conversion.BcdToIntString(M_receive[5])
                    + conversion.BcdToIntString(M_receive[6]);
                //清除数据高位0
                correlation_no = correlation_no.TrimStart(new char[] { '0' });
                //生产数量合并
                count = conversion.BcdToIntString(M_receive[8])
                    + conversion.BcdToIntString(M_receive[9])
                    + conversion.BcdToIntString(M_receive[10])
                    + conversion.BcdToIntString(M_receive[11]);
                Console.WriteLine("关联号为" + correlation_no + "\t生产数为" + count);
                //判别当前收到的机器关系生产数量是否为0
                if(string.Compare(count,"00000000") !=0)
                {
                    //去除生产数高位0，便于后面操作
                    count = count.TrimStart(new char[] { '0' });
                }
                else
                {
                    //当前机器生产数为0,
                    count = "0";
                }
                //数据采集终端为新制作，默认初始信息均为FF，此报文不做处理
                if (M_receive[3] == 0xFF && M_receive[4] == 0xFF && M_receive[5] == 0xFF && M_receive[6] == 0xFF)
                {
                    return "0";
                }
                for (int i = 0; i < ListMachine.Count; i++)
                {
                    //同关联号的生产数变更
                    if (string.Compare(ListCor[i], correlation_no) == 0)
                    {
                        ListCounts[i] = count;//更新生产数
                        ListTiming[i] = 0;//重置计时
                        break;
                    }
                }
                //状态号
                situation_no = conversion.BcdToIntString(M_receive[7]);
                situation_no = situation_no.TrimStart(new char[] { '0' });
                try
                {
                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                    {
                        WriteDebug.WriteToText(null);
                        myCon.Dispose();
                        myCon.Open();
                    }
                    //更新语句：更新当前收到的机器生产数量及时间
                    Update = string.Format("update `mes_agency` set `count` = {0} ,`situation_no`='{1}',`createdate` = '{2}' where `correlation_no` = '{3}'", count, situation_no, DateTime.Now.ToString(), correlation_no);
                  
                    myUpdate = new MySqlCommand(Update, myCon);
                    try
                    {
                        myUpdate.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                }
                catch (Exception ex)
                {
                    WriteDebug.WriteToText(ex);
                    Console.WriteLine(ex.Message);
                }

                //机器出现异常状态，保留异常状态报文信息
                if (M_receive[7] == 0x07)
                {
                    for (int i = 0; i < ListMachine.Count; i++)
                    {
                        if (string.Compare(ListCor[i], correlation_no) == 0)
                        {
                            machine_no = ListMachine[i];
                            break;
                        }
                    }
                    //故障编号合成
                    situation_no = conversion.BcdToIntString(M_receive[5]);

                    //获取状态信息
                    try
                    {
                        while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                        {
                            WriteDebug.WriteToText(null);
                            myCon.Dispose();
                            myCon.Open();
                        }
                        //查询语句：查询当前错误编号的描述信息
                        Select = string.Format("select * from `mes_situation` where `situation_no`='{0}'", situation_no);
                        mySelect = new MySqlCommand(Select, myCon);
                        try
                        {
                            Reader = mySelect.ExecuteReader();
                            if (Reader.Read())
                            {
                                explain = Reader[1].ToString();
                                Reader.Close();
                            }
                            else
                            {

                                Reader.Close();
                                //未获取到数据，终止程序
                                return "0";
                            }

                        }
                        catch (Exception ex)
                        {
                            this.Reader.Close();
                            WriteDebug.WriteToText(ex);
                            Console.WriteLine(ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        //Reader.Close();
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }
                    try
                    {
                        while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                        {
                            WriteDebug.WriteToText(null);
                            myCon.Dispose();
                            myCon.Open();
                        }
                        //数据插入故障表
                        //插入语句：将收到的机器异常信息存入故障表
                        Insert = string.Format("INSERT  `mes_stoppage`( `machine_no`, `situation_no`, `explain`, `createdate`) values('{0}','{1}','{2}','{3}')", machine_no, situation_no, explain, DateTime.Now.ToString());
                        myInsert = new MySqlCommand(Insert, myCon);
                        try
                        {
                            myInsert.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            WriteDebug.WriteToText(ex);
                            Console.WriteLine(ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                }
                return correlation_no;
            }

                //采集器重启
            else if (M_receive[2] == 0x03)
            {
                Console.WriteLine("掉线报文解析");
                correlation_no = conversion.BcdToIntString(M_receive[3])
                    + conversion.BcdToIntString(M_receive[4])
                    + conversion.BcdToIntString(M_receive[5])
                    + conversion.BcdToIntString(M_receive[6]);
                correlation_no = correlation_no.TrimStart(new char[] { '0' });

                //开发板初始化，发送的关联号为0xFF 0xFF 0xFF 0xFF
                if (M_receive[3] == 0xFF && M_receive[4] == 0xFF && M_receive[5] == 0xFF && M_receive[6] == 0xFF)
                {
                    return "0";
                }
                for (int i = 0; i < ListMachine.Count; i++)
                {
                    //if (Convert.ToInt32(ListCor[i]) == Convert.ToInt32(correlation_no))
                    if (string.Compare(ListCor[i], correlation_no) == 0)
                    {
                        //机器工作中，重置停机时间
                        ListTiming[i] = 0;
                        count = ListCounts[i];
                        break;
                    }
                }
                return count;
            }
            else
            {
                return "0";
            }

        }
        //整理新的机器数据
        public void DelAgency(string machine_no)
        {
            int AllCount=0;
            for (int i = 0; i < ListMachine.Count; i++)
            {

                //if (Convert.ToInt32(machine_no) == Convert.ToInt32(ListMachine[i]))
                //搜索相同机器号的顺序组并处理
                if (string.Compare(machine_no, ListMachine[i]) == 0)
                {
                    try
                    {
                        //同机器号、产品号的生产总数为历史生产总数与当前生产总数之和
                        AllCount = Convert.ToInt32(ListCounts[i]) + Convert.ToInt32(LastCount[i]);
                    }
                    catch(Exception ex)
                    {
                        WriteDebug.WriteToText(ex);
                    }
                    try
                    {
                        while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                        {
                            WriteDebug.WriteToText(null);
                            myCon.Dispose();
                            myCon.Open();
                        }
                        //查询语句：查询机器最新的汇总信息，用于判别机器是否已经汇总过生产数据
                        Select = string.Format("select `count` from `mes_collect` where `correlation_no`='{0}' order by `createdate` desc", ListCor[i]);
                        mySelect = new MySqlCommand(Select, myCon);
                        try
                        {
                            Reader = mySelect.ExecuteReader();
                            if (Reader.Read())
                            {
                                //未汇总数据
                                if (AllCount > Convert.ToInt32(Reader[0].ToString()))
                                {
                                    //更新历史生产数
                                    LastCount[i] = AllCount.ToString();
                                    try
                                    {
                                        while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                                        {
                                            WriteDebug.WriteToText(null);
                                            myCon.Dispose();
                                            myCon.Open();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Reader.Close();
                                        WriteDebug.WriteToText(ex);
                                    }

                                    Console.WriteLine("进入汇总阶段" + Reader[0].ToString());
                                    Reader.Close();
                                    //插入语句：机器未汇总过数据，将当前生产信息插入到汇总表
                                    Insert = string.Format("insert into `mes_collect`(`correlation_no`,`machine_no`,`products_no`,`last_count`,`count`,`createdate`) values('{0}','{1}','{2}','{3}','{4}','{5}')", ListCor[i], ListMachine[i], ListProducts[i], LastCount[i], AllCount.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH-00-00"));
                                    myInsert = new MySqlCommand(Insert, myCon);
                                    try
                                    {

                                        myInsert.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        //this.Reader.Close();
                                        WriteDebug.WriteToText(ex);
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                                else    //已汇总数据，不再进行汇总操作
                                {
                                    Reader.Close();
                                }
                            }
                                //当前机器为第一次生产产品，且已经有生产
                            else if (string.Compare(ListCounts[i], "0") != 0)
                            {

                                //更新历史生产数
                                LastCount[i] = AllCount.ToString();
                                Reader.Close();
                                try
                                {
                                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                                    {
                                        WriteDebug.WriteToText(null);
                                        myCon.Dispose();
                                        myCon.Open();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.Reader.Close();
                                    WriteDebug.WriteToText(ex);
                                }

                                if (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                                {
                                    break;
                                }
                                Console.WriteLine("进入未汇总阶段" + ListCounts[i]);
                                Insert = string.Format("insert into `mes_collect`(`correlation_no`,`machine_no`,`products_no`,`last_count`,`count`,`createdate`) values('{0}','{1}','{2}','{3}','{4}','{5}')", ListCor[i], ListMachine[i], ListProducts[i], LastCount[i], AllCount.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH-00-00"));
                                myInsert = new MySqlCommand(Insert, myCon);
                                try
                                {

                                    myInsert.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    //this.Reader.Close();
                                    WriteDebug.WriteToText(ex);
                                    Console.WriteLine(ex.Message);
                                }
                                //删除同机器号历史报文
                                Console.WriteLine("删除无用历史报文");
                            }
                            else//机器未生产
                            {
                                Reader.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDebug.WriteToText(ex);
                            Console.WriteLine(ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }
                    try
                    {
                        while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                        {
                            WriteDebug.WriteToText(null);
                            myCon.Dispose();
                            myCon.Open();
                        }
                        //删除语句：清楚同机器号的历史生产中间数据
                        Delete = string.Format("delete from `mes_agency` where `correlation_no` = '{0}'", ListCor[i]);
                        myDelete = new MySqlCommand(Delete, myCon);
                        try
                        {
                            myDelete.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            WriteDebug.WriteToText(ex);
                            Console.WriteLine(ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                    break;
                }
            }
        }
        //定时汇总
        public void HourCollect()
        {
            int AllCount = 0;
            for (int i = 0; i < ListMachine.Count; i++)
            {
                //生产总数为当前批次生产数加历史生产数
                AllCount = Convert.ToInt32(ListCounts[i]) + Convert.ToInt32(LastCount[i]);
                try
                {
                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                    {
                        WriteDebug.WriteToText(null);
                        myCon.Dispose();
                        myCon.Open();
                    }
                    //查询语句：查询机器生产数，用于判别机器是否已经汇总过数据
                    Select = string.Format("select count from mes_collect where correlation_no = '{0}' and count = '{1}'", ListCor[i], AllCount.ToString());
                    mySelect = new MySqlCommand(Select, myCon);
                    try
                    {
                        Reader = mySelect.ExecuteReader();
                        //搜索到数据，代表已经汇总过数据
                        if (Reader.Read())
                        {
                            Reader.Close();
                        }
                        //未汇总数据
                        else
                        {
                            Console.WriteLine("未汇总数据，开始汇总");
                            Reader.Close();
                            try
                            {
                                while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                                {
                                    WriteDebug.WriteToText(null);
                                    myCon.Dispose();
                                    myCon.Open();
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteDebug.WriteToText(ex);
                            }
                            //插入语句：机器未汇总
                            Insert = string.Format("insert into `mes_collect`(`correlation_no`,`machine_no`,`products_no`,`last_count`,`count`,`createdate`) values('{0}','{1}','{2}','{3}','{4}','{5}')", ListCor[i], ListMachine[i], ListProducts[i], LastCount[i], AllCount.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:00:00"));
                            myInsert = new MySqlCommand(Insert, myCon);
                            try
                            {
                                myInsert.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                WriteDebug.WriteToText(ex);
                                Console.WriteLine(ex.Message);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        this.Reader.Close();
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                }
                catch (Exception ex)
                {
                    WriteDebug.WriteToText(ex);
                    Console.WriteLine(ex.Message);
                }

            }
        }
        //初始化数组数据
        public int Initial(MySqlConnection myCon)
        {

            Console.WriteLine("机器数据表初始化");
            //程序开启初始化数据，从数据库中获取数据
            try
            {
                while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                {
                    WriteDebug.WriteToText(null);
                    myCon.Dispose();
                    myCon.Open();
                }
                //查询语句，遍历中介表中已存在的机器生产关系，并添加相应的List
                Select = string.Format("select `correlation_no`,`count` from `mes_agency`");
                mySelect = new MySqlCommand(Select, myCon);
                try
                {
                    Reader = mySelect.ExecuteReader();
                    while (Reader.Read())
                    {
                        //初始化机器关联数据，相同编号对应的为同一台机器同一批次的数据
                        ListCor.Add(Reader[0].ToString());
                        ListCounts.Add(Reader[1].ToString());
                        LastCount.Add("0");
                        ListMachine.Add("0");
                        ListProducts.Add("0");
                        ListHole.Add("0");
                        ListTime.Add(0);
                        ListTiming.Add(1);
                    }
                    Reader.Close();
                }
                catch (Exception ex)
                {
                    this.Reader.Close();
                    WriteDebug.WriteToText(ex);
                    Console.WriteLine(ex.Message);
                }

            }
            catch (Exception ex)
            {
                this.Reader.Close();
                WriteDebug.WriteToText(ex);
                Console.WriteLine(ex.Message);
            }

            //根据关联号从关联表中搜索机器号及产品号
            for (int i = 0; i < ListCor.Count; i++)
            {
                try
                {
                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                    {
                        WriteDebug.WriteToText(null);
                        myCon.Dispose();
                        myCon.Open();
                    }
                    //查询语句：通过关联号查询关联号所对应的机器号及产品号
                    Select = string.Format("select `machine_no`,`products_no` from `mes_correlation` where `correlation_no` = '{0}'", ListCor[i]);
                    mySelect = new MySqlCommand(Select, myCon);
                    try
                    {
                        Reader = mySelect.ExecuteReader();
                        if (Reader.Read())
                        {
                            //添加机器号、产品号
                            ListMachine[i] = Reader[0].ToString();
                            ListProducts[i] = Reader[1].ToString();
                        }
                        Reader.Close();
                    }
                    catch (Exception ex)
                    {
                        this.Reader.Close();
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                }
                catch (Exception ex)
                {
                    WriteDebug.WriteToText(ex);
                    Console.WriteLine(ex.Message);
                }
                try
                {
                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                    {
                        WriteDebug.WriteToText(null);
                        myCon.Dispose();
                        myCon.Open();
                    }
                    //查询语句：查询同机器号、产品号的历史生产数
                    Select = string.Format("select `last_count` from `mes_collect` where `machine_no` = '{0}' and `products_no` = '{1}'  order by `collect_no` desc", ListMachine[i], ListProducts[i]);
                    mySelect = new MySqlCommand(Select, myCon);
                    try
                    {
                        Reader = mySelect.ExecuteReader();
                        if (Reader.Read())
                        {
                            //获取历史生产数
                            LastCount[i] = Reader[0].ToString();
                        }
                        Reader.Close();
                    }
                    catch (Exception ex)
                    {
                        this.Reader.Close();
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                }

                catch (Exception ex)
                {
                    WriteDebug.WriteToText(ex);
                    Console.WriteLine(ex.Message);
                }
                //获取机器产品关联关系
                try
                {
                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                    {
                        WriteDebug.WriteToText(null);
                        myCon.Dispose();
                        myCon.Open();
                    }
                    //查询语句：查询产品对应的穴数
                    Select = string.Format("select `hole` from `mes_products` where `products_no` = '{0}'", ListProducts[i]);
                    mySelect = new MySqlCommand(Select, myCon);
                    try
                    {
                        Reader = mySelect.ExecuteReader();
                        if (Reader.Read())
                        {
                            //添加产品穴数
                            ListHole[i] = Reader[0].ToString();
                        }
                        Reader.Close();
                    }
                    catch (Exception ex)
                    {
                        this.Reader.Close();
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }


                }
                catch (Exception ex)
                {
                    WriteDebug.WriteToText(ex);
                    Console.WriteLine(ex.Message);
                }
                //时间判别单位，用于判断当前是否为新一天
                Today = DateTime.Now.ToString("yyyy-MM-dd");
                try
                {
                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                    {
                        WriteDebug.WriteToText(null);
                        myCon.Dispose();
                        myCon.Open();
                    }
                    //查询语句：查询机器当天的工作时间以及创建时间
                    Select = string.Format("select workingtime,createdate from `mes_machinetime` where`machine_no`='{0}' order by `time_no` desc", ListMachine[i]);
                    mySelect = new MySqlCommand(Select, myCon);
                    try
                    {
                        Reader = mySelect.ExecuteReader();
                        if (Reader.Read())
                        {
                            //当前日期与已存在时间为同一天
                            if (string.Compare(DateTime.Now.ToString("yyyy-MM-dd"), Convert.ToDateTime(Reader[1].ToString()).ToString("yyyy-MM-dd")) == 0)
                            {
                                ListTime[i] = Convert.ToInt32(Reader[0]);
                                Reader.Close();
                            }
                            else    //当前为新的一天，添加新日期下的机器生产时间标准
                            {
                                Reader.Close();
                                ListTime[i] = 0;
                                try
                                {
                                    while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                                    {
                                        WriteDebug.WriteToText(null);
                                        myCon.Dispose();
                                        myCon.Open();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.Reader.Close();
                                    WriteDebug.WriteToText(ex);
                                }
                                //插入语句：当前为新的一天，添加机器的当前工作时间
                                Insert = string.Format("insert into mes_machinetime(`machine_no`,`workingtime`,`workinghour`,`createdate`) values('{0}','{1}','{2}','{3}')", ListMachine[i], ListTime[i].ToString(), ListTime[i].ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
                                myInsert = new MySqlCommand(Insert, myCon);
                                try
                                {
                                    myInsert.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    //this.Reader.Close();
                                    WriteDebug.WriteToText(ex);
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        else
                        {
                            Reader.Close();
                            ListTime[i] = 0;
                            try
                            {
                                while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                                {
                                    WriteDebug.WriteToText(null);
                                    myCon.Dispose();
                                    myCon.Open();
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteDebug.WriteToText(ex);
                            }

                            //插入语句：当前为新的一天，添加机器的当前工作时间
                            Insert = string.Format("insert into mes_machinetime(`machine_no`,`workingtime`,`workinghour`,`createdate`) values('{0}','{1}','{2}','{3}')", ListMachine[i], ListTime[i].ToString(), ListTime[i].ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
                            myInsert = new MySqlCommand(Insert, myCon);
                            try
                            {

                                myInsert.ExecuteNonQuery();

                            }
                            catch (Exception ex)
                            {
                                //this.Reader.Close();
                                WriteDebug.WriteToText(ex);
                                Console.WriteLine(ex.Message);
                            }
                        }
                        Reader.Close();
                    }
                    catch (Exception ex)
                    {
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }

                }
                catch (Exception ex)
                {
                    WriteDebug.WriteToText(ex);
                    Console.WriteLine(ex.Message);
                }
                //判别当前日期是否为新一天

            }

            //开机汇总数据
            HourCollect();
            return ListMachine.Count;
        }
        //获取穴数
        public int GetHole(List<byte> M_receive)
        {
            try
            {
                while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                {
                    WriteDebug.WriteToText(null);
                    myCon.Dispose();
                    myCon.Open();
                }
            }
            catch (Exception ex)
            {
                WriteDebug.WriteToText(ex);
            }

            //初始化穴数为0
            int M_Hole = 1;
            //判别当前接收的报文为何类型
            if (M_receive[2] == 0x01)
            {
                //收到刷卡报文，以产品号为判断标准
                products_no = conversion.BcdToIntString(M_receive[5])
                    + conversion.BcdToIntString(M_receive[6])
                    + conversion.BcdToIntString(M_receive[7]);
                products_no = products_no.TrimStart(new char[] { '0' });
                for (int i = 0; i < ListMachine.Count; i++)
                {
                    //if(Convert.ToInt32(products_no)==Convert.ToInt32(ListProducts[i]))
                    if (string.Compare(products_no, ListProducts[i]) == 0)
                    {
                        M_Hole = Convert.ToInt32(ListHole[i]);
                        break;
                    }
                }
            }
            else
            {
                //收到2~3类报文，均以关联号为标准
                correlation_no = conversion.BcdToIntString(M_receive[3])
                    + conversion.BcdToIntString(M_receive[4])
                    + conversion.BcdToIntString(M_receive[5])
                    + conversion.BcdToIntString(M_receive[6]);
                correlation_no = correlation_no.TrimStart(new char[] { '0' });
                if (M_receive[3] == 0xFF && M_receive[4] == 0xFF && M_receive[5] == 0xFF && M_receive[6] == 0xFF)
                {
                    return 0;
                }
                for (int i = 0; i < ListMachine.Count; i++)
                {
                    // if (Convert.ToInt32(correlation_no) == Convert.ToInt32(ListCor[i]))
                    if (string.Compare(correlation_no, ListCor[i]) == 0)
                    {
                        //获取到穴数，退出循环
                        Console.WriteLine("机器"+ListMachine[i] +"产品"+ListProducts[i]+ "穴数" + ListHole[i]);
                        M_Hole = Convert.ToInt32(ListHole[i]);
                        break;
                    }
                }
            }
            //返回穴数
            return M_Hole;


        }
        //计时器
        public void Timing()
        {
            try
            {
                while (myCon.State != ConnectionState.Open)//判断数据库是否断开连接
                {
                    WriteDebug.WriteToText(null);
                    myCon.Dispose();
                    myCon.Open();
                }
            }
            catch (Exception ex)
            {
                WriteDebug.WriteToText(ex);
            }

            Console.WriteLine("计时器运转");
            for (int i = 0; i < ListMachine.Count; i++)
            {
                //计时编号累加
                ListTiming[i]++;
            }
        }
        //每隔5分钟更新一次机器工作时间
        public void TimeWork(MySqlConnection myCon)
        {
            Today = DateTime.Now.ToString("yyyy-MM-dd");
            //防止数据库超时断开连接
            if (myCon.State == ConnectionState.Open)
            {

                Console.WriteLine("********************重连数据库防止超时断开********************");
                myCon.Close();
                myCon.Open();
            }
            //DataConn();

            Console.WriteLine("到达5分钟，判别机器是否正常工作");
            for (int i = 0; i < ListMachine.Count; i++)
            {
                //机器正常工作，累加计时
                if (ListTiming[i] < 10)
                {
                    Console.WriteLine("机器" + i + "工作");
                    //查询语句：查询机器的工作时间及创建时间
                    Select = string.Format("select workingtime,createdate from `mes_machinetime` where`machine_no`='{0}' order by `time_no` desc", ListMachine[i]);
                    mySelect = new MySqlCommand(Select, myCon);
                    try
                    {
                        Reader = mySelect.ExecuteReader();
                        if (Reader.Read())
                        {
                            //当前为同一天
                            if (string.Compare(DateTime.Now.ToString("yyyy-MM-dd"), Convert.ToDateTime(Reader[1].ToString()).ToString("yyyy-MM-dd")) == 0)
                            {
                                Reader.Close();
                                ListTime[i] = ListTime[i] + 5;
                                double WorkingHour = (double)ListTime[i] / 60;
                                //更新语句：当前为同一天生产，机器的工作时间添加5分钟
                                Update = string.Format("update `mes_machinetime` set `workingtime` = '{0}',`workinghour`='{1}'where `machine_no`= '{2}' and `createdate`='{3}'", ListTime[i].ToString(), WorkingHour.ToString(), ListMachine[i], Today);
                                myUpdate = new MySqlCommand(Update, myCon);
                                try
                                {
                                    myUpdate.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else//当前为新的一天
                            {
                                Reader.Close();
                                ListTime[i] = 5;
                                double WorkingHour = (double)ListTime[i] / 60;
                                //插入语句：当前为新的一天，机器依然在工作，当前机器已工作5分钟（可能存在跨天情况）
                                Insert = string.Format("insert into mes_machinetime(`machine_no`,`workingtime`,`workinghour`,`createdate`) values('{0}','{1}','{2}','{3}')", ListMachine[i], ListTime[i].ToString(), WorkingHour.ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
                                myInsert = new MySqlCommand(Insert, myCon);
                                try
                                {
                                    myInsert.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDebug.WriteToText(ex);
                        Console.WriteLine(ex.Message);
                    }



                }
            }
        }
    }
}
