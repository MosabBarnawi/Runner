using UnityEngine.Rendering;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public enum PostProcesingType
    {
        Logo,
        Default,
        WinState
    }

    public class PostProcessingManager : MonoBehaviour
    {
        [Header("Post Processing")]
        [SerializeField] private Volume GlobalVolume;

        [Header("Post Processing Profiles")]
        [SerializeField] private VolumeProfile LogoProgfile;
        [SerializeField] private VolumeProfile Defualtprofile;
        [SerializeField] private VolumeProfile WinStaeprofile;

        public void SetPostProcessingType(PostProcesingType postProcesingType)
        {
            if (postProcesingType == PostProcesingType.Default)
            {
                GlobalVolume.profile = Defualtprofile;
                Time.timeScale = TIME_CONSTANTS.NORMAL_TIME;
            }

            else if (postProcesingType == PostProcesingType.WinState)
            {
                GlobalVolume.profile = WinStaeprofile;
            }

            else if (postProcesingType == PostProcesingType.Logo)
            {
                GlobalVolume.profile = LogoProgfile;
            }
        }
    }
}