using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.UIElements;

namespace UnityToolbarExtender
{
	public static class ToolbarCallback
	{
		private static readonly Type ToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
		private static ScriptableObject _currentToolbar;
		private static bool _initialized;

		public static Action OnToolbarGUILeft;
		public static Action OnToolbarGUIRight;
		
		static ToolbarCallback()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			if (_currentToolbar == null)
			{
				var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
				_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
				
				if (_currentToolbar != null && !_initialized)
				{
					_initialized = true;
					
					var rootField = _currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
					if (rootField == null)
					{
						Debug.LogWarning("CustomToolbar: m_Root field not found");
						return;
					}
					
					var rawRoot = rootField.GetValue(_currentToolbar);
					if (rawRoot == null)
					{
						Debug.LogWarning("CustomToolbar: m_Root value is null");
						return;
					}
					
					var root = rawRoot as VisualElement;
					if (root == null)
					{
						Debug.LogWarning("CustomToolbar: Root is not a VisualElement");
						return;
					}

					Debug.Log("CustomToolbar: Inspecting toolbar structure...");
					LogVisualTreeRecursive(root, 0);

					// Try multiple possible zone names for Unity 6.3+
					var leftZone = root.Q("ToolbarZoneLeftAlign") 
					            ?? root.Q("unity-toolbar-zone-left-align")
					            ?? root.Q("ToolbarZoneLeft");
					
					var rightZone = root.Q("ToolbarZoneRightAlign") 
					             ?? root.Q("unity-toolbar-zone-right-align")
					             ?? root.Q("ToolbarZoneRight");
					
					if (leftZone != null)
					{
						AddToolbarCallback(leftZone, OnToolbarGUILeft);
						Debug.Log("CustomToolbar: Left zone registered");
					}
					else
					{
						Debug.LogWarning("CustomToolbar: Left zone not found");
					}
					
					if (rightZone != null)
					{
						AddToolbarCallback(rightZone, OnToolbarGUIRight);
						Debug.Log("CustomToolbar: Right zone registered");
					}
					else
					{
						Debug.LogWarning("CustomToolbar: Right zone not found");
					}
				}
			}
			else if (_currentToolbar == null && _initialized)
			{
				// Toolbar was destroyed, reset for re-initialization
				_initialized = false;
			}
		}

		private static void AddToolbarCallback(VisualElement zone, Action callback)
		{
			var container = new IMGUIContainer();
			container.style.flexGrow = 1;
			container.onGUIHandler += () => callback?.Invoke();
			zone.Add(container);
		}

		private static void LogVisualTreeRecursive(VisualElement element, int depth)
		{
			var indent = new string(' ', depth * 2);
			var name = string.IsNullOrEmpty(element.name) ? "<no name>" : element.name;
			Debug.Log($"{indent}Element: {name} (Type: {element.GetType().Name})");
			
			foreach (var child in element.Children())
			{
				LogVisualTreeRecursive(child, depth + 1);
			}
		}
	}
}
