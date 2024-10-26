using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{

    //Handles Game States
    public static GameManager current;

    public GameObject PauseMenuref;
    public bool gameOverBool = false;
    public GameObject GameEndref;
    public GameObject blackfade;
    public AudioSource audiosourceref;
    public GameObject gameendrefpic;
    public GameObject youwonrefpic;
    private bool endgamelocker = false;
    public enum GameState
    {
        Playing,      //Playing Core loop
        Options,       //player is adjusting game options
        Pause,         //player paused the game
        Exit,          //player exiting game
    }


    private void Awake()
    {
        current = this;
    }

    void Start()
    {
    }

    private void OnDestroy()
    {

    }
    void Update()
    {
       /* if(Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 10;
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            EventManager.current.YouWon();
        }*/
        UpdateGameState();
    }

    //STATE FUNCTIONS--------------------------
    #region PlayingFunctions

    public void Playing_PauseGame()
    {
        
        Time.timeScale = 0f;
        PauseMenuref.SetActive(true);
        CurrentGameState = GameState.Pause;

    }

    public void Playing_GameOver()
    {
        if (endgamelocker == false)
        {
            endgamelocker = true;
            Time.timeScale = 0f;
            gameOverBool = true;
            gameendrefpic.SetActive(true);
        }
    }
    public void Playing_YouWon()
    {
      if(endgamelocker == false)
        {
            endgamelocker = true;
            Time.timeScale = 0f;
            gameOverBool = true;
            youwonrefpic.SetActive(true);
        }
        
    }

    public void Playing_RestartGame()
    {
       
        Time.timeScale = 1f;
        StartCoroutine(TransitionToRestart());
    }


    #endregion


    #region PauseFunctions
   public void Paused_UnpauseGame()
    {
       
        Time.timeScale = 1f;
        PauseMenuref.SetActive(false);
        CurrentGameState = GameState.Playing;
    }
    IEnumerator TransitionToRestart()
    {
        Time.timeScale = 1f;
        blackfade.GetComponent<Animator>().SetTrigger("FadeOut");
        //StartCoroutine(FadeAudioSource.StartFade(audiosourceref, 4, 0));
        
        yield return new WaitForSeconds(3);
        //Time.timeScale = 1f;
        blackfade.GetComponent<Animator>().SetTrigger("FadeIn");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    IEnumerator TransitiontoMain()
    {

        Time.timeScale = 1f;
        blackfade.GetComponent<Animator>().SetTrigger("FadeOut");
       // StartCoroutine(FadeAudioSource.StartFade(audiosourceref, 4, 0));
      
        yield return new WaitForSeconds(3);
        blackfade.GetComponent<Animator>().SetTrigger("FadeIn");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }
    public void Paused_ReturntoMainMenu()
    {
      
        Time.timeScale = 1f;
        StartCoroutine(TransitiontoMain());  
    }

    public void Paused_Quit()
    {
        Application.Quit();
    }
    #endregion


    #region OptionsFunctions

    #endregion


    #region ExitFunctions
    #endregion

    public GameState CurrentGameState = GameState.Playing;
    public void UpdateGameState()
    {
        switch (CurrentGameState)
        {
            case GameState.Playing:
                {
                   
                    //Check for Pause
                    if (Input.GetKeyDown(KeyCode.Escape))
                        Playing_PauseGame();

                   if(gameOverBool == true)
                    {
                        //end game
                        GameEndref.SetActive(true);
                    }
                }

                break;
            case GameState.Options:
                {
                    
                    //In Main Menu
                }

                break;
            case GameState.Pause:
                {
                  
                    //Check for Pause
                    if (Input.GetKeyDown(KeyCode.Escape))
                        Paused_UnpauseGame();
                }

                break;

            case GameState.Exit:
                {


                }

                break;
        }



    }
 
    
}
