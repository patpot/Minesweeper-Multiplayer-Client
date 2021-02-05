using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using Networking;

public class Client : MonoBehaviour
{
    public static Client Instance;
    public static int DataBufferSize = 4096;

    public string Ip = "51.132.34.221";
    public int Port = 26950;
    public int MyId = 0;
    public TCP Tcp;

    private bool _isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> _packetHandlers;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        Tcp = new TCP();
    }

    private void OnApplicationQuit()
    {
        _disconnect();
    }

    public void ConnectToServer()
    {
        InitalizeClientData();

        _isConnected = true;
        Tcp.Connect();
    }

    public class TCP
    {
        public TcpClient Socket;
        private Packet _receivedData;
        private NetworkStream _stream;
        private byte[] _receiveBuffer;

        public void Connect()
        {
            Socket = new TcpClient
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };
            _receiveBuffer = new byte[DataBufferSize];
            Socket.BeginConnect(Instance.Ip, Instance.Port, ConnectCallback, Socket);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            Socket.EndConnect(_result);

            if (!Socket.Connected)
            {
                return;
            }

            _stream = Socket.GetStream();

            _receivedData = new Packet();
            _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (Socket != null)
                {
                    _stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = _stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Instance._disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(_receiveBuffer, _data, _byteLength);

                _receivedData.Reset(HandleData(_data));
                _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving TCP data: {_ex}");
                Disconnect();
            }
        }
        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            _receivedData.SetBytes(_data);

            if (_receivedData.UnreadLength() >= 4)
            {
                _packetLength = _receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= _receivedData.UnreadLength())
            {
                byte[] _packetBytes = _receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        _packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (_receivedData.UnreadLength() >= 4)
                {
                    _packetLength = _receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        public void Disconnect(bool _return = true)
        {
            //temp
            if (_stream == null) return;
            //Handles necessary server disconnection
            Instance._disconnect();

            _stream = null;
            _receivedData = null;
            _receiveBuffer = null;
            Socket = null;
            //Handles necessary client disconnection
            foreach (var board in GameManager.Boards)
                Destroy(board.Value.gameObject);
            GameManager.Boards.Clear();
            if (_return)
                UIManager.Instance.ReturnToMenu();
        }
    }

    private void InitalizeClientData()
    {
        _packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.WELCOME, ClientHandle.Welcome },
            { (int)ServerPackets.SPAWN_PLAYER, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.START_GAME, ClientHandle.StartGame },
            { (int)ServerPackets.SEND_REVEAL_TILE, ClientHandle.RevealTile },
            { (int)ServerPackets.SEND_FLAG, ClientHandle.ReceiveFlag },
            { (int)ServerPackets.CLIENT_DISCONNECT, ClientHandle.ReceiveDisconnect },
            { (int)ServerPackets.PLAYER_HIT_MINE, ClientHandle.PlayerHitMine },
            { (int)ServerPackets.SEND_MESSAGE, ClientHandle.ReceiveMessage },
            { (int)ServerPackets.GAME_END, ClientHandle.ReceiveEndGame }
        };
        Debug.Log("Initialized packets.");
    }

    private void _disconnect()
    {
        if (_isConnected)
        {
            _isConnected = false;
            Tcp.Socket.Close();
        }
    }
}
