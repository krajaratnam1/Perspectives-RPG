using UnityEngine;
using UnityEditor;
using System.Collections;

public class ActionResources
{
public delegate bool OnListItemGUI(object item, bool selected, ICollection list);
	private static Texture2D _expanded;
	private static Texture2D _collapsed;
	private static Texture2D _selected;
	private static Texture2D _unfocusedSelected;
	private static Texture2D _editorCreateNewSkill;
	private static Texture2D _editorCreateNewConnection;
	private static Texture2D _editorEdit;
	private static Texture2D _editorDel;
	private static Texture2D _connectionArrow;
	private static Texture2D _line;
	private static Texture2D _editorShowArrows;
	private static Texture2D _editorShowNumbers;
	private static Texture2D _white;
	
	private static GUIStyle _list;
	private static GUIStyle _selectedList;
	private static GUIStyle _unfocusedSelectedList;
	private static GUIStyle _listItem;
	private static GUIStyle _selectedListItem;
	private static GUIStyle _unfocusedSelectedListItem;
	private static GUIStyle _editorTopToolBar;
	private static GUIStyle _editorButton;	
	private static GUIStyle _bottomTooltipBox;
	
	private static GUIStyle _editorButtonCreate;
	
	public static Texture2D expanded
	{
		get
		{
			if(_expanded == null)
			{
				_expanded = Resources.Load("Expanded",typeof(Texture2D)) as Texture2D;
			}	
			
			return _expanded;
		}
	}	
	public static Texture2D collapsed
	{
		get
		{
			if(_collapsed == null)
			{
				_collapsed = Resources.Load ("Collapsed",typeof(Texture2D)) as Texture2D;
			}
			
			return _collapsed;
		}
	}
	public static Texture2D selected
	{
		get
		{
			if(_selected == null)
			{
				_selected = Resources.Load("SelectedListItem",typeof(Texture2D)) as Texture2D;
			}
			
			return _selected;
		}
	}
	public static Texture2D unfocusedSelected
	{
		get
		{
			if(_unfocusedSelected == null)
			{
				_unfocusedSelected = Resources.Load("UnfocusedSelectedListItem",typeof(Texture2D)) as Texture2D;
			}
			
			return _unfocusedSelected;
		}		
	}
	
	public static Texture2D editorCreateNewSkill
	{
		get
		{
			if(_editorCreateNewSkill == null)
			{
				_editorCreateNewSkill = Resources.Load("Network_Create_icon", typeof(Texture2D)) as Texture2D;
			}
			
			return _editorCreateNewSkill;
		}
	}
	
	public static Texture2D editorCreateNewConnection
	{
		get
		{
			if(_editorCreateNewConnection == null)
			{
				_editorCreateNewConnection = Resources.Load("Network_Connection_icon", typeof(Texture2D)) as Texture2D;
			}
			
			return _editorCreateNewConnection;
		}
	}
	
	public static Texture2D editorEdit
	{
		get
		{
			if(_editorEdit == null)
			{
				_editorEdit = Resources.Load("Edit_icon", typeof(Texture2D)) as Texture2D;
			}
			
			return _editorEdit;
		}
		
	}
	
	public static Texture2D editorDel
	{
		get
		{
			if(_editorDel == null)
			{
				_editorDel = Resources.Load("del_icon", typeof(Texture2D)) as Texture2D;
			}
			
			return _editorDel;
		}
		
	}	
	
	public static Texture2D connectionArrow
	{
		get
		{
			if(_connectionArrow == null)
			{
				_connectionArrow = Resources.Load("coAdjoint_Arrow_Collapse",typeof(Texture2D)) as Texture2D;
			}
			
			return _connectionArrow;
		}
	}	
	
	public static Texture2D editorShowNumbers
	{
		get
		{
			if(_editorShowNumbers == null)
			{
				_editorShowNumbers = Resources.Load ("Show_numbers_icon",typeof(Texture2D)) as Texture2D;
			}
			
			return _editorShowNumbers;
		}
	}
	
	public static Texture2D line
	{
		get
		{
			if(_line == null)
			{
				_line = Resources.Load ("Line",typeof(Texture2D)) as Texture2D;
			}
			
			return _line;
		}
	}
	
	public static Texture2D editorShowArrows
	{
		get
		{
			if(_editorShowArrows == null)
			{
				_editorShowArrows = Resources.Load ("Show_arrow_icon", typeof(Texture2D)) as Texture2D;
			}
			
			return _editorShowArrows;
		}
	}
	
	public static Texture2D white
	{
		get
		{
			if(_white == null)
			{
				_white = Resources.Load("1x2AA",typeof(Texture2D)) as Texture2D;
			}
			
			return _white;
		}
	}
	
	public static GUIStyle list
	{
		get
		{
			if (_list == null)
			{
				_list = new GUIStyle();
				_list.normal.textColor = GUI.skin.GetStyle("Label").normal.textColor;
			}
			return _list;
		}		
	}
	
