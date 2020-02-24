using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//Displays the Network Editor
public class ActionNetworkEditor : EditorWindow {

	private static ActionNetworkEditor _instance;
	
	private Rect _toolTipRect;
	private Rect _toolBarRect;
	private Rect _networkRect;
	
	public ActionSkill selectedSkill;
	public ActionSkill skillBeingDrawn;
	
	public bool positioningSkill = false;
	public bool creatingNewSkill = false;
	public bool inspectorNeedsRepaint = false;
		
	private bool isCreatingConnection = false;
	private bool showNumbers = true;
	private bool showArrows = true;
	
	public List<int> selectedSkills = new List<int>();
	
	public List<ActionSkill> skillsToConnect = new List<ActionSkill>();
	
	public List<ActionConnection> connectionToRemove = new List<ActionConnection>();
	
	public int? selectedConnection = null;
	
	public EditorTwoVector offset = new EditorTwoVector();
	
    private const float kZoomMin = 0.1f;
    private const float kZoomMax = 1.0f;
 	
    public float _zoom = 1.0f;
	
	private Texture2D _bezierTexture;

	private Vector2 _currentMousePosition = Vector2.zero;
		
    public Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
    {
        return (screenCoords - _networkRect.TopLeft()) / _zoom + new Vector2(offset.X,offset.Y);
    }	
	
	public Vector2 ConvertZoomCoordsToScreenCoords(Vector2 zoomCoords)
	{
		return (zoomCoords - new Vector2(offset.X,offset.Y))*_zoom + _networkRect.TopLeft();
	}
	
	private readonly List<Rect> _reservedArea = new List<Rect>();
	private bool InsideReservedArea( Vector2 thePosition )
	{
		if( _reservedArea.Any( x => x.Contains( thePosition ) ) )
		{
			return true;
		}
		return false;
	}	
	
	public static ActionNetworkEditor instance
	{
		get
		{
			if (_instance == null)
			{
				GetWindow (typeof (ActionNetworkEditor),false);
			}
			
			return _instance;
		}
	}	
	
	public ActionNetworkEditor()
	{
		if (_instance != null)
		{
			Debug.LogError ("Instance already exists");
			DestroyImmediate (this);
			return;
		}
		
		_instance = this;
		
		titleContent = new GUIContent("Network Editor");
		
		currentContent = empty;
	}
	
	public void Update()
	{		
		if(positioningSkill)
			Repaint();	
	}
	
	public void OnInspectorUpdate ()
	{
		if(inspectorNeedsRepaint)
		{
			Repaint();
			
			inspectorNeedsRepaint = false;
		}
	}	
	
	new public void Show()
	{
		base.Show();
	}
	
    new public void Repaint ()
	{
		base.Repaint ();		
	}
	
	public void OnFocus()
	{
		Selection.activeObject = instance;		
	}
	
	new public void Focus()
	{
		base.Focus();
	}
	
	public enum Toolbar
	{
		NoLibrarySelected,
	}
	
	public void OnDestroy()
	{
		_instance = null;
	}
	
	public GUIContent noLibrarySelected = new GUIContent("No library selected", "Please select a Library to edit in the Library Explorer.");
	public GUIContent noNetworkSelected = new GUIContent("No network selected", "Please select a Network from the current Library");
	public GUIContent positioningNewSkill = new GUIContent("Left click to place your new skill on the network");
	public GUIContent creatingConnection = new GUIContent("Click on the two skills you wish to connect, or press escape to cancel");
	public GUIContent sameSkillForConnection = new GUIContent("Cannot create circular connection! Please select another skill or press escape to cancel");
	public GUIContent empty = new GUIContent(string.Empty);
	public GUIContent currentContent;
	
