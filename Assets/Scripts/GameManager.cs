using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BLOCKCOLOR
{ PINK, YELLOW, ORANGE, VIOLET, BLUE, RED, NAVY, GREEN }

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private GameObject startButton, endPanel, player, left, right, blockPrefab;

    [SerializeField]
    private TMP_Text scoreText, highScoreText, highScoreEndText;

    private int score, highScore;
    private BLOCKCOLOR currentColor;

    private const float maxSize = 60f;
    private int currentLevel;
    readonly Dictionary<int, int> levelToBlock = new Dictionary<int, int>()
    {
        {1,1},{2,2},{3,3},{4,4},{5,5},{6,6},{7,8}
    };
    readonly Dictionary<int, int> levelToScore = new Dictionary<int, int>()
    {
        {1,1},{2,2},{3,5},{4,6},{5,75},{6,100},{7,150}
    };

    [SerializeField]
    private List<Color32> colors = new List<Color32>();

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        startButton.SetActive(true);
        endPanel.SetActive(false);

        Time.timeScale = 1f;
        score = 0;
        highScore = PlayerPrefs.HasKey("HighScore") ? PlayerPrefs.GetInt("HighScore") :0;
        scoreText.text = score.ToString();
        highScoreText.text = "BEST " +  highScore.ToString();

        currentLevel = 1;
        SpawnBlocks();

        currentColor = (BLOCKCOLOR)Random.Range(0, 8);
        player.GetComponent<SpriteRenderer>().color = colors[(int)currentColor];
        player.GetComponent<Player>().playerColor = currentColor;
        SetColors();
    }

    void SpawnBlocks()
    {
        int numOfBlocks = levelToBlock[currentLevel];
        for (int i = 0; i < numOfBlocks; i++)
        {
            GameObject tempBlock = Instantiate(blockPrefab);
            tempBlock.transform.parent = left.transform;
            Vector3 tempScale = tempBlock.transform.localScale;
            tempScale.y = maxSize / numOfBlocks;
            tempBlock.transform.localScale = tempScale;
            Vector3 tempPos = Vector3.zero;
            tempPos.y = tempScale.y * (numOfBlocks / 2 - i) - (numOfBlocks % 2 == 0 ? tempScale.y / 2 : 0);
            tempBlock.transform.localPosition = tempPos;

            tempBlock = Instantiate(blockPrefab);
            tempBlock.transform.parent = right.transform;
            tempBlock.transform.localScale = tempScale;
            tempBlock.transform.localPosition = tempPos;
        }
    }

    void SetColors()
    {
        List<GameObject> tempList = new List<GameObject>();
        List<BLOCKCOLOR> tempColorList = new List<BLOCKCOLOR>() {BLOCKCOLOR.PINK,BLOCKCOLOR.YELLOW,BLOCKCOLOR.ORANGE,
            BLOCKCOLOR.VIOLET,BLOCKCOLOR.BLUE,BLOCKCOLOR.RED,BLOCKCOLOR.NAVY,BLOCKCOLOR.GREEN
        };
        List<BLOCKCOLOR> currentColorList = new List<BLOCKCOLOR>() { currentColor };
        tempColorList.Remove(currentColor);
        tempList.Add(left.transform.GetChild(0).gameObject);

        for (int i = 1; i < left.transform.childCount; i++)
        {
            tempList.Add(left.transform.GetChild(i).gameObject);
            BLOCKCOLOR tempColor = tempColorList[Random.Range(0, tempColorList.Count)];
            currentColorList.Add(tempColor);
            tempColorList.Remove(tempColor);
        }

        for (int i = 0; i < currentColorList.Count; i++)
        {
            GameObject temp = tempList[Random.Range(0, tempList.Count)];
            temp.GetComponent<Block>().myColor = currentColorList[i];
            temp.GetComponent<SpriteRenderer>().color = colors[(int)currentColorList[i]];
            tempList.Remove(temp);
        }

        tempList = new List<GameObject>();
        for (int i = 0; i < left.transform.childCount; i++)
        {
            tempList.Add(right.transform.GetChild(i).gameObject);            
        }
        for (int i = 0; i < currentColorList.Count; i++)
        {
            GameObject temp = tempList[Random.Range(0, tempList.Count)];
            temp.GetComponent<Block>().myColor = currentColorList[i];
            temp.GetComponent<SpriteRenderer>().color = colors[(int)currentColorList[i]];
            tempList.Remove(temp);
        }
    }


    void Reshuffle()
    {
        currentLevel++;
        List<GameObject> tempList = new List<GameObject>();

        for (int i = 0; i < left.transform.childCount; i++)
        {
            tempList.Add(left.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            DestroyImmediate(tempList[i]);
        }

        tempList = new List<GameObject>();

        for (int i = 0; i < right.transform.childCount; i++)
        {
            tempList.Add(right.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            DestroyImmediate(tempList[i]);
        }

        SpawnBlocks();
    }

    public void UpdateScore()
    {
        score++;
        scoreText.text = score.ToString();
        Invoke("SetBlocks", 0.05f);
    }

    void SetBlocks()
    {
        currentColor = (BLOCKCOLOR)Random.Range(0, 8);
        player.GetComponent<SpriteRenderer>().color = colors[(int)currentColor];
        player.GetComponent<Player>().playerColor = currentColor;

        if(levelToScore.TryGetValue(currentLevel + 1, out int tempScore))
        {
            if(score > tempScore)
            {
                Reshuffle();
            }
        }

        SetColors();
    }

    public void GameStart()
    {
        startButton.SetActive(false);
        player.GetComponent<Player>().GameStart();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        endPanel.SetActive(true);
        if(score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        highScoreEndText.text = "BEST " + highScore.ToString();
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void GameRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
