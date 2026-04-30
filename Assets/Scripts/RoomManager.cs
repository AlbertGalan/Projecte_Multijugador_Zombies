using System.Numerics;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager sharedInstance;
    public static string sceneToLoad = "GameOnline";
    public Transform spawnPosition;


    void Awake()
    {
        sharedInstance = this;
    }

    void OnEnable()
    {
        //Ens hem de subscirure a l'esdeveniment
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        //Hem d'anular la subscripció a l'esdeveniment
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameOnline")
        {
            //UnityEngine.Vector3 spawnPosition = new UnityEngine.Vector3(Random.Range(-2f, 2f), 1f, Random.Range(-2f, 2f));

            if(PhotonNetwork.InRoom)
            {
                PhotonNetwork.Instantiate("First_Person_Player", spawnPosition.position, spawnPosition.rotation);
            }
            else
            {
                Instantiate(Resources.Load("First_Person_Player"), spawnPosition.position, spawnPosition.rotation);
            }
        }
    }


}