	void OnGUI ()			
	{		
		_reservedArea.Clear();
		
		_toolTipRect = new Rect(0f,Screen.height-42.5f,Screen.width,42.5f);
		_toolBarRect = new Rect(0f,0f,Screen.width,37.5f);
		_networkRect = new Rect(0f,37.5f,Screen.width,Screen.height-80f);
		
		_reservedArea.Add(_toolTipRect);
		_reservedArea.Add(_toolBarRect);
				
		//TOOLBAR AREA
		GUILayout.BeginArea(_toolBarRect);
		{		
			
			_currentMousePosition.x = Event.current.mousePosition.x;
			_currentMousePosition.y = Event.current.mousePosition.y;
			
			GUILayout.BeginHorizontal(ActionResources.editorTopToolBar, new GUILayoutOption[0]);
			{
				if(GUILayout.Button (new GUIContent(ActionResources.editorCreateNewSkill,"Create a new skill"),new GUILayoutOption[]
				{
					GUILayout.Width(30f),
					GUILayout.Height(30f)
				}))
				{
					isCreatingConnection = false;
					
					currentContent = empty;
					skillsToConnect.Clear();
					
					if(ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null)
					{
						GetWindow(typeof(ActionSkillCreation), true);
						
						ActionEditor.instance.SaveLibrary();				
					}					
				}	
				
				if(GUILayout.Button (new GUIContent(ActionResources.editorCreateNewConnection,"Create a new connection between skills"),new GUILayoutOption[]
				{
					GUILayout.Width(30f),
					GUILayout.Height(30f)
				}))
				{
					if(ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null)
					{
						if(selectedSkills.Count == 2)
						{
							skillsToConnect.Add(ActionEditor.instance.currentLibraryNetwork.skills[selectedSkills[0]]);
							skillsToConnect.Add(ActionEditor.instance.currentLibraryNetwork.skills[selectedSkills[1]]);
							
							GetWindow(typeof(ActionConnectionCreation), true);
						}
						else
						{
							currentContent = creatingConnection;
							
							this.Repaint();
							isCreatingConnection = true;							
						}
					}					
				}	
				
				GUILayout.FlexibleSpace();	
																
				showArrows = ToggleButton(showArrows,new GUIContent(ActionResources.editorShowArrows,"Toggle showing arrows on connections"),new GUILayoutOption[]
				{
					GUILayout.Width(30f),
					GUILayout.Height(30f)
				});
				
				showNumbers = ToggleButton(showNumbers,new GUIContent(ActionResources.editorShowNumbers,"Toggle showing numbers on connections"),new GUILayoutOption[]
				{
					GUILayout.Width(30f),
					GUILayout.Height(30f)
				});				
				
//				if(GUILayout.Button (new GUIContent(ActionResources.editorEdit,"Edit a skill or connection"),new GUILayoutOption[]
//				{
//					GUILayout.Width(30f),
//					GUILayout.Height(30f)
//				}))
//				{
//					if(ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null)
//					{			
//					}					
//				}				
//				
//				if(GUILayout.Button (new GUIContent(ActionResources.editorDel,"Delete a skill or connection"),new GUILayoutOption[]
//				{
//					GUILayout.Width(30f),
//					GUILayout.Height(30f)
//				}))
//				{
//					if(ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null)
//					{			
//					}					
//				}
				
				GUILayout.Space(8f);
				
			} GUILayout.EndHorizontal();
		}		
		GUILayout.EndArea();
		
		//HANDLE EVENTS
		if(ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null)		
			HandleEvents();	
		
		//NETWORK AREA
		DrawZoomArea();
		
		//DRAG BOX		
		if(doingSelectionBox)
		{
			var oldColor = GUI.color;
			var newColor = GUI.color;
			newColor.a = 0.4f;
			GUI.color = newColor;
			GUI.Box( GetSelectionArea(), "" );
			GUI.color = oldColor;
		}		
		
		//TOOLTIP BAR		
		
		if(ActionEditor.instance == null)
		{
			currentContent = noLibrarySelected;
			
		}
		else if(ActionEditor.instance.currentLibraryNetwork == null)
		{
			currentContent = noNetworkSelected;
			
		}
		else if(positioningSkill)
		{
			currentContent = positioningNewSkill;
			
		}

		GUILayout.BeginArea(_toolTipRect);
		{
			GUI.Box(new Rect(0f,0f,Screen.width,37.5f),currentContent,ActionResources.bottomTooltipBox);			
		}GUILayout.EndArea();	
		
		if(ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null && connectionToRemove.Count != 0)
		{
			ActionEditor.instance.currentLibraryNetwork.connections.RemoveAll(con => connectionToRemove.Contains(con));
			connectionToRemove.Clear();
			ActionEditor.instance.SaveLibrary();
		}
	}
	
