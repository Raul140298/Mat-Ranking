using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ClockStone
{
    abstract public class EditorEx : UnityEditor.Editor
    {
        protected GUILayoutOption labelFieldOption;
        protected GUIStyle styleLabel;
        protected GUIStyle styleUnit;
        protected GUIStyle styleFloat;
        protected GUIStyle styleFloatIndi;
        protected GUIStyle stylePopup;
        protected GUIStyle styleEnum;

        bool setFocusNextField;
        bool userChanges;
        int fieldIndex;

        virtual protected void LogUndo( string label )
        {

        }

        protected void SetFocusForNextEditableField()
        {
            setFocusNextField = true;
        }

        protected void ShowFloat( float f, string label )
        {
            EditorGUILayout.LabelField( label, f.ToString() );
        }

        protected void ShowString( string text, string label )
        {
            EditorGUILayout.LabelField( label, text );
        }

        private float GetFloat( float f, string label, string unit, string tooltip = null )
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( new GUIContent( label, tooltip ), labelFieldOption );
            //GUILayout.Label( label, styleLabel );
            //EditorGUILayout.Space();
            float f_ret = EditorGUILayout.FloatField( f, styleFloat );

            if( !string.IsNullOrEmpty( unit ) )
            {
                GUILayout.Label( unit, styleUnit );
            }
            else
            {
                GUILayout.Label( " ", styleUnit );
            }

            EditorGUILayout.EndHorizontal();

            //float f_ret = EditorGUILayout.FloatField( label, f, styleFloat );
            return f_ret;
        }

        private float GetFloatInherited( float f, string label, string unit, bool individual, out bool reset, string tooltip = null )
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( new GUIContent( label, tooltip ), labelFieldOption );
            //GUILayout.Label( label, styleLabel );
            //EditorGUILayout.Space();
            float f_ret = EditorGUILayout.FloatField( f, individual ? styleFloatIndi : styleFloat );

            if( !string.IsNullOrEmpty( unit ) )
            {
                GUILayout.Label( unit, styleUnit );
            }
            else
            {
                GUILayout.Label( " ", styleUnit );
            }


            GUI.enabled = individual;

            reset = GUILayout.Button( new GUIContent( "R", "Reset to AudioItem value" ), GUILayout.Width( 20f ) );

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            //float f_ret = EditorGUILayout.FloatField( label, f, styleFloat );
            return f_ret;
        }

        private int GetInt( int f, string label, string unit, string tooltip = null )
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label( new GUIContent( label, tooltip ), styleLabel );
            //EditorGUILayout.Space();
            int f_ret = EditorGUILayout.IntField( f, styleFloat );
            if( !string.IsNullOrEmpty( unit ) )
            {
                GUILayout.Label( unit, styleUnit );
            }
            else
            {
                GUILayout.Label( " ", styleUnit );
            }
            EditorGUILayout.EndHorizontal();
            return f_ret;
        }

        private float GetFloat( float f, string label, float sliderMin, float sliderMax, string unit )
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label( label, styleLabel );
            //EditorGUILayout.Space();
            float f_ret = f;
            f_ret = EditorGUILayout.FloatField( f_ret, styleFloat, GUILayout.Width( 50 ) );
            f_ret = GUILayout.HorizontalSlider( f_ret, sliderMin, sliderMax );

            if( !string.IsNullOrEmpty( unit ) )
            {
                GUILayout.Label( unit, styleUnit );
            }
            else
            {
                GUILayout.Label( " ", styleUnit );
            }

            EditorGUILayout.EndHorizontal();
            return f_ret;
        }

        private float GetFloatPercent( float f, string label, string unit, string tooltip = null )
        {
            EditorGUILayout.BeginHorizontal();
            //GUILayout.Label( label, styleLabel );
            EditorGUILayout.LabelField( new GUIContent( label, tooltip ), labelFieldOption );
            //EditorGUILayout.Space();
            float f_ret = f;
            f_ret = (float)EditorGUILayout.IntField( Mathf.RoundToInt( f_ret * 100f ), styleFloat, GUILayout.Width( 50 ) ) / 100f;
            f_ret = GUILayout.HorizontalSlider( f_ret, 0f, 1f );

            if( !string.IsNullOrEmpty( unit ) )
            {
                GUILayout.Label( unit, styleUnit );
            }
            else
            {
                GUILayout.Label( " ", styleUnit );
            }

            EditorGUILayout.EndHorizontal();
            return f_ret;
        }

        private float GetFloatPercentInherited( float f, string label, string unit, bool individual, out bool reset, string tooltip = null )
        {
            EditorGUILayout.BeginHorizontal();
            //GUILayout.Label( label, styleLabel );
            EditorGUILayout.LabelField( new GUIContent( label, tooltip ), labelFieldOption );
            //EditorGUILayout.Space();
            float f_ret = f;

            //Here we have to handle the float and int value seperately because due to rounding for the integer display, the inheritance connection would be lost
            int i_ret = Mathf.RoundToInt( f_ret * 100f );
            int old_i_ret = i_ret;

            i_ret = EditorGUILayout.IntField( i_ret, individual ? styleFloatIndi : styleFloat, GUILayout.Width( 50 ) );

            if( old_i_ret != i_ret )
                f_ret = i_ret / 100f;

            f_ret = GUILayout.HorizontalSlider( f_ret, 0f, 1f );

            if( !string.IsNullOrEmpty( unit ) )
            {
                GUILayout.Label( unit, styleUnit );
            }
            else
            {
                GUILayout.Label( " ", styleUnit );
            }

            GUI.enabled = individual;

            reset = GUILayout.Button( new GUIContent( "R", "Reset to AudioItem value" ), GUILayout.Width( 20f ) );

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            return f_ret;
        }

        private float GetFloatPlusMinusPercent( float f, string label, string unit )
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label( label, styleLabel );
            //EditorGUILayout.Space();
            float f_ret = f;
            f_ret = (float)EditorGUILayout.IntField( Mathf.RoundToInt( f_ret * 100 ), styleFloat, GUILayout.Width( 50 ) ) / 100;
            f_ret = GUILayout.HorizontalSlider( f_ret, -1, 1 );
            if( !string.IsNullOrEmpty( unit ) )
            {
                GUILayout.Label( unit, styleUnit );
            }
            else
            {
                GUILayout.Label( " ", styleUnit );
            }
            EditorGUILayout.EndHorizontal();
            return f_ret;
        }

        protected bool EditFloat( ref float f, string label )
        {
            float new_f = GetFloat( f, label, null );

            if( Mathf.Abs( new_f - f ) > float.Epsilon )
            {
                LogUndo( label );
                f = new_f;
                return true;
            }

            return false;
        }

        protected bool EditFloat( ref float f, string label, string unit, string tooltip = null )
        {
            float new_f = GetFloat( f, label, unit, tooltip );

            if( Mathf.Abs( new_f - f ) > float.Epsilon )
            {
                LogUndo( label );
                f = new_f;
                return true;
            }

            return false;
        }

        protected bool EditFloatInherited( ref float f, string label, string unit, bool individual, out bool reset, string tooltip = null )
        {
            float new_f = GetFloatInherited( f, label, unit, individual, out reset, tooltip );

            if( Mathf.Abs( new_f - f ) > float.Epsilon )
            {
                LogUndo( label );
                f = new_f;
                return true;
            }

            return false;
        }

        protected bool EditFloat01Inherited( ref float f, string label, string unit, bool individual, out bool reset, string tooltip = null )
        {
            float new_f = GetFloatPercentInherited( f, label, unit, individual, out reset, tooltip );

            if( Mathf.Abs( new_f - f ) > float.Epsilon )
            {
                LogUndo( label );
                f = new_f;
                return true;
            }

            return false;
        }

        private float GetFloat01( float f, string label )
        {
            return Mathf.Clamp01( GetFloatPercent( f, label, null ) );
        }

        private float GetFloat01( float f, string label, string unit, string tooltip = null )
        {
            return Mathf.Clamp01( GetFloatPercent( f, label, unit, tooltip ) );
        }

        private float GetFloatPlusMinus1( float f, string label, string unit )
        {
            return Mathf.Clamp( GetFloatPlusMinusPercent( f, label, unit ), -1, 1 );
        }

        private float GetFloatWithinRange( float f, string label, float minValue, float maxValue )
        {
            return Mathf.Clamp( GetFloat( f, label, minValue, maxValue, null ), minValue, maxValue );
        }

        protected bool EditFloatWithinRange( ref float f, string label, float minValue, float maxValue )
        {
            float new_f = GetFloatWithinRange( f, label, minValue, maxValue );

            if( Mathf.Abs( new_f - f ) > float.Epsilon )
            {
                LogUndo( label );
                f = new_f;
                return true;
            }

            return false;
        }

        protected bool EditInt( ref int f, string label )
        {
            int new_f = GetInt( f, label, null );

            if( new_f != f )
            {
                LogUndo( label );
                f = new_f;
                return true;
            }
            return false;
        }

        protected bool EditInt( ref int f, string label, string unit, string tooltip = null )
        {
            int new_f = GetInt( f, label, unit, tooltip );

            if( new_f != f )
            {
                LogUndo( label );
                f = new_f;
                return true;
            }
            return false;
        }

        protected bool EditFloat01( ref float f, string label )
        {
            float new_f = GetFloat01( f, label );

            if( Mathf.Abs( new_f - f ) > float.Epsilon )
            {
                LogUndo( label );
                f = new_f;
                return true;
            }

            return false;
        }

        protected bool EditFloat01( ref float f, string label, string unit, string tooltip = null )
        {
            float new_f = GetFloat01( f, label, unit, tooltip );

            if( Mathf.Abs( new_f - f ) > float.Epsilon )
            {
                LogUndo( label );
                f = new_f;
                return true;
            }

            return false;
        }

        protected bool EditFloatPlusMinus1( ref float f, string label, string unit )
        {
            float new_f = GetFloatPlusMinus1( f, label, unit );

            if( Mathf.Abs( new_f - f ) > float.Epsilon )
            {
                LogUndo( label );
                f = new_f;
                return true;
            }

            return false;

        }

        private bool GetBool( bool b, string label, string tooltip = null )
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( new GUIContent( label, tooltip ), labelFieldOption );
            //GUILayout.Label( label, styleLabel );
            //EditorGUILayout.Space();
            bool b_ret = EditorGUILayout.Toggle( b, GUILayout.Width( 20 ) );
            EditorGUILayout.EndHorizontal();
            return b_ret;
        }

        protected bool EditBool( ref bool b, string label, string tooltip = null ) // returns was changed state
        {
            bool new_b = GetBool( b, label, tooltip );

            if( new_b != b )
            {
                LogUndo( label );
                b = new_b;
                return true;
            }

            return false;
        }

        protected bool EditPrefab<T>( ref T prefab, string label, string tooltip = null ) where T : UnityEngine.Object
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( new GUIContent( label, tooltip ), labelFieldOption );
            //GUILayout.Label( label, styleLabel );
            T new_f = (T)EditorGUILayout.ObjectField( prefab, typeof( T ), false );
            EditorGUILayout.EndHorizontal();

            if( new_f != prefab )
            {
                LogUndo( label );
                prefab = new_f;
                return true;
            }
            return false;
        }

        protected bool EditString( ref string txt, string label, GUIStyle styleText = null, string tooltip = null )
        {
            return EditString( ref txt, label, styleLabel, styleText, tooltip );
        }

        protected bool EditString( ref string txt, string label, GUIStyle styleLable, GUIStyle styleText, string tooltip = null )
        {
            EditorGUILayout.BeginHorizontal();
            //GUILayout.Label( label, styleLabel );
            EditorGUILayout.LabelField( new GUIContent( label, tooltip ), styleLable, labelFieldOption );
            //EditorGUILayout.Space();
            BeginEditableField();

            string newTxt;
            if( styleText != null )
            {
                newTxt = EditorGUILayout.TextField( txt, styleText );
            }
            else
            {
                newTxt = EditorGUILayout.TextField( txt );
            }
            EndEditableField();
            EditorGUILayout.EndHorizontal();

            if( newTxt != txt )
            {
                LogUndo( label );
                txt = newTxt;
                return true;
            }
            return false;
        }

        protected int Popup( string label, int selectedIndex, string[] content, string tooltip = null, bool sortAlphabetically = true )
        {
            return PopupWithStyle( label, selectedIndex, content, stylePopup, tooltip, sortAlphabetically );
        }

        public class ContentWithIndex
        {
            public string content;
            public int index;

            public ContentWithIndex( string content, int index )
            {
                this.content = content;
                this.index = index;
            }
        }

        protected int PopupWithStyle( string label, int selectedIndex, string[] content, GUIStyle style, string tooltip = null, bool sortAlphabetically = true )
        {
            string[] contentSorted;

            List<ContentWithIndex> list = null;

            if( content.Length == 0 )
            {
                sortAlphabetically = false;
            }

            if( sortAlphabetically )
            {
                list = _CreateContentWithIndexList( content );
                contentSorted = new string[content.Length];
                int index = 0;
                foreach( var el in list )
                {
                    contentSorted[index++] = el.content;
                }
            }
            else
                contentSorted = content;

            EditorGUILayout.BeginHorizontal();
            //GUILayout.Label( label, styleLabel );
            EditorGUILayout.LabelField( new GUIContent( label, tooltip ), labelFieldOption );
            int newIndex;
            if( sortAlphabetically )
            {
                newIndex = EditorGUILayout.Popup( list.FindIndex( x => x.index == selectedIndex ), contentSorted, style );

                newIndex = list[newIndex].index;
            }
            else
            {
                newIndex = EditorGUILayout.Popup( selectedIndex, contentSorted, style );
            }
            EditorGUILayout.EndHorizontal();
            if( newIndex != selectedIndex )
            {
                LogUndo( label );
            }
            return newIndex;
        }

        private List<ContentWithIndex> _CreateContentWithIndexList( string[] content )
        {
            var list = new List<ContentWithIndex>();
            for( int i = 0; i < content.Length; i++ )
            {
                list.Add( new ContentWithIndex( content[i], i ) );
            }
            return list.OrderBy( x => x.content ).ToList();
        }

        protected Enum EnumPopup( string label, Enum selectedEnum, string tooltip = null )
        {
            EditorGUILayout.BeginHorizontal();
            //GUILayout.Label( label, styleLabel );
            EditorGUILayout.LabelField( new GUIContent( label, tooltip ), labelFieldOption );
            Enum newEnum = EditorGUILayout.EnumPopup( selectedEnum, styleEnum );
            EditorGUILayout.EndHorizontal();

            if( !object.Equals( newEnum, selectedEnum ) )
            {
                LogUndo( label );
            }
            return newEnum;
        }

        private void EndEditableField()
        {
            if( setFocusNextField )
            {
                setFocusNextField = false;
                //GUI.FocusControl( GetCurrentFieldControlName() );  // TODO: not working for some reason
                //Debug.Log( "Set focus to: " + GetCurrentFieldControlName() );
                //Debug.Log( "Currently focused: " + GUI.GetNameOfFocusedControl() );
            }
        }

        private void BeginEditableField()
        {
            fieldIndex++;
            if( setFocusNextField )
            {
                GUI.SetNextControlName( GetCurrentFieldControlName() );
            }
        }

        private String GetCurrentFieldControlName()
        {
            return string.Format( "field{0}", fieldIndex );
        }

        protected void BeginInspectorGUI()
        {
            serializedObject.Update();
            SetStyles();

            fieldIndex = 0;
            userChanges = false;
        }

        protected void SetStyles()
        {
            labelFieldOption = GUILayout.Width( 180 );
            styleLabel = new GUIStyle( EditorStyles.label );
            styleUnit = new GUIStyle( EditorStyles.label );
            styleFloat = new GUIStyle( EditorStyles.numberField );
            stylePopup = new GUIStyle( EditorStyles.popup );
            styleEnum = new GUIStyle( EditorStyles.popup );
            //styleFloat.alignment = TextAnchor.MiddleRight;
            //styleFloat.fixedWidth = 100;
            //styleFloat.stretchWidth = true;
            //styleFloat.contentOffset = new Vector2( styleFloat.contentOffset.x + 50, styleFloat.contentOffset.y );
            //GUILayout.Width( 60 );
            styleLabel.fixedWidth = 180;
            styleUnit.fixedWidth = 65;
        }

        protected void EndInspectorGUI()
        {
            if( GUI.changed || userChanges )
            {
                EditorUtility.SetDirty( target );
            }
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }

        protected void KeepChanges()
        {
            userChanges = true;
            //EditorUtility.SetDirty( target );
        }
    }
}
