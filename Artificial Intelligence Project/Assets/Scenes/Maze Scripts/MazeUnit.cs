using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MazeUnit
{
    public bool HasBeenReached;

    public bool TopWall;
    public bool BottomWall;
    public bool LeftWall;
    public bool RightWall;

    public MazeUnit(bool hasBeenReached, bool topWall, bool bottomWall, bool leftWall, bool rightWall)
    {
        HasBeenReached = hasBeenReached;
        TopWall = topWall;
        BottomWall = bottomWall;
        LeftWall = leftWall;
        RightWall = rightWall;
    }
}