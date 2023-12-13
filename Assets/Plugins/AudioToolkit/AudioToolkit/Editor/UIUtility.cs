//#if UNITY_EDITOR // Unity bug workaround: this way this file can be in subdirectorey of Standard Assets

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ClockStone
{
    static class UIUtility
    {
        /// <summary>
        /// Return last control ID setted in GUI
        /// </summary>
        /// <returns>Last control ID setted</returns>
        public static int GetLastControlId()
        {
            FieldInfo getLastControlId = typeof( EditorGUIUtility ).GetField( "s_LastControlID", BindingFlags.Static | BindingFlags.NonPublic );
            if( getLastControlId != null )
                return (int)getLastControlId.GetValue( null );
            return 0;
        }

        /// <summary>
        /// Make a search field GUI like Project, Hierarchy or Scene views
        /// </summary>
        /// <param name="_maxWidth">Maximum width of field</param>
        /// <param name="_searchFilter">Which search string is set to field</param>
        /// <returns></returns>
        public static bool SearchFieldGUI( float _maxWidth, ref string _searchFilter )
        {
            GUIStyle toolbarSearchFieldStyle = new GUIStyle( "ToolbarSeachTextField" );
            GUIStyle toolbarSearchFieldCancelButtonStyle = new GUIStyle( "ToolbarSeachCancelButton" );
            GUIStyle toolbarSearchFieldCancelButtonEmptyStyle = new GUIStyle( "ToolbarSeachCancelButtonEmpty" );

            Event current = Event.current;

            Rect rect = GUILayoutUtility.GetRect( _maxWidth * 0.2f, _maxWidth, 16f, 16f, toolbarSearchFieldStyle );
            rect.width -= 16f;

            var filter = _searchFilter;

            string str = EditorGUI.TextField( rect, filter, toolbarSearchFieldStyle );
            int controlId = GetLastControlId();

            rect.x += rect.width;
            rect.width = 16f;
            // Clear search filter
            if( GUI.Button( rect, GUIContent.none, _searchFilter == string.Empty ? toolbarSearchFieldCancelButtonEmptyStyle : toolbarSearchFieldCancelButtonStyle ) || ( current.type == EventType.KeyDown && current.keyCode == KeyCode.Escape ) )
            {
                _searchFilter = string.Empty;
                GUIUtility.keyboardControl = 0;
                return true;
            }
            // Typing search filter
            else if( str != _searchFilter )
            {
                _searchFilter = str;
                return true;
            }
            // Unfocus search field
            else if( current.type == EventType.MouseUp )
            {
                if( controlId != GUIUtility.hotControl && controlId == GUIUtility.keyboardControl )
                {
                    GUIUtility.keyboardControl = 0;
                    EditorGUIUtility.editingTextField = false;
                    current.Use();
                    return false;
                }
            }

            return false;
        }
    }
}