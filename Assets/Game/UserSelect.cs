using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Mirror;

namespace Game
{

    public class UserSelect : MonoBehaviour
    {
        //[SerializeField] Inventory inventory ;
        [SerializeField] InputField username ;
        [SerializeField] InputField password ;
        [SerializeField] Button LoginButton ;
        [SerializeField] Button RegisterButton ;
        [SerializeField] Button SaveButton ;
        [SerializeField] Button LogoutButton ;
        [SerializeField] Button TeamA ;
        [SerializeField] Button TeamB ;
        [SerializeField] GameObject SCENE ;
        //[SerializeField] GameObject Inventory ;
        //[SerializeField] GameObject PF_Player;
        [SerializeField] GameObject Authentification_menu ;
        [SerializeField] GameObject game ;
        [SerializeField] GameObject Network_manager ;
        [SerializeField] Connection_tab conn_tab;
        [SerializeField] GameObject Background ;
        Mirror.NetworkManager manager;
        [Space]
        //[SerializeField] GameObject LoginScene ;
        //[SerializeField] GameObject GameScene ;

        [SerializeField] Text errorMessages ;

        public int team;

        WWWForm form ;
        int counter = 0 ;

        // Start is called before the first frame update
        string URL_Login = "http://13.36.61.82/userSelect.php";
        string URL_Register = "http://13.36.61.82/userinsert.php";
        string URL_Save = "http://13.36.61.82/Save.php";
        string URL_Disconnect = "http://13.36.61.82/userDisconnect.php";
        public string[] usersData;
        public string UsernameString ;

        void Start()
        {
            //GameScene.setActive(false);
            //LoginScene.setActive(false);

            manager = Network_manager.GetComponent<Mirror.NetworkManager>();

            LoginButton.onClick.AddListener(CoroutineButtonLogin);
            RegisterButton.onClick.AddListener(CoroutineButtonRegister);
            SaveButton.onClick.AddListener(CoroutineButtonSave);
            SCENE.SetActive(false);
            //PF_Player.SetActive(false);
            game.SetActive(false);
            //Inventory.SetActive(false);
            Network_manager.SetActive(false);
            Authentification_menu.SetActive(true);

            LogoutButton.onClick.AddListener(CoroutineButtonLogout);
            LogoutButton.interactable = false ;

            TeamA.onClick.AddListener(CoroutineTeamA);
            TeamA.gameObject.SetActive(false);

            TeamB.onClick.AddListener(CoroutineTeamB);
            TeamB.gameObject.SetActive(false);

            team = 2;
        }

        void CoroutineTeamA()
        {
            team = 3;
            //conn_tab.nA += 1;
            Destroy(GameObject.Find("TeamA"));
            Destroy(GameObject.Find("TeamB"));
            
        }

        void CoroutineTeamB()
        {
            team = 1;
            //conn_tab.nB += 1;
            Destroy(GameObject.Find("TeamA"));
            Destroy(GameObject.Find("TeamB"));
            
        }

        void CoroutineButtonLogin()
        {
            LoginButton.interactable = false ;
            //progressCircle.setActive(true);
            StartCoroutine(Login());
            password.text = "";
            LogoutButton.interactable = true ;
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

        void CoroutineButtonLogout()
        {
            SCENE.SetActive(false);
            game.SetActive(false);
            Network_manager.SetActive(false);
            Authentification_menu.SetActive(true);

            LogoutButton.interactable = false ;
            LoginButton.interactable = true ;

            Destroy(GameObject.Find("Player(Clone)"));
            Destroy(GameObject.Find("Inventory(Clone)"));
            Destroy(GameObject.Find("Camera(Clone)"));
            Destroy(GameObject.Find("HUDInventory(Clone)"));

            manager.StopHost();

            Application.Quit();
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
                    UsernameString = username.text;
                    /*inventory.health = int.Parse(values[0]);
                    inventory.defense = int.Parse(values[1]);
                    inventory.speed = int.Parse(values[2]);
                    inventory.username = username.text ;*/

                    //Inventory.SetActive(true);
                    //PF_Player.SetActive(true);
                    SCENE.SetActive(true);
                    game.SetActive(true);
                    Network_manager.SetActive(true);
                    Authentification_menu.SetActive(false);
                    Background.SetActive(false);
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
                    form.AddField("addLevel", 1);
                    form.AddField("addExperience", 0);



            using(UnityWebRequest users =UnityWebRequest.Post(URL_Register,form))
            {
                yield return users.SendWebRequest();
                if (users.error !=null) {
                    errorMessages.text = "404 not found";
                                    print(users.error);
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

        public void CoroutineDisconnect(string username){
            //StartCoroutine(Disconnect(username));
        }

        public IEnumerator Disconnect(string username)
        {

            form = new WWWForm();
            form.AddField("Username",username);

            using(UnityWebRequest save = UnityWebRequest.Post(URL_Disconnect,form))
            {
                yield return save.SendWebRequest();
                if(save.error !=null){
                    print("Nous n'avons pas pu sauvegarder");
                }else{
                    string SaveString = save.downloadHandler.text;
                    print(SaveString);
                }





            }

            //SaveButton.interactable = true;

        }
    }
}