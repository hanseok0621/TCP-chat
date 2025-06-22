using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TCPServer
{
    public partial class Form1 : Form
    {
        TcpListener Server; // 서버 소켓
        TcpClient Client; // 클라이언트 소켓

        NetworkStream Stream; // 네트워크 스트림
        StreamReader Reader; // 클라이언트에서 읽기 위한 스트림 리더
        StreamWriter Writer; // 클라이언트로 쓰기 위한 스트림 라이터

        Thread receiveThread; // 수신 스레드

        bool Connected; // 연결 상태

        private delegate void AddTextDelegate(string strText);
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            txtView.AppendText("나 : " + txtInput.Text + Environment.NewLine); // 나의 메시지 출력
            Writer.WriteLine(txtInput.Text); // 상대방에게 메시지 전송
            Writer.Flush(); // 버퍼 비우기
            txtInput.Clear(); // 입력창 비우기
            txtInput.Focus(); // 입력창 포커스
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ThreadStart ts = new ThreadStart(Listen);
            Thread thread = new Thread(ts);
            thread.Start();
        }

        private void Listen()
        {
            AddTextDelegate AddText = new AddTextDelegate(txtView.AppendText);

            // 소켓 생성
            IPAddress add = new IPAddress(0);
            int port = 9000;

            Server = new TcpListener(add, port); // 생성 및 바인딩
            Server.Start(); // 서버 시작

            Invoke(AddText, "서버 시작" + Environment.NewLine);

            Client = Server.AcceptTcpClient(); // 클라이언트 연결 수락

            Connected = true; // 연결 상태 변경

            Invoke(AddText, "클라이언트 연결됨" + Environment.NewLine);

            Stream = Client.GetStream(); // 네트워크 스트림 생성
            Reader = new StreamReader(Stream); // 스트림 리더 생성
            Writer = new StreamWriter(Stream); // 스트림 라이터 생성

            // 수신을 위한 스레드
            ThreadStart ts = new ThreadStart(Receive);
            Thread rcvthread = new Thread(ts);
            rcvthread.Start();
        }

        private void Receive()
        {
            AddTextDelegate AddText = new AddTextDelegate(txtView.AppendText);
            while(Connected)
            {
                if(Stream.CanRead)
                {
                    string temp = Reader.ReadLine();
                    if(temp.Length > 0)
                    {
                        Invoke(AddText, "상대방 : " + temp + Environment.NewLine);
                    }
                }
            }
        }
    }
}
