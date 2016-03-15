using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class PlayerData
{
    public int sd_PuzzlesSolved = 0;
    public int sd_CorrectSubmits = 0;
    public int sd_IncorrectSubmits = 0;
}
