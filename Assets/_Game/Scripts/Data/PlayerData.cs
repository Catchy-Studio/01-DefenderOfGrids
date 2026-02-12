using System;

[Serializable] // <--- Vital! Allows Unity to turn this into JSON (text)
public class PlayerData
{
    public int TotalXP;
    public int HighestLevelBeat;

    // We will add "List<int> UnlockedSkills" here later!

    // Constructor to set defaults
    public PlayerData()
    {
        TotalXP = 0;
        HighestLevelBeat = 0;
    }
}