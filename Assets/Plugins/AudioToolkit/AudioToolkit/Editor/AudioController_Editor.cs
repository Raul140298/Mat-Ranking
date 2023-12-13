using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ClockStone
{
    [CustomEditor( typeof( AudioController ) )]
    public partial class AudioController_Editor : EditorEx
    {
        AudioController AC;

        int currentCategoryIndex
        {
            get
            {
                return AC._currentInspectorSelection.currentCategoryIndex;
            }
            set
            {
                AC._currentInspectorSelection.currentCategoryIndex = value;
            }
        }
        int currentItemIndex
        {
            get
            {
                return AC._currentInspectorSelection.currentItemIndex;
            }
            set
            {
                if ( value != AC._currentInspectorSelection.currentItemIndex )
                {
                    AC._currentInspectorSelection.currentItemIndex = value;
                }
            }
        }

        int currentSubitemIndex
        {
            get
            {
                return AC._currentInspectorSelection.currentSubitemIndex;
            }
            set
            {
                AC._currentInspectorSelection.currentSubitemIndex = value;
            }
        }

        int currentPlaylistEntryIndex
        {
            get
            {
                return AC._currentInspectorSelection.currentPlaylistEntryIndex;
            }
            set
            {
                AC._currentInspectorSelection.currentPlaylistEntryIndex = value;
            }
        }

        int currentPlaylistIndex
        {
            get
            {
                return AC._currentInspectorSelection.currentPlaylistIndex;
            }
            set
            {
                AC._currentInspectorSelection.currentPlaylistIndex = value;
            }
        }

        public static bool globalFoldout = true;
        public static bool playlistFoldout = true;
        public static bool musicFoldout = true;
        public static bool categoryFoldout = true;
        public static bool itemFoldout = true;
        public static bool subitemFoldout = true;
        public static bool? detectAudioItemGroup = null;

        private static AudioItem _clipBoardItem = null;

        GUIStyle foldoutStyle;
        GUIStyle centeredTextStyle;
        GUIStyle popupStyleColored;
        GUIStyle styleChooseItem;
        GUIStyle textAttentionStyle;
        GUIStyle textAttentionStyleLabel;
        GUIStyle textInfoStyleLabel;
        GUIStyle boxStyle;

        int lastCategoryIndex = -1;
        int lastItemIndex = -1;
        int lastSubItemIndex = -1;

        AudioCategory currentCategory
        {
            get
            {
                if ( currentCategoryIndex < 0 || AC.AudioCategories == null || currentCategoryIndex >= AC.AudioCategories.Length )
                {
                    return null;
                }
                return AC.AudioCategories[ currentCategoryIndex ];
            }
        }
        AudioItem currentItem
        {
            get
            {
                AudioCategory curCategory = currentCategory;

                if ( currentCategory == null )
                {
                    return null;
                }

                if ( currentItemIndex < 0 || curCategory.AudioItems == null || currentItemIndex >= curCategory.AudioItems.Length )
                {
                    return null;
                }
                return currentCategory.AudioItems[ currentItemIndex ];
            }
        }

        AudioSubItem currentSubItem
        {
            get
            {
                AudioItem curItem = currentItem;

                if ( curItem == null )
                {
                    return null;
                }

                if ( currentSubitemIndex < 0 || curItem.subItems == null || currentSubitemIndex >= curItem.subItems.Length )
                {
                    return null;
                }
                return curItem.subItems[ currentSubitemIndex ];
            }
        }

        public int currentCategoryCount
        {
            get
            {
                if ( AC.AudioCategories != null )
                {
                    return AC.AudioCategories.Length;
                }
                else
                    return 0;
            }
        }

        public int currentItemCount
        {
            get
            {
                if ( currentCategory != null )
                {
                    if ( currentCategory.AudioItems != null )
                    {
                        return currentCategory.AudioItems.Length;
                    }
                    return 0;
                }
                else
                    return 0;
            }
        }

        public int currentSubItemCount
        {
            get
            {
                if ( currentItem != null )
                {
                    if ( currentItem.subItems != null )
                    {
                        return currentItem.subItems.Length;
                    }
                    return 0;
                }
                else
                    return 0;
            }
        }

        const string _playWithInspectorNotice = "Volume and pitch of audios are only correct when played during playmode. You can ignore the following Unity warning (if any).";
        const string _playNotSupportedOnMac = "On MacOS playing audios is only supported during play mode.";
        const string _nameForNewCategoryEntry = "!!! Enter Unique Category Name Here !!!";
        const string _nameForNewAudioItemEntry = "!!! Enter Unique Audio ID Here !!!";
        const string _nameForNewPlaylist = "!!! Enter Unique Playlist Name here !!!";

        //public void OnEnable()
        //{

        //}

        protected override void LogUndo( string label )
        {
            //Debug.Log( "Undo: " + label );
            UnityEditor.Undo.RecordObject( AC, "AudioToolkit: " + label );
        }

        public new void SetStyles()
        {
            base.SetStyles();

            foldoutStyle = new GUIStyle( EditorStyles.foldout );

            //var foldoutColor = new UnityEngine.Color( 0.3f, 0.75f, 0.75f );
            //var foldoutColor = new UnityEngine.Color( 0.1f, 0.6f, 0.05f );
            var foldoutColor = new UnityEngine.Color( 0.0f, 0.0f, 0.2f );

            //foldoutStyle.normal.background = EditorStyles.boldLabel.onNormal.background;
            //foldoutStyle.focused.background = EditorStyles.boldLabel.onNormal.background;
            //foldoutStyle.active.background = EditorStyles.boldLabel.onNormal.background;
            //foldoutStyle.hover.background = EditorStyles.boldLabel.onNormal.background;

            foldoutStyle.onNormal.background = EditorStyles.boldLabel.onNormal.background;
            foldoutStyle.onFocused.background = EditorStyles.boldLabel.onNormal.background;
            foldoutStyle.onActive.background = EditorStyles.boldLabel.onNormal.background;
            foldoutStyle.onHover.background = EditorStyles.boldLabel.onNormal.background;


            foldoutStyle.normal.textColor = foldoutColor;
            foldoutStyle.focused.textColor = foldoutColor;
            foldoutStyle.active.textColor = foldoutColor;
            foldoutStyle.hover.textColor = foldoutColor;
            foldoutStyle.fixedWidth = 1000;

            //foldoutStyle.onNormal.textColor = foldoutColor;
            //foldoutStyle.onFocused.textColor = foldoutColor;
            //foldoutStyle.onActive.textColor = foldoutColor;
            //foldoutStyle.onHover.textColor = foldoutColor;

            centeredTextStyle = new GUIStyle( EditorStyles.label );
            centeredTextStyle.alignment = TextAnchor.UpperCenter;
            centeredTextStyle.stretchWidth = true;


            popupStyleColored = new GUIStyle( stylePopup );
            styleChooseItem = new GUIStyle( stylePopup );
            styleFloatIndi = new GUIStyle( styleFloat );

            bool isDarkSkin = popupStyleColored.normal.textColor.grayscale > 0.5f;
            boxStyle = new GUIStyle( EditorStyles.label );

            boxStyle.wordWrap = true;
            boxStyle.alignment = TextAnchor.MiddleCenter;

            if ( isDarkSkin )
            {
                popupStyleColored.normal.textColor = new Color( 0.9f, 0.9f, 0.5f );
            }
            else
                popupStyleColored.normal.textColor = new Color( 0.6f, 0.1f, 0.0f );

            if ( isDarkSkin )
            {
                styleFloatIndi.normal.textColor = new Color( 0.9f, 0.9f, 0.5f );
                styleFloatIndi.focused.textColor = new Color( 0.9f, 0.9f, 0.5f );
            }
            else
            {
                styleFloatIndi.normal.textColor = new Color( 0.6f, 0.1f, 0.0f );
                styleFloatIndi.focused.textColor = new Color( 0.6f, 0.1f, 0.0f );
            }


            textAttentionStyle = new GUIStyle( EditorStyles.textField );

            if ( isDarkSkin )
            {
                textAttentionStyle.normal.textColor = new Color( 1, 0.3f, 0.3f );
            }
            else
                textAttentionStyle.normal.textColor = new Color( 1, 0f, 0f );

            textAttentionStyleLabel = new GUIStyle( EditorStyles.label );

            if ( isDarkSkin )
            {
                textAttentionStyleLabel.normal.textColor = new Color( 1, 0.3f, 0.3f );
            }
            else
                textAttentionStyleLabel.normal.textColor = new Color( 1, 0f, 0f );

            textInfoStyleLabel = new GUIStyle( EditorStyles.label );

            if ( isDarkSkin )
            {
                textInfoStyleLabel.normal.textColor = new Color( 0.4f, 0.4f, 0.4f );
            }
            else
                textInfoStyleLabel.normal.textColor = new Color( 0.6f, 0.6f, 0.6f );
        }

        bool duplicatedItemNameEntered = false;

        public override void OnInspectorGUI()
        {
            SetStyles();

            BeginInspectorGUI();

            Event evt = Event.current;

            AC = (AudioController) target;

            _ValidateCurrentCategoryIndex();
            _ValidateCurrentItemIndex();
            _ValidateCurrentSubItemIndex();

            if ( lastCategoryIndex != currentCategoryIndex ||
                lastItemIndex != currentItemIndex ||
                lastSubItemIndex != currentSubitemIndex )
            {
                GUIUtility.keyboardControl = 0; // workaround for Unity weirdness not changing the value of a focused GUI element when changing a category/item
                lastCategoryIndex = currentCategoryIndex;
                lastItemIndex = currentItemIndex;
                lastSubItemIndex = currentSubitemIndex;
            }



            EditorGUILayout.Space();

            if ( globalFoldout = EditorGUILayout.Foldout( globalFoldout, "Global Audio Settings", foldoutStyle ) )
            {
                bool currentlyAdditionalController = AC.isAdditionalAudioController;

                bool changed = EditBool( ref currentlyAdditionalController, "Additional Audio Controller", "A scene can contain multiple AudioControllers. All but the main AudioController must be marked as 'additional'." );
                if ( changed )
                {
                    AC.isAdditionalAudioController = currentlyAdditionalController;
                }
                EditBool( ref AC.Persistent, "Persist Scene Loading", "A non-persisting AudioController will get destroyed when loading the next scene." );
                EditBool( ref AC.UnloadAudioClipsOnDestroy, "Unload Audio On Destroy", "This option forces Unity to unload all AudioClips from memory which are referenced by this AudioController if the controller gets destroyed (e.g. when loading a new scene and the AudioController is not persistent). \n\n" +
                    "Use this option in combination with additional none-persistent AudioControllers to keep only those audios in memory that are used by the current scene. Use a primary persistent AudioController for all global audio that is used throughout all scenes."
                    );

                bool currentlyDisabled = AC.DisableAudio;

                changed = EditBool( ref currentlyDisabled, "Disable Audio", "Disables all audio" );
                if ( changed )
                {
                    AC.DisableAudio = currentlyDisabled;
                    if ( currentlyDisabled && AudioController.DoesInstanceExist() )
                    {
                        AudioController.StopAll();
                    }
                }

                float vol = AC.Volume;

                EditFloat01( ref vol, "Volume", "%" );

                AC.Volume = vol;

                EditPrefab( ref AC.AudioObjectPrefab, "Audio Object Prefab", "You must specify a prefab here that will get instantiated for each played audio. This prefab must contain the following components: AudioSource, AudioObject, PoolableObject." );
                EditBool( ref AC.UsePooledAudioObjects, "Use Pooled AudioObjects", "Pooling increases performance when playing many audio files. Strongly recommended particularly on mobile platforms." );
                EditBool( ref AC.PlayWithZeroVolume, "Play With Zero Volume", "If disabled Play() calls with a volume of zero will not create an AudioObject." );

                EditBool( ref AC.EqualPowerCrossfade, "Equal-power crossfade", "Unfortunatly not 100% correct due to unknown volume formulas used by Unity" );
            }

            VerticalSpace();

            // music specific
            if ( musicFoldout = EditorGUILayout.Foldout( musicFoldout, "Music / Ambience Settings", foldoutStyle ) )
            {
                EditBool( ref AC.specifyCrossFadeInAndOutSeperately, "Separate crossfade in/out", "Allows to specify a separate fade-in and out value for all music and ambience sounds" );
                if ( AC.specifyCrossFadeInAndOutSeperately )
                {
                    float v_in = AC.musicCrossFadeTime_In;
                    EditFloat( ref v_in, "   Music Crossfade-in Time", "sec" ); AC.musicCrossFadeTime_In = v_in;

                    float v_out = AC.musicCrossFadeTime_Out;
                    EditFloat( ref v_out, "   Music Crossfade-out Time", "sec" ); AC.musicCrossFadeTime_Out = v_out;

                    v_in = AC.ambienceSoundCrossFadeTime_In;
                    EditFloat( ref v_in, "   Ambience Crossfade-in Time", "sec" ); AC.ambienceSoundCrossFadeTime_In = v_in;

                    v_out = AC.ambienceSoundCrossFadeTime_Out;
                    EditFloat( ref v_out, "   Ambience Crossfade-out Time", "sec" ); AC.ambienceSoundCrossFadeTime_Out = v_out;
                }
                else
                {
                    EditFloat( ref AC.musicCrossFadeTime, "Music Crossfade Time", "sec" );
                    EditFloat( ref AC.ambienceSoundCrossFadeTime, "Ambience Crossfade Time", "sec" );
                }
            }

            VerticalSpace();

            // playlist specific
            if ( playlistFoldout = EditorGUILayout.Foldout( playlistFoldout, "Playlist Settings", foldoutStyle ) )
            {
                EditorGUILayout.BeginHorizontal();
                var playlistNames = GetPlaylistNames();

                currentPlaylistIndex = PopupWithStyle( "Playlist", currentPlaylistIndex, playlistNames, popupStyleColored, "List of playlists, click on '+' to add a new playlist", false );

                if ( GUILayout.Button( "+", GUILayout.Width( 25 ) ) && AC.musicPlaylists != null && AC.musicPlaylists.Length > 0 )
                {
                    LogUndo( "Add Playlist" );
                    ArrayHelper.AddArrayElement( ref AC.musicPlaylists, new Playlist { name = _nameForNewPlaylist } );
                    currentPlaylistIndex = AC.musicPlaylists.Length - 1;
                    KeepChanges();
                }

                if ( GUILayout.Button( "-", GUILayout.Width( 25 ) ) && AC.musicPlaylists != null && AC.musicPlaylists.Length > 0 )
                {
                    if ( AC.musicPlaylists.Length > 1 )
                    {
                        LogUndo( "Del Playlist" );
                        ArrayHelper.DeleteArrayElement( ref AC.musicPlaylists, currentPlaylistIndex );
                        currentPlaylistIndex = Mathf.Clamp( currentPlaylistIndex - 1, 0, AC.musicPlaylists.Length - 1 );
                        KeepChanges();
                    }
                    else
                    {
                        AC.musicPlaylists[ 0 ] = new Playlist();
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditString( ref AC.musicPlaylists[ currentPlaylistIndex ].name, "Playlist Name", AC.musicPlaylists[ currentPlaylistIndex ].name == _nameForNewPlaylist ? textAttentionStyle : null );


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                var playlistEntryNames = GetPlaylistEntryNames();
                currentPlaylistEntryIndex = Popup( "Playlist Entry", currentPlaylistEntryIndex, playlistEntryNames, "List of audioIDs, click on 'add to playlist' to add audio items", false );
                GUI.enabled = playlistEntryNames.Length > 0;
                if ( GUILayout.Button( "Up", GUILayout.Width( 35 ) ) && AC.musicPlaylists != null && AC.musicPlaylists.Length > 0 )
                {
                    LogUndo( "Move Playlist entry" );
                    if ( SwapArrayElements( AC.musicPlaylists[ currentPlaylistIndex ].playlistItems, currentPlaylistEntryIndex, currentPlaylistEntryIndex - 1 ) )
                    {
                        currentPlaylistEntryIndex--;
                        KeepChanges();
                    }
                }
                if ( GUILayout.Button( "Dwn", GUILayout.Width( 40 ) ) && AC.musicPlaylists != null && AC.musicPlaylists.Length > 0 )
                {
                    LogUndo( "Move Playlist entry" );
                    if ( SwapArrayElements( AC.musicPlaylists[ currentPlaylistIndex ].playlistItems, currentPlaylistEntryIndex, currentPlaylistEntryIndex + 1 ) )
                    {
                        currentPlaylistEntryIndex++;
                        KeepChanges();
                    }
                }
                if ( GUILayout.Button( "-", GUILayout.Width( 25 ) ) && AC.musicPlaylists != null && AC.musicPlaylists.Length > 0 )
                {
                    LogUndo( "Del Playlist entry" );
                    ArrayHelper.DeleteArrayElement( ref AC.musicPlaylists[ currentPlaylistIndex ].playlistItems, currentPlaylistEntryIndex );
                    currentPlaylistEntryIndex = Mathf.Clamp( currentPlaylistEntryIndex - 1, 0, AC.musicPlaylists.Length - 1 );
                    KeepChanges();
                }

                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();

                string itemToAdd = _ChooseItem( "Add to Playlist" );
                if ( !string.IsNullOrEmpty( itemToAdd ) )
                {
                    LogUndo( "Add Playlist entry" );
                    AddToPlayList( playlistNames[ currentPlaylistIndex ], itemToAdd );
                }

                EditBool( ref AC.loopPlaylist, "Loop Playlist" );
                EditBool( ref AC.shufflePlaylist, "Shuffle Playlist", "Enables random playback of music playlists. Takes care that the same audio will not get played again too early" );
                EditBool( ref AC.crossfadePlaylist, "Crossfade Playlist" );
                EditFloat( ref AC.delayBetweenPlaylistTracks, "Delay Betw. Playlist Tracks", "sec" );

                GUI.enabled = _IsAudioControllerInPlayMode() && playlistEntryNames.Length > 0;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label( "" );
                if ( GUILayout.Button( "Play", GUILayout.Width( 60 ) ) )
                {
                    AudioController.PlayMusicPlaylist( AC.musicPlaylists[ currentPlaylistIndex ].name );
                }
                EditorGUILayout.EndHorizontal();
                GUI.enabled = true;
            }

            VerticalSpace();

            int categoryCount = AC.AudioCategories != null ? AC.AudioCategories.Length : 0;
            currentCategoryIndex = Mathf.Clamp( currentCategoryIndex, 0, categoryCount - 1 );

            if ( categoryFoldout = EditorGUILayout.Foldout( categoryFoldout, "Category Settings", foldoutStyle ) )
            {

                // Audio Items 
                EditorGUILayout.BeginHorizontal();

                bool justCreatedNewCategory = false;

                var categoryNames = GetCategoryNames();

                int newCategoryIndex = PopupWithStyle( "Category", currentCategoryIndex, categoryNames, popupStyleColored );
                if ( GUILayout.Button( "+", GUILayout.Width( 30 ) ) )
                {
                    bool lastEntryIsNew = false;

                    if ( categoryCount > 0 )
                    {
                        lastEntryIsNew = AC.AudioCategories[ currentCategoryIndex ].Name == _nameForNewCategoryEntry;
                    }

                    if ( !lastEntryIsNew )
                    {
                        newCategoryIndex = AC.AudioCategories != null ? AC.AudioCategories.Length : 0;
                        ArrayHelper.AddArrayElement( ref AC.AudioCategories, new AudioCategory( AC ) );
                        AC.AudioCategories[ newCategoryIndex ].Name = _nameForNewCategoryEntry;
                        justCreatedNewCategory = true;
                        KeepChanges();
                    }
                }

                if ( GUILayout.Button( "-", GUILayout.Width( 30 ) ) && categoryCount > 0 )
                {

                    if ( currentCategoryIndex < AC.AudioCategories.Length - 1 )
                    {
                        newCategoryIndex = currentCategoryIndex;
                    }
                    else
                    {
                        newCategoryIndex = Mathf.Max( currentCategoryIndex - 1, 0 );
                    }
                    ArrayHelper.DeleteArrayElement( ref AC.AudioCategories, currentCategoryIndex );
                    KeepChanges();
                }

                EditorGUILayout.EndHorizontal();

                if ( newCategoryIndex != currentCategoryIndex )
                {
                    currentCategoryIndex = newCategoryIndex;
                    currentItemIndex = 0;
                    currentSubitemIndex = 0;
                    duplicatedItemNameEntered = false;
                    _ValidateCurrentItemIndex();
                    _ValidateCurrentSubItemIndex();
                }


                AudioCategory curCat = currentCategory;

                if ( curCat != null )
                {
                    if ( curCat.audioController == null )
                    {
                        curCat.audioController = AC;
                    }
                    if ( justCreatedNewCategory )
                    {
                        SetFocusForNextEditableField();
                    }
                    EditString( ref curCat.Name, "Name", curCat.Name == _nameForNewCategoryEntry ? textAttentionStyle : null );

                    float volTmp = curCat.Volume;
                    EditFloat01( ref volTmp, "Volume", " %" );
                    curCat.Volume = volTmp;

                    /*if ( Application.isPlaying )  // repaint consumes too much UI performance
                    {
                        volTmp = curCat.VolumeTotal;
                        GUI.enabled = false;
                        EditFloat01( ref volTmp, " -> Effective Volume", " %" );
                        GUI.enabled = true;
                        KeepChanges(); // otherwise inspector does not update every frame 
                    }*/

                    EditPrefab( ref curCat.AudioObjectPrefab, "Audio Object Prefab Override", "Use different Audio Object prefabs if you want to specify different parameters such as the volume rolloff etc. per category" );
                    EditPrefab( ref curCat.audioMixerGroup, "Audio Mixer Group", "You can specify a Unity 5 Audio Mixer Group here" );

                    int selectedParentCategoryIndex;

                    var catList = _GenerateCategoryListIncludingNone( out selectedParentCategoryIndex, curCat.parentCategory );

                    int newIndex = Popup( "Parent Category", selectedParentCategoryIndex, catList, "The effective volume of a category is multiplied with the volume of the parent category." );
                    if ( newIndex != selectedParentCategoryIndex )
                    {
                        KeepChanges();

                        if ( newIndex <= 0 )
                        {
                            curCat.parentCategory = null;
                        }
                        else
                            curCat.parentCategory = _GetCategory( catList[ newIndex ] );
                    }

                    int itemCount = currentItemCount;
                    _ValidateCurrentItemIndex();

                    EditorGUILayout.BeginHorizontal();

                    if ( GUILayout.Button( "Copy current AudioItem" ) )
                    {
                        _clipBoardItem = new AudioItem( currentItem );
                    }

                    if ( GUILayout.Button( "Paste AudioItem" ) )
                    {
                        if ( _clipBoardItem == null )
                            return;

                        AudioItem pasteItem = new AudioItem( _clipBoardItem );

                        if ( curCat.AudioItems != null )
                        {
                            bool duplicateFound = true;
                            while ( duplicateFound )
                            {
                                duplicateFound = false;
                                foreach ( AudioItem aItem in curCat.AudioItems )
                                {
                                    if ( aItem.Name == pasteItem.Name )
                                    {
                                        pasteItem.Name = aItem.Name + " Copy";
                                        duplicateFound = true;
                                    }
                                }

                            }
                        }

                        ArrayHelper.AddArrayElement( ref curCat.AudioItems, pasteItem );
                        currentItemIndex = curCat.AudioItems.Length - 1;
                    }

                    EditorGUILayout.EndHorizontal();

                    /*if ( GUILayout.Button( "Add all items in this category to playlist" ) )
                    {
                        for ( int i = 0; i < itemCount; i++ )
                        {
                            ArrayHelper.AddArrayElement( ref AC.musicPlaylist, curCat.AudioItems[i].Name );
                        }
                        currentPlaylistIndex = AC.musicPlaylist.Length - 1;
                        KeepChanges();
                    }*/



                    VerticalSpace();

                    AudioItem curItem = currentItem;

                    if ( itemFoldout = EditorGUILayout.Foldout( itemFoldout, "Audio Item Settings", foldoutStyle ) )
                    {
                        EditorGUILayout.BeginHorizontal();
                        if( detectAudioItemGroup == null )
                        {
                            detectAudioItemGroup = EditorPrefs.GetBool( "ATK_detectAudioItemGroup", true );
                        }
                        if ( GUILayout.Button( new GUIContent( "Add selected audio clips", "Adds all the currently selected audio filed to the category as new audio items. Hint:use inspector lock to select files."), EditorStyles.miniButton, GUILayout.Width( 200 ) ) )
                        {
                            AudioClip[ ] audioClips = GetSelectedAudioclips();
                            if ( audioClips.Length > 0 )
                            {
                                LogUndo( "Add selected audio clips" );
                                if( audioClips.Length == 1 || detectAudioItemGroup == false )
                                {
                                    foreach( var a in audioClips )
                                    {
                                        AddAudioItemFromClip( curCat, a );
                                    }
                                    currentItemIndex++;
                                } else
                                {
                                    var items = AudioItemFinder.FindAudioItemsFromClips( audioClips );
                                    foreach( var a in items )
                                    {
                                        var clips = a.audioClips;
                                        if( clips.Count > 1 )
                                        {
                                            AddAudioItemFromClips( curCat, clips, a.audioID );
                                        } else
                                        {
                                            AddAudioItemFromClip( curCat, clips[0] );
                                        }
                                        currentItemIndex++;
                                    }
                                }

                                KeepChanges();
                            }
                        }

                        var newdetectAudioItemGroup = EditorGUILayout.ToggleLeft( new GUIContent( "auto group", "Tries to detect groups of audio clips by searching for number indices at the end of the file name.\n" + 
                                                                                                               "e.g. 'FireShot 1' and 'FireShot 2' will create a single audio item with two subitems"), 
                                                                                detectAudioItemGroup.Value, GUILayout.Width( 100 ) );

                        if( newdetectAudioItemGroup != detectAudioItemGroup )
                        {
                            detectAudioItemGroup = newdetectAudioItemGroup;
                            EditorPrefs.SetBool( "ATK_detectAudioItemGroup", newdetectAudioItemGroup );
                        }
                        EditorGUILayout.EndHorizontal();

                        // AudioItems

                        EditorGUILayout.BeginHorizontal();

                        int newItemIndex = PopupWithStyle( "Item", currentItemIndex, GetItemNames(), popupStyleColored );
                        bool justCreatedNewItem = false;


                        if ( GUILayout.Button( "+", GUILayout.Width( 30 ) ) )
                        {
                            LogUndo( "Add Audio Item" );
                            bool lastEntryIsNew = false;

                            if ( itemCount > 0 )
                            {
                                lastEntryIsNew = curCat.AudioItems[ currentItemIndex ].Name == _nameForNewAudioItemEntry;
                            }

                            if ( !lastEntryIsNew )
                            {
                                newItemIndex = curCat.AudioItems != null ? curCat.AudioItems.Length : 0;
                                ArrayHelper.AddArrayElement( ref curCat.AudioItems );
                                curCat.AudioItems[ newItemIndex ].Name = _nameForNewAudioItemEntry;
                                justCreatedNewItem = true;
                                KeepChanges();
                            }
                        }

                        if ( GUILayout.Button( "-", GUILayout.Width( 30 ) ) && itemCount > 0 )
                        {
                            LogUndo( "Del Audio Item" );
                            if ( currentItemIndex < curCat.AudioItems.Length - 1 )
                            {
                                newItemIndex = currentItemIndex;
                            }
                            else
                            {
                                newItemIndex = Mathf.Max( currentItemIndex - 1, 0 );
                            }
                            ArrayHelper.DeleteArrayElement( ref curCat.AudioItems, currentItemIndex );
                            KeepChanges();
                        }



                        if ( newItemIndex != currentItemIndex )
                        {
                            currentItemIndex = newItemIndex;
                            currentSubitemIndex = 0;
                            duplicatedItemNameEntered = false;
                            _ValidateCurrentSubItemIndex();
                        }

                        curItem = currentItem;

                        EditorGUILayout.EndHorizontal();

                        if ( curItem != null )
                        {
                            GUILayout.BeginHorizontal();
                            if ( justCreatedNewItem )
                            {
                                SetFocusForNextEditableField();
                            }

                            bool isNewDummyName = curItem.Name == _nameForNewAudioItemEntry;

                            string originalName = curItem.Name;
                            string nameToChange = originalName;

                            if ( EditString( ref nameToChange, duplicatedItemNameEntered ? "Name duplicate" : "Name", duplicatedItemNameEntered ? textAttentionStyleLabel : styleLabel, isNewDummyName ? textAttentionStyle : null, "You must specify a unique name here (=audioID). This is the ID used in the script code to play this audio item." ) )
                            {
                                if ( !curCat.AudioItems.Any( x => x.Name == nameToChange ) )
                                {
                                    duplicatedItemNameEntered = false;
                                    curItem.Name = nameToChange;
                                    if ( !isNewDummyName )
                                    {
                                        _RenamePlaylistEntries( originalName, curItem.Name );
                                    }
                                }
                                else
                                {
                                    duplicatedItemNameEntered = true;
                                }
                            }
                            else
                            {
                                // info: while focus is still on input the EditString does not display nameToChange.
                                // if user inputs nameToChange again, then EditString returns false (incorrectly detecting no text change),
                                // and we need to check if the name is now valid
                                if ( duplicatedItemNameEntered )
                                {
                                    duplicatedItemNameEntered = curCat.AudioItems.Any( x => x.Name == nameToChange );
                                }
                            }


                            /*if ( GUILayout.Button( "Add to playlist" ) )
                            {
                                AddToPlayList( curItem.Name );
                            }*/

                            GUILayout.EndHorizontal();

                            int newItemCategoryIndex = Popup( "Move to Category", currentCategoryIndex, GetCategoryNames() );

                            if ( newItemCategoryIndex != currentCategoryIndex )
                            {
                                LogUndo( "Move Audio Item" );
                                var newCat = AC.AudioCategories[ newItemCategoryIndex ];
                                var oldCat = currentCategory;
                                ArrayHelper.AddArrayElement( ref newCat.AudioItems, curItem );
                                ArrayHelper.DeleteArrayElement( ref oldCat.AudioItems, currentItemIndex );
                                currentCategoryIndex = newItemCategoryIndex;
                                KeepChanges();
                                AC.InitializeAudioItems();
                                currentItemIndex = newCat.AudioItems.Length - 1;
                            }

                            if ( EditFloat01( ref curItem.Volume, "Volume", " %" ) )
                            {
                                _AdjustVolumeOfAllAudioItems( curItem, null );
                            }
                            EditFloat( ref curItem.Delay, "Delay", "sec", "Delays the playback" );

                            if ( EditFloat01( ref curItem.RandomVolume, "Random Volume", "±%" ) )
                            {
                                foreach ( AudioSubItem audioSubItem in curItem.subItems )
                                {
                                    if ( !audioSubItem.individualSettings.Contains( "RandomVolume" ) )
                                        audioSubItem.RandomVolume = curItem.RandomVolume;
                                }
                            }

                            if ( EditFloat( ref curItem.PitchShift, "Pitch Shift", "semitone" ) )
                            {
                                foreach ( AudioSubItem audioSubItem in curItem.subItems )
                                {
                                    if ( !audioSubItem.individualSettings.Contains( "PitchShift" ) )
                                        audioSubItem.PitchShift = curItem.PitchShift;
                                }
                            }

                            if ( EditFloat( ref curItem.RandomPitch, "Random Pitch", "±semitone" ) )
                            {
                                foreach ( AudioSubItem audioSubItem in curItem.subItems )
                                {
                                    if ( !audioSubItem.individualSettings.Contains( "RandomPitch" ) )
                                        audioSubItem.RandomPitch = curItem.RandomPitch;
                                }
                            }

                            if ( EditFloat( ref curItem.RandomDelay, "Random Delay", "sec" ) )
                            {
                                foreach ( AudioSubItem audioSubItem in curItem.subItems )
                                {
                                    if ( !audioSubItem.individualSettings.Contains( "RandomDelay" ) )
                                        audioSubItem.RandomDelay = curItem.RandomDelay;
                                }
                            }

                            EditFloat( ref curItem.MinTimeBetweenPlayCalls, "Min Time Between Play", "sec", "If the same audio item gets played multiple times within this time frame the playback is skipped. This can prevent unwanted audio artifacts." );
                            EditInt( ref curItem.MaxInstanceCount, "Max Instance Count", "", "Sets the maximum number of simultaneously playing audio files of this particular audio item. If the maximum number would be exceeded, the oldest playing audio gets stopped." );

                            EditBool( ref curItem.DestroyOnLoad, "Stop When Scene Changes", "If disabled, this audio item will continue playing if the current scene is unloaded. Note that if the audio is parented to another game object it will nevertheless stop playing if the parent object gets destroyed during the scene change." );

                            if ( (int) curItem.Loop == 3 ) // deprecated gapless looping
                            {
                                curItem.Loop = AudioItem.LoopMode.LoopSequence;
                                KeepChanges();
                            }

                            curItem.Loop = (AudioItem.LoopMode) EnumPopup( "Loop Mode", curItem.Loop, "The Loop mode determines how the audio subitems are looped. \n'LoopSubitem' means that the chosen sub-item will loop. \n'LoopSequence' means that one subitem is played after the other. In which order the subitems are chosen depends on the subitem pick mode." );

                            if ( curItem.Loop == AudioItem.LoopMode.LoopSequence ||
                                 curItem.Loop == AudioItem.LoopMode.PlaySequenceAndLoopLast ||
                                 curItem.Loop == AudioItem.LoopMode.IntroLoopOutroSequence )
                            {
                                EditInt( ref curItem.loopSequenceCount, "   Stop after subitems", "", "Playing will stop after this number of different subitems were played. Specify zero to play endlessly in LoopSequence mode or play all sub-items in <c>PlaySequenceAndLoopLast</c> and <c>IntroLoopOutroSequence</c> mode" );
                                EditFloat( ref curItem.loopSequenceOverlap, "   Overlap", "sec", "Positive values mean that subitems will play overlapping, negative values mean that a delay is inserted before playing the next subitem in the 'LoopSequence'." );
                                EditFloat( ref curItem.loopSequenceRandomDelay, "   Random Delay", "sec", "A random delay between 0 and this value will be added between two subsequent subitems. Can be combined with the 'Overlap' value." );
                                EditFloat01( ref curItem.loopSequenceRandomVolume, "   Random Volume", "±%", "A random volume value % will be added to each subitem played in the 'LoopSequence'. Will be combined with subitem random volume value." );
                                EditFloat( ref curItem.loopSequenceRandomPitch, "   Random Pitch", "±semitone", "A random pitch between 0 and this value will be added to each subitem played in the 'LoopSequence'. Will be combined with subitem random pitch value." );
                            }

                            EditBool( ref curItem.overrideAudioSourceSettings, "Override AudioSource Settings" );

                            if ( curItem.overrideAudioSourceSettings )
                            {
                                //EditorGUI.indentLevel++;
                                EditFloat01( ref curItem.spatialBlend, "   Spatial Blend", "%", "0% = 2D  100% = 3D" );
                                EditFloat( ref curItem.audioSource_MinDistance, "   Min Distance", "", "Overrides the 'Min Distance' parameter in the AudioSource settings of the AudioObject prefab (for 3d sounds)" );
                                EditFloat( ref curItem.audioSource_MaxDistance, "   Max Distance", "", "Overrides the 'Max Distance' parameter in the AudioSource settings of the AudioObject prefab (for 3d sounds)" );

                                //EditorGUI.indentLevel--;
                            }

                            if ( curItem.Loop == AudioItem.LoopMode.IntroLoopOutroSequence )
                            {
                                EditorGUI.BeginDisabledGroup( true );
                                curItem.SubItemPickMode = AudioPickSubItemMode.StartLoopSequenceWithFirst;
                            }
                            curItem.SubItemPickMode = (AudioPickSubItemMode) EnumPopup( "Pick Subitem Mode", curItem.SubItemPickMode, "Determines which subitem is chosen when the audio item is played." );
                            if ( curItem.Loop == AudioItem.LoopMode.IntroLoopOutroSequence )
                            {
                                EditorGUI.EndDisabledGroup();
                            }
                            //EditString( ref curItem.PlayAdditional, "Play Additional" );
                            //EditString( ref curItem.PlayInstead, "Play Instead" );

                            EditorGUILayout.BeginHorizontal();

                            GUILayout.Label( "" );

                            bool isItemNotLooping = ( curItem != null && curItem.Loop == AudioItem.LoopMode.DoNotLoop );

                            GUI.enabled = isItemNotLooping;

                            if ( GUILayout.Button( "Play", GUILayout.Width( 60 ) ) && curItem != null )
                            {
                                if ( _IsAudioControllerInPlayMode() )
                                {
                                    AudioController.Play( curItem.Name );
                                }
                                else
                                {
#if !UNITY_2019_OR_NEWER
                                    if ( Application.platform == RuntimePlatform.OSXEditor )
                                    {
                                        Debug.Log( _playNotSupportedOnMac );
                                    }
                                    else
#endif
                                    {
                                        previewAudioItem( curItem );
                                    }
                                }
                            }

                            GUI.enabled = true;


                            EditorGUILayout.EndHorizontal();

                            VerticalSpace();

                            int subItemCount = curItem.subItems != null ? curItem.subItems.Length : 0;
                            currentSubitemIndex = Mathf.Clamp( currentSubitemIndex, 0, subItemCount - 1 );
                            AudioSubItem subItem = currentSubItem;

                            if ( subitemFoldout = EditorGUILayout.Foldout( subitemFoldout, "Audio Sub-Item Settings", foldoutStyle ) )
                            {

                                //---------------- Drag and drop of AudioClips -----------------------------//
                                EditorGUILayout.BeginHorizontal();

                                Rect audioClipDropArea = GUILayoutUtility.GetRect( 0.0f, 30.0f, GUILayout.MaxWidth( EditorGUIUtility.labelWidth ) );
                                GUI.Label( audioClipDropArea, "Drop AudioClips here, use inspector lock!", boxStyle );

                                switch ( evt.type )
                                {
                                case EventType.DragUpdated:
                                case EventType.DragPerform:
                                    if ( !audioClipDropArea.Contains( evt.mousePosition ) )
                                        break;


                                    bool acceptItems = true;

                                    foreach ( UnityEngine.Object draggedObject in DragAndDrop.objectReferences )
                                    {
                                        if ( draggedObject as AudioClip == null )
                                        {
                                            acceptItems = false;
                                            break;
                                        }
                                    }

                                    if ( acceptItems )
                                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                    else
                                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

                                    if ( evt.type == EventType.DragPerform && acceptItems )
                                    {
                                        DragAndDrop.AcceptDrag();

                                        int firstIndex = subItemCount;
                                        currentSubitemIndex = firstIndex;
                                        LogUndo( "Set Audio Clip" );

                                        foreach ( AudioClip audioClip in DragAndDrop.objectReferences )
                                        {
                                            ArrayHelper.AddArrayElement( ref curItem.subItems ).Clip = audioClip;
                                            currentSubitemIndex++;
                                        }

                                        currentSubitemIndex = firstIndex;
                                        KeepChanges();
                                        evt.Use();
                                    }
                                    break;
                                }

                                //-------------------------------------------------------------------//

                                if ( GUILayout.Button( "Add selected audio clips", GUILayout.ExpandHeight( true ), GUILayout.ExpandWidth( true ) ) )
                                {
                                    AudioClip[ ] audioClips = GetSelectedAudioclips();
                                    if ( audioClips.Length > 0 )
                                    {
                                        LogUndo( "Add Audio Clip" );
                                        int firstIndex = subItemCount;
                                        currentSubitemIndex = firstIndex;
                                        foreach ( AudioClip audioClip in audioClips )
                                        {
                                            ArrayHelper.AddArrayElement( ref curItem.subItems ).Clip = audioClip;
                                            currentSubitemIndex++;
                                        }
                                        currentSubitemIndex = firstIndex;
                                        KeepChanges();
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();

                                currentSubitemIndex = PopupWithStyle( "SubItem", currentSubitemIndex, GetSubitemNames(), popupStyleColored );

                                if ( GUILayout.Button( "+", GUILayout.Width( 30 ) ) )
                                {
                                    bool lastEntryIsNew = false;

                                    AudioSubItemType curSubItemType = AudioSubItemType.Clip;

                                    if ( subItemCount > 0 )
                                    {
                                        curSubItemType = curItem.subItems[ currentSubitemIndex ].SubItemType;
                                        if ( curSubItemType == AudioSubItemType.Clip )
                                        {
                                            lastEntryIsNew = curItem.subItems[ currentSubitemIndex ].Clip == null;
                                        }
                                        if ( curSubItemType == AudioSubItemType.Item )
                                        {
                                            lastEntryIsNew = curItem.subItems[ currentSubitemIndex ].ItemModeAudioID == null ||
                                                             curItem.subItems[ currentSubitemIndex ].ItemModeAudioID.Length == 0;
                                        }
                                    }

                                    if ( !lastEntryIsNew )
                                    {
                                        LogUndo( "Add Audio Subitem" );
                                        currentSubitemIndex = subItemCount;
                                        ArrayHelper.AddArrayElement( ref curItem.subItems );
                                        curItem.subItems[ currentSubitemIndex ].SubItemType = curSubItemType;
                                        KeepChanges();
                                    }
                                }

                                if ( GUILayout.Button( "-", GUILayout.Width( 30 ) ) && subItemCount > 0 )
                                {
                                    LogUndo( "Del Audio Subitem" );
                                    ArrayHelper.DeleteArrayElement( ref curItem.subItems, currentSubitemIndex );
                                    if ( currentSubitemIndex >= curItem.subItems.Length )
                                    {
                                        currentSubitemIndex = Mathf.Max( curItem.subItems.Length - 1, 0 );
                                    }
                                    KeepChanges();
                                }
                                EditorGUILayout.EndHorizontal();

                                subItem = currentSubItem;

                                if ( subItem != null )
                                {
                                    _SubitemTypePopup( subItem );


                                    if ( subItem.SubItemType == AudioSubItemType.Item )
                                    {
                                        _DisplaySubItem_Item( subItem );

                                    }
                                    else
                                    {
                                        _DisplaySubItem_Clip( subItem, subItemCount, curItem );
                                    }
                                }
                            }
                        }
                    }
                }
            }

            VerticalSpace();

            EditorGUILayout.BeginHorizontal();

            if ( GUILayout.Button( "Show Audio Log" ) )
            {
                var win = EditorWindow.GetWindow( typeof( AudioLogView ), false, "Audio Log" );
                win.Show();
            }

            if ( GUILayout.Button( "Show Item Overview" ) )
            {
                AudioItemOverview win = EditorWindow.GetWindow( typeof( AudioItemOverview ), false, "Audio Items" ) as AudioItemOverview;
                win.Show( AC );
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if ( EditorApplication.isPlaying )
            {
                EditorGUILayout.BeginHorizontal();
                if ( GUILayout.Button( "Stop All Sounds" ) )
                {
                    if ( EditorApplication.isPlaying && AudioController.DoesInstanceExist() )
                    {
                        AudioController.StopAll();
                    }
                }
                if ( GUILayout.Button( "Stop Music Only" ) )
                {
                    if ( EditorApplication.isPlaying && AudioController.DoesInstanceExist() )
                    {
                        AudioController.StopMusic();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            GUILayout.Label( string.Format( "----- ClockStone Audio Toolkit v{0} -----  ", AudioController.AUDIO_TOOLKIT_VERSION ), centeredTextStyle );

            EndInspectorGUI();

            //Debug.Log( "currentCategoryIndex: " + currentCategoryIndex );
        }

        private void AddAudioItemFromClips( AudioCategory curCat, List<AudioClip> clips, string audioID )
        {
            string name = audioID;
            if( curCat.AudioItems.Any( x => x.Name == name ) )
            {
                for( int index = 1; ; index++ )
                {
                    name = audioID + " " + index;
                    if( !curCat.AudioItems.Any( x => x.Name == name ) ) break;
                }
            }
            var audioItem = ArrayHelper.AddArrayElement( ref curCat.AudioItems );
            audioItem.Name = name;
            audioItem.subItems = new AudioSubItem[clips.Count];
            for( int i = 0; i < clips.Count; i++ )
            {
                audioItem.subItems[i] = new AudioSubItem();
                audioItem.subItems[i].Clip = clips[i];
            }
        }

        private void AddAudioItemFromClip( AudioCategory curCat, AudioClip audioClip )
        {
            string name = audioClip.name;
            // make sure name is unique
            if ( curCat.AudioItems.Any( x => x.Name == name ) )
            {
                for ( int index = 1; ; index++ )
                {
                    name = audioClip.name + " " + index;
                    if ( !curCat.AudioItems.Any( x => x.Name == name ) ) break;
                }
            }
            var audioItem = ArrayHelper.AddArrayElement( ref curCat.AudioItems );
            audioItem.Name = name;
            ArrayHelper.AddArrayElement( ref audioItem.subItems ).Clip = audioClip;
        }

        private void _RenamePlaylistEntries( string originalName, string newName )
        {
            if ( AC.musicPlaylists == null ) return;

            for ( int playlistI = 0; playlistI < AC.musicPlaylists.Length; ++playlistI )
            {
                for ( int i = 0; i < AC.musicPlaylists[ playlistI ].playlistItems.Length; i++ )
                {
                    if ( AC.musicPlaylists[ playlistI ].playlistItems[ i ] == originalName )
                    {
                        AC.musicPlaylists[ playlistI ].playlistItems[ i ] = newName;
                    }
                }

            }

        }

        private string[ ] _GenerateCategoryListIncludingNone( out int selectedParentCategoryIndex, AudioCategory selectedAudioCategory )
        {
            string[ ] names;
            selectedParentCategoryIndex = 0;

            if ( AC.AudioCategories != null )
            {
                names = new string[ AC.AudioCategories.Length ];

                int index = 1;

                var curCat = currentCategory;

                for ( int i = 0; i < AC.AudioCategories.Length; i++ )
                {
                    if ( _IsCategoryChildOf( AC.AudioCategories[ i ], curCat ) ) // prevent loops in tree
                    {
                        continue;
                    }
                    names[ index ] = AC.AudioCategories[ i ].Name;
                    if ( selectedAudioCategory == AC.AudioCategories[ i ] )
                    {
                        selectedParentCategoryIndex = index;
                    }

                    index++;
                    if ( index == names.Length )
                    {
                        break; // in case currentCategory is not found
                    }
                }

                if ( index < names.Length )
                {
                    var newNames = new string[ index ];
                    Array.Copy( names, newNames, index );
                    names = newNames;
                }
            }
            else
            {
                names = new string[ 1 ];
            }

            names[ 0 ] = "*none*";
            return names;
        }

        bool _IsCategoryChildOf( AudioCategory toTest, AudioCategory parent )
        {
            var cat = toTest;
            while ( cat != null )
            {
                if ( cat.audioController == null )
                {
                    cat.audioController = AC;
                }

                if ( cat == parent ) return true;

                cat = cat.parentCategory;
            }
            return false;
        }

        private bool _IsAudioControllerInPlayMode()
        {
            return EditorApplication.isPlaying && AudioController.DoesInstanceExist();
        }
        private void _ValidateCurrentCategoryIndex()
        {
            int categoryCount = currentCategoryCount;
            if ( categoryCount > 0 ) currentCategoryIndex = Mathf.Clamp( currentCategoryIndex, 0, categoryCount - 1 );
            else currentCategoryIndex = -1;
        }

        private void _ValidateCurrentSubItemIndex()
        {
            int subitemCount = currentSubItemCount;
            if ( subitemCount > 0 ) currentSubitemIndex = Mathf.Clamp( currentSubitemIndex, 0, subitemCount - 1 );
            else currentSubitemIndex = -1;
        }

        private void _ValidateCurrentItemIndex()
        {
            int itemCount = currentItemCount;
            if ( itemCount > 0 ) currentItemIndex = Mathf.Clamp( currentItemIndex, 0, itemCount - 1 );
            else currentItemIndex = -1;
        }

        private void _SubitemTypePopup( AudioSubItem subItem )
        {
            var typeNames = new string[ 2 ];
            typeNames[ 0 ] = "Single Audio Clip";
            typeNames[ 1 ] = "Other Audio Item";

            int curIndex = 0;
            switch ( subItem.SubItemType )
            {
            case AudioSubItemType.Clip: curIndex = 0; break;
            case AudioSubItemType.Item: curIndex = 1; break;
            }

            switch ( Popup( "SubItem Type", curIndex, typeNames ) )
            {
            case 0: subItem.SubItemType = AudioSubItemType.Clip; break;
            case 1: subItem.SubItemType = AudioSubItemType.Item; break;
            }

            //subItem.SubItemType = (AudioSubItemType) EnumPopup( "SubItem Type", subItem.SubItemType );
        }

        public void AddToPlayList( string playlistName, string entryName )
        {
            Playlist playlist = AC.GetPlaylistByName( playlistName );

            if ( playlist == null )
                return;

            ArrayHelper.AddArrayElement( ref playlist.playlistItems, entryName );
            currentPlaylistEntryIndex = playlist.playlistItems.Length - 1;
            KeepChanges();
        }

        protected void EditAudioClip( ref AudioSubItem subItem, string label )
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label( label, styleLabel );
            AudioClip clip = subItem.Clip;

            clip = (AudioClip) EditorGUILayout.ObjectField( clip, typeof( AudioClip ), false );
            if ( clip )
            {
                EditorGUILayout.Space();
                GUILayout.Label( string.Format( "{0:0.0} sec", clip.length ), GUILayout.Width( 60 ) );
                subItem.Clip = clip;
            }

            bool showPlayButton = true;

            if( !_IsAudioControllerInPlayMode() )
            {
                if( subItem.SubItemType == AudioSubItemType.Clip && subItem.Clip != null )
                {
                    if( AudioUtility.IsClipPlaying( subItem.Clip ) )
                    {
                        if( GUILayout.Button( new GUIContent( "\u25a0", "Stop AudioClip" ), GUILayout.Width( 20f ) ) )
                        {
                            AudioUtility.StopClip( subItem.Clip );
                        }
                        showPlayButton = false;
                    }
                }
            } 

            if ( showPlayButton && GUILayout.Button( new GUIContent( "\u25b6", "Preview AudioClip" ), GUILayout.Width( 20f ) ) && subItem != null )
            {
                if ( _IsAudioControllerInPlayMode() )
                {
                    var audioListener = AudioController.GetCurrentAudioListener();
                    Vector3 pos;
                    if ( audioListener != null )
                    {
                        pos = audioListener.transform.position + audioListener.transform.forward;
                    }
                    else
                        pos = Vector3.zero;

                    AudioController.Instance.PlayAudioSubItem( subItem, 1, pos, null, 0, 0, false, null );

                }
                else
                {
#if !UNITY_2019_OR_NEWER
                    if ( Application.platform == RuntimePlatform.OSXEditor )
                    {
                        Debug.Log( _playNotSupportedOnMac );
                    }
                    else
#endif
                    {
                        previewAudioSubItem( subItem );
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void _DisplaySubItem_Clip( AudioSubItem subItem, int subItemCount, AudioItem curItem )
        {

            // AudioSubItems

            if ( subItem != null )
            {
                EditAudioClip( ref subItem, "Audio Clip" );

                if ( EditFloat01( ref subItem.Volume, "Volume", " %" ) )
                {
                    _AdjustVolumeOfAllAudioItems( curItem, subItem );
                }

                EditSubItemFloat01Inherited( subItem, ref subItem.RandomVolume, "RandomVolume", "Random Volume", "±%" );

                EditFloat( ref subItem.Delay, "Delay", "sec" );
                //EditFloatWithinRange( ref subItem.Pan2D, "Pan2D [left..right]", -1.0f, 1.0f);
                EditFloatPlusMinus1( ref subItem.Pan2D, "Pan2D", "%left/right" );
                if ( _IsRandomItemMode( curItem.SubItemPickMode ) )
                {
                    EditFloat01( ref subItem.Probability, "Probability", " %", "Choose a higher value (in comparison to the probability values of the other audio clips) to increase the probability for this clip when using a random subitem pick mode." );
                    EditBool( ref subItem.DisableOtherSubitems, "Disable Other Subitems", "If enabled all other subitems which do not have this option enabled will not be played. Useful for testing specific subitmes within a large list of subitems." );
                }
                EditSubItemFloatInherited( subItem,ref subItem.PitchShift, "PitchShift", "Pitch Shift", "semitone" );

                EditSubItemFloatInherited( subItem, ref subItem.RandomPitch, "RandomPitch", "Random Pitch", "±semitone" );

                EditSubItemFloatInherited( subItem, ref subItem.RandomDelay, "RandomDelay", "Random Delay", "sec" );


                EditFloat( ref subItem.FadeIn, "Fade-in", "sec" );
                EditFloat( ref subItem.FadeOut, "Fade-out", "sec" );
                EditFloat( ref subItem.ClipStartTime, "Start at", "sec" );
                EditFloat( ref subItem.ClipStopTime, "Stop at", "sec" );
                EditBool( ref subItem.RandomStartPosition, "Random Start Position", "Starts playing at a random position. Useful when looping." );
            }

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label( " " );



            EditorGUILayout.EndHorizontal();


        }

        private void _AdjustVolumeOfAllAudioItems( AudioItem curItem, AudioSubItem subItem )
        {
            if ( _IsAudioControllerInPlayMode() )
            {
                AudioController.InvokeForAllPlayingAudioObjects( ( o ) =>
                {
                    if( curItem != o.audioItem ) return;
                    if( subItem != null )
                    {
                        if( subItem != o.subItem ) return;
                    }
                    o.volumeItem = o.audioItem.Volume * o.subItem.Volume;
                } );
            }
        }

        private bool _IsRandomItemMode( AudioPickSubItemMode audioPickSubItemMode )
        {
            switch ( audioPickSubItemMode )
            {
            case AudioPickSubItemMode.Random: return true;
            case AudioPickSubItemMode.RandomNotSameTwice: return true;
            case AudioPickSubItemMode.TwoSimultaneously: return true;
            case AudioPickSubItemMode.RandomNotSameTwiceOddsEvens: return true;
            }
            return false;
        }

        private string _ChooseItem( string label )
        {
            string[ ] possibleAudioIDs_withCategory = _GetPossibleAudioIDs( true, "Choose Audio Item..." );

            int selected = PopupWithStyle( label, 0, possibleAudioIDs_withCategory, styleChooseItem );
            if ( selected != 0 )
            {
                string[ ] possibleAudioIDs = _GetPossibleAudioIDs( false, "Choose Audio Item..." );
                return possibleAudioIDs[ selected ];
            }
            return null;
        }

        private bool EditSubItemFloatInherited( AudioSubItem subItem, ref float value, string attributeName, string label, string units )
        {
            Type itemType = subItem.GetType();

            FieldInfo fieldInfo = itemType.GetField( attributeName );

            string reflectedAttributeName = fieldInfo.Name;

            bool individualSetting =
                subItem.individualSettings.Contains( reflectedAttributeName );

            bool reset = false;

            if ( EditFloatInherited( ref value, label, units, individualSetting, out reset ) )
            {
                if ( !individualSetting )
                    subItem.individualSettings.Add( reflectedAttributeName );
                return true;
            }

            if ( reset )
            {
                subItem.individualSettings.Remove( reflectedAttributeName );
                value = (float) currentItem.GetType().GetField( reflectedAttributeName ).GetValue( currentItem );
            }

            return false;
        }

        private bool EditSubItemFloat01Inherited( AudioSubItem subItem, ref float value, string attributeName, string label,
            string units )
        {
            Type itemType = subItem.GetType();

            FieldInfo fieldInfo = itemType.GetField( attributeName );

            string reflectedAttributeName = fieldInfo.Name;

            bool individualSetting = subItem.individualSettings.Contains( reflectedAttributeName );

            bool reset = false;

            if ( EditFloat01Inherited( ref value, label, units, individualSetting, out reset ) )
            {
                if ( !individualSetting )
                    subItem.individualSettings.Add( reflectedAttributeName );
                return true;
            }

            if ( reset )
            {
                subItem.individualSettings.Remove( reflectedAttributeName );
                value = (float) currentItem.GetType().GetField( reflectedAttributeName ).GetValue( currentItem );
            }

            return false;
        }

        private void _DisplaySubItem_Item( AudioSubItem subItem )
        {
            EditFloat01( ref subItem.Probability, "Probability", " %" );
            int audioIndex = 0;
            string[ ] possibleAudioIDs = _GetPossibleAudioIDs( false, "*undefined*" );
            string[ ] possibleAudioIDs_withCategory = _GetPossibleAudioIDs( true, "*undefined*" );

            if ( subItem.ItemModeAudioID != null && subItem.ItemModeAudioID.Length > 0 )
            {
                string idToSearch = subItem.ItemModeAudioID.ToLowerInvariant();

                for ( int i = 1; i < possibleAudioIDs.Length; i++ )
                {
                    if ( possibleAudioIDs[ i ].ToLowerInvariant() == idToSearch )
                    {
                        audioIndex = i; break;
                    }
                }
            }

            bool wasUndefinedBefore = ( audioIndex == 0 );

            audioIndex = Popup( "AudioItem", audioIndex, possibleAudioIDs_withCategory );
            if ( audioIndex > 0 )
            {
                subItem.ItemModeAudioID = possibleAudioIDs[ audioIndex ];
            }
            else
            {
                if ( !wasUndefinedBefore )
                {
                    subItem.ItemModeAudioID = null;
                }
            }
        }

        private string[ ] _GetPossibleAudioIDs( bool withCategoryName, string firstEntryName )
        {
            var audioIDs = new List<string>();
            audioIDs.Add( firstEntryName );
            if ( AC.AudioCategories != null )
            {
                foreach ( var category in AC.AudioCategories )
                {
                    _GetAllAudioIDs( audioIDs, category, withCategoryName );
                }
            }
            return audioIDs.ToArray();
        }

        private void _GetAllAudioIDs( List<string> audioIDs, AudioCategory c, bool withCategoryName )
        {
            if ( c.AudioItems != null )
            {
                foreach ( var audioItem in c.AudioItems )
                {
                    if ( audioItem.Name.Length > 0 )
                    {
                        if ( withCategoryName )
                        {
                            audioIDs.Add( string.Format( "{0}/{1}", c.Name, audioItem.Name ) );
                        }
                        else
                            audioIDs.Add( audioItem.Name );
                    }
                }
            }
        }

        private bool SwapArrayElements<T>( T[ ] array, int index1, int index2 )
        {
            if ( array == null || index1 < 0 || index2 < 0 || index1 >= array.Length || index2 >= array.Length )
            {
                return false;
            }

            T tmp = array[ index1 ];
            array[ index1 ] = array[ index2 ];
            array[ index2 ] = tmp;
            return true;
        }

        private void VerticalSpace()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private string[ ] GetCategoryNames()
        {
            if ( AC.AudioCategories == null )
            {
                return new string[ 0 ];
            }
            var names = new string[ AC.AudioCategories.Length ];
            for ( int i = 0; i < AC.AudioCategories.Length; i++ )
            {
                names[ i ] = AC.AudioCategories[ i ].Name;

                if ( names[ i ] == _nameForNewCategoryEntry )
                {
                    names[ i ] = "---";
                }
            }
            return names;
        }

        private string[ ] GetItemNames()
        {
            AudioCategory curCat = currentCategory;
            if ( curCat == null || curCat.AudioItems == null )
            {
                return new string[ 0 ];
            }

            var names = new string[ curCat.AudioItems.Length ];
            for ( int i = 0; i < curCat.AudioItems.Length; i++ )
            {
                names[ i ] = curCat.AudioItems[ i ] != null ? curCat.AudioItems[ i ].Name : "";

                if ( names[ i ] == _nameForNewAudioItemEntry )
                {
                    names[ i ] = "---";
                }
            }
            return names;
        }

        private string[ ] GetSubitemNames()
        {
            AudioItem curItem = currentItem;
            if ( curItem == null || curItem.subItems == null )
            {
                return new string[ 0 ];
            }

            var names = new string[ curItem.subItems.Length ];
            for ( int i = 0; i < curItem.subItems.Length; i++ )
            {
                AudioSubItemType subitemType = curItem.subItems[ i ] != null ? curItem.subItems[ i ].SubItemType : AudioSubItemType.Clip;

                if ( subitemType == AudioSubItemType.Item )
                {
                    names[ i ] = string.Format( "ITEM {0}: {1}", i, ( curItem.subItems[ i ].ItemModeAudioID ?? "*undefined*" ) );
                }
                else
                {
                    names[ i ] = string.Format( "CLIP {0}: {1}", i, ( curItem.subItems[ i ] != null ? curItem.subItems[ i ].Clip ? curItem.subItems[ i ].Clip.name : "*unset*" : "" ) );
                }
            }
            return names;
        }

        private string[ ] GetPlaylistNames()
        {
            if ( AC.musicPlaylists == null )
            {
                return new string[ 0 ];
            }

            var names = new string[ AC.musicPlaylists.Length ];
            for ( int i = 0; i < AC.musicPlaylists.Length; ++i )
            {
                names[ i ] = AC.musicPlaylists[ i ].name;

                if ( names[ i ] == _nameForNewPlaylist )
                {
                    names[ i ] = "---";
                }
            }
            return names;
        }

        private string[ ] GetPlaylistEntryNames()
        {
            if ( AC.musicPlaylists == null )
            {
                return new string[ 0 ];
            }

            if ( AC.musicPlaylists[ currentPlaylistIndex ].playlistItems == null )
            {
                return new string[ 0 ];
            }

            var names = new string[ AC.musicPlaylists[ currentPlaylistIndex ].playlistItems.Length ];
            for ( int i = 0; i < AC.musicPlaylists[ currentPlaylistIndex ].playlistItems.Length; i++ )
            {
                names[ i ] = string.Format( "{0}: {1}", i, AC.musicPlaylists[ currentPlaylistIndex ].playlistItems[ i ] );
            }
            return names;
        }

        static AudioClip[ ] GetSelectedAudioclips()
        {
            var objList = Selection.GetFiltered( typeof( AudioClip ), SelectionMode.DeepAssets );
            var clipList = new AudioClip[ objList.Length ];

            for ( int i = 0; i < objList.Length; i++ )
            {
                clipList[ i ] = (AudioClip) objList[ i ];
            }

            return clipList;
        }

        AudioCategory _GetCategory( string name )
        {
            foreach ( AudioCategory cat in AC.AudioCategories )
            {
                if ( cat.Name == name )
                {
                    return cat;
                }
            }
            return null;
        }

        AudioItem _GetAudioItemByName( string audioID )
        {
            foreach ( AudioCategory cat in AC.AudioCategories )
                foreach ( AudioItem aitem in cat.AudioItems )
                    if ( aitem.Name == audioID )
                        return aitem;
            return null;
        }

        private void previewAudioItem( AudioItem item )
        {
            previewAudioSubItem( AudioControllerHelper._ChooseSingleSubItem( item ) );
        }

        private void previewAudioSubItem( AudioSubItem item )
        {
            if ( item.SubItemType == AudioSubItemType.Clip )
            {
#if UNITY_EDITOR // workaround for strange compiler error during build process
                AudioUtility.PlayClip( item.Clip );
#endif
            }
            else if ( item.SubItemType == AudioSubItemType.Item )
            {
                AudioItem linkedItem = _GetAudioItemByName( item.ItemModeAudioID );
                if ( linkedItem != null )
                    previewAudioItem( linkedItem );
            }
        }
    }
}
