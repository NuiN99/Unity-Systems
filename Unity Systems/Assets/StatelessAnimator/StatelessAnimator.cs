using System.Collections.Generic;
using UnityEngine;

namespace NuiN.StatelessAnimator
{
    public class StatelessAnimator : MonoBehaviour
    {
        const string DUPLICATE_SUFFIX = "_Duplicate";
        
        string _activeClipName = string.Empty;
        Dictionary<AnimationClip, StatelessClip> _clips = new();
        
        [SerializeField] Animation controller;
    
        void Reset()
        {
            controller = GetComponentInChildren<Animation>();
            if (controller == null) controller = gameObject.AddComponent<Animation>(); 
        }
    
        /// <summary>Play an animation clip directly </summary>
        /// <param name="clip">Clip to play</param>
        /// <param name="fadeTime">Optional cross fade duration [0-1] (seconds)</param>
        /// <param name="wrapMode">Can enable looping using WrapMode.Loop - WrapMode.ClampForever allows crossfading when playback has stopped</param>
        public void Play(AnimationClip clip, float fadeTime = 0f, WrapMode wrapMode = WrapMode.ClampForever, bool overrideCurrent = true)
        {
            clip.legacy = true;
            clip.wrapMode = wrapMode;
            
            string clipName = clip.name;

            if (!overrideCurrent)
            {
                if (clipName == _activeClipName) return;
                
                if(!controller.GetClip(clipName)) controller.AddClip(clip, clipName);
                controller.CrossFade(clipName, Mathf.Min(fadeTime, 1));
                _activeClipName = clipName;
                
                return;
            }
            
            StatelessClip statelessClip = new(clipName, clipName + DUPLICATE_SUFFIX);
            if (_clips.TryAdd(clip, statelessClip))
            {
                controller.AddClip(clip, statelessClip.original);
                controller.AddClip(clip, statelessClip.duplicate);
            }
    
            bool playingOriginal = _activeClipName == statelessClip.original;
    
            string clipToPlay = playingOriginal ? statelessClip.duplicate : statelessClip.original;
            
            controller.CrossFade(clipToPlay, Mathf.Min(fadeTime, 1));
            _activeClipName = clipToPlay;
        }
    
        internal struct StatelessClip
        {
            public readonly string original;
            public readonly string duplicate;
    
            public StatelessClip(string original, string duplicate)
            {
                this.original = original;
                this.duplicate = duplicate;
            }
        }
    }
}

