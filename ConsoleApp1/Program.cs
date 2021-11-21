using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
      
        private static String destIP;
        static String DestIP
        {
            get { return destIP; }
            set {
                if (String.IsNullOrEmpty(destIP))
                {
                    destIP = value;
                }
            }
        }
        static int RequestNum { get; set; }
        static int TaskNum { get; set; }
        static long TotalRequest { get; set; }
        static long SendRequest { get; set; }

        static void Main()
        {
            Console.WriteLine("목적지 IP 입력 : ");
            String ip = Console.ReadLine();
            if (!IPAddress.TryParse(ip, out _))
            {
                Console.WriteLine("IP 입력 오류");
                return;
            }
            DestIP = ip;

            Console.WriteLine("쓰레드 갯수 입력 : ");
            if (!Int32.TryParse(Console.ReadLine(), out int threadNum))
            {

                Console.WriteLine("IP 입력 오류");
                return;
            }

            Console.WriteLine("태스크 갯수 : ");
            if (!Int32.TryParse(Console.ReadLine(), out int taskNum))
            {
                Console.WriteLine("입력 오류");
                return;
            }
            TaskNum = taskNum;

            Console.WriteLine("태스크 당 요청 갯수 : ");
            if (!Int32.TryParse(Console.ReadLine(), out int requestNum))
            {
                Console.WriteLine("입력 오류");
                return;
            }
            TotalRequest = threadNum * taskNum * requestNum;
            SendRequest = 0;
            RequestNum = requestNum;

            List<Thread> threadBox = new List<Thread>();
            for (int i = 0; i < threadNum; i++)
            {
                threadBox.Add(new Thread(new ThreadStart(DoStart)));
            }
            for (int i = 0; i < threadNum; i++)
            {
                threadBox[i].Name = $"{i} 번 쓰레드";
                threadBox[i].Start();
            }
            for (int i = 0; i < threadNum; i++)
            {
                threadBox[i].Join();
            }

        }
        static void DoStart()
        {
            List<Task> taskBox = new List<Task>();
            HttpClient client = new HttpClient();
            var i = 0;
            while (i < TaskNum)
            {
                Console.WriteLine($"{Thread.CurrentThread.Name} 쓰레드 - task {i} 수행 - 요청: {RequestNum} -시작");
                taskBox.Add( RunHttpAsync(client,i));
                i++;
            }
            for(int j = 0; j < taskBox.Count; j++)
            {
                taskBox[j].Wait();
                Console.WriteLine($"{Thread.CurrentThread.Name} 쓰레드 - task {j}종료");
            }
        }

        static async Task<bool> RunHttpAsync(HttpClient client, int i)
        {

            try
            {
                for(int c = 0; c < RequestNum; c++) { 
                HttpResponseMessage response = await client.GetAsync("http://"+destIP);
                response.EnsureSuccessStatusCode();
               string responseBody = await response.Content.ReadAsStringAsync();
                    SendRequest++;
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await client.GetStringAsync(uri);

                    //Console.WriteLine(responseBody);
                    Console.WriteLine($"{SendRequest}/{TotalRequest}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return false;
            }
            catch(Exception e2)
            {
                Console.WriteLine(e2.ToString());
                return false;
            }
            return true;
        }


    }
}
