using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class BackEnd : MonoBehaviour
{

    const string DatabaseLink = 
        "https://docs.google.com/spreadsheets/d/1qzc_NXa0slJjoGExU37vnw0cmDaYvsT70XZK29i8n1I/export?format=tsv";

    const string ScriptLink =
        "https://script.google.com/macros/s/AKfycbyc3kLfHJOdIZ5klrKfoMZSDAyfGnocv24dTIB5ynV6zTVzs-U/exec";
         
    [SerializeField]
    TMP_InputField IDInput;

    [SerializeField]
    TMP_InputField PassInput;

    string id, pass;

    static bool isOnline;

    void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 오프라인모드 실행
            // StartCoroutine(SystemShutdown());
            isOnline = false;
        }
        else
        {
            // 데이터베이스 접근 시작
            StartCoroutine(DatabaseStart());
            isOnline = true;
        }
    }

    void Update()
    {
        
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

        StartCoroutine(Post(form));
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
        Debug.LogError("인터넷에 연결되있지 않습니다.\nEnter를 눌러 시스템을 종료합니다.");
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
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
