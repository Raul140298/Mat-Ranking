using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ClockStone
{
    partial class AudioController_Editor
    {
        class AudioItemNameAndClips
        {
            public string audioID;
            public List<AudioClip> audioClips;
        }
        static class AudioItemFinder
        {
            public static List<AudioItemNameAndClips> FindAudioItemsFromClips( AudioClip[] clips )
            {
                var sortedClips = clips.ToList();
                
                sortedClips.Sort( ( x, y ) => string.Compare( x.name, y.name, true, System.Globalization.CultureInfo.InvariantCulture ) );

                var retList = new List<AudioItemNameAndClips>();
                string lastUnifiedName = "";
                AudioItemNameAndClips lastItem = null;

                for( int i = 0; i < sortedClips.Count; i++ )
                {
                    var nameUnified = _UnifyAudioItemName( sortedClips[i].name );
                    var nameUnifiedLower = nameUnified.ToLowerInvariant();
                    if( lastItem == null || lastUnifiedName != nameUnifiedLower )
                    {
                        lastUnifiedName = nameUnifiedLower;
                        lastItem = new AudioItemNameAndClips();
                        lastItem.audioID = nameUnified;
                        lastItem.audioClips = new List<AudioClip>();
                        retList.Add( lastItem );
                    }
                    lastItem.audioClips.Add( sortedClips[i] );
                }

                return retList;
            }

            static private string _UnifyAudioItemName( string name )
            {
                if( name.Length <= 1 ) return name;
                int pos = name.Length - 1;
                pos = _StringSkipWhiteSpace( name, pos );
                pos = _StringSkipDigits( name, pos );
                pos = _StringSkipWhiteSpace( name, pos );
                return name.Substring( 0, pos + 1 );
            }

            static private int _StringSkipWhiteSpace( string name, int startIndex )
            {
                for( int i = startIndex; i >= 0; i-- )
                {
                    if( char.IsLetter( name[i] ) || char.IsDigit( name[i] ) ) return i;
                }
                return -1;
            }

            static private int _StringSkipDigits( string name, int startIndex )
            {
                for( int i = startIndex; i >= 0; i-- )
                {
                    if( !char.IsDigit( name[i] ) ) return i;
                }
                return -1;
            }
        }
    }
}
