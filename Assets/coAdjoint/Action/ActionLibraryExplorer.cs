using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

//Allows user to browse Library networks
public class ActionLibraryExplorer : EditorWindow {
	
	public enum ItemType
	{
		ActionNetwork,
		ActionSkill
	}
	
	private ItemType _highlightedItemType;
	public ItemType highlightedItemType
	{
		get
		{
			return _highlightedItemType;
		}
		set
		{
			if(_highlightedItemType != value)
			{
				this.Repaint();
			}
			_highlightedItemType = value;
		}
	}	
	
	private ActionSkill _highlightedSkill;
	public ActionSkill highlightedSkill
	{
		get
		{
			return _highlightedSkill;
		}
		set
		{			
			_highlightedSkill = value;
		}
	}

	private Vector2 explorerScroll;

	private static ActionLibraryExplorer _instance;
	
	public static ActionLibraryExplorer instance
	{
		get
		{	
			if (_instance == null)
			{				
				GetWindow (typeof (ActionLibraryExplorer),false);
			}
			
			return _instance;
		}
	}	
	
	private string editingString = string.Empty;
	
	private bool _editingNames = false;
	public bool editingNames
	{
		get
		{
			return _editingNames;
		}
		set
		{
			if (!_editingNames && value)
			{
				if (highlightedItemType == ItemType.ActionNetwork && ActionEditor.instance.currentLibraryNetwork != null)
				{
					editingString = ActionEditor.instance.currentLibraryNetwork.name;
				}
				else
				{
					if (highlightedItemType == ItemType.ActionSkill && highlightedSkill != null)
					{
						editingString = highlightedSkill.Name;
					}
				}
			}
			else
			{
				if (_editingNames && !value && editingString != null && editingString != string.Empty)
				{
					if (highlightedItemType == ItemType.ActionNetwork && ActionEditor.instance.currentLibraryNetwork != null && ActionEditor.instance.currentAssetLibrary.Get(editingString) == null)
					{
						ActionEditor.instance.currentLibraryNetwork.name = editingString;
					}
					else
					{
						if (highlightedItemType == ItemType.ActionSkill && highlightedSkill != null && ActionEditor.instance.currentLibraryNetwork.Get(editingString) == null)
						{
							highlightedSkill.Name = editingString;
						}
					}
					
					ActionEditor.instance.SaveLibrary();
				}
			}
			bool flag = _editingNames != value;
			_editingNames = value;
			
			if (flag)
			{
				this.Repaint();
			}
		}
	}
	
	public ActionLibraryExplorer()
	{
		if (_instance != null)
		{
			Debug.LogError ("Instance already exists");
			DestroyImmediate (this);
			return;
		}
		
		_instance = this;

		titleContent = new GUIContent("Library Explorer");
	}

	public void OnDestroy()
	{
		_instance = null;
	}
	
	new public void Show()
	{
		base.Show();
	}
	
    new public void Repaint ()
	{
		base.Repaint ();		
	}	
	
	new public void Focus()
	{
		base.Focus();
	}
	
	public bool HasFocus
	{
		get
		{
			return EditorWindow.focusedWindow == this;
		}
	}	
		
