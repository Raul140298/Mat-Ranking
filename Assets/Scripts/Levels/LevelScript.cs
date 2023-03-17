using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private FromLevelSO fromLevelSO;
    [SerializeField] private CurrentLevelSO currentLevelSO;
    [SerializeField] private SaveSystemScript saveSystem;
    [SerializeField] private GameSystemScript gameSystem;
    [SerializeField] private Text zone, level;
    [SerializeField] private CapsuleCollider2D playerDialogueArea;
    [SerializeField] private Animator dialoguePanel;
    [SerializeField] private GameObject topBar, bottomBar;
    [SerializeField] private SoundtracksScript soundtracks;
    [SerializeField] private LevelInteractionsScript playerLevelInteractions;

    public void StartScene()
    {
        GameSystemScript.Instance.DialogueSystem.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;

        if (fromLevelSO.fromLevel == false)
        {
            currentLevelSO.playerLives = 3;
            fromLevelSO.fromLevel = true;
        }

        currentLevelSO.heart = false;

        currentLevelSO.playerKeyParts = 0;

        playerLevelInteractions.setLives();
        playerLevelInteractions.setKeys();

        CheckIfLevelDataIsEmpty();
    }

    private void CheckIfLevelDataIsEmpty()
    {
        //If there aren't enemys in the zone
        if ((currentLevelSO.currentZone == 0 &&
            gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].selected == false && //L1
            (gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].selected == false || //L2 or
            (gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].selected == false && //L2.1
            gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].selected == false))) ||//L2.2

            (currentLevelSO.currentZone == 1 &&
            gameSystem.RemoteSO.dgbl_features.ilos[1].ilos[0].selected == false && //L8
            gameSystem.RemoteSO.dgbl_features.ilos[1].ilos[1].selected == false) || //L9

            (currentLevelSO.currentZone == 2 &&
            (gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].selected == false || //L13
            (gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].selected == false && //L13.1
            gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].selected == false && //L13.2
            gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].selected == false))) || //L13.3

            (currentLevelSO.currentZone == 3 &&
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
            zone.text += " " + (currentLevelSO.currentZone + 1).ToString();
            level.text += " " + currentLevelSO.currentLevel.ToString();

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
        SoundtracksScript.PlaySoundtrack("LEVEL" + currentLevelSO.currentZone.ToString());
        yield return new WaitForSeconds(0.9f);

        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel").transform.GetChild(1).GetComponent<Animator>();
        dialoguePanel.ResetTrigger("Hide");
        dialoguePanel.ResetTrigger("Show");

        playerDialogueArea.enabled = true;
    }

    public void LoadAdventure(float transitionTime)
    {
        StartCoroutine(CRTLoadAdventure(transitionTime));
    }

    public void LoadNextLevel()
    {
        saveSystem.SaveLocal();
        topBar.SetActive(false);
        bottomBar.SetActive(false);

        if (currentLevelSO.currentLevel >= 4) //Max floors == 4 -> editable
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
        saveSystem.SaveLocal();
        topBar.SetActive(false);
        bottomBar.SetActive(false);

        soundtracks.ReduceVolume();

        if (currentLevelSO.currentLevel <= 0)
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
            soundtracks.ReduceVolume();
            Debug.Log("Moriste");
            yield return new WaitForSeconds(2f);
            dialoguePanel.SetTrigger("Hide");
            transitionAnimator.SetBool("lastFloor", false);
            transitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }
        else if (transitionTime == 1)
        {
            Debug.Log("Perdiste la mazmorra");
            yield return new WaitForSeconds(1f);
            dialoguePanel.SetTrigger("Hide");
            transitionAnimator.SetBool("lastFloor", false);
            transitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }
        else if (transitionTime == 5)
        {
            Debug.Log("Ganaste, ver el resumen");
            yield return new WaitForSeconds(1f);
            dialoguePanel.SetTrigger("Hide");
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
            dialoguePanel.SetTrigger("Hide");
            yield return new WaitForSeconds(transitionTime);
            transitionAnimator.SetBool("lastFloor", false);
            transitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }

        dialoguePanel.ResetTrigger("Hide");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(1);
    }

    IEnumerator CRTLoadNextLevel()
    {
        Debug.Log("Subiste de piso");
        yield return new WaitForSeconds(1f);
        dialoguePanel.SetTrigger("Hide");

        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
    }

    IEnumerator CRTLoadPrevLevel()
    {
        Debug.Log("Bajaste de piso");
        yield return new WaitForSeconds(0.7f);
        dialoguePanel.SetTrigger("Hide");

        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
    }
}
