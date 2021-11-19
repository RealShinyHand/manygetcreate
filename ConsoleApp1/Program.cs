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
        static readonly HttpClient client = new HttpClient();
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

            Console.WriteLine("각 쓰레드마다 보낼 요청 수 : ");
            if (!Int32.TryParse(Console.ReadLine(), out int requestNum))
            {
                Console.WriteLine("입력 오류");
                return;
            }
            RequestNum = requestNum;

            List<Thread> threadBox = new List<Thread>();
            // Call asynchronous network methods in a try/catch block to handle exceptions.
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
            var i = 0;
            while (i < RequestNum)
            {
                Task<bool> a = RunHttpAsync(i);
                Console.WriteLine(i + " - 실행 - " + Thread.CurrentThread.Name);
                i++;
            }
        }

        static async Task<bool> RunHttpAsync(int i)
        {

            try
            {
                HttpResponseMessage response = await client.GetAsync("http://"+destIP);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                //Console.WriteLine(responseBody);
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
            }
            Console.WriteLine(i + " - 완료");
            return true;
        }


    }
}