	private void DrawZoomArea()
	{
		_bezierTexture =_bezierTexture ?? Resources.Load("Internal/1x2AA", typeof(Texture2D)) as Texture2D;
			
		
		EditorZoomArea.Begin(_zoom, _networkRect);
		{		
			if(ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null)
			{	
				Handles.BeginGUI();
				{		
					Handles.color = Color.black;
					
					int i = 0;
					
					foreach(ActionConnection ac in ActionEditor.instance.currentLibraryNetwork.connections)
					{	
						Vector2 toMiddle = ac.To.drawPos.Middle();
						Vector2 fromMiddle = ac.From.drawPos.Middle();
						
						Vector2 arrow1Pos = 0.5f*toMiddle + 0.5f*fromMiddle - new Vector2(5.5f,5f);
						Vector2 arrow2Pos = 0.5f*toMiddle + 0.5f*fromMiddle - new Vector2(5.5f,5f);					
				
						ActionSkill top = ac.To.Position.Y > ac.From.Position.Y ? ac.From : ac.To;
						ActionSkill bottom = ac.OtherEnd(top);
						
						Vector3 start = new Vector3(top.drawPos.Middle().x,top.drawPos.Middle().y);
						Vector3 end = new Vector3(bottom.drawPos.Middle().x,bottom.drawPos.Middle().y);
						
						if(i == selectedConnection)
						{
							Handles.DrawBezier(start,
								end,
								start,
								end,
								new Color(1,0f,0f,1f),
								_bezierTexture,
								8f);							
						}						
						
						Handles.DrawBezier(start,
							end,
							start,
							end,
							new Color(1,125f/255f,0f,1f),
							_bezierTexture,
							6f);
						
						Vector3 arrowDirection = new Vector3(toMiddle.x-fromMiddle.x,toMiddle.y-fromMiddle.y,0f);
						
						float arrowDirectionLength = arrowDirection.magnitude;
						
						Handles.color = new Color(1f,125f/255f,0f,1f);
						
						float angle1 = Mathf.Atan2(toMiddle.y-fromMiddle.y,toMiddle.x-fromMiddle.x);
						
						Vector2 labelPos = RotatePoint(new Vector2(0,-25f),angle1);
						
						Rect arrow1box = new Rect(arrow1Pos.x + 35f*arrowDirection.x/arrowDirectionLength + labelPos.x + 5f,
							arrow1Pos.y + 35f*arrowDirection.y/arrowDirectionLength + labelPos.y + 5f, 7.5f + ac.fromValue.ToString().Length*7.5f,18f);
						
						arrow1box.center -= new Vector2(arrow1box.width/2,arrow1box.height/2);
						
						Rect arrow2box = new Rect(arrow2Pos.x - 35f*arrowDirection.x/arrowDirectionLength - labelPos.x + 5f,
							arrow2Pos.y - 35f*arrowDirection.y/arrowDirectionLength - labelPos.y + 5f,7.5f + ac.toValue.ToString().Length*7.5f,18f);
						
						arrow2box.center -= new Vector2(arrow2box.width/2,arrow2box.height/2);
												
						Vector3 toVect3 = new Vector3(arrow1Pos.x + 5f + 20f*arrowDirection.x/arrowDirectionLength,arrow1Pos.y + 5f + 20f*arrowDirection.y/arrowDirectionLength,-10f);
						
						if(showNumbers)
						{
							if(ac.toValue != 0)
							{	
								GUI.Label(arrow2box,ac.toValue.ToString(),GUI.skin.GetStyle("TextField"));
								
								//Handles.ArrowCap(0,toVect3,Quaternion.LookRotation(-arrowDirection),60f);
							}
								
							//Vector3 fromVec3 = new Vector3(arrow2Pos.x + 5f - 20f*arrowDirection.x/arrowDirectionLength,arrow2Pos.y + 5f - 20f*arrowDirection.y/arrowDirectionLength,-10f);
							
							if(ac.fromValue != 0)
							{	
								GUI.Label(arrow1box,ac.fromValue.ToString (),GUI.skin.GetStyle("TextField"));
								//Handles.ArrowCap(0,fromVec3,Quaternion.LookRotation(arrowDirection),60f);
							}
						}
						
						if(showArrows)
						{
							if(ac.toValue != 0)
							{	
								//GUI.Label(arrow2box,ac.toValue.ToString(),GUI.skin.GetStyle("TextField"));
								if(Mathf.Approximately(arrowDirection.sqrMagnitude,0f))
								{
									Handles.ArrowHandleCap(0,toVect3,Quaternion.identity,60f, EventType.Repaint);
								}
								else Handles.ArrowHandleCap(0,toVect3,Quaternion.LookRotation(-arrowDirection),60f, EventType.Repaint);
							}
								
							Vector3 fromVec3 = new Vector3(arrow2Pos.x + 5f - 20f*arrowDirection.x/arrowDirectionLength,arrow2Pos.y + 5f - 20f*arrowDirection.y/arrowDirectionLength,-10f);
							
							if(ac.fromValue != 0)
							{	
								//GUI.Label(arrow1box,ac.fromValue.ToString (),GUI.skin.GetStyle("TextField"));
								if(!Mathf.Approximately(arrowDirection.sqrMagnitude,0f))
									Handles.ArrowHandleCap(0,fromVec3,Quaternion.LookRotation(arrowDirection),60f, EventType.Repaint);
								else
									Handles.ArrowHandleCap(0,fromVec3,Quaternion.identity,60f, EventType.Repaint);
							}
						}
						
//						if(ac.toValue == 0 && ac.fromValue == 0)
//						{
//							connectionToRemove.Add(ac);
//						}
						
						++i;
						
					}			
					
					if(isCreatingConnection && skillsToConnect.Count == 1)
					{
						Vector2 zoomMousePos = ConvertScreenCoordsToZoomCoords(new Vector2(_currentMousePosition.x,_currentMousePosition.y)) - new Vector2(offset.X,offset.Y);
						
						Vector2 mouseDirection = skillsToConnect[0].drawPos.Middle() - zoomMousePos;						
												
						Handles.DrawBezier(new Vector3(skillsToConnect[0].drawPos.Middle().x,skillsToConnect[0].drawPos.Middle().y),
							new Vector3(zoomMousePos.x,zoomMousePos.y),
							new Vector3(skillsToConnect[0].drawPos.Middle().x-mouseDirection.x,skillsToConnect[0].drawPos.Middle().y-mouseDirection.y),
							new Vector3(zoomMousePos.x+mouseDirection.x,zoomMousePos.y+mouseDirection.y),
							new Color(1,125f/255f,0f,1f),
							_bezierTexture,
							6f);
						
						this.Repaint();
					}
				
				}Handles.EndGUI();
				
				
				if(ActionEditor.instance.currentLibraryNetwork.skills != null)
				{
					int i = 0;
					
					foreach(ActionSkill actSkill in ActionEditor.instance.currentLibraryNetwork.skills)
					{
						actSkill.Draw(selectedSkills.Contains(i), actSkill == skillBeingDrawn);
						
						++i;
					}
				}
				
						
			}
			
		}EditorZoomArea.End();	
	}
	
