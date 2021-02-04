using Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.Instance.MyId = _myId;
        ClientSend.WelcomeReceived();
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        int _boardX = _packet.ReadInt();
        int _boardY = _packet.ReadInt();

        GameManager.Instance.SpawnPlayer(_id, _username, _boardX, _boardY);
        if (_id == Client.Instance.MyId) // SpawnPlayer is also called for the other player's board, so do this check so it only sends once
            ClientSend.SpawnReceived();
    }

    public static void StartGame(Packet _packet)
    {
        GameManager.Instance.StartGame();
        int _id = _packet.ReadInt();
        if (_id == Client.Instance.MyId)
            GameManager.Boards[_id].ActivateBoard();
    }

    public static void RevealTile(Packet _packet)
    {
        int id = _packet.ReadInt();
        int x = _packet.ReadInt();
        int y = _packet.ReadInt();
        int tileType = _packet.ReadInt();
        int numMines = _packet.ReadInt();

        BoardManager.TileType _tileType = (BoardManager.TileType)Enum.Parse(typeof(BoardManager.TileType), tileType.ToString());
        if (Client.Instance.MyId == id)
        {
            GameManager.Boards[Client.Instance.MyId].RevealTile(x, y, _tileType, numMines);
            print(_tileType.ToString());
        }
    }

    public static void ReceiveFlag(Packet _packet)
    {
        int id = _packet.ReadInt();
        int x = _packet.ReadInt();
        int y = _packet.ReadInt();
        int tileType = _packet.ReadInt();
        BoardManager.TileType _tileType = (BoardManager.TileType)Enum.Parse(typeof(BoardManager.TileType), tileType.ToString());

        GameManager.Boards[id].SetFlag(x, y, _tileType);
        if (_tileType == BoardManager.TileType.Flag)
            GameManager.Boards[id].FlagNumber.text = (int.Parse(GameManager.Boards[id].FlagNumber.text) - 1).ToString();
        else
            GameManager.Boards[id].FlagNumber.text = (int.Parse(GameManager.Boards[id].FlagNumber.text) + 1).ToString();
    }

    public static void ReceiveDisconnect(Packet _packet)
    {
        int id = _packet.ReadInt();

        // The other player disconnected, pseudo end the game
        // TODO: make this proper
        foreach (var board in GameManager.Boards)
            Destroy(board.Value.gameObject);
        GameManager.Boards.Clear();
        //Other client has already disconnected, this one needs to as well
        Client.Instance.Tcp.Disconnect();
        UIManager.Instance.ReturnToMenu();
    }
}
