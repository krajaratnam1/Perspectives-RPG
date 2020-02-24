using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ActionEditor : ScriptableObject
{
//	private const int _stackSize = 10;
//
//	private byte[] lastSave;

	public static ActionEditor _instance;
	public static ActionEditor instance
	{
		get
		{
			return _instance;
		}
	}
	
	private ActionAsset _currentAsset;	
	public ActionAsset currentAsset
	{
		get
		{
			return _currentAsset;	
		}
		set
		{
			if(_currentAsset == value)
				return;
			
			this._currentAsset = value;
			
			this.currentAssetLibrary = ActionLibrary.LoadData(this.currentAsset.libraryData);
			
			_currentAsset = value;
		}
	}
	
	private ActionLibrary _currentAssetLibrary;
	public ActionLibrary currentAssetLibrary
	{
		get
		{
			return _currentAssetLibrary;
		}
		set
		{
			if(_currentAssetLibrary == value)
				return;
			
			currentLibraryNetwork = null;
			
			_currentAssetLibrary = value;
		}
	}
	
	private ActionNetwork _currentLibraryNetwork;
	public ActionNetwork currentLibraryNetwork
	{
		get
		{
			return _currentLibraryNetwork;
		}
		set
		{			
			if(_currentLibraryNetwork == value)
			{
				return;
			}
			else 
			{
//				if(value != null)
//				{
//					lastSave = value.GetData();
//
//					if(_currentLibraryNetwork != null && value.name != _currentLibraryNetwork.name)
//					{
//						undoStack.Clear();
//						redoStack.Clear();
//					}
//				}

				ActionNetworkEditor.instance.Repaint();
				_currentLibraryNetwork = value;
			}			
		}
	}

//	private LinkedList<byte[]> undoStack;
//	private LinkedList<byte[]> redoStack;

	public void Init()
	{
		hideFlags = HideFlags.DontSave;
		
		if (instance != null)
		{
			Debug.LogError ("Instance already exists");
			DestroyImmediate (this);
			return;
		}

//		undoStack = new LinkedList<byte[]>();
//		redoStack = new LinkedList<byte[]>();

		_instance = this;		
	}
	
	public void OnDestroy()
	{
		ActionEditor._instance = null;
	}
	
	public void CreateNetwork(string name)
	{
		int i = 0;
		
		string copy = name.Clone ().ToString();
		
		if(currentAssetLibrary == null)
			return;
		
		if(this.currentAssetLibrary.Get(name) != null)
		{				
			while(this.currentAssetLibrary.Get(name + i) != null)
			{
				++i;
			}
			
			copy += i;
		}
		
		ActionNetwork newNetwork = this.currentAssetLibrary.Add(new ActionNetwork(copy));

		if(ActionNetworkEditor.instance != null)
		{
			ActionNetworkEditor.instance.selectedConnection = null;
			ActionNetworkEditor.instance.selectedSkills.Clear();
		}

		currentLibraryNetwork = newNetwork; 
		
		this.SaveLibrary();
	}
	
	public void Repaint ()
	{
		EditorUtility.SetDirty (this);
		
		UnityEditor.Editor[] inspectors = FindObjectsOfType (typeof (UnityEditor.Editor)) as UnityEditor.Editor[];

		foreach (UnityEditor.Editor inspector in inspectors)
		{
			inspector.Repaint ();
		}
	}	

//	public void Undo()
//	{
//		if(undoStack.Count > 0)
//		{
//			Push<byte[]>(redoStack, currentLibraryNetwork.GetData());
//
//			byte[] undoArray = Pop(undoStack);
//
//			if(undoArray != null)
//			{
//				ActionNetwork prev = ActionNetwork.Load(undoArray);
//				
//				if(prev != null)
//				{
//					int ind = Array.IndexOf<ActionNetwork>(currentAssetLibrary.networks, currentLibraryNetwork);
//
//					int hs = currentAssetLibrary.networks[ind].skills.IndexOf(ActionLibraryExplorer.instance.highlightedSkill);
//
//					currentAssetLibrary.networks[ind] = prev;
//					currentLibraryNetwork = prev;
//
//					if(hs != -1)
//						ActionLibraryExplorer.instance.highlightedSkill = currentAssetLibrary.networks[ind].skills[hs];
//
//					SaveLibrary(true);
//					Repaint();
//				}
//			}
//		}
//	}
//
//	public void Redo()
//	{
//		if(redoStack.Count > 0)
//		{
//			Push<byte[]>(undoStack, currentLibraryNetwork.GetData());
//
//			byte[] redoArray = Pop(redoStack);
//
//			if(redoArray != null)
//			{
//				ActionNetwork prev = ActionNetwork.Load(redoArray);
//				
//				if(prev != null)
//				{
//					int ind = Array.IndexOf<ActionNetwork>(currentAssetLibrary.networks, currentLibraryNetwork);
//
//					int hs = currentAssetLibrary.networks[ind].skills.IndexOf(ActionLibraryExplorer.instance.highlightedSkill);
//					
//					currentAssetLibrary.networks[ind] = prev;
//					currentLibraryNetwork = prev;
//					
//					if(hs != -1)
//						ActionLibraryExplorer.instance.highlightedSkill = currentAssetLibrary.networks[ind].skills[hs];
//					
//					SaveLibrary(true);
//
//					Repaint();
//				}
//			}
//		}
//	}
//
//	private T Peek<T>(LinkedList<T> stack)
//	{
//		return stack.Last.Value;
//	}
//	
//	private T Pop<T>(LinkedList<T> stack)
//	{
//		LinkedListNode<T> node = stack.Last;
//		
//		if(node != null)
//		{
//			stack.RemoveLast();
//			return node.Value;
//		}
//		else
//		{
//			return default(T);
//		}
//	}
//	
//	private void Push<T>(LinkedList<T> stack, T value)
//	{
//		LinkedListNode<T> node = new LinkedListNode<T>(value);
//
//		stack.AddLast(node);
//
//		if (stack.Count > _stackSize)
//		{
//			stack.RemoveFirst();
//		}
//	}

	public void SaveLibrary()
	{
		if(currentAssetLibrary != null)
		{
			byte[] newData = currentAssetLibrary.GetData();
			
			if (!currentAsset.libraryData.Equals(newData))
			{
				currentAsset.libraryData = newData;
				EditorUtility.SetDirty ((ActionAsset) currentAsset);
			}
			
			//Debug.Log ("Saving");
			
			this.currentAssetLibrary.IsDirty = false;
			ActionLibraryExplorer.instance.Repaint();
			ActionNetworkEditor.instance.Repaint();
		}
	}

//	public void SaveLibrary(bool isUndoing = false)
//	{
//		if(currentAssetLibrary != null)
//		{
//			byte[] newData = currentAssetLibrary.GetData();
//
//			if (!currentAsset.libraryData.Equals(newData))
//			{
//				if(isUndoing == false)
//				{
//					Push<byte[]>(undoStack, (byte[])(lastSave.Clone()));
//				}
//
//				if(currentLibraryNetwork != null)
//				{
//					byte[] newNetworkData = currentLibraryNetwork.GetData();
//					
//					lastSave = new byte[newNetworkData.Length];
//					
//					Array.Copy(newNetworkData,lastSave,newNetworkData.Length);
//				}
//
//				currentAsset.libraryData = newData;
//
//				EditorUtility.SetDirty ((ActionAsset) currentAsset);
//			}
//			
//			this.currentAssetLibrary.IsDirty = false;
//			ActionLibraryExplorer.instance.Repaint();
//			ActionNetworkEditor.instance.Repaint();
//		}
//	}
	
}