	private void HandleEvents()
	{
		switch(Event.current.type)
		{
		case EventType.MouseDown:

			if(Event.current.button == 0)
			{				
				if(LeftMouseDown())
				{					
					Event.current.Use ();
					
					return;
				}
			}		
			
			break;
		case EventType.MouseDrag:
			
			if (Event.current.button == 0 )
			{
				if( LeftMouseDragged() )
				{
					Event.current.Use();
				}
			}
			
			if (Event.current.button == 2 || (Event.current.button == 1 && Event.current.modifiers == EventModifiers.Alt) )
			{
	            Vector2 delta = Event.current.delta;
	            delta /= _zoom;
	            
				offset.X += delta.x;
				offset.Y += delta.y;
				
				Event.current.Use();
			}			
							
			break;
			
		case EventType.MouseUp:
			
			if(Event.current.button == 0)
			{				
				if(doingSelectionBox)
				{
					doingSelectionBox = false;
					
					Rect getSelected = GetSelectionArea();					
					Vector2 zoomGetSelectionAreaConvertedTopLeft = ConvertScreenCoordsToZoomCoords(getSelected.TopLeft());
					
					Select (new Rect(zoomGetSelectionAreaConvertedTopLeft.x - offset.X,zoomGetSelectionAreaConvertedTopLeft.y - offset.Y,getSelected.width/_zoom,getSelected.height/_zoom), Event.current.modifiers == EventModifiers.Shift);
					
					Event.current.Use();

				}
				else
				{
					if(selectedSkills.Count > 0)
					{
						ActionEditor.instance.SaveLibrary();
					}
					
					Event.current.Use();					
				}
			}
			else if(Event.current.button == 2)
			{
				ActionEditor.instance.SaveLibrary();
				
				Event.current.Use();
			}		
		
			break;
			
		case EventType.ScrollWheel:

            Vector2 d = Event.current.delta;
            float zoomDelta = -d.y / 150.0f;
            _zoom += zoomDelta;
            _zoom = Mathf.Clamp(_zoom, kZoomMin, kZoomMax);
            //offset.X += (zoomCoordsMousePos.x - offset.X) - (oldZoom / _zoom) * (zoomCoordsMousePos.x - offset.X);
			//offset.Y += (zoomCoordsMousePos.y - offset.Y) - (oldZoom / _zoom) * (zoomCoordsMousePos.y - offset.Y);		
			
            Event.current.Use();			
			
			break;
		}
		
		if(isCreatingConnection && Event.current.keyCode == KeyCode.Escape)
		{
			Debug.Log ("Cancelling connection");
			
			isCreatingConnection = false;
			skillsToConnect.Clear();

			
			currentContent = empty;
			
			this.Repaint();
			
			Event.current.Use ();
		}		
		
		if(Event.current.keyCode == KeyCode.Delete)
		{
			if(selectedSkills.Count > 0)
			{
				List<ActionSkill> skillsToDelete = new List<ActionSkill>();
				
				foreach(int i in selectedSkills)
				{
					skillsToDelete.Add(ActionEditor.instance.currentLibraryNetwork.skills[i]);
				}
				
				selectedSkills.Clear();
				
				foreach(ActionSkill actS in skillsToDelete)
				{
					ActionEditor.instance.currentLibraryNetwork.DeleteSkill(actS);

					ActionEditor.instance.SaveLibrary();
					this.Repaint();
				}				
				
				Event.current.Use ();
			}
			
			if(selectedConnection.HasValue)
			{
				ActionConnection connectionToDelete = ActionEditor.instance.currentLibraryNetwork.connections[selectedConnection.Value];
				selectedConnection = null;
				
				ActionEditor.instance.currentLibraryNetwork.connections.Remove(connectionToDelete);
				
				ActionEditor.instance.SaveLibrary();
			}
		}
		
		if(Event.current.modifiers == EventModifiers.Control && Event.current.keyCode == KeyCode.F)
		{
			if(!creatingNewSkill)
			{
				if(ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null)
				{
					GetWindow(typeof(ActionSkillCreation), true);	

					ActionEditor.instance.SaveLibrary();						
				}			
			}
		}
		
		if(Event.current.modifiers == EventModifiers.Control && Event.current.keyCode == KeyCode.D)
		{
			if(!isCreatingConnection)
			{
				if(ActionEditor.instance != null && ActionEditor.instance.currentLibraryNetwork != null)
				{			
					if(selectedSkills.Count == 2)
					{
						skillsToConnect.Add(ActionEditor.instance.currentLibraryNetwork.skills[selectedSkills[0]]);
						skillsToConnect.Add(ActionEditor.instance.currentLibraryNetwork.skills[selectedSkills[1]]);
						
						GetWindow(typeof(ActionConnectionCreation), true);
					}
					else
					{
						currentContent = creatingConnection;
						
						this.Repaint();
						isCreatingConnection = true;							
					}
				}
			}
		}
		
		if(Event.current.modifiers == EventModifiers.Control && Event.current.keyCode == KeyCode.A)
		{
			if(selectedSkills.Count != ActionEditor.instance.currentLibraryNetwork.skills.Count)
			{
				for(int i=0; i < ActionEditor.instance.currentLibraryNetwork.skills.Count; ++i)
				{
					selectedSkills.Add(i);
				}
				
				this.Repaint();
				inspectorNeedsRepaint = true;
			}
		}

//		if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Z && Event.current.modifiers != EventModifiers.Control)
//		{
//			ActionEditor.instance.Undo();
//
//		}
//
//		if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.X && Event.current.modifiers != EventModifiers.Control)
//		{
//			ActionEditor.instance.Redo();
//		}
		
	}

