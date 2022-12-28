using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Windows;
using System;

public class cmd : MonoBehaviour
{
    public void OffBtn()
    {
        OffComputer();
    }
    void OffComputer()
    {
        ProcessStartInfo proInfo = new ProcessStartInfo();
        Process pro = new Process();
       

        proInfo.FileName = @"cmd"; // 실행할 파일명 입력

        proInfo.CreateNoWindow= false; // cmd 창 띄우기 true(띄우지않기), false(띄우기)
        proInfo.UseShellExecute = false; 
        proInfo.RedirectStandardOutput = true; // cmd 데이터받기
        proInfo.RedirectStandardInput= true; // cmd 데이터 보내기
        proInfo.RedirectStandardError= true; // 오류내용 받기
     

        pro.StartInfo = proInfo; 
        pro.Start();
        pro.StandardInput.Write(@"net use \\192.168.10.23 /user: PC" + Environment.NewLine);
        pro.StandardInput.Write(@"shutdown /s /m \\192.168.10.23" + Environment.NewLine);
        pro.StandardInput.Close();


        pro.WaitForExit();
        pro.Close();

    }
}
