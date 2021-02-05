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

    public Transform TileParent;
    public GameObject TilePrefab;
    public GameObject[,] Tiles;
    public GameObject[] Lives;
    public Button LockInButton;
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
                _tile.GetComponent<Button>().enabled = false;
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
            }
        }
    }

    public void ActivateBoard()
    {
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                GameObject _tile = Tiles[x, y];
                _tile.GetComponent<Button>().enabled = true;
            }
        }
    }

    public void DeactivateBoard()
    {
        LockInButton.enabled = false;
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                GameObject _tile = Tiles[x, y];
                _tile.GetComponent<Button>().enabled = false;
            }
        }
    }

    public void SetLives(int lives)
    {
        for (int i = 0; i < lives; i++)
            Lives[i].SetActive(true);

        for (int j = lives; j < Lives.Length; j++)
            Lives[j].SetActive(false);
    }

    public void RevealTile(int x, int y, TileType tileType, int numMines)
    {
        switch (tileType)
        {
            case TileType.Revealed:
                Tiles[x, y].GetComponent<Image>().sprite = GameManager.Instance.Mines[numMines];
                break;
            case TileType.Unrevealed:
                Tiles[x, y].GetComponent<Image>().sprite = GameManager.Instance.Flag;
                break;
        }
    }

    public void SetFlag(int x, int y, TileType tileType)
    {
        if(tileType == TileType.Flag)
            Tiles[x, y].GetComponent<Image>().sprite = GameManager.Instance.Flag;
        else
            Tiles[x, y].GetComponent<Image>().sprite = GameManager.Instance.BlankTile;
    }

    public void LockIn()
    {
        DeactivateBoard();
        ClientSend.LockIn();
    }

    public enum TileType
    {
        Unrevealed,
        Revealed,
        Flag
    }
}
