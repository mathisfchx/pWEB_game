using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class UserSelect : MonoBehaviour
{
	//[SerializeField] Inventory inventory ;
	[SerializeField] InputField username ;
    [SerializeField] InputField password ;
    [SerializeField] Button LoginButton ;
    [SerializeField] Button RegisterButton ;
    [SerializeField] Button SaveButton ; 
    [SerializeField] GameObject SCENE ; 
    [SerializeField] GameObject Inventory ;
    //[SerializeField] GameObject PF_Player;
    [SerializeField] GameObject Authentification_menu ; 
    [SerializeField] GameObject game ; 
    [SerializeField] GameObject Network_manager ;
    [Space]
    //[SerializeField] GameObject LoginScene ;
    //[SerializeField] GameObject GameScene ;

    [SerializeField] Text errorMessages ;

    WWWForm form ;
    int counter = 0 ;

    // Start is called before the first frame update
    string URL_Login = "http://13.36.61.82/userSelect.php";
    string URL_Register = "http://13.36.61.82/userinsert.php";
    string URL_Save = "http://13.36.61.82/Save.php";
    public string[] usersData;

    void Start()
    {
        //GameScene.setActive(false);
        //LoginScene.setActive(false);

        LoginButton.onClick.AddListener(CoroutineButtonLogin);
        RegisterButton.onClick.AddListener(CoroutineButtonRegister);
        SaveButton.onClick.AddListener(CoroutineButtonSave);
        SCENE.SetActive(false);
        //PF_Player.SetActive(false);
        game.SetActive(false); 
        Inventory.SetActive(false);
        Network_manager.SetActive(false);
        Authentification_menu.SetActive(true);


    }

    void CoroutineButtonLogin()
    {
        LoginButton.interactable = false ;
        //progressCircle.setActive(true);
        StartCoroutine(Login());
        password.text = "";
    }

    void CoroutineButtonRegister()
    {
        RegisterButton.interactable = false ;
        StartCoroutine(Register());
        password.text ="" ;


    }
    void CoroutineButtonSave()
    {

        SaveButton.interactable = false ; 
        StartCoroutine(Save());

    }


    IEnumerator Login()
    {
        errorMessages.text = "" ;
        form = new WWWForm();

        form.AddField("username",username.text);
        form.AddField("password",password.text);



        using(UnityWebRequest users = UnityWebRequest.Post(URL_Login,form))
        {
            yield return users.SendWebRequest();
            string usersDataString = users.downloadHandler.text;

            if(usersDataString.Contains("Error")){
                errorMessages.text = usersDataString ;
            }
            else{
                //GameScene.setActive(true);
                //LoginScene.setActive(true);
                print("Welcome");
                print(usersDataString);
                string[] values = usersDataString.Split(';');
                /*inventory.health = int.Parse(values[0]);
                inventory.defense = int.Parse(values[1]);
                inventory.speed = int.Parse(values[2]);
                inventory.username = username.text ;*/
                SCENE.SetActive(true);
                Inventory.SetActive(true);
                //PF_Player.SetActive(true);
                game.SetActive(true); 
                Network_manager.SetActive(true);
                Authentification_menu.SetActive(false); 
            }
        }

        LoginButton.interactable = true ;


    }

    IEnumerator Register()
    {
        errorMessages.text = "" ;
        form = new WWWForm();

        form.AddField("addUsername",username.text);
        form.AddField("addPassword",password.text);



        using(UnityWebRequest users =UnityWebRequest.Post(URL_Register,form))
        {
            yield return users.SendWebRequest();
            if (users.error !=null) {
                errorMessages.text = "404 not found";
            }else{
                string usersDataString = users.downloadHandler.text;
                print(usersDataString);

                if(usersDataString.Contains("Error")){
                    errorMessages.text = usersDataString ;
                }
                else{
                    print("Welcome in our register list");
                }
            }
        }
        RegisterButton.interactable = true ;

    }
    IEnumerator Save()
    {

        form = new WWWForm();
        /*form.AddField("SaveHealth",inventory.health);
        form.AddField("SaveDefense",inventory.defense);
        form.AddField("SaveSpeed",inventory.speed);
        form.AddField("Username",inventory.username);*/

        using(UnityWebRequest save = UnityWebRequest.Post(URL_Save,form))
        {
            yield return save.SendWebRequest();
            if(save.error !=null){
                print("Nous n'avons pas pu sauvegarder");
            }else{
                string SaveString = save.downloadHandler.text;
                print(SaveString);
            }





        }

        SaveButton.interactable = true;


    }

    // Update is called once per frame
    void Update()
    {   /*
        if(SCENE.activeSelf)
        {
            if (counter < 1000) {
                counter++ ; 
            }
            else{
                counter = 0 ; 
                StartCoroutine(Save());
            }
        }
        */
    }
}
