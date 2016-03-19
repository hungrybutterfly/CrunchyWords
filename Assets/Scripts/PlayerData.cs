using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class PlayerData
{
    // update this when the contents change. You'll also need to add an upgrade process in the SessionManager
    public const int sd_CurrentVersion = 1;

    public int sd_Version = sd_CurrentVersion;
    public int sd_PuzzlesSolved = 0;
    public int sd_CorrectSubmits = 0;
    public int sd_IncorrectSubmits = 0;
    public bool[] sd_WordFound;
    public int[] sd_WordFoundCounts;
}