	public void OnGUI ()
	{
		GUILayout.BeginHorizontal("Toolbar", new GUILayoutOption[0]);
		{
			if (GUILayout.Button("Add", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				if(ActionEditor.instance != null)
				{
					ActionNetworkCreation.GetWindow(typeof(ActionNetworkCreation),true);
				}
			}
			
			if (GUILayout.Button("Duplicate", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				if(ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null)
				{
					if(highlightedItemType == ItemType.ActionNetwork)
					{
						int ind = 1;
						
						string copy = ActionEditor.instance.currentLibraryNetwork.name.Clone().ToString();
										
						while(ActionEditor.instance.currentAssetLibrary.Get(copy+ind) != null)
						{
							++ind;
						}
						
						copy += ind;
						
						ActionEditor.instance.currentLibraryNetwork = ActionEditor.instance.currentAssetLibrary.Add(new ActionNetwork(copy, ActionEditor.instance.currentLibraryNetwork));
						
						ActionEditor.instance.SaveLibrary();
					}
					else if(highlightedItemType == ItemType.ActionSkill)
					{
						int ind = 1;
						
						string copy = highlightedSkill.Name.Clone().ToString();
						
						while(ActionEditor.instance.currentLibraryNetwork.Get(copy+ind) != null)
						{
							++ind;
						}

						ActionSkill oldSkill = ActionEditor.instance.currentLibraryNetwork.Get(highlightedSkill.Name);

						copy += ind;
						
						highlightedSkill = ActionEditor.instance.currentLibraryNetwork.AddSkill(copy,highlightedSkill.Q,highlightedSkill.B,highlightedSkill.v, new EditorTwoVector(highlightedSkill.drawPos.TopLeft().x
							+ 10f - ActionNetworkEditor.instance.offset.X,highlightedSkill.drawPos.TopLeft().y + 10f - ActionNetworkEditor.instance.offset.Y));
											
						List<ActionConnection> newConnections = new List<ActionConnection>();

						foreach(ActionConnection actC in ActionEditor.instance.currentLibraryNetwork.connections)
						{
							if(actC.Contains(oldSkill))
							{
								if(actC.isTo(oldSkill))
								{
									newConnections.Add(new ActionConnection(highlightedSkill,actC.From,actC.toValue,actC.fromValue,ActionEditor.instance.currentLibraryNetwork));
								}
								else
								{
									newConnections.Add(new ActionConnection(actC.To,highlightedSkill,actC.toValue,actC.fromValue,ActionEditor.instance.currentLibraryNetwork));
								}
							}
						}

						foreach(ActionConnection actC in newConnections)
						{
							ActionEditor.instance.currentLibraryNetwork.connections.Add(actC);
						}

						ActionNetworkEditor.instance.selectedSkills.Clear();
						
						ActionNetworkEditor.instance.selectedSkills.Add(ActionEditor.instance.currentLibraryNetwork.skills.IndexOf(highlightedSkill));
						
						ActionEditor.instance.SaveLibrary();
						
						ActionNetworkEditor.instance.Repaint();				
					}
				}
			}	
			
			if (ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null)
			{
				if (GUILayout.Button("Rename", EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					editingNames = !editingNames;
				}
			}
			else
			{
				if (GUILayout.Button("Rename", EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
				}			
			}		
			
			if (GUILayout.Button("Delete", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				DeleteItems();			
			}
			
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();		
				
		if(ActionEditor.instance == null || ActionEditor.instance.currentAssetLibrary == null)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("No library loaded.\n\nTo load an Action Library, select a library in the project folder then press edit in the Inspector panel", new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();	
		}
		else
		{			
			this.explorerScroll = GUILayout.BeginScrollView(this.explorerScroll, new GUILayoutOption[0]);
			
			GUILayout.Label(new GUIContent(ActionEditor.instance.currentAsset.name), new GUILayoutOption[0]);
			
			ActionEditor.instance.currentLibraryNetwork = (ActionNetwork)ActionResources.SelectList(ActionEditor.instance.currentAssetLibrary.networks,ActionEditor.instance.currentLibraryNetwork,new ActionResources.OnListItemGUI(OnNetworkListItemGUI));

			GUILayout.EndScrollView();
			
			if(highlightedItemType == ItemType.ActionSkill)
			{
				ActionEditor.instance.currentLibraryNetwork = highlightedSkill.owner;
			}
		}	
		
		HandleShortcutEvents();
	}
	
	public bool OnNetworkListItemGUI(object item, bool selected, ICollection list)
	{
		ActionNetwork myItem = (ActionNetwork)item;
		
		GUIStyle style = (!selected || highlightedItemType != ItemType.ActionNetwork) ? ActionResources.listItem : ((!this.HasFocus) ? ActionResources.unfocusedSelectedListItem : ActionResources.selectedListItem);
		GUIStyle style2 = (!selected || highlightedItemType != ItemType.ActionNetwork) ? ActionResources.list : ((!this.HasFocus) ? ActionResources.unfocusedSelectedList : ActionResources.selectedList);		

		GUILayout.BeginHorizontal(style,new GUILayoutOption[0]);
		
		GUILayout.Space(20f);
		
		if(GUILayout.Button((myItem.explorerExpanded)?ActionResources.expanded:ActionResources.collapsed,style2,new GUILayoutOption[]
		{
			GUILayout.Width(14f),
			GUILayout.Height(14f)
		}))
		{
			myItem.explorerExpanded = !myItem.explorerExpanded;
		}

		if(editingNames && _highlightedItemType == ItemType.ActionNetwork && selected)
		{
			editingString = GUILayout.TextField(editingString,EditorStyles.textField, new GUILayoutOption[]
			{
				GUILayout.Height(14f)
			});
		}
		else if(GUILayout.Button(myItem.name,style2,new GUILayoutOption[]
		{
			GUILayout.Height(14f)
		}))
		{
			editingNames = false;

			if(highlightedItemType == ItemType.ActionNetwork)
				ActionEditor.instance.currentLibraryNetwork.name = ActionEditor.instance.currentLibraryNetwork.name.Replace(" ", string.Empty);
			else
				highlightedSkill.Name = highlightedSkill.Name.Replace(" ", string.Empty);

			selected = true;
			this.highlightedItemType = ItemType.ActionNetwork;	
						
			ActionNetworkEditor.instance.selectedSkills.Clear();
			
			//ActionNetworkEditor.instance.CentreNetwork(ActionEditor.instance.currentLibraryNetwork);
			
		}
		GUILayout.EndHorizontal();
		
		if(myItem.explorerExpanded)
		{
			if(myItem.skills.Count > 0)
			{
				ActionSkill[] temp = myItem.skills.ToArray();
				List<object> list2 = new List<object>(temp);
				list2.Sort((x, y) => string.Compare(x.ToString(),y.ToString()));
				
				highlightedSkill = (ActionSkill)ActionResources.SelectList(list2,highlightedSkill,new ActionResources.OnListItemGUI(OnSubCollectionListItemGUI));
			}
			else
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(40f);
					GUILayout.Label(new GUIContent("Empty Network"),new GUILayoutOption[0]);
				}GUILayout.EndHorizontal();
			}
		}
		
		return selected;		
	}
	
	public bool OnSubCollectionListItemGUI(object item, bool selected, ICollection list)
	{
		ActionSkill mySubItem = (ActionSkill)item;
		
		GUIStyle style = (!selected || highlightedItemType != ItemType.ActionSkill) ? ActionResources.listItem : ((!this.HasFocus) ? ActionResources.unfocusedSelectedListItem : ActionResources.selectedListItem);
		GUIStyle style2 = (!selected || highlightedItemType != ItemType.ActionSkill) ? ActionResources.list : ((!this.HasFocus) ? ActionResources.unfocusedSelectedList : ActionResources.selectedList);		
		
		GUILayout.BeginHorizontal(style, new GUILayoutOption[0]);
		{
			GUILayout.Space(40f);
			
			if(editingNames && _highlightedItemType == ItemType.ActionSkill && selected)
			{
				editingString = GUILayout.TextField(editingString,EditorStyles.textField, new GUILayoutOption[]
				{
					GUILayout.Height(14f)
				});
			}			
			else if(GUILayout.Button(mySubItem.Name,style2,new GUILayoutOption[]
			{
				GUILayout.Height(14f)
			}))
			{
				editingNames = false;
				selected = true;

				if(highlightedItemType == ItemType.ActionNetwork)
					ActionEditor.instance.currentLibraryNetwork.name = ActionEditor.instance.currentLibraryNetwork.name.Replace(" ", string.Empty);
				else
					highlightedSkill.Name = highlightedSkill.Name.Replace(" ", string.Empty);

				highlightedSkill = mySubItem;
								
				ActionEditor.instance.currentLibraryNetwork = highlightedSkill.owner;	
				ActionNetworkEditor.instance.selectedSkills.Clear();
				
				highlightedItemType = ItemType.ActionSkill;				
				ActionNetworkEditor.instance.selectedSkills.Add(ActionEditor.instance.currentLibraryNetwork.skills.IndexOf(mySubItem));
				ActionNetworkEditor.instance.CentreSkill();
				
				//Debug.Log ("Point is " + new Vector2(highlightedSkill.Position.X + ActionNetworkEditor.instance.offset.X,highlightedSkill.Position.Y+ ActionNetworkEditor.instance.offset.Y));
			}
			
		} GUILayout.EndHorizontal();
		
		return selected;
	}
	
	private void DeleteItems()
	{
		if(highlightedItemType == ItemType.ActionNetwork && ActionEditor.instance.currentLibraryNetwork != null)
		{
			if (EditorUtility.DisplayDialog("Delete Network?", "Are you sure you want to delete '" + ActionEditor.instance.currentLibraryNetwork.name + "'?", "Yes", "Cancel"))
			{
				int curInd = Array.IndexOf<ActionNetwork>(ActionEditor.instance.currentAssetLibrary.networks,ActionEditor.instance.currentLibraryNetwork);
								
				highlightedItemType = ItemType.ActionNetwork;
				
				ActionNetworkEditor.instance.selectedSkills.Clear();
				ActionNetworkEditor.instance.selectedConnection = null;

				ActionEditor.instance.currentAssetLibrary.Delete(ActionEditor.instance.currentLibraryNetwork);
				
				if (curInd != 0)
					ActionEditor.instance.currentLibraryNetwork = ActionEditor.instance.currentAssetLibrary.networks[curInd - 1];
				else if (ActionEditor.instance.currentAssetLibrary.networks.Length > 0)
				{
					ActionEditor.instance.currentLibraryNetwork = ActionEditor.instance.currentAssetLibrary.networks[0];
				}
				else ActionEditor.instance.currentLibraryNetwork = null;
				
				ActionEditor.instance.SaveLibrary();
			}				
		}
		
		if(highlightedItemType == ItemType.ActionSkill && highlightedSkill != null)
		{
			if (EditorUtility.DisplayDialog("Delete Skill?", "Are you sure you want to delete '" + highlightedSkill.Name + "'?", "Yes", "Cancel"))
			{
				highlightedItemType = ItemType.ActionNetwork;

				if(ActionNetworkEditor.instance.selectedConnection.HasValue && ActionEditor.instance.currentLibraryNetwork.connections[ActionNetworkEditor.instance.selectedConnection.Value].Contains(highlightedSkill))
					ActionNetworkEditor.instance.selectedConnection = null;
								
				int skillInd = Array.IndexOf<ActionSkill>(ActionEditor.instance.currentLibraryNetwork.skills.ToArray(),highlightedSkill);

				ActionNetworkEditor.instance.selectedSkills.Clear();

				if(ActionNetworkEditor.instance.selectedSkills.Contains(skillInd))
					ActionNetworkEditor.instance.selectedSkills.Remove(skillInd);
				
				highlightedSkill.owner.DeleteSkill(highlightedSkill.Name);
				
				highlightedSkill = null;					
				
				ActionEditor.instance.SaveLibrary();
			}
		}	
	}
	
	private void HandleShortcutEvents()
	{
		switch(Event.current.keyCode)
		{
		case KeyCode.Escape:
			
			if(editingNames)
				editingNames = false;
			
			break;
		case KeyCode.Return:
			
			if(editingNames)
			{
				editingNames = false;

				if(highlightedItemType == ItemType.ActionSkill)
					highlightedSkill.Name = highlightedSkill.Name.Replace(" ", string.Empty);
				else
					ActionEditor.instance.currentLibraryNetwork.name = ActionEditor.instance.currentLibraryNetwork.name.Replace(" ", string.Empty);
			}
			break;
		case KeyCode.Delete:
			break;
		}		
	}	
}

