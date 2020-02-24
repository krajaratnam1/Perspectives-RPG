using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class RectExtensions
{	
    public static Vector2 TopLeft(this Rect rect)
    {
        return new Vector2(rect.xMin, rect.yMin);
    }
	
	public static Vector2 TopRight(this Rect rect)
	{
		return new Vector2(rect.xMax,rect.yMin);
	}
	
	public static Vector2 BottomLeft(this Rect rect)
	{
		return new Vector2(rect.xMin,rect.yMax);
	}
	
	public static Vector2 BottomRight(this Rect rect)
	{
		return new Vector2(rect.xMax,rect.yMax);
	}
	
	public static Vector2 TopMiddle(this Rect rect)
	{
		return (rect.TopLeft() + rect.TopRight())/2f;
	}
	
	public static Vector2 LeftMiddle(this Rect rect)
	{
		return (rect.TopLeft() + rect.BottomLeft())/2f;
	}
	
	public static Vector2 RightMiddle(this Rect rect)
	{
		return (rect.TopRight() + rect.BottomRight())/2f;
	}
	
	public static Vector2 BottomMiddle(this Rect rect)
	{
		return (rect.BottomLeft() + rect.BottomRight())/2f;
	}
	
	public static Vector2 Middle(this Rect rect)
	{
		return (rect.TopMiddle() + rect.BottomMiddle())/2f;
	}
	
    public static Rect ScaleSizeBy(this Rect rect, float scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }
    public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale;
        result.xMax *= scale;
        result.yMin *= scale;
        result.yMax *= scale;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }
    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }
    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale.x;
        result.xMax *= scale.x;
        result.yMin *= scale.y;
        result.yMax *= scale.y;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }
	
	public static bool OverLaps( this Rect rect, Rect other )
	{
		var points = new List<Vector2> {new Vector2( other.xMin, other.yMin ),
										new Vector2( other.xMax, other.yMin ),
										new Vector2( other.xMin, other.yMax ),
										new Vector2( other.xMax, other.yMax ) };
		
		if( points.Any( x => rect.Contains( x ) ) )
		{
			return true;
		}
		return false;
	}	
}


