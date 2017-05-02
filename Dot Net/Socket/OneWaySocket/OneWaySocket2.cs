﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISocket;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Simulation1
{
    public class OneWaySocket2 : ISocket.IOneWaySocket2
    {
        public Thread myThread1;             //声明一个线程实例
        string ActiveIP = string.Empty;

        public OneWaySocket2()
        {
            pool = new Dictionary<string, Socket>();
        }

        public void Listen()
        {
            if (this.InputIP == string.Empty || this.InputPort == string.Empty)
            {
                Message = "ListenIP are Unavailable!"; return;
            }

            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(this.InputIP), int.Parse(this.InputPort));

            Local = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Local.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            try
            {
                
                Local.Bind(localEP);
                Local.Listen(0);
                ListenState = true;
                Local.BeginAccept(new AsyncCallback(OnConnectRequest), Local);
            }
            catch (Exception ex)
            {
                Local.Dispose();
                Message = ex.Message; return;
            }
            Message = "Listen " + Local.LocalEndPoint.ToString() + " Successful!";
        }

        private void OnConnectRequest(IAsyncResult ar)
        {
            if (!ListenState) return;

            //初始化一个SOCKET，用于其它客户端的连接
            Socket server1 = (Socket)ar.AsyncState;
            Socket Client = server1.EndAccept(ar);
            Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            ActiveIP = Client.RemoteEndPoint.ToString();
            if (!pool.Keys.Contains(ActiveIP))
            {
                pool.Add(ActiveIP, Client);
            }

            #region 客户端连接成功，返回成功标识。
            //将要发送给连接上来的客户端的提示字符串
            Byte[] byteDateLine = new byte[2048];
            byteDateLine = System.Text.Encoding.ASCII.GetBytes("Connect to " + server1.LocalEndPoint.ToString() + " Successful!");

            //将提示信息发送给客户端
            if (Client.Connected)
            {
                Client.Send(byteDateLine, byteDateLine.Length, 0);
            }
            #endregion

            Local.BeginAccept(new AsyncCallback(OnConnectRequest), Local);
            if (OnConnected != null)
            {
                OnConnected(this, null);
            }
            ReceiveFormClient(Client);
        }

        public void Connect()
        {
            if (this.InputIP == string.Empty || this.InputPort == string.Empty)
            {
                Message = "BindIP are Unavailable!"; return;
            }

            if (this.OutIP == string.Empty || this.OutPort == string.Empty)
            {
                Message = "ConnectIP are Unavailable!"; return;
            }

            Remote = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Remote.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            if (!ListenState)
            {
                IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(this.InputIP), int.Parse(this.InputPort));
                Local = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Local.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                Remote.Bind(localEP);
            }

            IPEndPoint RemoteEP = new IPEndPoint(IPAddress.Parse(this.OutIP), int.Parse(this.OutPort));

            try
            {
                Remote.Connect(RemoteEP);
                ConnectState = true;
            }
            catch (Exception ex)
            {
                if (Remote != null)
                {
                    Remote.Close();
                    Remote = null;
                }
                if (Local != null)
                {
                    Local.Close();
                    Local = null;
                }
                ListenState = false;
                ConnectState = false;
                Message = ex.Message; return;
            }
            Byte[] byteDateLine = new byte[2048];
            int recv = Remote.Receive(byteDateLine);
            Message = Encoding.ASCII.GetString(byteDateLine, 0, recv);

            ThreadStart myThreadDelegate = new ThreadStart(ReceiveFormServer);
            myThread1 = new Thread(myThreadDelegate);
            myThread1.Start();
        }

        public void Stop()
        {
            try
            {
                ListenState = false;
                ConnectState = false;
                sendmsg("XSTOPX");

                if (pool != null && pool.Count > 0)
                {
                    pool.Clear();
                }

                if (Remote != null)
                {
                    Remote.Shutdown(SocketShutdown.Both);
                    Remote.Close();
                    Remote = null;
                }
                if (Local != null)
                {
                    Local.Close();
                    Local = null;
                }

            

                //foreach (var item in pool)
                //{
                //    item.Value.Close();
                //}
                //pool.Clear();

                if (myThread1 != null)
                {
                    if (myThread1.ThreadState != ThreadState.Aborted && myThread1.ThreadState != ThreadState.Stopped)
                    {
                        myThread1.Abort();
                    }
                }
            }
            catch { }
        }

        public void Send(string msg)
        {
            Message = sendmsg(msg);
        }

        string sendmsg(string msg)
        {
            try
            {
                byte[] b = Encoding.ASCII.GetBytes(msg);
                if (Remote != null && Remote.Connected)
                {
                    Remote.Send(b, b.Length, 0);
                }

                foreach (var item in pool)
                {
                    item.Value.Send(b, b.Length, 0);
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return "OK";
        }


        public void ReceiveFormClient(Socket s)
        {
            while (ListenState || ConnectState)
            {
                Byte[] byteDateLine = new byte[2048];
                try
                {
                    int recv = s.Receive(byteDateLine);
                    string stringdata = Encoding.ASCII.GetString(byteDateLine, 0, recv);
                    if (stringdata == string.Empty)
                    {
                        continue;
                    }
                    switch (stringdata)
                    {
                        case "XSTOPX":
                            {
                                Message = "[" + s.RemoteEndPoint.ToString() + "]Stopped!";
                                if (ListenState)
                                {
                                    //pool[s.RemoteEndPoint.ToString()].Shutdown(SocketShutdown.Both);
                                    //pool[s.RemoteEndPoint.ToString()].Disconnect(true);
                                    //pool[s.RemoteEndPoint.ToString()].Close();
                                    //pool[s.RemoteEndPoint.ToString()] = null;
                                    pool.Remove(s.RemoteEndPoint.ToString());
                                }

                                //if (Local != null)
                                //{
                                //    Local.BeginAccept(new AsyncCallback(OnConnectRequest), Local);
                                //}

                                if (OnMessageReceived != null)
                                {
                                    OnMessageReceived(this, null);
                                }

                                return;
                            }
                        default:
                            {
                                Message = "[Message from " + s.RemoteEndPoint.ToString() + "] " + Encoding.ASCII.GetString(byteDateLine, 0, recv);
                                if (OnMessageReceived != null)
                                {
                                    OnMessageReceived(this, null);
                                }
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    if (Local != null)
                    {
                        Local.Close();
                        Local = null;
                    }
                    if (s != null)
                    {
                        s.Close();
                        s = null;
                    }
                    Message = ex.Message; return;
                }
            }
        }

        public void ReceiveFormServer()
        {
            while (ListenState || ConnectState)
            {
                Byte[] byteDateLine = new byte[2048];
                try
                {
                    int recv = Remote.Receive(byteDateLine);
                    string stringdata = Encoding.ASCII.GetString(byteDateLine, 0, recv);
                    if (stringdata == string.Empty)
                    {
                        continue;
                    }
                    switch (stringdata)
                    {
                        case "XSTOPX":
                            {
                                Message = "[" + Remote.RemoteEndPoint.ToString() + "]Stopped!";

                                if (ConnectState)
                                {
                                    ConnectState = false;
                                    if (Local != null)
                                    {
                                        Local.Close();
                                        Local = null;
                                    }
                                    if (Remote != null)
                                    {
                                        Remote.Shutdown(SocketShutdown.Both);
                                        Remote.Close();
                                        Remote = null;
                                    }
                                }

                                if (OnMessageReceived != null)
                                {
                                    OnMessageReceived(this, null);
                                }

                                if (OnStopped != null)
                                {
                                    OnStopped(this, null);
                                }
                                if (myThread1 != null)
                                {
                                    if (myThread1.ThreadState != ThreadState.Aborted && myThread1.ThreadState != ThreadState.Stopped)
                                    {
                                        myThread1.Abort();
                                    }
                                }
                                return;
                            }
                        default:
                            {
                                Message = "[Message from " + Remote.RemoteEndPoint.ToString() + "] " + Encoding.ASCII.GetString(byteDateLine, 0, recv);
                                if (OnMessageReceived != null)
                                {
                                    OnMessageReceived(this, null);
                                }
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    if (Local != null)
                    {
                        Local.Close();
                        Local = null;
                    }
                    if (Remote != null)
                    {
                        Remote.Disconnect(true);
                        Remote.Close();
                        Remote = null;

                    }
                    Message = ex.Message; return;
                }
            }
        }

        #region
        public string Name { get; set; }

        public string InputIP { get; set; }

        public string InputPort { get; set; }

        public string OutIP { get; set; }

        public string OutPort { get; set; }

        public bool ListenState { get; set; }

        public bool ConnectState { get; set; }

        public string Message { get; set; }

        public Socket Remote { get; set; }

        public Socket Local { get; set; }

        public event EventHandler OnMessageReceived;
        public event EventHandler OnConnected;
        public event EventHandler OnStopped;

        public Dictionary<string, Socket> pool
        {
            get;
            set;
        }
        #endregion
    }
}