using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 1591 // undocumented XML code warning

#if UNITY_EDITOR && !AUDIO_TOOLKIT_DEMO

namespace ClockStone
{
    public static class AudioLog
    {
        static public LinkedList<LogData> logData;

        static public Action onLogUpdated;

        public abstract class LogData
        {
            public float time;
        }

        public abstract class LogData_AudioID : LogData
        {
            public string audioID;
            public string category;
            public Vector3 position;
            public GameObject parentObject;
            public GameObject gizmoDummy;
            public string parentObjectName;
            public string clipName;
        }

        public class LogData_PlayClip : LogData_AudioID
        {
            public float volume;
            public float startTime;
            public float delay;
            public float scheduledDspTime;
            public float pitch;
        }

        public class LogData_Stop : LogData_AudioID
        {

        }

        public class LogData_Destroy : LogData_AudioID
        {

        }

        public class LogData_SkippedPlay : LogData_AudioID
    {
            public string reasonForSkip;
            public float volume;
            public float startTime;
            public float delay;
            public float scheduledDspTime;
        }

        static AudioLog()
        {
            logData = new LinkedList<LogData>();
            _OnLogUpdated();
        }

        public static void Clear()
        {
            foreach( var log in logData )
            {
                var a = log as LogData_AudioID;
                if( a.gizmoDummy )
                {
                    GameObject.DestroyImmediate( a.gizmoDummy );
                }
            }
            logData.Clear();
            _OnLogUpdated();
        }

        public static void Log( LogData playClipData )
        {
            playClipData.time = Time.time;

            if ( logData.Count >= 1024 )
            {
                logData.RemoveLast();
            }

            logData.AddFirst( playClipData );

            _OnLogUpdated();

        }

        private static void _OnLogUpdated()
        {
            if ( onLogUpdated != null )
            {
                onLogUpdated.Invoke();
            }
        }
    }
}

#endif