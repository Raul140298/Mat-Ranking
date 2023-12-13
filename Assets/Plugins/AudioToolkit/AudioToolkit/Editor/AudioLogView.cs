#if UNITY_EDITOR // Unity bug workaround: this way this file can be in subdirectorey of Standard Assets

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ClockStone
{
    public class AudioLogView : EditorWindow
    {
        [MenuItem( "Window/Audio Toolkit/Log" )]
        static void ShowWindow()
        {
            EditorWindow.GetWindow( typeof( AudioLogView ), false, "Audio Log" );
        }

        static Vector2 _scrollPos;

#if AUDIO_TOOLKIT_DEMO
    void OnGUI()
    {
         EditorGUILayout.LabelField( "Audio Log is not available in the FREE version of Audio Toolkit. Please buy the full version." );
    }
#else
        bool showStopEvents = true;
        string filter;

        const float defaultColumnWidth = 120;
        const float audioIDColumnWidth = 160;
        const float timeColumnWidth = 60;
        const float typeColumnWidth = 50;
        const float nameColumnWidth = 200;
        const float categoryColumnWidth = 100;
        const float parentColumnWidth = 120;

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if( GUILayout.Button( "Clear", GUILayout.Width( 120 ) ) )
            {
                AudioLog.Clear();
            }

            showStopEvents = GUILayout.Toggle( showStopEvents, "Show Stop Events" );
            GUILayout.FlexibleSpace();
            //filter = EditorGUILayout.TextField( "               audioID filter:", filter, GUILayout.Width( 300 ) );
            //EditorGUILayout.LabelField( "audioID filter:" );

            var filterHasChanged = UIUtility.SearchFieldGUI( 300, ref filter );
           
            EditorGUILayout.EndHorizontal();

            // header

            GUIStyle headerStyle = new GUIStyle( EditorStyles.boldLabel );

            

            // data

            AudioLog.LogData_PlayClip loggedClip;

            _scrollPos = EditorGUILayout.BeginScrollView( _scrollPos );

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField( "time", headerStyle, GUILayout.Width( timeColumnWidth ) );
            EditorGUILayout.LabelField( "type", headerStyle, GUILayout.Width( typeColumnWidth ) );
            EditorGUILayout.LabelField( "audioID", headerStyle, GUILayout.Width( audioIDColumnWidth ) );
            EditorGUILayout.LabelField( "clipName", headerStyle, GUILayout.Width( nameColumnWidth ) );
            EditorGUILayout.LabelField( "category", headerStyle, GUILayout.Width( categoryColumnWidth ) );
            EditorGUILayout.LabelField( "volume", headerStyle, GUILayout.Width( timeColumnWidth ) );
            EditorGUILayout.LabelField( "pitch", headerStyle, GUILayout.Width( timeColumnWidth ) );
            EditorGUILayout.LabelField( "startTime", headerStyle, GUILayout.Width( timeColumnWidth ) );
            EditorGUILayout.LabelField( "schedule", headerStyle, GUILayout.Width( timeColumnWidth ) );
            EditorGUILayout.LabelField( "delay", headerStyle, GUILayout.Width( timeColumnWidth ) );
            EditorGUILayout.LabelField( "parent", headerStyle, GUILayout.Width( parentColumnWidth ) );
            EditorGUILayout.LabelField( "worldPos", headerStyle, GUILayout.Width( defaultColumnWidth ) );

            EditorGUILayout.EndHorizontal();

            foreach( var log in AudioLog.logData )
            {
                if( !string.IsNullOrEmpty( filter ) )
                {
                    var log_audioID = log as AudioLog.LogData_AudioID;
                    var filterLower = filter.ToLowerInvariant();
                    if( !log_audioID.audioID.ToLowerInvariant().Contains( filterLower ) )
                    {
                        continue;
                    }
                }
                EditorGUILayout.BeginHorizontal();

                loggedClip = log as AudioLog.LogData_PlayClip;
                if( loggedClip != null )
                {
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", loggedClip.time ), GUILayout.Width( timeColumnWidth ) );
                    EditorGUILayout.LabelField( "PLAY", GUILayout.Width( typeColumnWidth ) );
                    DrawWithFit( loggedClip.audioID, audioIDColumnWidth );
                    DrawWithFit( loggedClip.clipName, nameColumnWidth );
                    EditorGUILayout.LabelField( loggedClip.category, GUILayout.Width( categoryColumnWidth ) );
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", loggedClip.volume ), GUILayout.Width( timeColumnWidth ) );
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", loggedClip.pitch ), GUILayout.Width( timeColumnWidth ) );
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", loggedClip.startTime ), GUILayout.Width( timeColumnWidth ) );
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", loggedClip.scheduledDspTime ), GUILayout.Width( timeColumnWidth ) );
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", loggedClip.delay ), GUILayout.Width( timeColumnWidth ) );
                    DrawPositionAndParent( log as AudioLog.LogData_AudioID );
                }

                var skippedClip = log as AudioLog.LogData_SkippedPlay;
                if( skippedClip != null )
                {
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", skippedClip.time ), GUILayout.Width( timeColumnWidth ) );
                    EditorGUILayout.LabelField( "SKIP", GUILayout.Width( typeColumnWidth ) );
                    DrawWithFit( skippedClip.audioID, audioIDColumnWidth );
                    EditorGUILayout.LabelField( skippedClip.reasonForSkip, GUILayout.Width( nameColumnWidth ) );
                    EditorGUILayout.LabelField( skippedClip.category, GUILayout.Width( categoryColumnWidth ) );
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", skippedClip.volume ), GUILayout.Width( timeColumnWidth ) );
                    EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", skippedClip.startTime ), GUILayout.Width( timeColumnWidth ) );
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", skippedClip.scheduledDspTime ), GUILayout.Width( timeColumnWidth ) );
                    EditorGUILayout.LabelField( string.Format( "{0:0.00}", skippedClip.delay ), GUILayout.Width( timeColumnWidth ) );
                    DrawPositionAndParent( log as AudioLog.LogData_AudioID );
                }

                if( showStopEvents )
                {
                    var stopClip = log as AudioLog.LogData_Stop;
                    if( stopClip != null )
                    {
                        EditorGUILayout.LabelField( string.Format( "{0:0.00}", stopClip.time ), GUILayout.Width( timeColumnWidth ) );
                        EditorGUILayout.LabelField( "STOP", GUILayout.Width( typeColumnWidth ) );
                        DrawWithFit( stopClip.audioID, audioIDColumnWidth );
                        DrawWithFit( stopClip.clipName, nameColumnWidth );
                        EditorGUILayout.LabelField( stopClip.category, GUILayout.Width( categoryColumnWidth ) );
                        EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                        EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                        EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                        EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                        EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                        DrawPositionAndParent( log as AudioLog.LogData_AudioID );
                    }

                    var destroyClip = log as AudioLog.LogData_Destroy;
                    if( destroyClip != null )
                    {
                        EditorGUILayout.LabelField( string.Format( "{0:0.00}", destroyClip.time ), GUILayout.Width( timeColumnWidth ) );
                        EditorGUILayout.LabelField( "DESTROY", GUILayout.Width( typeColumnWidth ) );
                        DrawWithFit( destroyClip.audioID, audioIDColumnWidth );
                        DrawWithFit( destroyClip.clipName, nameColumnWidth );
                        EditorGUILayout.LabelField( destroyClip.category, GUILayout.Width( categoryColumnWidth ) );
                        EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                        EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                        EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                        EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                        EditorGUILayout.LabelField( "", GUILayout.Width( timeColumnWidth ) );
                        DrawPositionAndParent( log as AudioLog.LogData_AudioID );
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private static void DrawWithFit( string txt, float width )
        {
            EditorGUILayout.LabelField( FitLabelTextInWidth( txt, width ), GUILayout.Width( width ) );
        }

        private static string FitLabelTextInWidth( string txt, float width  )
        {
            string txt_out;
            int cutPos = txt.Length;
            int cutLength = 0;
            int charWidth = 8;

            for(;;)
            {
                txt_out = txt.Substring( 0, cutPos );
                if( cutLength > 0 )
                {
                    txt_out += "[…]";
                    txt_out += txt.Substring( txt.Length - cutPos, cutPos );
                }
                var content = new GUIContent( txt_out );
                var size = GUI.skin.label.CalcSize( content );
                float tooMuch = size.x - width;
                if( tooMuch > 0 )
                {
                    cutLength += Mathf.RoundToInt( tooMuch / charWidth ) + 1;
                    cutPos = ( txt.Length - cutLength ) / 2;
                }
                else
                    break;
            } 
            return txt_out;
        }

        private static void DrawPositionAndParent( AudioLog.LogData_AudioID log )
        {
            if( log == null ) return;
            if( log.parentObject )
            {
                if( GUILayout.Button( log.parentObjectName, GUILayout.Width( parentColumnWidth ) ) )
                {
                    EditorGUIUtility.PingObject( log.parentObject );
                }
            }
            else
            {
                EditorGUILayout.LabelField( "", GUILayout.Width( parentColumnWidth ) );
            }

            if( GUILayout.Button( string.Format( "{0:0.0} / {1:0.0} / {2:0.0}", log.position.x, log.position.y, log.position.z ), GUILayout.Width( defaultColumnWidth ) ) )
            {
                if( !log.gizmoDummy )
                {
                    log.gizmoDummy = new GameObject( "*" + log.audioID + "*" );
                    log.gizmoDummy.transform.position = log.position;
                }
                EditorGUIUtility.PingObject( log.gizmoDummy );
            }
        }

        void OnNewLogEntry()
        {
            Repaint();
        }

        void OnEnable()
        {
            AudioLog.onLogUpdated += OnNewLogEntry;
        }

        void OnDisable()
        {
            AudioLog.onLogUpdated -= OnNewLogEntry;

        }
#endif
    }
}
#endif