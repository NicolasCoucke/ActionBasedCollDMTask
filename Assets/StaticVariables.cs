using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Collective DM 
public static class StaticVariables
{
    private static int numPlayers = 2;

    private static int levelNumber = 1;

    private static int pucklevelNumber = 65;

    private static int tutLevelNumber = 1;

    private static string path;

    private static string condition;

    private static bool definedpath = false;

    private static int conditionNumber = 1;

    public static int NumPlayers
    {
        get
        {
            return numPlayers;
        }
        set
        {
            numPlayers = value;
        }
    }

    public static int TutLevelNumber
    {
        get
        {
            return tutLevelNumber;
        }
        set
        {
            tutLevelNumber = value;
        }
    }

    public static int LevelNumber
    {
        get
        {
            return levelNumber;
        }
        set
        {
            levelNumber = value;
        }
    }

    public static int PuckLevelNumber
    {
        get
        {
            return pucklevelNumber;
        }
        set
        {
            pucklevelNumber = value;
        }
    }

    public static bool Definedpath
    {
        get
        {
            return definedpath;
        }
        set
        {
            definedpath = value;
        }
    }

    public static string Path
    {
        get
        {
            return path;
        }
        set
        {
            path = value;
        }
    }

    public static string Condition
    {
        get
        {
            return condition;
        }
        set
        {
            condition = value;
        }
    }

    public static int ConditionNumber
    {
        get
        {
            return conditionNumber;
        }
        set
        {
            conditionNumber = value;
        }
    }
}
