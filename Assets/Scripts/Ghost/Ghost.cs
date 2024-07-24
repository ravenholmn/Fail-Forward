using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu]
public class Ghost : ScriptableObject
{
    public bool record;
    public bool replay;
    public float recordFrequency;

    // public List<GhostData> ghostDataList = new List<GhostData>();

    public List<float> timeStamps = new List<float>();
    public List<Vector2> positions = new List<Vector2>();
    public List<float> speedValues = new List<float>();
    public List<bool> jumpBools = new List<bool>();
    public List<bool> crouchBools = new List<bool>();
    public List<bool> deadBools = new List<bool>();
    public List<GhostData> ghostDatas = new List<GhostData>();

    public void Add(float timeStamp, Vector2 position, float speed, bool jumping, bool crouching, bool dead)
    {
        timeStamps.Add(timeStamp);
        positions.Add(position);
        speedValues.Add(speed);
        jumpBools.Add(jumping);
        crouchBools.Add(crouching);
        deadBools.Add(dead);
    }

    public void TransferData()
    {
        if (ghostDatas.Count > 0)
        {
            ghostDatas.Clear();
        }
        for (int i = 0; i < timeStamps.Count; i++)
        {
            GhostData data = new GhostData(timeStamps[i], positions[i], speedValues[i], jumpBools[i], crouchBools[i], deadBools[i]);
            ghostDatas.Add(data);
        }
    }

    public void ResetData()
    {
        timeStamps.Clear();
        positions.Clear();
        speedValues.Clear();
        jumpBools.Clear();
        crouchBools.Clear();
        deadBools.Clear();
    }

    public void ResetGhostData()
    {
        ghostDatas.Clear();
    }
}

public class GhostData
{
    public GhostData(float timeStamp, Vector2 position, float speed, bool jumping, bool crouching, bool dead)
    {
        this.timeStamp = timeStamp;
        this.position = position;
        this.speed = speed;
        this.jumping = jumping;
        this.crouching = crouching;
        this.dead = dead;
    }
    public float timeStamp;
    public Vector2 position;
    public float speed;
    public bool jumping;
    public bool crouching;
    public bool dead;
}