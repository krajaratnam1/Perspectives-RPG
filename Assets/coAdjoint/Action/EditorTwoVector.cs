using System;

[Serializable]
public class EditorTwoVector
{
	[NonSerialized]
	protected bool m_IsDirty;
	public virtual bool IsDirty
	{
		get
		{
			return this.m_IsDirty;
		}
		set
		{
			this.m_IsDirty = value;
		}
	}	
	
	private float m_X;
	private float m_Y;
	public float X
	{
		get
		{
			return this.m_X;
		}
		set
		{
			if (this.m_X == value)
			{
				return;
			}
			this.m_X = value;
			this.IsDirty = true;
		}
	}
	public float Y
	{
		get
		{
			return this.m_Y;
		}
		set
		{
			if (this.m_Y == value)
			{
				return;
			}
			this.m_Y = value;
			this.IsDirty = true;
		}
	}
	public EditorTwoVector()
	{
		this.m_X = 0f;
		this.m_Y = 0f;
	}
	public EditorTwoVector(float x, float y)
	{
		this.m_X = x;
		this.m_Y = y;
	}
	public EditorTwoVector(EditorTwoVector point)
	{
		this.m_X = point.X;
		this.m_Y = point.Y;
	}
	public override bool Equals(object other)
	{
		EditorTwoVector point = other as EditorTwoVector;
		return point != null && this.m_X == point.X && this.m_Y == point.Y;
	}
	public override int GetHashCode()
	{
		return this.m_X.GetHashCode() ^ this.m_Y.GetHashCode();
	}
}

