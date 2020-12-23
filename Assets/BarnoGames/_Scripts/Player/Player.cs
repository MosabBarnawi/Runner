using BarnoGames.Runner2020;
using UnityEngine;
using TMPro;
using System;
using System.Collections;

[SelectionBase]
public class Player : MonoBehaviour
{
    [SerializeField] private Character MainPlayer;
    [SerializeField] private Character SecondaryPlayer;
    [SerializeField] private PlayerType currentPlayerType = PlayerType.Empty;
    public bool CanSwitchPlayers { get; set; }

    #region Unity Callbacks

    private void Awake()
    {
        if (PlayerInputControls.PlayerScript == null)
            PlayerInputControls.PlayerScript = this;
        else Debug.LogWarning("Player Script Was not Null Handle this");

        PlayerInputControls.SpecialAbility = SwitchPlayers;
        MainPlayer.OnDeath += () => PlayerHasDiedGameManagerCallback(GameManager.SharedInstance.PlayerHasDied);
        SecondaryPlayer.OnDeath += () => PlayerHasDiedGameManagerCallback(GameManager.SharedInstance.PlayerHasDied);
    }

    private void OnDestroy() => PlayerInputControls.SpecialAbility -= SwitchPlayers;

    #endregion

    private void SwitchPlayers()  // ALOS HANDLES ENABLEING CORRECT PLAYERS
    {
        if (currentPlayerType == PlayerType.Empty)
        {
            MainPlayer.GetComponent<IPlayerSwitchable>().SwitchToMe();
            SecondaryPlayer.GetComponent<IPlayerSwitchable>().UnSwitchFromMe(MainPlayer.transform);

            currentPlayerType = PlayerType.MainPlayer;
        }
        else if (currentPlayerType == PlayerType.Secondary)
        {
            MainPlayer.GetComponent<IPlayerSwitchable>().SwitchToMe();
            SecondaryPlayer.GetComponent<IPlayerSwitchable>().UnSwitchFromMe(MainPlayer.transform);
            currentPlayerType = PlayerType.MainPlayer;
        }
        else if (currentPlayerType == PlayerType.MainPlayer)
        {
            if (CanSwitchPlayers)
            {
                SecondaryPlayer.GetComponent<IPlayerSwitchable>().SwitchToMe();
                MainPlayer.GetComponent<IPlayerSwitchable>().UnSwitchFromMe(SecondaryPlayer.transform);
                currentPlayerType = PlayerType.Secondary;
            }
        }
    }

    public void SwitchToMainPlayer() => SwitchPlayers();

    private void PlayerHasDiedGameManagerCallback(Action playerHasDiedGameManagerCallback)
    {
        playerHasDiedGameManagerCallback?.Invoke();
    }

    private void RestartPlayerSettings()
    {
        currentPlayerType = PlayerType.Empty;
        SwitchPlayers();
    }

    public void StartLevel()
    {
        MainPlayer.imove.EnableMovement();
    }


    public void ReSpawnToPosition(Vector3 positionToSpawn)
    {
        RestartPlayerSettings();
        MainPlayer.Respawn(positionToSpawn);
    }

    public void PlayerFalling(Vector3 fallingPosition)
    {
        RestartPlayerSettings();
        MainPlayer.GetComponent<IPlayerTAG>().PlayerIsFalling(fallingPosition, () => GameManager.SharedInstance.LevelStart());
    }

    #region RAY CASTINGS AND SLOPE DETECTION LOGIC

    //protected override void SlopPlayerAngleAdjustment(in bool hitGround, in Transform forwardPosition, in Transform middlePosition, in Transform backwardPosition)
    //{
    //    //float minSlopAngle = 0.1f;
    //    float minSlopAngle = 0;
    //    // TODO FIX SLOPE ISSUE
    //    Quaternion rotationOnSlope = new Quaternion(0, 0, 0, 0);
    //    IsOnSlope = false;

    //    if (hitGround)
    //    {
    //        if (forwardPosition != null)
    //        {
    //            if (forwardPosition.localRotation.z >= minSlopAngle || forwardPosition.localRotation.z <= -minSlopAngle)
    //            {
    //                IsOnSlope = true;
    //                rotationOnSlope = forwardPosition.localRotation;
    //            }
    //        }
    //        else
    //        {
    //            if (middlePosition != null)
    //            {
    //                if (middlePosition.localRotation.z >= minSlopAngle || middlePosition.localRotation.z <= -minSlopAngle)
    //                {
    //                    IsOnSlope = true;
    //                    rotationOnSlope = middlePosition.localRotation;
    //                }
    //            }
    //            else
    //            {
    //                if (backwardPosition != null)
    //                {
    //                    if (backwardPosition.localRotation.z >= minSlopAngle || backwardPosition.localRotation.z <= -minSlopAngle)
    //                    {
    //                        IsOnSlope = true;
    //                        rotationOnSlope = backwardPosition.localRotation;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationOnSlope, speedToTransitionIntoSlopAngle);
    //}

    #endregion
}
