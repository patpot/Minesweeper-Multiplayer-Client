using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Sprite[] Mines;
    public Sprite Flag;
    public Sprite BlankTile;

    public static Dictionary<int, BoardManager> Boards = new Dictionary<int, BoardManager>();

    public Canvas MainCanvas;
    public GameObject LocalBoardPrefab;
    public Vector3 Player1Board;
    public GameObject BoardPrefab;
    public Vector3 Player2Board;

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

    public void SpawnPlayer(int _id, string _username, int boardX, int boardY)
    {
        GameObject _board;
        if (_id == Client.Instance.MyId)
        {
            _board = Instantiate(LocalBoardPrefab, Player1Board, Quaternion.identity);
            _board.GetComponent<BoardManager>().FillBoard(boardX, boardY, true);
        }
        else
        {
            _board = Instantiate(BoardPrefab, Player2Board, Quaternion.identity);
            _board.GetComponent<BoardManager>().FillBoard(boardX, boardY, false);
        }
        _board.transform.SetParent(MainCanvas.transform, false);

        _board.GetComponent<BoardManager>().Id = _id;
        _board.GetComponent<BoardManager>().Username = _username;
        _board.GetComponentInChildren<Text>().text = _username;
        Boards.Add(_id, _board.GetComponent<BoardManager>());
    }

    public void StartGame()
    {
        Debug.Log("Game started");
    }
}
