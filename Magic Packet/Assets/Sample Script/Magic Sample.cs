using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Globalization;
using System;

public class ddd : MonoBehaviour
{
    string macaddress = "00E0914E5D32";

    public void btn()
    {
        Wake(macaddress);
    }

    static void Wake(string macaddress)
    {
        UdpClient udpClient = new UdpClient();
        udpClient.EnableBroadcast= true;

        var dgram = new byte[1024];

        for(int i = 0; i<6; i++)
        {
            dgram[i] = 255;
        }

        byte[] address_bytes = new byte[6];

        for(int i = 0; i<6; i++)
        {
            address_bytes[i] = byte.Parse(macaddress.Substring(2 * i, 2), NumberStyles.HexNumber);
        }

        var macaddress_block = dgram.AsSpan(6, 16 * 6);
        for(int i=0; i<16; i++)
        {
            address_bytes.CopyTo(macaddress_block.Slice(6 * i));
        }

        udpClient.Send(dgram, dgram.Length, new System.Net.IPEndPoint(IPAddress.Broadcast, 0)); 
        udpClient.Close();
    }
}
