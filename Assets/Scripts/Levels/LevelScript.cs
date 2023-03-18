using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private Text zone, level;
    [SerializeField] private CapsuleCollider2D playerDialogueArea;
    [SerializeField] private GameObject topBar, bottomBar;
    [SerializeField] private LevelInteractionsScript playerLevelInteractions;

    [SerializeField] private Slider soundtracksSlider;
    [SerializeField] private Slider soundsSlider;

    private GameSystemScript gameSystem;

    private void Start()
    {
        gameSystem = GameSystemScript.Instance;

        gameSystem.DialogueSystem.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
        gameSystem.CurrentLevelSO.heart = false;
        gameSystem.CurrentLevelSO.playerKeyParts = 0;
        gameSystem.StartSounds(soundsSlider);
        gameSystem.StartSoundtracks(soundtracksSlider);
        if (gameSystem.FromLevelSO.fromLevel == false)
        {
            gameSystem.CurrentLevelSO.playerLives = 3;
            gameSystem.FromLevelSO.fromLevel = true;
        }

        playerLevelInteractions.setLives();
        playerLevelInteractions.setKeys();

        CheckIfLevelDataIsEmpty();
    }

    private void CheckIfLevelDataIsEmpty()
    {
        //If there aren't enemys in the zone
        if ((gameSystem.CurrentLevelSO.currentZone == 0 &&
            gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].selected == false && //L1
            (gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].selected == false || //L2 or
            (gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].selected == false && //L2.1
            gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].selected == false))) ||//L2.2

            (gameSystem.CurrentLevelSO.currentZone == 1 &&
            gameSystem.RemoteSO.dgbl_features.ilos[1].ilos[0].selected == false && //L8
            gameSystem.RemoteSO.dgbl_features.ilos[1].ilos[1].selected == false) || //L9

            (gameSystem.CurrentLevelSO.currentZone == 2 &&
            (gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].selected == false || //L13
            (gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].selected == false && //L13.1
            gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].selected == false && //L13.2
            gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].selected == false))) || //L13.3

            (gameSystem.CurrentLevelSO.currentZone == 3 &&
            gameSystem.RemoteSO.dgbl_features.ilos[3].ilos[1].selected == false && //L19
            gameSystem.RemoteSO.dgbl_features.ilos[3].ilos[3].selected == false || //L21
            (gameSystem.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].selected == false && //L21.1
            gameSystem.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[1].selected == false))) //L21.2
        {
            switch (Localization.language)
            {
                case "es":
                    zone.text = "Desafío Desactivado";
                    level.text = "No hay ningun enemigo";
                    break;
                case "en":
                    zone.text = "Challenge off";
                    level.text = "There is no enemy";
                    break;
                case "qu":
                    zone.text = "Atipanakuy nisqa cancelasqa";
                    level.text = "Mana awqa kanchu";
                    break;
                default:
                    // code block
                    break;
            }

            StartCoroutine(CRTNoChallenge());
        }
        else
        {
            gameSystem.SetKnowledgePoints();

            switch (Localization.language)
            {
                case "es":
                    zone.text = "Desafío";
                    level.text = "Piso";
                    break;
                case "en":
                    zone.text = "Challenge";
                    level.text = "Floor";
                    break;
                case "qu":
                    zone.text = "Atipanakuy";
                    level.text = "Panpa";
                    break;
                default:
                    // code block
                    break;
            }
            zone.text += " " + (gameSystem.CurrentLevelSO.currentZone + 1).ToString();
            level.text += " " + gameSystem.CurrentLevelSO.currentLevel.ToString();

            StartCoroutine(CRTPlayerDialogueStart());
        }
    }

    IEnumerator CRTNoChallenge()
    {
        yield return new WaitForSeconds(2.3f);
        Debug.Log("No habia enemigos en la mazmorra");
        SceneManager.LoadScene(1);
    }

    IEnumerator CRTPlayerDialogueStart()
    {
        yield return new WaitForSeconds(0.1f);
        SoundsScript.PlaySound("LEVEL START");
        yield return new WaitForSeconds(2f);
        SoundtracksScript.PlaySoundtrack("LEVEL" + gameSystem.CurrentLevelSO.currentZone.ToString());
        yield return new WaitForSeconds(0.9f);

        gameSystem.DialoguePanel.ResetTrigger("Hide");
        gameSystem.DialoguePanel.ResetTrigger("Show");

        playerDialogueArea.enabled = true;
    }

    public void LoadAdventure(float transitionTime)
    {
        StartCoroutine(CRTLoadAdventure(transitionTime));
    }

    public void LoadNextLevel()
    {
        gameSystem.SaveSystem.SaveLocal();
        topBar.SetActive(false);
        bottomBar.SetActive(false);

        if (gameSystem.CurrentLevelSO.currentLevel >= 4) //Max floors == 4 -> editable
        {
            LoadAdventure(5); //time for end level UI menu

            playerLevelInteractions.averageTimePerQuestions();
        }
        else
        {
            StartCoroutine(CRTLoadNextLevel());
        }
    }

    public void LoadPrevLevel()
    {
        gameSystem.SaveSystem.SaveLocal();
        topBar.SetActive(false);
        bottomBar.SetActive(false);

        //---------------------------------------------------------------SoundtracksScript.ReduceVolume();

        if (gameSystem.CurrentLevelSO.currentLevel <= 0)
        {
            LoadAdventure(1);
        }
        else
        {
            StartCoroutine(CRTLoadPrevLevel());
        }
    }

    IEnumerator CRTLoadAdventure(float transitionTime)
    {
        if (transitionTime == -1)
        {
            //---------------------------------------------------------------SoundtracksScript.ReduceVolume();
            Debug.Log("Moriste");
            yield return new WaitForSeconds(2f);
            gameSystem.DialoguePanel.SetTrigger("Hide");
            transitionAnimator.SetBool("lastFloor", false);
            transitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }
        else if (transitionTime == 1)
        {
            Debug.Log("Perdiste la mazmorra");
            yield return new WaitForSeconds(1f);
            gameSystem.DialoguePanel.SetTrigger("Hide");
            transitionAnimator.SetBool("lastFloor", false);
            transitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }
        else if (transitionTime == 5)
        {
            Debug.Log("Ganaste, ver el resumen");
            yield return new WaitForSeconds(1f);
            gameSystem.DialoguePanel.SetTrigger("Hide");
            transitionAnimator.SetBool("lastFloor", true);
            transitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(3.5f);
            transitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }
        else
        {
            Debug.Log("Acabaste el nivel");
            yield return new WaitForSeconds(1f);
            gameSystem.DialoguePanel.SetTrigger("Hide");
            yield return new WaitForSeconds(transitionTime);
            transitionAnimator.SetBool("lastFloor", false);
            transitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }

        gameSystem.DialoguePanel.ResetTrigger("Hide");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(1);
    }

    IEnumerator CRTLoadNextLevel()
    {
        Debug.Log("Subiste de piso");
        yield return new WaitForSeconds(1f);
        gameSystem.DialoguePanel.SetTrigger("Hide");

        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
    }

    IEnumerator CRTLoadPrevLevel()
    {
        Debug.Log("Bajaste de piso");
        yield return new WaitForSeconds(0.7f);
        gameSystem.DialoguePanel.SetTrigger("Hide");

        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
    }
}
