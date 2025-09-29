using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button ruleBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button backBtn;

    private enum Sub
    {
        Main,
        Rule
    }

    private void Awake()
    {
        playBtn.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        });

        ruleBtn.onClick.AddListener(() =>
        {
            ShowSub(Sub.Rule);
        });

        backBtn.onClick.AddListener(() =>
        {
            ShowSub(Sub.Main);
        });

        quitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        ShowSub(Sub.Main);
    }

    private void ShowSub(Sub sub)
    {
        transform.Find("mainSub").gameObject.SetActive(false);
        transform.Find("ruleSub").gameObject.SetActive(false);

        switch (sub)
        {
            case Sub.Main:
                transform.Find("mainSub").gameObject.SetActive(true);
                break;
            case Sub.Rule:
                transform.Find("ruleSub").gameObject.SetActive(true);
                break;
        }
    }
}
