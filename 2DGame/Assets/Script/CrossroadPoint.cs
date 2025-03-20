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

    // �e�������L�����ǂ�����Ԃ����{�̃��\�b�h
    public bool HasUp() => up.type != Direction.DirectionType.None;
    public bool HasDown() => down.type != Direction.DirectionType.None;
    public bool HasLeft() => left.type != Direction.DirectionType.None;
    public bool HasRight() => right.type != Direction.DirectionType.None;

    // �w�肳�ꂽ��������X�e�[�W�Z���N�^�[���擾���郁�\�b�h
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

    // �w�肳�ꂽ�������玟�̕���_���擾���郁�\�b�h
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
