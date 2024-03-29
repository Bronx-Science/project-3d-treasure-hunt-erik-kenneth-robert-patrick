  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public GameObject MazeWall;
    public GameObject MazeWallTorch;
    public float ChanceOfTorch;
    public float WallWidth;
    public int MazeDimensions;
    public MazeUnit[,] mazeUnits;
    public GameObject MazeStartPosition;
    public float ChanceOfHigherWall;

    public GameObject WallToBeRemoved;
    private bool bRemovingWall;

    // Start is called before the first frame update
    void Start()
    {
        bRemovingWall = false;
        WallToBeRemoved.SetActive(false);
        Invoke(nameof(SpawnWall), 1.5f);
        Invoke(nameof(RemoveWall), 4);

        mazeUnits = new MazeUnit[MazeDimensions, MazeDimensions];

        for (int i = 0; i < MazeDimensions; i++)
        {
            for(int j = 0; j < MazeDimensions; j++)
            {
                mazeUnits[i, j] = new MazeUnit(false, true, true, true, true);
            }
        }

        GenerateMaze(0, 0, 0, 0, 0);

        float XOffset = MazeStartPosition.transform.position.x;
        float YOffset = MazeStartPosition.transform.position.y;
        float ZOffset = MazeStartPosition.transform.position.z;

        for (int i = 0; i < MazeDimensions; i++)
        {
            for (int j = 0; j < MazeDimensions; j++)
            {
                if (mazeUnits[i, j].TopWall)
                {
                    GameObject Wall;

                    if (Random.Range(0.0f, 1.0f) > ChanceOfTorch)
                    {
                        Wall = Instantiate(MazeWallTorch);
                    }

                    else
                    {
                        Wall = Instantiate(MazeWall);
                    }

                    Wall.transform.parent = transform;
                    Wall.transform.Rotate(0, 90, 0);
                    Wall.transform.localPosition = (new Vector3(XOffset + (i * WallWidth + WallWidth / 2), YOffset, ZOffset + (j * WallWidth)) - Wall.transform.forward * 0.1f);
                }

                else
                {
                    if (Random.Range(0.0f, 1.0f) > ChanceOfHigherWall && (i > 4 || j > 4))
                    {
                        GameObject Wall = Instantiate(MazeWall);

                        Wall.transform.parent = transform;
                        Wall.transform.localPosition = new Vector3(XOffset + (i * WallWidth + WallWidth / 2), YOffset + 2f, ZOffset + (j * WallWidth));
                        Wall.transform.Rotate(0, 90, 0);
                    }
                }

                if (mazeUnits[i, j].BottomWall)
                {
                    GameObject Wall;

                    if (Random.Range(0.0f, 1.0f) > ChanceOfTorch)
                    {
                        Wall = Instantiate(MazeWallTorch);
                    }

                    else
                    {
                        Wall = Instantiate(MazeWall);
                    }

                    Wall.transform.parent = transform;
                    Wall.transform.Rotate(0, 270, 0);
                    Wall.transform.localPosition = new Vector3(XOffset + (i * WallWidth - WallWidth / 2), YOffset, ZOffset + (j * WallWidth)) - Wall.transform.forward * 0.1f;
                }

                else
                {
                    if (Random.Range(0.0f, 1.0f) > ChanceOfHigherWall && (i > 4 || j > 4))
                    {
                        GameObject Wall = Instantiate(MazeWall);

                        Wall.transform.parent = transform;
                        Wall.transform.localPosition = new Vector3(XOffset + (i * WallWidth - WallWidth / 2), YOffset + 2f, ZOffset + (j * WallWidth));
                        Wall.transform.Rotate(0, 90, 0);
                    }
                }

                if (mazeUnits[i, j].RightWall)
                {
                    GameObject Wall;

                    if (Random.Range(0.0f, 1.0f) > ChanceOfTorch)
                    {
                        Wall = Instantiate(MazeWallTorch);
                    }

                    else
                    {
                        Wall = Instantiate(MazeWall);
                    }

                    Wall.transform.parent = transform;
                    Wall.transform.localPosition = new Vector3(XOffset + (i * WallWidth), YOffset, ZOffset + (j * WallWidth + WallWidth / 2)) - Wall.transform.forward * 0.1f;
                }

                else
                {
                    if (Random.Range(0.0f, 1.0f) > ChanceOfHigherWall && (i > 4 || j > 4))
                    {
                        GameObject Wall = Instantiate(MazeWall);

                        Wall.transform.parent = transform;
                        Wall.transform.localPosition = new Vector3(XOffset + (i * WallWidth), YOffset + 2f, ZOffset + (j * WallWidth + WallWidth / 2));
                    }
                }

                if (mazeUnits[i, j].LeftWall)
                {
                    GameObject Wall;

                    if (Random.Range(0.0f, 1.0f) > ChanceOfTorch)
                    {
                        Wall = Instantiate(MazeWallTorch);
                    }

                    else
                    {
                        Wall = Instantiate(MazeWall);
                    }

                    Wall.transform.parent = transform;
                    Wall.transform.Rotate(0, 180, 0);
                    Wall.transform.localPosition = new Vector3(XOffset + (i * WallWidth), YOffset, ZOffset + (j * WallWidth - WallWidth / 2)) - Wall.transform.forward * 0.1f;
                }

                else
                {
                    if (Random.Range(0.0f, 1.0f) > ChanceOfHigherWall && (i > 4 || j > 4))
                    {
                        GameObject Wall = Instantiate(MazeWall);

                        Wall.transform.parent = transform;
                        Wall.transform.localPosition = new Vector3(XOffset + (i * WallWidth), YOffset + 2f, ZOffset + (j * WallWidth - WallWidth / 2));
                    }
                }

                if(i == MazeDimensions - 1 && j == MazeDimensions - 1)
                {
                    GameObject Wall = Instantiate(MazeWall);

                    Wall.transform.parent = transform;
                    Wall.transform.localPosition = new Vector3(XOffset + (i * WallWidth + WallWidth / 2), YOffset + 2f, ZOffset + (j * WallWidth));
                    Wall.transform.Rotate(0, 90, 0);
                }
            }
        }
    }

    void GenerateMaze(int xPos, int yPos, int TRBL, int prevxPos, int prevyPos)
    {
        if(xPos < 0 || yPos < 0 || xPos >= MazeDimensions || yPos >= MazeDimensions)
        {
            return;
        }

        if (mazeUnits[xPos, yPos].HasBeenReached)
        {
            return;
        }

        mazeUnits[xPos, yPos].HasBeenReached = true;

        if(xPos == MazeDimensions - 1 && yPos == MazeDimensions - 1)
        {
            mazeUnits[xPos, yPos].TopWall = false;
        }

        if(xPos == 0 && yPos == 0)
        {
            mazeUnits[xPos, yPos].LeftWall = false;
        }

        if (TRBL == 1)
        {
            mazeUnits[xPos, yPos].TopWall = false;
            mazeUnits[prevxPos, prevyPos].BottomWall = false;
        }

        if(TRBL == 2)
        {
            mazeUnits[xPos, yPos].LeftWall = false;
            mazeUnits[prevxPos, prevyPos].RightWall = false;
        }

        if(TRBL == 3)
        {
            mazeUnits[xPos, yPos].BottomWall = false;
            mazeUnits[prevxPos, prevyPos].TopWall = false;
        }

        if(TRBL == 4)
        {
            mazeUnits[xPos, yPos].RightWall = false;
            mazeUnits[prevxPos, prevyPos].LeftWall = false;
        }

        bool[] BoxsNearCurrentBox = new bool[4];
        bool HasNotCheckedEveryBoxNear = true;
        
        while(HasNotCheckedEveryBoxNear)
        {
            int RandNum = Random.Range(1, 5);

            if (RandNum == 1)
            {
                GenerateMaze(xPos - 1, yPos, 1, xPos, yPos);
                BoxsNearCurrentBox[RandNum - 1] = true;
            }

            if (RandNum == 2)
            {
                GenerateMaze(xPos, yPos + 1, 2, xPos, yPos);
                BoxsNearCurrentBox[RandNum - 1] = true;
            }

            if (RandNum == 3)
            {
                GenerateMaze(xPos + 1, yPos, 3, xPos, yPos);
                BoxsNearCurrentBox[RandNum - 1] = true;
            }

            if (RandNum == 4)
            {
                GenerateMaze(xPos, yPos - 1, 4, xPos, yPos);
                BoxsNearCurrentBox[RandNum - 1] = true;
            }

            HasNotCheckedEveryBoxNear = false;

            for(int i = 0; i < 4; i++)
            {
                if (BoxsNearCurrentBox[i] == false)
                {
                    HasNotCheckedEveryBoxNear = true;
                }
            }
        }
    }

    void SpawnWall()
    {
        WallToBeRemoved.SetActive(true);
    }

    void RemoveWall()
    {
        bRemovingWall = true;

        Invoke(nameof(StopRemovingWall), 2);
    }

    void StopRemovingWall()
    {
        bRemovingWall = false;
    }

    private void FixedUpdate()
    {
        if (bRemovingWall)
        {
            WallToBeRemoved.transform.position = WallToBeRemoved.transform.position - new Vector3(0, 20f / 120.0f, 0);
        }
    }
}
        