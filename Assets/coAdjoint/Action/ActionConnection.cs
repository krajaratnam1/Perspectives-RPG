using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ActionConnection
{
	public ActionSkill To;
	public ActionSkill From;
	
	public double toValue;
	public double fromValue;
	
	public ActionNetwork owner
	{
		get 
		{
			return ActionEditor.instance.currentLibraryNetwork;
		}
	}

	
	private bool _IsDirty;
	public bool IsDirty
	{
		get
		{
			return _IsDirty;
		}
		set
		{
			_IsDirty = value;
		}
	}	
	
	[Obsolete("For XML Serialization Only", true)]
	public ActionConnection(){}
	
	public ActionConnection(ActionSkill toSkill, ActionSkill fromSkill, double toV, double fromV, ActionNetwork owner)
	{
		this.To = toSkill;
		this.From = fromSkill;
		
		this.toValue = toV;
		this.fromValue = fromV;
	}
	
	public bool Contains(ActionSkill endpoint)
	{
		return (endpoint == To || endpoint == From);
	}
	
	public bool Contains(Vector2 position)
	{
		Vector2 dir = (From.drawPos.Middle() - To.drawPos.Middle())/(From.drawPos.Middle() - To.drawPos.Middle()).magnitude;
		
		float furthestX = From.drawPos.x > To.drawPos.x ? From.drawPos.Middle().x : To.drawPos.Middle().x; 
		float nearestX = From.drawPos.x > To.drawPos.x ? To.drawPos.Middle().x : From.drawPos.Middle().x;
		
		if(position.x < furthestX && position.x > nearestX && ((To.drawPos.Middle() - position) - Vector2.Dot(To.drawPos.Middle() - position,dir)*dir).magnitude < 5f)
			return true;
		else return false;
	}
	
	public bool IsThereConnectionBetween(ActionSkill skill1, ActionSkill skill2)
	{
		return ((skill1 == To && skill2 == From) || (skill1 == From && skill2 == To));
	}
	
	public ActionSkill OtherEnd(ActionSkill oneEnd)
	{
		if(oneEnd == To) return From;
		else return To;
	}
	
	public bool isTo(ActionSkill skill)
	{
		return skill == To;
	}
}
