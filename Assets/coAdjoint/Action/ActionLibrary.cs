using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ActionLibrary {
	
	[NonSerialized]
	protected bool _isDirty;
	public virtual bool IsDirty
	{
		get
		{
			return this._isDirty;
		}
		set
		{
			this._isDirty = value;
		}
	}	
	
	public ActionNetwork[] networks;
	
	public ActionLibrary()
	{
		this.networks = new ActionNetwork[0];
	}
	
	public byte[] GetData()
	{
		return CoAdjointProjectAsset.GetData(this);
	}
	
	public static ActionLibrary LoadData(byte[] data)
	{
		return CoAdjointProjectAsset.Load(data) as ActionLibrary;
	}
	
	public ActionNetwork Get(string name)
	{
		ActionNetwork[] networks = this.networks;
		
		for (int i = 0; i < networks.Length; i++)
		{
			ActionNetwork network = networks[i];
			if (network.name == name)
			{
				return network;
			}
		}
		return null;		
	}
	
	public ActionNetwork Add(ActionNetwork actionNetwork)
	{
		ActionNetwork[] array = new ActionNetwork[this.networks.Length+1];
		Array.Copy(this.networks, 0, array, 0, this.networks.Length);
		array[this.networks.Length] = actionNetwork;
		this.networks = array;
		this.IsDirty = true;
		return actionNetwork;
	}
	
	public void Delete(ActionNetwork actionNetworkToDelete)
	{
		int num = Array.IndexOf<ActionNetwork>(networks,actionNetworkToDelete);
		if (num == -1)
		{
			return;
		}
		
		ActionNetwork[] array = new ActionNetwork[networks.Length - 1];
		Array.Copy(networks, 0, array, 0, num);
		Array.Copy(networks, num + 1, array, num, networks.Length - num - 1);
		this.networks = array;
		
		ActionEditor.instance.currentLibraryNetwork = null;
			
		IsDirty = true;
	}
}
