using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public int Id;
    public string Username;

    public bool GameStarted = false;
    public Transform TileParent;
    public GameObject TilePrefab;
    public GameObject[,] Tiles;
    public Text FlagNumber;
    public void FillBoard(int boardX, int boardY, bool _localBoard)
    {
        Tiles = new GameObject[boardX, boardY];
        FlagNumber.text = ((int)math.ceil(boardX * boardY / 5)).ToString();
        for (int x = 0; x < boardX; x++)
        {
            for (int y = 0; y < boardY; y++)
            {
                GameObject _tile = Instantiate(TilePrefab);
                _tile.transform.SetParent(TileParent, false);
                Tiles[x, y] = _tile;
                if (_localBoard)
                {
                    int localX = x;
                    int localY = y;
                    _tile.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        ClientSend.SendRevealTile(localX, localY);
                    });
                    _tile.GetComponent<SetFlag>().X = localX;
                    _tile.GetComponent<SetFlag>().Y = localY;

                }
                else
                    _tile.GetComponent<Button>().enabled = false;
            }
        }
    }

    public void RevealTile(int x, int y, TileType tileType, int numMines)
    {
        switch (tileType)
        {
            case TileType.Revealed:
                Tiles[x, y].GetComponent<Button>().GetComponentInChildren<Text>().text = numMines.ToString();
                break;
            case TileType.Unrevealed:
                Tiles[x, y].GetComponent<Button>().GetComponentInChildren<Text>().text = "?";
                break;
        }
    }

    public void SetFlag(int x, int y, TileType tileType)
    {
        if(tileType == TileType.Flag)
            Tiles[x, y].GetComponent<Button>().GetComponentInChildren<Text>().text = "FLAG";
        else
            Tiles[x, y].GetComponent<Button>().GetComponentInChildren<Text>().text = "?";
    }
    public enum TileType
    {
        Unrevealed,
        Revealed,
        Mine,
        Flag
    }
}
