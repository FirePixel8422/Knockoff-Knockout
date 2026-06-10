using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Dummy Recording", menuName = "ScriptableObjects/Combat/DummyRecordingSO", order = -1002)]
public class DummyRecordingSO : ScriptableObject
{
    public List<FrameInput> Timeline;
    public bool IsLeftPlayer;
}