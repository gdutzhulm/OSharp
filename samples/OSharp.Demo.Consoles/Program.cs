﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using Autofac;

using OSharp.App.Local.Initialize;
using OSharp.AutoMapper;
using OSharp.Core;
using OSharp.Core.Caching;
using OSharp.Core.Data;
using OSharp.Data.Entity;
using OSharp.Core.Dependency;
using OSharp.Core.Reflection;
using OSharp.Core.Security;
using OSharp.Demo.Contracts;
using OSharp.Demo.Models.Identity;
using OSharp.Logging.Log4Net;
using OSharp.Redis;
using OSharp.Utility.Extensions;

using StackExchange.Redis;


namespace OSharp.Demo.Consoles
{
    internal class Program : ISingletonDependency
    {
        private static Program _program;
        private static readonly Stopwatch Watch = new Stopwatch();

        public IIocResolver IocResolver { get; set; }

        public IIdentityContract IdentityContract { get; set; }

        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("正在初始化，请稍候……");
                Stopwatch watch = Stopwatch.StartNew();

                //IServicesBuilder builder = new ServicesBuilder(new ServiceBuildOptions());
                //IServiceCollection services = builder.Build();
                //services.AddLog4NetServices();
                //services.AddDataServices();
                //services.AddAutoMapperServices();
                //IIocBuilder iocBuilder = new LocalAutofacIocBuilder(services);
                //IFrameworkInitializer initializer = new FrameworkInitializer();
                //initializer.Initialize(iocBuilder);

