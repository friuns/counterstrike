﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using doru;
using System.IO;
using System.Net.Sockets;
using doru.TcpSilverlight;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CSLIVE
{

    public class BossClientA : BossClient, IUpdate
    {
        public BossClientA()
            : base()
        {

        }
        public Dispatcher Dispatcher;
        public void Load()
        {
            _RemoteSharedObj._OnSerialized += new Action(Connect);
        }
        void Connect()
        {

            if (_Server)
            {                                
                Helper.Connect(_IpAddress, Dispatcher, Connected);                
            }
        }        

        
        public new LocalSharedObj<BossClientA> _LocalSharedObj { get { return (LocalSharedObj<BossClientA>)_SharedObj; } }
        public new RemoteSharedObj<BossClientA> _RemoteSharedObj { get { return (RemoteSharedObj<BossClientA>)_SharedObj; } }
        public void Connected(SocketAsyncEventArgs e)
        {            
            Trace.Assert(e.SocketError == SocketError.Success);
            Socket _Socket = (Socket)e.UserToken;
            NetworkStream nw = new NetworkStream(_Socket);
            _Listener = new Listener { _NetworkStream = nw };
            _Listener.StartAsync();
            _Sender = new Sender() { _NetworkStream = nw };
            _Sender.Send(PacketType.getrooms);
            new DispatcherTimer().RepeatMethod(1, Ping);            
        }
        Listener _Listener;
        Sender _Sender;
        DateTime _PingDate;

        public void Pong()
        {
            TimeSpan ts = DateTime.Now - _PingDate;
            _Ping = (int)ts.TotalMilliseconds;
        }
        private ObservableCollection<RoomDb> roomList = new ObservableCollection<RoomDb>();
        public ObservableCollection<RoomDb> _RoomList { get { return roomList; } set { roomList = value; } }

        public void Ping()
        {
            ("connecting to server3" + _Server).Trace();
            _PingDate = DateTime.Now;
            _Sender.Send(PacketType.ping);
        }

        
        public void Update()
        {
            if (_Listener != null)            
                foreach (byte[] msg in _Listener.GetMessages())
                {
                    MemoryStream _MemoryStream = new MemoryStream(msg);
                    byte idFrom = _MemoryStream.ReadB();
                    PacketType pk = (PacketType)_MemoryStream.ReadB();
                    if (idFrom == Common._ServerId)
                        switch (pk)
                        {
                            case PacketType.pong:
                                Pong();
                                break;
                            case PacketType.rooms:
                                List<RoomDb> _rooms = (List<RoomDb>)Common._XmlSerializerRoom.Deserialize(_MemoryStream);
                                foreach (RoomDb room in _rooms)
                                    if (room is CSRoom)
                                        _RoomList.Add(room);                                
                                break;
                            default:                            
                                Trace.Fail("wrong packet");
                                break;
                        } else Trace.Fail("wrong packet");
                }
            
        }
    }
}

