using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Direction
{
    public enum DirectionType { None, Stage, Crossroad }
    public DirectionType type = DirectionType.None;
    public SelectSceneManager stageSelector;
    public CrossroadPoint crossRoad;
}

public class CrossroadPoint : MonoBehaviour
{
    public Direction up;
    public Direction down;
    public Direction left;
    public Direction right;

    // 各方向が有効かどうかを返す式本体メソッド
    public bool HasUp() => up.type != Direction.DirectionType.None;
    public bool HasDown() => down.type != Direction.DirectionType.None;
    public bool HasLeft() => left.type != Direction.DirectionType.None;
    public bool HasRight() => right.type != Direction.DirectionType.None;

    // 指定された方向からステージセレクターを取得するメソッド
    public SelectSceneManager GetStageSelector(Direction.DirectionType direction)
    {
        switch (direction)
        {
            case Direction.DirectionType.Stage:
                if (up.type == Direction.DirectionType.Stage) return up.stageSelector;
                if (down.type == Direction.DirectionType.Stage) return down.stageSelector;
                if (left.type == Direction.DirectionType.Stage) return left.stageSelector;
                if (right.type == Direction.DirectionType.Stage) return right.stageSelector;
                break;
        }
        return null;
    }

    // 指定された方向から次の分岐点を取得するメソッド
    public CrossroadPoint GetNextCrossroad(string direction)
    {
        switch (direction)
        {
            case "up":
                if (up.type == Direction.DirectionType.Crossroad) return up.crossRoad;
                break;
            case "down":
                if (down.type == Direction.DirectionType.Crossroad) return down.crossRoad;
                break;
            case "left":
                if (left.type == Direction.DirectionType.Crossroad) return left.crossRoad;
                break;
            case "right":
                if (right.type == Direction.DirectionType.Crossroad) return right.crossRoad;
                break;
        }
        return null;
    }
}