                //_program = iocBuilder.ServiceProvider.GetService<Program>();
                watch.Stop();
                Console.WriteLine("程序初始化完毕并启动成功，耗时：{0}", watch.Elapsed);
            }
            catch (ReflectionTypeLoadException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("LoaderExceptions:");
                Exception[] exs = e.LoaderExceptions;
                foreach (Exception ex in exs)
                {
                    Console.WriteLine(ex);
                }
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
                return;
            }
            bool exit = false;
            while (true)
            {
                try
                {
                    Console.WriteLine(@"请输入命令：0; 退出程序，功能命令：1 - n");
                    string input = Console.ReadLine();
                    if (input == null)
                    {
                        continue;
                    }
                    switch (input.ToLower())
                    {
                        case "0":
                            exit = true;
                            break;
                        case "1":
                            Method01();
                            break;
                        case "2":
                            Method02();
                            break;
                        case "3":
                            Method03();
                            break;
                        case "4":
                            Method04();
                            break;
                        case "5":
                            Method05();
                            break;
                        case "6":
                            Method06();
                            break;
                        case "7":
                            Method07();
                            break;
                        case "8":
                            Method08();
                            break;
                        case "9":
                            Method09();
                            break;
                        case "10":
                            Method10();
                            break;
                        case "11":
                            Method11();
                            break;
                        case "12":
                            Method12();
                            break;
                        case "13":
                            Method13();
                            break;
                        case "14":
                            Method14();
                            break;
                        case "15":
                            Method15();
                            break;
                        case "16":
                            Method16();
                            break;
                        case "17":
                            Method17();
                            break;
                        case "18":
                            Method18();
                            break;
                    }
                    if (exit)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.FormatMessage());
                }
            }
        }

        private static void Method01()
        {
            Console.WriteLine("IoC注入解析测试：");
            Console.WriteLine("_program == null: {0}", _program == null);
            Console.WriteLine("_program.IocResolver: {0}", _program.IocResolver.GetType());
            Console.WriteLine("IUnitOfWork: {0}", _program.IocResolver.Resolve<IUnitOfWork>().GetType());
            Console.WriteLine("IRepository<Function, Guid>: {0}", _program.IocResolver.Resolve<IRepository<Function, Guid>>().GetType());
            Console.WriteLine(_program.IdentityContract.Roles.Count());
        }

        private static void Method02()
        {
            IServiceProvider provider = _program.IocResolver.Resolve<IServiceProvider>();
            Console.WriteLine(provider.GetType());
            provider.GetServices<IUnitOfWork>().ToList().ForEach(Console.WriteLine);
            provider.GetServices<IFinder<Assembly>>().ToList().ForEach(Console.WriteLine);
            Console.WriteLine(provider.GetService<IServiceCollection>());

        }

        private static void Method03()
        {
            string path = Console.ReadLine();
            if (path.IsMissing())
            {
                path = @"D:\WorkSpace\Source\Repos\osharp";
            }

            string[] files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            Console.WriteLine("cs文件个数：{0}", files.Length);
            int total = files.Sum(file => File.ReadAllLines(file).Count(m => !m.Trim().IsNullOrEmpty()));
            Console.WriteLine("代码行数：{0}", total);
        }

        private static void Method04()
        {
            string path = @"D:\ValidateCode\WeixiaoxinVipvote\source";
            string[] files = Directory.GetFiles(path);
            int count = 0;
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                string ext = Path.GetExtension(file);
                Console.WriteLine(file);
                Bitmap bmp = new Bitmap(file).GrayByPixels().ClearBorder(2).ClearNoise(200, 2);//.DeepByPixels(150);
                bmp.Save($@"{path}\{name}-gray{ext}".Replace("source", "output"));
                if (++count % 50 == 0)
                {
                    Console.ReadLine();
                }
            }
        }

        private static void Method05()
        {
            RandomNumberGenerator generator = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[96];
            generator.GetBytes(bytes);
            Console.WriteLine(Convert.ToBase64String(bytes));
        }

        private static void Method06()
        {
            string file = Console.ReadLine().Replace("\"", "");//@"D:\ValidateCode\DiamondVote\source\24113532531.jpg";
            string name = Path.GetFileNameWithoutExtension(file);
            Bitmap bmp = new Bitmap(file);
            Bitmap newBmp = (Bitmap)bmp.Clone();
            newBmp = newBmp.GrayByPixels();

            Dictionary<byte, int> dict = new Dictionary<byte, int>();
            Dictionary<byte, List<Tuple<int, int>>> dict2 = new Dictionary<byte, List<Tuple<int, int>>>();
            for (int y = 0; y < newBmp.Height; y++)
            {
                for (int x = 0; x < newBmp.Width; x++)
                {
                    byte gray = newBmp.GetPixel(x, y).R;
                    if (dict.ContainsKey(gray))
                    {
                        dict[gray]++;
                        dict2[gray].Add(new Tuple<int, int>(x, y));
                    }
                    else
                    {
                        dict.Add(gray, 1);
                        dict2.Add(gray, new List<Tuple<int, int>>() { new Tuple<int, int>(x, y) });
                    }
                }
            }

            //dict = dict.OrderByDescending(m => m.Value).ToDictionary(m => m.Key, n => n.Value);
            //foreach (var key in dict.Keys)
            //{
            //    Console.WriteLine($"{key}: {dict[key]}");
            //}

            dict2 = dict2.OrderByDescending(m => m.Value.Count).ToDictionary(m => m.Key, m => m.Value);
            //foreach (var key in dict2.Keys)
            //{
            //    Console.WriteLine($"{key}: {dict2[key].Count}");
            //}
            var points = dict2.Take(2).OrderBy(m => m.Key).First();
            Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height);
            foreach (var point in points.Value)
            {
                byte gray = points.Key;
                bmp2.SetPixel(point.Item1, point.Item2, Color.FromArgb(gray, gray, gray));
            }
            bmp2.Threshoding(200).ClearNoise(200, 4).Save(file.Replace(name, name + "-new"));

        }

        private static void Method07()
        {
            string file = Console.ReadLine().Replace("\"", "");
            string name = Path.GetFileNameWithoutExtension(file);
            Bitmap bmp = new Bitmap(file);
            byte[,] grayBytes = bmp.ToGrayArray2D();
            var splits = grayBytes.SplitShadowY();
            for (int i = 0; i < splits.Count; i++)
            {
                var m = splits[i];
                m.ToBitmap().Save(file.Replace(name, name + "-split" + i));
            }




            //byte[,] grayBytes = bmp.ToValid(200).ToGrayArray2D();
            //grayBytes = grayBytes.Binaryzation(180);
            //File.WriteAllText(file.Replace(name + ".jpg", name + ".txt"), grayBytes.ToCodeString(200, true));
            //int[] nums = grayBytes.ShadowY();
            //string str = nums.Select((m, i) => $"{i}: {m}").ExpandAndToString("\n");
            //Console.WriteLine(str);
        }

        private static void Method08()
        {

        }

        private static void Method09()
        {
            Console.WriteLine("请输入字符串source");
            string source = Console.ReadLine();
            Console.WriteLine("请输入字符串target");
            string target = Console.ReadLine();

            double point;
            Console.WriteLine(source.LevenshteinDistance(target, out point, true));
            Console.WriteLine(point);
            Console.WriteLine(source.GetSimilarityWith(target));
        }

        private static void Method10()
        {
            RedisClient redis = new RedisClient();
            const string key = "key001";
            Console.WriteLine(redis.StringGet(key));
            Console.WriteLine(redis.StringSet(key, "Hello World."));
            Console.WriteLine(redis.StringGet(key));
            Console.WriteLine(redis.StringIncrement(key));
            Console.WriteLine(redis.StringDecrement(key));
        }

        private static void Method11()
        {
            string key = ".OnlineClient";
            RedisClient redis = new RedisClient();
            //var vals = redis.HashKeys<OnlineClientDto>(key);
            //Console.WriteLine(vals.Count);
            OnlineClientDto dto = new OnlineClientDto()
            {
                Name = Guid.NewGuid().ToString(),
                ConnectionId = Guid.NewGuid().ToString(),
                IpAddress = "127.0.5.1",
                ConnectedTime = DateTime.Now
            };
            Console.WriteLine(redis.HashSet(key, "key001", dto));
            var vals = redis.HashKeys<string>(key);
            Console.WriteLine(vals.ToJsonString());
            Console.WriteLine(redis.HashGet<OnlineClientDto>(key, vals[0]).ToJsonString());

        }

        private static void Method12()
        {

        }

        private static void Method13()
        {

        }

        private static void Method14()
        {
            throw new NotImplementedException();
        }

        private static void Method15()
        {
            throw new NotImplementedException();
        }

        private static void Method16()
        {
            throw new NotImplementedException();
        }

        private static void Method17()
        {
            throw new NotImplementedException();
        }

        private static void Method18()
        {
            throw new NotImplementedException();
        }
    }
    public class OnlineClientDto
    {
        /// <summary>
        /// 获取或设置 客户端名，以名为准来寻找客户端
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置 客户端连接Id
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// 获取或设置 客户端所在电脑名称
        /// </summary>
        public string ClientHostName { get; set; }

        /// <summary>
        /// 获取或设置 客户端版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 获取或设置 客户端IP
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 获取或设置 是否正在工作
        /// </summary>
        public bool IsWorking { get; set; }

        /// <summary>
        /// 获取或设置 是否自动工作的客户端
        /// </summary>
        public bool IsAutoRunWork { get; set; }

        /// <summary>
        /// 获取或设置 连接时间
        /// </summary>
        public DateTime ConnectedTime { get; set; }

        /// <summary>
        /// 获取或设置 最后工作时间
        /// </summary>
        public DateTime? LastWorkTime { get; set; }

        /// <summary>
        /// 获取或设置 正在投票的订单编号
        /// </summary>
        public int? VotingOrderId { get; set; }
    }
}
