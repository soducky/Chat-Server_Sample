using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;

public class ClientCode : MonoBehaviour
{
    public string recvStr;
    private string UDPClientIP;
    string str = "c";
    Socket socket;
    EndPoint serverEnd;
    IPEndPoint ipEnd;

    byte[] recvData = new byte[1024];
    byte[] sendData = new byte[1024];
    int recvLen = 0;
    Thread connectThread;

    void Start()
    {
        UDPClientIP = "127.0.0.1";
        UDPClientIP = UDPClientIP.Trim();
        InitSocket();
    }

    public void InitSocket()
    {
        ipEnd = new IPEndPoint(IPAddress.Parse(UDPClientIP), 3040);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
       
        SocketSend(str);
      
        //스레드 연결
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
       
    }
    void SocketSend(string sendStr)
    {
       
        sendData = new byte[1024];
        sendData = Encoding.UTF8.GetBytes(sendStr);
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
        
    }


    void SocketReceive()
    {
        while (true)
        {

            recvData = new byte[1024];
            try
            {
                recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

           
            if (recvLen > 0)
            {
                recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);
                if(recvStr == "s")
                {
                    OffComputer();
                }
            }

           
        }
    }

    //연결 끝
    void SocketQuit()
    {
        //스레드 닫기
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //소켓 닫기
        if (socket != null)
            socket.Close();
    }
    void OnApplicationQuit()
    {
        SocketQuit();
    }

    void OffComputer()
    {
        ProcessStartInfo proInfo = new ProcessStartInfo();
        Process pro = new Process();


        proInfo.FileName = @"cmd"; // 실행할 파일명 입력

        proInfo.CreateNoWindow = false; // cmd 창 띄우기 true(띄우지않기), false(띄우기)
        proInfo.UseShellExecute = false;
        proInfo.RedirectStandardOutput = true; // cmd 데이터받기
        proInfo.RedirectStandardInput = true; // cmd 데이터 보내기
        proInfo.RedirectStandardError = true; // 오류내용 받기


        pro.StartInfo = proInfo;
        pro.Start();
        pro.StandardInput.Write(@"shutdown -s -t 0" + Environment.NewLine);
        pro.StandardInput.Close();


        pro.WaitForExit();
        pro.Close();

    }
}
