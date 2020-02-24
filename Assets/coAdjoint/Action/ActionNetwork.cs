using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class ActionNetwork
{
	public string name = "ActionNetwork";

	public List<ActionSkill> skills = new List<ActionSkill>();
		
	public bool explorerExpanded = false;

	public ActionNetwork(string name)
	{
		this.name = name;
	}
	
	public List<ActionConnection> connections = new List<ActionConnection>();
	
	public ActionNetwork(string name, ActionNetwork original)
	{
		this.name = name;
		
		skills = new List<ActionSkill>();
		
		foreach(ActionSkill val in original.skills)
		{
			skills.Add(new ActionSkill(val.Name, this));
			skills.Last().Q = val.Q;
			skills.Last().B = val.B;
			skills.Last().v = val.v;
			
			skills.Last().Position = val.Position;
		}
		
		foreach(ActionConnection actC in original.connections)
		{
			AddConnection(Get (actC.To.Name),Get(actC.From.Name),actC.toValue,actC.fromValue);
		}
	}
	
	public ActionSkill Get(string name)
	{
		foreach(ActionSkill actS in skills)
		{
			if(actS.Name.Equals(name))
				return actS;			
		}
		
		return null;
	}
	
	public void AddSkill(string Name)
	{
		if(Get(Name) == null)
		{
			skills.Add(new ActionSkill(Name, this));
		}
		else 
		{
			int index = 0;
			
			while(Get(Name + index) != null)
			{
				++index;
			}
			
			skills.Add(new ActionSkill(Name+index,this));			
		}
	}
	
	public ActionSkill AddSkill(string Name, double Q, double B, double v, EditorTwoVector position)
	{
		int index = 0;
		
		if(Get(Name) == null)
		{
			skills.Add(new ActionSkill(Name, Q, B, v,position));
			
			return skills.Last();
		}
		else 
		{			
			while(Get(Name + index) != null)
			{
				++index;
			}
			
			skills.Add(new ActionSkill(Name+index,Q,B,v));
			
			return skills.Last();
		}		
	}
	
	public ActionConnection AddConnection(ActionSkill ToSkill, ActionSkill FromSkill, double toValue, double fromValue)
	{
		ActionConnection newConnection = new ActionConnection(ToSkill,FromSkill, toValue, fromValue, this);

		bool alreadyExists = false;
		
		foreach(ActionConnection ac in connections)
		{
			if(ac.IsThereConnectionBetween(ToSkill,FromSkill))
			{
				alreadyExists = true;

				break;
			}			
		}		
		
		if(alreadyExists)
		{
			Debug.Log ("Editting existing connection");
		}
		else connections.Add (newConnection);
		
		return newConnection;
	}
	
	public void DeleteSkill(string name)
	{
		if(Get(name) != null)
		{			
			DeleteConnection(Get(name));
			skills.Remove(Get(name));
		}
	}
	
	public void DeleteSkill(ActionSkill skillToDelete)
	{
		if(skills.Contains(skillToDelete))
		{			
			DeleteConnection(skillToDelete);
			skills.Remove(skillToDelete);
		}
	}	
	
	public void DeleteConnection(ActionSkill deletedSkill)
	{		
		List<ActionConnection> connectionsToDelete = new List<ActionConnection>();
		
		foreach(ActionConnection ac in connections)
		{
			if(ac.Contains(deletedSkill))
				connectionsToDelete.Add (ac);
			
			if(ActionNetworkEditor.instance.selectedConnection.HasValue && connections.IndexOf(ac) == ActionNetworkEditor.instance.selectedConnection.Value)
			{
				ActionNetworkEditor.instance.selectedConnection = null;
			}
		}	
		
		connections.RemoveAll(n => connectionsToDelete.Contains(n));
	}

	public byte[] GetData()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryFormatter binaryFormatter = new BinaryFormatter();		

		binaryFormatter.Serialize(memoryStream, this);
		memoryStream.Close();

		return memoryStream.ToArray();
	}

	public static ActionNetwork Load(byte[] data)
	{
		object obj = new BinaryFormatter().Deserialize(new MemoryStream(data));
		
		if(obj is ActionNetwork)
		{
			return obj as ActionNetwork;
		}
		else throw new ApplicationException("Unable to deserialise type " + obj.GetType());
	}
}
