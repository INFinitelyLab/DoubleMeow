using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

// Static

public static class Extentions
{
    public static bool IsVertical( this Direction direction )
    {
        return direction == Direction.Up || direction == Direction.Down;
    }


    public static bool IsHorizontal(this Direction direction)
    {
        return !direction.IsVertical();
    }


    public static bool TrySurf( this ref RoadLine line, Direction direction )
    {
            if ((line == RoadLine.Mercury && direction == Direction.Left) || (line == RoadLine.Earth && direction == Direction.Right)) return false;

            line += direction.ToInt();

            return true;
    }


    public static void SurfTo( this ref RoadLine line, RoadLine to )
    {
        if (line == to) return;

        line.TrySurf( line.ToInt() < to.ToInt()? Direction.Right : Direction.Left );
    }


    public static bool TrySurfRandom( this ref RoadLine line )
    {
        return line.TrySurf( UnityEngine.Random.Range(0, 2) == 0 ? Direction.Left : Direction.Right );
    }


    public static int ToInt<T>(this T enumerable) where T : System.Enum
    {
        return (int)(System.IConvertible)enumerable;
    }


    public static RoadLine GetRandom( this RoadLineFlags line )
    {
        

        return line.ToRoadLine().Random();
    }


    public static List<RoadLine> ToRoadLine( this RoadLineFlags line )
    {
        List<RoadLine> lines = new List<RoadLine>();

        if (line.HasFlag(RoadLineFlags.Mercury)) lines.Add(RoadLine.Mercury);
        if (line.HasFlag(RoadLineFlags.Earth)) lines.Add(RoadLine.Earth);
        if (line.HasFlag(RoadLineFlags.Venus)) lines.Add(RoadLine.Venus);

        return lines;
    }


    public static T Random<T>(this List<T> list)
    {
        return list.Count > 0? list[ UnityEngine.Random.Range(0, list.Count) ] : default;
    }


    public static T ClosestRandom<T>(this List<T> list, T origin)
    {
        if (list.Count < 2)
            return origin;

        int returned = list.FindIndex( p => p.Equals(origin) );

        if (returned == 0)
            return list[1];
        else if (returned == list.Count)
            return list[returned - 1];
        else
            return list[ UnityEngine.Random.Range(0,1) == 0? returned - 1 : returned + 1 ];
    }


    public static int GetRandomIndexByChance( this Placeable[] chanceables )
    {
        int value = 0;

        Vector2Int[] chances = new Vector2Int[ chanceables.Length ];

        for( int index = 0; index < chanceables.Length; index++ )
        {
            chances[index] = new Vector2Int( value, chanceables[index].chance );

            value += chanceables[index].chance;
        }

        int randomValue = UnityEngine.Random.Range(0, value);

        for (int index = 0; index < chanceables.Length; index++)
        {
            if (chances[index].x < randomValue && chances[index].y > randomValue) return index;
        }

        return 0;
    }

    public static int Crop(this int value, int length)
    {
        if (value.GetLength() < length) return 0;

        return Mathf.FloorToInt(value / Mathf.Pow(10, value.GetLength() - length));
    }

    public static int GetLength(this int value)
    {
        return value.ToString().Length;
    }
}

public static class PlayerPrefsExtentions
{
    private const string FirstPathSuffix = "_first";
    private const string SecondPathSuffix = "_second";
    private const string ThirdPathSuffix = "_third";

    public static void SetSecure(string key, Secure value)
    {
        if (value.ToString() == null)
            throw new Exception("Некорректные данные для сохранения");

        SecureData data = value.Encrypted;

        PlayerPrefs.SetString(key + FirstPathSuffix, data.firstValue);
        PlayerPrefs.SetString(key + SecondPathSuffix, data.secondValue);
        PlayerPrefs.SetString(key + ThirdPathSuffix, data.thirdValue);
    }

    public static Secure GetSecure(string key, string name)
    {
        SecureData data;

        data.firstValue = PlayerPrefs.GetString(key + FirstPathSuffix);
        data.secondValue = PlayerPrefs.GetString(key + SecondPathSuffix);
        data.thirdValue = PlayerPrefs.GetString(key + ThirdPathSuffix);

        //Debug.Log( data.firstValue + " : " + data.secondValue + " : " + data.thirdValue );

        return new Secure( Secure.Decrypt(data, name), name );
    }

    public static bool HasSecure(string key)
    {
        return PlayerPrefs.HasKey(key + FirstPathSuffix) && PlayerPrefs.HasKey(key + SecondPathSuffix) && PlayerPrefs.HasKey(key + ThirdPathSuffix);
    }
}


// Interface's

public interface IChanceable
{
    public int chance { get; }
}

public interface ILowereable
{
    public abstract void Low(float height);

    public abstract void Up();
}

public interface IGroundeable
{

}



// Enum's

public enum RoadLine
{
    Mercury = -1,
    Venus = 0,
    Earth = 1
}

public enum Direction
{
    Left = -1,
    Right = 1,
    Up,
    Down
}

[Flags]
public enum BuildingType : int
{
    Flat = 1,
    Center = 10,
    Borderless = 100,
    Window = 1000,
    Rope = 10000,
    Plank = 100000,
    Vehicle = 1000000
}

[Flags]
public enum RoadLineFlags
{
    Mercury = 1,
    Venus = 10,
    Earth = 100
}
