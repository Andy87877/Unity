using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ch : MonoBehaviour
{
    // 四個瞄點的位置
    public GameObject LeftTop;
    public GameObject RightTop;
    public GameObject LeftBottom;
    public GameObject RightBottom;

    // 按鈕的UI面板
    public GameObject buttomPanel;

    // 主攝影機
    public Camera cam;

    // 黑白棋圖片的紋理
    public Texture2D black;
    public Texture2D white;

    // 黑白勝利圖片
    public Texture2D blackWin;
    public Texture2D whiteWin;

    // 重新開始按鈕
    public Button restartBtn;

    // 四個瞄點在螢幕上的位置
    Vector3 LTPos;
    Vector3 RTPos;
    Vector3 LBPos;
    Vector3 RBPos;

    // 儲存棋盤上每個交點的位置(落的位置)
    List<List<Vector2>> chessPos;

    // 定義棋盤的寬度和高度
    float gridWidth = 1;
    float gridHeight = 1;
    // 取網格寬度和高度最小的
    float minGridDis;

    // 棋盤上的落子狀態
    List<List<int>> chessState;
    int flag = 0; // 判斷是哪一方下棋

    // Start is called before the first frame update
    void Start() {
        // 初始化成 15*15 大小的二維列表
        chessPos = new List<List<Vector2>>();
        chessState = new List<List<int>>();

        for (int i = 0; i < 15; i++) {
            chessPos.Add(new List<Vector2>());
            chessState.Add(new List<int>());
            for (int j = 0; j < 15; j++) {
                chessPos[i].Add(Vector2.zero);
                chessState[i].Add(0);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        // 計算瞄點座標 (2維 vector)
        LTPos = cam.WorldToScreenPoint(LeftTop.transform.position);
        RTPos = cam.WorldToScreenPoint(RightTop.transform.position);
        LBPos = cam.WorldToScreenPoint(LeftBottom.transform.position);
        RBPos = cam.WorldToScreenPoint(RightBottom.transform.position);

        // 計算棋盤寬度和長度
        gridWidth = (RTPos.x - LTPos.x) / 14;
        gridHeight = (LTPos.y - LBPos.y) / 14;
        // (三元運算式)取較小的長寬度，確保網格是正方形
        minGridDis = gridWidth < gridHeight ? gridWidth : gridHeight;

        // 計算棋盤上可以落子的位置
        for (int i = 0; i < 15; i++) { 
            for (int j = 0; j < 15; j++) {
                chessPos[i][j] = new Vector2(LBPos.x + gridWidth * i, LBPos.y + gridHeight * j);
            }
        }

        Vector3 PointPos;
        if (Input.GetMouseButtonDown(0)) { //滑鼠按下左鍵
            PointPos = Input.mousePosition;
            if (PlaceChess(PointPos)) {
                flag = 1 - flag;
            }
        }
    }


    private void OnGUI() {
        //繪製棋子
        for (int i = 0; i < 15; i++) {
            for (int j = 0; j < 15; j++) {
                if (chessState[i][j] == 1) { // 繪製黑棋
                    // Rect(x, y, width, height)
                    GUI.DrawTexture(new Rect(chessPos[i][j].x - gridWidth / 2, Screen.height - chessPos[i][j].y - gridHeight/2, gridWidth, gridHeight), black);
                }
                if (chessState[i][j] == -1) { // 繪製白棋
                    GUI.DrawTexture(new Rect(chessPos[i][j].x - gridWidth / 2, Screen.height - chessPos[i][j].y - gridHeight / 2, gridWidth, gridHeight), white);
                }
            }
        }
    }

    // 計算兩點的距離(歐幾里得)
    private float Distance(Vector3 mPos, Vector2 gridPos) {
        return Mathf.Sqrt(Mathf.Pow(mPos.x - gridPos.x, 2) + Mathf.Pow(mPos.y - gridPos.y, 2));
    }

    // 根據玩家點擊的位置找到最近的棋盤位置
    private bool PlaceChess(Vector3 PointPos) {
        float minDis = float.MaxValue; // 最小距離
        Vector2 closestPos = Vector2.zero; //最近的位置
        int closestX = -1, closestY = -1; //最近位置的座標(索引值)

        // 找距離最近的值
        for (int i = 0; i < 15; i++) { 
            for (int j = 0; j < 15; j++) {
                float dist = Distance(PointPos, chessPos[i][j]);
                if (dist < minDis/2 && chessState[i][j] == 0)
                {
                    minDis = dist;
                    closestPos = chessPos[i][j];
                    closestX = i;
                    closestY = j;
                }
            }
        }
        if (closestX != -1 && closestY != -1) {
            chessState[closestX][closestY] = flag == 0 ? 1 : -1;
            return true; // 成功放置棋子
        }
        return false; // 沒有放置棋子
    }
}