	private Rect GetSelectionArea()
	{
		float left = selectBoxStart.x < _currentMousePosition.x ? selectBoxStart.x : _currentMousePosition.x;
		float right = selectBoxStart.x > _currentMousePosition.x ? selectBoxStart.x : _currentMousePosition.x;
		float top = selectBoxStart.y < _currentMousePosition.y ? selectBoxStart.y : _currentMousePosition.y;
		float bottom = selectBoxStart.y > _currentMousePosition.y ? selectBoxStart.y : _currentMousePosition.y;
		return new Rect( left, top, right - left, bottom - top );
	}	
	
	public bool Select ( Rect area, bool addSelect )
	{
		var toSelect = (from skill in ActionEditor.instance.currentLibraryNetwork.skills
			where area.OverLaps( skill.drawPos )
			select skill);
		
		if( !addSelect )
		{
			selectedSkills.Clear();
		}
		
		foreach( var n in toSelect )
		{
			//Force the selected to be the last in the list
			if( selectedSkills.Contains(ActionEditor.instance.currentLibraryNetwork.skills.IndexOf(n)))
			{
				selectedSkills.Remove(ActionEditor.instance.currentLibraryNetwork.skills.IndexOf(n));
			}
			
			
			selectedSkills.Add(ActionEditor.instance.currentLibraryNetwork.skills.IndexOf(n));
		}
		
		inspectorNeedsRepaint = true;

		return toSelect.Count() > 0;
	}
	
