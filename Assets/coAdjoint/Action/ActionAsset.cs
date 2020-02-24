using UnityEngine;
using System.Collections;

[System.Serializable]
public class ActionAsset : ScriptableObject
{
	[HideInInspector]
	public byte[] _libraryData;
	
	public byte[] libraryData
	{
		get
		{
			return _libraryData;
		}
		set
		{
			_libraryData = value;
		}
	}
	
}
