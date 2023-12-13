using System    ;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR // workaround for strange compiler error during build process

namespace UnityEditor
{
    public static class AudioUtility
    {
#if UNITY_2020_2_OR_NEWER
        const string methodName_PlayClip = "PlayPreviewClip";
        const string methodName_StopClip = "StopAllPreviewClips";
        const string methodName_PauseClip = "PausePreviewClip";
        const string methodName_ResumeClip = "ResumePreviewClip";
        const string methodName_IsClipPlaying = "IsPreviewClipPlaying";
#else
        const string methodName_PlayClip = "PlayClip";
        const string methodName_StopClip = "StopClip";
        const string methodName_PauseClip = "PauseClip";
        const string methodName_ResumeClip = "ResumeClip";
        const string methodName_IsClipPlaying = "IsClipPlaying";
#endif

        public static void PlayClip( AudioClip clip, int startSample = 0, bool loop = false )
        {
            Type audioUtilClass = GetAudioUtilClass();
            if( audioUtilClass == null ) return;
            MethodInfo method = audioUtilClass.GetMethod(
                methodName_PlayClip,
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[ ] {
                    typeof(AudioClip),
                    typeof(int), // int startSample
                    typeof(bool) // bool loop
                },
                null
            );
            if( method == null )
            {
                ThrowUnityUpgradeError();
                return;
            }
            method.Invoke(
                null,
                new object[ ] {
                    clip,
                    startSample,
                    loop
                }
            );
        }

        public static void StopClip( AudioClip clip )
        {
            Type audioUtilClass = GetAudioUtilClass();
            if( audioUtilClass == null ) return;
#if UNITY_2020_2_OR_NEWER
            MethodInfo method = audioUtilClass.GetMethod(
                methodName_StopClip,
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[ ] {
            },
            null
            );
#else
            MethodInfo method = audioUtilClass.GetMethod(
            methodName_StopClip,
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip)
            },
            null
            );
#endif
            if( method == null )
            {
                ThrowUnityUpgradeError();
                return;
            }
#if UNITY_2020_2_OR_NEWER
            method.Invoke(
                null,
                new object[ ] {
            }
            );
#else
            method.Invoke(
                null,
                new object[ ] {
                clip
            }
            );
#endif
        }

        public static void PauseClip( AudioClip clip )
        {
            Type audioUtilClass = GetAudioUtilClass();
            if( audioUtilClass == null ) return;
            MethodInfo method = audioUtilClass.GetMethod(
                methodName_PauseClip,
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[ ] {
                typeof(AudioClip)
            },
            null
            );
            if( method == null )
            {
                ThrowUnityUpgradeError();
                return;
            }
            method.Invoke(
                null,
                new object[ ] {
                clip
            }
            );
        }

        public static void ResumeClip( AudioClip clip )
        {
            Type audioUtilClass = GetAudioUtilClass();
            if( audioUtilClass == null ) return;
            MethodInfo method = audioUtilClass.GetMethod(
                methodName_ResumeClip,
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[ ] {
                typeof(AudioClip)
            },
            null
            );
            if( method == null )
            {
                ThrowUnityUpgradeError();
                return;
            }
            method.Invoke(
                null,
                new object[ ] {
                clip
            }
            );
        }

        public static bool IsClipPlaying( AudioClip clip )
        {
            Type audioUtilClass = GetAudioUtilClass();
            if( audioUtilClass == null ) return false;
#if UNITY_2020_2_OR_NEWER
            MethodInfo method = audioUtilClass.GetMethod(
                methodName_IsClipPlaying,
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
            },
            null
            );
#else
            MethodInfo method = audioUtilClass.GetMethod(
                methodName_IsClipPlaying,
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip)
            },
            null
            );
#endif
            if( method == null )
            {
                ThrowUnityUpgradeError();
                return false;
            }
#if UNITY_2020_2_OR_NEWER
            return (bool)method.Invoke(
            null,
            new object[] {
            }
            );
#else
            return (bool)method.Invoke(
            null,
            new object[] {
                clip
            }
            );
#endif
        }

        static bool unityUpgradeErrorThrown = false;

        static Type GetAudioUtilClass()
        {
            Assembly unityEditorAssembly = typeof( AudioImporter ).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType( "UnityEditor.AudioUtil" );
            if( audioUtilClass == null )
            {
                ThrowUnityUpgradeError();

            }
            return audioUtilClass;
        }

        private static void ThrowUnityUpgradeError()
        {
            if( !unityUpgradeErrorThrown )
            {
                Debug.LogWarning( "Internal AudioToolkit error: UnityEditor.AudioUtil has changed (only relevant while in Unity Editor)" );
                unityUpgradeErrorThrown = true;
            }
        }
    }
}

#endif