	public static GUIStyle selectedList
	{
		get
		{
			if(_selectedList == null)
			{
				_selectedList = new GUIStyle();
				_selectedList.normal.textColor = Color.white;
				_selectedList.normal.background = selected;
			}
			return _selectedList;
		}
	}
	public static GUIStyle unfocusedSelectedList
	{
		get
		{
			if(_unfocusedSelectedList == null)
			{
				_unfocusedSelectedList = new GUIStyle();
				_unfocusedSelectedList.normal.textColor = Color.white;
				_unfocusedSelectedList.normal.background = unfocusedSelected;
			}
			return _unfocusedSelectedList;
		}
	}
	public static GUIStyle listItem
	{
		get
		{
			if (_listItem == null)
			{
				_listItem = new GUIStyle();
				_listItem.normal.textColor = GUI.skin.GetStyle("Label").normal.textColor;
				_listItem.padding.left = 2;
				_listItem.padding.right = 2;
				_listItem.padding.top = 2;
				_listItem.padding.bottom = 2;
			}
			return _listItem;
		}		
	}
	public static GUIStyle selectedListItem
	{
		get
		{
			if(_selectedListItem == null)
			{
				_selectedListItem = new GUIStyle();
				_selectedListItem.normal.background = selected;
				_selectedListItem.normal.textColor = Color.white;
				_selectedListItem.padding.left = 2;
				_selectedListItem.padding.right = 2;
				_selectedListItem.padding.top = 2;
				_selectedListItem.padding.bottom = 2;
			}
			
			return _selectedListItem;
		}
	}
	public static GUIStyle unfocusedSelectedListItem
	{
		get
		{
			if(_unfocusedSelectedListItem == null)
			{
				_unfocusedSelectedListItem = new GUIStyle();
				_unfocusedSelectedListItem.normal.textColor = Color.white;
				_unfocusedSelectedListItem.normal.background = unfocusedSelected;
				_unfocusedSelectedListItem.padding.left = 2;
				_unfocusedSelectedListItem.padding.right = 2;
				_unfocusedSelectedListItem.padding.top = 2;
				_unfocusedSelectedListItem.padding.bottom = 2;				
			}
			return _unfocusedSelectedListItem;
		}		
	}
	
	public static GUIStyle bottomTooltipBox
	{
		get
		{
			if(_bottomTooltipBox == null)
			{
				_bottomTooltipBox = new GUIStyle();

				if(EditorGUIUtility.isProSkin)
					_bottomTooltipBox.normal.background = GUI.skin.FindStyle("Box").normal.background;
				else
				{
					_bottomTooltipBox.normal.background = Resources.Load("Toolbar",typeof(Texture2D)) as Texture2D;
				}
				_bottomTooltipBox.padding.top = 4;
				_bottomTooltipBox.padding.left = 8;

				_bottomTooltipBox.normal.textColor = Color.white;

				_bottomTooltipBox.alignment = TextAnchor.UpperLeft;
			}
			
			return _bottomTooltipBox;
		}
	}
	
	public static GUIStyle editorTopToolBar
	{
		get
		{
			if(_editorTopToolBar == null)
			{
				_editorTopToolBar = new GUIStyle();
				_editorTopToolBar.normal.background = Resources.Load("Toolbar",typeof(Texture2D)) as Texture2D;
			}
			
			return _editorTopToolBar;
		}
	}
	
	public static GUIStyle editorButton
	{
		get
		{
			if (_editorButton == null)
			{
				_editorButton = new GUIStyle(GUI.skin.button);
				_editorButton.active = _editorButton.onActive;
				_editorButton.normal = _editorButton.onNormal;
				_editorButton.hover  = _editorButton.onHover;				
			}
			
			return _editorButton;
		}
	}
	
	public static GUIStyle editorButtonCreate
	{
		get
		{
			if(_editorButtonCreate == null)
			{
				_editorButtonCreate = new GUIStyle();
				_editorButtonCreate.normal.background = Resources.Load("Network_Create_icon", typeof(Texture2D)) as Texture2D;
			}
			
			return _editorButtonCreate;
		}
	}
	
	private static GUIStyle _inspectorLabel;
	public static GUIStyle inspectorLabel
	{
		get
		{
			if(_inspectorLabel == null)
			{
				_inspectorLabel = new GUIStyle(GUI.skin.GetStyle("Label"));
				_inspectorLabel.margin = new RectOffset(3,3,0,0);
				_inspectorLabel.wordWrap = true;				
			}
			
			return _inspectorLabel;
		}
	}
	
	private static GUIStyle _boldLabel;
	public static GUIStyle boldLabel
	{
		get
		{
			if (_boldLabel == null)
			{
				_boldLabel = new GUIStyle(GUI.skin.GetStyle("Label"));
				_boldLabel.margin = new RectOffset();
				_boldLabel.font = EditorStyles.boldFont;
			}
			
			return _boldLabel;
		}
	}	
	
	private static GUIStyle _lineStyle;
	public static GUIStyle lineStyle
	{
		get
		{
			if(_lineStyle == null)
			{
				_lineStyle = new GUIStyle();
				_lineStyle.normal.background = line;
				_lineStyle.margin = new RectOffset(5,5,0,0);
			}
			
			return _lineStyle;
			
		}
	}
	
	public static void ActionSeparator()
	{
		GUILayout.Space(2.5f);
		
		GUILayout.Label("",lineStyle, new GUILayoutOption[]
		{
			GUILayout.Height(1f),
			GUILayout.ExpandWidth(true)
		});
		
		GUILayout.Space(2.5f);
	}
	
	public static object SelectList(ICollection list, object selected, OnListItemGUI itemHandler)
	{
		foreach (object current in list)
		{
			if (itemHandler(current, selected == current, list))
			{
				selected = current;
			}
			else
			{
				if (selected == current)
				{
					selected = null;
				}
			}
		}
		return selected;
	}	
	
}