	private Vector2 selectBoxStart = Vector2.zero;
	private bool doingSelectionBox = false;
	
	private bool LeftMouseDown()
	{
		if(positioningSkill && skillBeingDrawn != null && !InsideReservedArea(new Vector2(Event.current.mousePosition.x,Event.current.mousePosition.y)))
		{					
			positioningSkill = false;
			skillBeingDrawn.DeltaMove(new Vector2(-offset.X,-offset.Y));
			skillBeingDrawn = null;
			
			
			ActionEditor.instance.SaveLibrary ();
			
			currentContent = empty;

			return true;
		}	

		//var clicked = SkillAt(new Vector2(Event.current.mousePosition.x,Event.current.mousePosition.y - 37.5f));
		var clicked = SkillAt(ConvertScreenCoordsToZoomCoords(Event.current.mousePosition)- new Vector2(offset.X,offset.Y));
				
		if(clicked != null)
		{
			selectedConnection = null;
			
			if(isCreatingConnection)
			{
				if(skillsToConnect.Count == 0)
				{
					skillsToConnect.Add(clicked);
				}				
				else if(skillsToConnect.Count == 1)
				{
					if(clicked != skillsToConnect[0])
					{
						skillsToConnect.Add(clicked);
						isCreatingConnection = false;
						currentContent = empty;
						
						GetWindow(typeof(ActionConnectionCreation), true);
					}
					else
					{						
						currentContent = sameSkillForConnection;
						
						this.Repaint();
					}
				}
				else return false;			
				
				return true;
			}
			
			
			if(!selectedSkills.Contains(ActionEditor.instance.currentLibraryNetwork.skills.IndexOf(clicked)))
			{
				if(Event.current.modifiers != EventModifiers.Shift)
					selectedSkills.Clear();
				
				selectedSkills.Add(ActionEditor.instance.currentLibraryNetwork.skills.IndexOf(clicked));
				
				inspectorNeedsRepaint = true;				
			}
			else
			{
				if(Event.current.modifiers == EventModifiers.Shift)
					selectedSkills.Remove(ActionEditor.instance.currentLibraryNetwork.skills.IndexOf(clicked));
			}
			
			return true;
		}
		else if(clicked == null && Event.current.modifiers != EventModifiers.Alt)
		{		
			var clickedConnection = ConnectionAt(ConvertScreenCoordsToZoomCoords(Event.current.mousePosition)- new Vector2(offset.X,offset.Y));			
			
			if(clickedConnection != null)
			{			
				if (Event.current.clickCount == 2)
				{
					selectedSkills.Clear();

					selectedSkills.Add(ActionEditor.instance.currentLibraryNetwork.skills.IndexOf(clickedConnection.From));
					selectedSkills.Add(ActionEditor.instance.currentLibraryNetwork.skills.IndexOf(clickedConnection.To));
				}			
				
				selectedConnection = ActionEditor.instance.currentLibraryNetwork.connections.IndexOf(clickedConnection);
				return true;
			}
			else
			{
				selectedConnection = null;
			}			
			
			if(Event.current.modifiers != EventModifiers.Shift)
				selectedSkills.Clear();
			
			selectBoxStart = Event.current.mousePosition;		
			doingSelectionBox = true;
			
			return true;
		}	
		
		return false;
	}
	
