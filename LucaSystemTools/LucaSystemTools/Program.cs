﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LucaSystem.Utils;
using McMaster.Extensions.CommandLineUtils;
using ProtScript;
using ProtPak;
using ProtImage;
using RealLive;
namespace LucaSystemTools
{
    /**
     * 此项目未完工，大概率已坑
     * 
     * 
     */
    /** SP 和 ISLAND 需要屏蔽的文件
     * _BUILD_COUNT
     * _VARNUM
     * _MESSAGE
     * _TASK
     * _SCR_LABEL
     * _CGMODE
     * _VOICE_PARAM
     */
    class Program
    {
        public static bool debug = false;
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                CommandLineApplication.Execute<CommandLineUtil>(args);
                Console.WriteLine("ok!");
            }
            else
            {
                Console.WriteLine("please input params");
                Console.ReadLine();
            }
          
           
        }
    }
}