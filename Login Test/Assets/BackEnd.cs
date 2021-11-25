using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class BackEnd : MonoBehaviour
{
    const string ver = "0.1a";

    const string DatabaseLink = 
        "https://docs.google.com/spreadsheets/d/1qzc_NXa0slJjoGExU37vnw0cmDaYvsT70XZK29i8n1I/export?format=tsv";

    const string ScriptLink =
        "https://script.google.com/macros/s/AKfycbyc3kLfHJOdIZ5klrKfoMZSDAyfGnocv24dTIB5ynV6zTVzs-U/exec";
         
    [SerializeField]
    TMP_InputField IDInput;

    [SerializeField]
    TMP_InputField PassInput;

    [SerializeField]
    TextMeshPro[] AccountInfo;

    string id, pass;

    public bool isOnline;

    void Start()
    {
        foreach (TextMeshPro tmp in AccountInfo)
        {
            tmp.text = "----";
        }
        isOnline = CheckOnline();

        WWWForm form = new WWWForm();
        form.AddField("order", "version");
        form.AddField("version", ver);

        StartCoroutine(VersionPost(form));
    }

    private bool CheckOnline()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return false;
        else return true;
    }

    private bool SetIDPass()
    {
        id = IDInput.text.Trim();
        pass = PassInput.text.Trim();

        if (id == "" || pass == "") return false;
        else return true;
    }

    public void Login()
    {
        if (!SetIDPass())
        {
            return;
        }

        WWWForm form = new WWWForm();
        form.AddField("order", "login");
        form.AddField("id", id);
        form.AddField("pass", pass);

        StartCoroutine(LoginPost(form));
    }

    public void Register()
    {
        if (!SetIDPass())
        {
            return;
        }

        WWWForm form = new WWWForm();
        form.AddField("order", "register");
        form.AddField("id", id);
        form.AddField("pass", pass);

        StartCoroutine(Post(form));
    }

    IEnumerator VersionPost(WWWForm form)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(ScriptLink, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
            {
                print(www.downloadHandler.text);
                if (www.downloadHandler.text == "true") print("true");
                else StartCoroutine(SystemShutdown());
            }
            else
            {

            }
        }
    }

    IEnumerator LoginPost(WWWForm form)
    {
        char seperator = ',';
        string[] textLines;
        using (UnityWebRequest www = UnityWebRequest.Post(ScriptLink, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
            {
                print(www.downloadHandler.text);
                textLines = www.downloadHandler.text.Split(seperator);

                print("Force는 " + textLines[0] + "입니다");
                print("닉네임는 " + textLines[1] + "입니다");
                print("소지 골드량은 " + textLines[2] + "입니다");
                print("소지 보석량은 " + textLines[3] + "입니다");
                print("계정 레벨은 " + textLines[4] + "입니다");
                print("계정 경험치는 " + textLines[5] + "입니다");

                for (int i = 0; i < 6; i++)
                {
                    AccountInfo[i].text = textLines[i];
                }
            }
            else
            {

            }
        }
    }

    IEnumerator Post(WWWForm form)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(ScriptLink, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
            {
                print(www.downloadHandler.text);
            }
            else
            {

            }
        }
    }

    IEnumerator SystemShutdown()
    {
        Debug.LogError("버전이 일치하지 않습니다.\nEnter를 누르거나 화면을 클릭해 시스템을 종료합니다.");
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit(); // 어플리케이션 종료
#endif
            }
            yield return null;
        }
    }

    IEnumerator DatabaseStart()
    {
        UnityWebRequest www = UnityWebRequest.Get(DatabaseLink);
        yield return www.SendWebRequest();

        string data;
        data = www.downloadHandler.text;
        print(data);
    }
}
