using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.Analytics;


public class Server : MonoBehaviour
{
    public InputField Input_port; // 인풋

    List<ServerClient> clients; 
    List<ServerClient> disconnectList; // tcp 관계가 서버와 클라이언트가 1:N 이므로 여러개 클라이언트 대비

    TcpListener server;
    bool serverStarted;

    public Image PcReadyImage;
    public Sprite PcOnImage;
    public Sprite ReChangePcOffImage; // 이미지를 교체하면서 pc on/off가 잘 되는지 확인 

    private int port;
    bool ServerSwitch = false; // 서버생성 중복방지 

    void CreateServer()
    {
        if (Input.GetButtonDown("Submit") && !ServerSwitch) // 포트 입력 후 엔터키를 누르면 실행)
        {
            port = Input_port.text == "" ? 0000 : int.Parse(Input_port.text); // 포트 번호 바꾸세요
  
            if (port == 0000 || Input_port.text.Length > 0) // 포트 번호 바꾸세요
            {
                ServerCreate();
                ServerSwitch= true; 
            }
        }
    }
 
    public void OffBtn()
    {
        Broadcast($"s", clients); // 모든 클라이언트에게 s신호 보냄 
        Chat.instance.ShowMessage($"종료신호를 보냈습니다.");
    }

    public void ServerCreate()
    {
        clients = new List<ServerClient>(); // 맨밑
        disconnectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port); // 자신의 컴퓨터
            server.Start(); //bind

            StartListening(); // 비동기 듣기
            serverStarted = true;
            Chat.instance.ShowMessage($"서버가 {port}에서 시작되었습니다.");
        }
        catch (Exception e)
        {
            Chat.instance.ShowMessage($"Socket error: {e.Message}");
        }
    }

    void Update()
    {
        CreateServer(); //서버생성 서버스위치가 참인 상태에는 동작 안함.

        if (!serverStarted) return;

        foreach (ServerClient c in clients)
        {
            // 클라이언트가 여전히 연결되있나?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close(); // 소켓 닫기
                disconnectList.Add(c); //비연결 
                continue;
            }
            // 클라이언트로부터 체크 메시지를 받는다
            else
            {
                NetworkStream s = c.tcp.GetStream(); // 연결 됨 메세지 보낼 수 있는 상태 
                if (s.DataAvailable)
                {
                    string data = new StreamReader(s, true).ReadLine(); // 데이터 읽음
                    if (data != null)
                    {
                        OnIncomingData(c, data); // 호출
                    }
                }
            }
        }

        for (int i = 0; i < disconnectList.Count - 1; i++)
        {
            Broadcast($"{disconnectList[i].clientName} 연결이 끊어졌습니다",clients);
            Chat.instance.ShowMessage($"{disconnectList[i].clientName} 연결이 끊어졌습니다.");
            clients.Remove(disconnectList[i]); 
            disconnectList.RemoveAt(i);  // 리스트에서 클라이언트 제거 

            if(disconnectList[i].clientName == "PC1")
            {
                PcReadyImage.sprite = ReChangePcOffImage; // 상태 이미지 교체 
            }
        }
    }



    bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead)) //테스트 1바이트 보냄
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true; // 제대로 받으면 트루 
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server); // 비동기 듣기 
    } 

    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar))); // 0개에서 리스트 추가, 무한반복 
        StartListening();

        // 메시지를 연결된 모두에게 보냄
        Broadcast("%NAME", new List<ServerClient>() { clients[clients.Count - 1] });
    }


    void OnIncomingData(ServerClient c, string data)
    {
        if (data.Contains("&NAME"))
        {
            c.clientName = data.Split('|')[1];
            Chat.instance.ShowMessage($"{c.clientName}이 연결되었습니다");
            Broadcast($"{c.clientName}이 연결되었습니다", clients);
            if(c.clientName == "PC1")
            {
                PcReadyImage.sprite = PcOnImage;
            }
            return;
        }

        Broadcast($"{c.clientName} : {data}", clients); // 모든 클라이언트에게 stream을 보냄 
    }

    void Broadcast(string data, List<ServerClient> cl) //쓰기 모드 
    {
        foreach (var c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream()); // 쓰기 모드 활성화
                writer.WriteLine(data); // 데이터 쓰기 
                writer.Flush(); // 데이터 내보내기
            }
            catch (Exception e)
            {
                //Chat.instance.ShowMessage($"쓰기 에러 : {e.Message}를 클라이언트에게 {c.clientName}");\
                Debug.Log(e);
            }
        }
    }

    private void OnApplicationQuit()
    {
        ServerSwitch = false;
    }
}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}