	private bool LeftMouseDragged()
	{
		if(Event.current.modifiers != EventModifiers.Shift && Event.current.modifiers != EventModifiers.Alt)
		{
			foreach(int i in selectedSkills)
			{
				ActionEditor.instance.currentLibraryNetwork.skills[i].DeltaMove(Event.current.delta/_zoom);
			}
		}
		else if(Event.current.modifiers == EventModifiers.Alt)
		{
            Vector2 delta = Event.current.delta;
            float zoomDelta = -delta.y / 150.0f;
            _zoom += zoomDelta;
            _zoom = Mathf.Clamp(_zoom, kZoomMin, kZoomMax);
			
			return true;
		}
		
		return true;

	}
	
	public ActionSkill SkillAt(Vector2 position)
	{
		var clicked = (from skill in ActionEditor.instance.currentLibraryNetwork.skills
			where skill.Contains(position) select skill).LastOrDefault();
		
		if(clicked != null)
		{
			return clicked;
		}
		
		return null;
	}
	
	public ActionConnection ConnectionAt(Vector2 position)
	{
		var clicked = (from actionConnection in ActionEditor.instance.currentLibraryNetwork.connections
			where actionConnection.Contains(position) select actionConnection).LastOrDefault();
		
		if(clicked != null)
		{
			return clicked;
		}
		
		return null;		
	}
	
	public void CentreSkill()
	{
		_zoom = 1.0f;
		
		float width = _networkRect.width;
		float height = _networkRect.height;
						
		ActionEditor.instance.currentLibraryNetwork.skills[selectedSkills[0]].Draw(false,false);
		
		Vector2 distToMove = ActionEditor.instance.currentLibraryNetwork.skills[selectedSkills[0]].drawPos.center - ConvertScreenCoordsToZoomCoords(new Vector2(width/2,height/2)) + new Vector2(offset.X,offset.Y);
		
		offset.X -= distToMove.x;
		offset.Y -= distToMove.y;
	}
	
	public void CentreNetwork(ActionNetwork actionNetwork)
	{
		_zoom = 1.0f;
				
		Vector2 centreOfMass = new Vector2();
		
		float width = _networkRect.width;
		float height = _networkRect.height;
		
		foreach(ActionSkill actS in actionNetwork.skills)
		{
			//actS.Draw(false,false);
			centreOfMass += actS.drawPos.center;
		}
		
		centreOfMass /= actionNetwork.skills.Count;
		
		Vector2 distToMove = centreOfMass - ConvertScreenCoordsToZoomCoords(new Vector2(width/2,height/2))
			+ new Vector2(offset.X,offset.Y);
		
		offset.X -= distToMove.x;
		offset.Y -= distToMove.y;	
		
	}
	
	public Vector2 RotatePoint(Vector2 point, float angle)
	{
		return new Vector2(Mathf.Cos(angle)*point.x - Mathf.Sin(angle)*point.y, Mathf.Sin(angle)*point.x + Mathf.Cos(angle)*point.y);
	}
	
	public static bool ToggleButton(bool value, string label, string tooltip) {
		
		GUIStyle state = new GUIStyle(GUI.skin.button);
		
		if (value)
			state.normal = state.onNormal;
		
		GUIContent content = new GUIContent(label,tooltip);
		
		bool pressed = GUILayout.Button(content,state);
		
		if (pressed)
			return !value;
		return value;
	}	
	
	public static bool ToggleButton(bool value, GUIContent content, GUILayoutOption[] options) {
		
		GUIStyle state = new GUIStyle(GUI.skin.button);
		
		if (value)
			state.normal = state.onNormal;
		
		bool pressed = GUILayout.Button(content,state,options); //GUILayout.Button(content,options);
		
		if (pressed)
			return !value;
		return value;
	}	
	
}
