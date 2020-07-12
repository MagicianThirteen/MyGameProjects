using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace TankServer
{


    class Program
    {


        static void Main(string[] args)
        {
            if (!DbManager.Connect("game", "127.0.0.1", 3306, "这里写自己的数据库用户名", "这里写自己的数据库密码")) ;
           
            NetManager.StartLoop(8888);
        }
    }
}

       
    
    

