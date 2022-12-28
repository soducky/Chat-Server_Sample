using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using UnityEngine.UI;

public class SeverSample : MonoBehaviour
{
    private Socket m_ServerSocket = null;
    private AsyncCallback m_fnReceiveHandler;
    private AsyncCallback m_fnSendHandler;
    private AsyncCallback m_fnAcceptHandler;

    public Image pcoffimage;
    public Sprite pconimage;
    public Sprite rechagePcOffImage;

    public InputField input_port;
    private UInt16 m_Port = 3040;

    private string str = "s";

    public class AsyncObject
    {
        public Byte[] Buffer;
        public Socket WorkingSocket;
        public AsyncObject(Int32 bufferSize)
        {
            this.Buffer = new Byte[bufferSize];
        }
    }

    public void Start()
    {
        StartServer(m_Port);
    }

    public void StartServer(UInt16 ChangePort)
    {
        // 비동기 작업에 사용될 대리자 초기화
        m_fnReceiveHandler = new AsyncCallback(handleDataReceive);
        m_fnSendHandler = new AsyncCallback(handleDataSend);
        m_fnAcceptHandler = new AsyncCallback(handleClientConnectionRequest);

        // 소켓 생성
        m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        // 사용한 포트: 3040
        if (ChangePort == 3040)
        {
            m_ServerSocket.Bind(new IPEndPoint(IPAddress.Any, ChangePort));
            Debug.Log("3040 맞음");
        }

        else if (ChangePort != 3040)
        {
            UnityEngine.Debug.Log("포트를 바꾸세요 3040");
        }
  
        // 연결 요청 받음
        m_ServerSocket.Listen(5);

        // BeginAccept 메서드를 이용해 들어오는 연결 요청을 비동기적으로 처리합니다.
        // 연결 요청을 처리하는 함수는 handleClientConnectionRequest 입니다.
        m_ServerSocket.BeginAccept(m_fnAcceptHandler, null);

        Debug.Log("서버연결 완료" + ChangePort);
    }


    public void StopServer()
    {
   
        m_ServerSocket.Close();
    }

    public new void SendMessage(String message)
    {
        // 추가 정보를 넘기기 위한 변수 선언
        // 크기를 설정하는게 의미가 없습니다.
        // 왜냐하면 바로 밑의 코드에서 문자열을 유니코드 형으로 변환한 바이트 배열을 반환하기 때문에
        // 최소한의 크기르 배열을 초기화합니다.
        AsyncObject ao = new AsyncObject(1);

        // 문자열을 바이트 배열으로 변환
        ao.Buffer = Encoding.Unicode.GetBytes(message);

        // 사용된 소켓을 저장
        ao.WorkingSocket = m_ServerSocket;

        // 전송 시작!
        m_ServerSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnSendHandler, ao);
    }



    private void handleClientConnectionRequest(IAsyncResult ar)
    {
        // 클라이언트의 연결 요청을 수락합니다.
        Socket sockClient = m_ServerSocket.EndAccept(ar);

        // 4096 바이트의 크기를 갖는 바이트 배열을 가진 AsyncObject 클래스 생성
        AsyncObject ao = new AsyncObject(4096);

        // 작업 중인 소켓을 저장하기 위해 sockClient 할당
        ao.WorkingSocket = sockClient;

        // 비동기적으로 들어오는 자료를 수신하기 위해 BeginReceive 메서드 사용!
        sockClient.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);
    }
    private void handleDataReceive(IAsyncResult ar)
    {

        // 넘겨진 추가 정보를 가져옵니다.
        // AsyncState 속성의 자료형은 Object 형식이기 때문에 형 변환이 필요합니다~!
        AsyncObject ao = (AsyncObject)ar.AsyncState;

        // 자료를 수신하고, 수신받은 바이트를 가져옵니다.
        Int32 recvBytes = ao.WorkingSocket.EndReceive(ar);

        // 수신받은 자료의 크기가 1 이상일 때에만 자료 처리
        if (recvBytes > 0)
        {
            Debug.Log("메세지 받음: {0}"+Encoding.Unicode.GetString(ao.Buffer));

            // 자료 처리가 끝났으면~
            // 이제 다시 데이터를 수신받기 위해서 수신 대기를 해야 합니다.
            // Begin~~ 메서드를 이용해 비동기적으로 작업을 대기했다면
            // 반드시 대리자 함수에서 End~~ 메서드를 이용해 비동기 작업이 끝났다고 알려줘야 합니다!
            ao.WorkingSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);
            
            if(Encoding.Unicode.GetString(ao.Buffer) == "c")
            {
                pcoffimage.sprite = pconimage;
            }

            else if(Encoding.Unicode.GetString(ao.Buffer) != "c")
            {
                pcoffimage.sprite = rechagePcOffImage;
            }

            else
            {
                pcoffimage.sprite = rechagePcOffImage;
            }
        }
    }
    private void handleDataSend(IAsyncResult ar)
    {

        // 넘겨진 추가 정보를 가져옵니다.
        AsyncObject ao = (AsyncObject)ar.AsyncState;

        // 자료를 전송하고, 전송한 바이트를 가져옵니다.
        Int32 sentBytes = ao.WorkingSocket.EndSend(ar);

        if (sentBytes > 0)
            Debug.Log("메세지 보냄: {0}"+Encoding.Unicode.GetString(ao.Buffer));
    }

    public void BtnClik()
    {
        //UInt16.TryParse(input_port.text, out m_Port);
       /* if(m_Port >= 1)
        {
            StartServer(m_Port);
        }*/
        SendMessage(str);
        Debug.Log("전송완료 : " + str);
    }

    private void OnApplicationQuit()
    {
        StopServer();
    }
}

