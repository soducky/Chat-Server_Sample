using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Globalization;
using System;
using UnityEngine.UI;

public class MagicPacket : MonoBehaviour
{

    public InputField Input_macAddress;
    public InputField Input_port;

    private string macaddress ;
    private static int m_Port ;

    public Image PcOffImage;
    public Sprite PcReadyImage;

    public void OnBtn()
    {
        macaddress= Input_macAddress.text == "" ? "XX-XX-XX-XX-XX-XX" : Input_macAddress.text; // 맥 주소 입력하세요
        m_Port = Input_port.text == "" ? 0000 : int.Parse(Input_port.text); // 포트번호 입력하세요 
        Wake(macaddress);
    }

    void Wake(string macaddress)
    {
        UdpClient udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;

        var dgram = new byte[1024];
        
        for (int i = 0; i < 6; i++)
        {
            dgram[i] = 255;
        }

        byte[] address_bytes = new byte[6];

        for (int i = 0; i < 6; i++)
        {
            address_bytes[i] = byte.Parse(macaddress.Substring(3 * i, 2), NumberStyles.HexNumber);
        }

        var macaddress_block = dgram.AsSpan(6, 16 * 6);
        for (int i = 0; i < 16; i++)
        {
            address_bytes.CopyTo(macaddress_block.Slice(6 * i));
        }

        udpClient.Send(dgram, dgram.Length, new System.Net.IPEndPoint(IPAddress.Broadcast, m_Port));

        ImageChange();

        Debug.Log("m_port : " + m_Port);
        Debug.Log("macaddres : " + macaddress);

        udpClient.Close();
    }

    void ImageChange()
    {
        PcOffImage.sprite = PcReadyImage; 
    }
}
