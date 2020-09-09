using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LoadInputField : MonoBehaviour
{
    PlayerData a;


    //string path = Application.persistentDataPath + "/";
    //string fileName = Application.companyName + "." + Application.productName + ".savedata.json";

    const string AES_KEY = "8sm9Kusy121Z95QQ";
    const string AES_IV = "UA5498MjET4gao7Q";

    [SerializeField] GameObject stringInput;
    [SerializeField] GameObject intInput;
    [SerializeField] GameObject floatInput;
    [SerializeField] GameObject saveButton;
    [SerializeField] GameObject loadButton;
    [SerializeField] Text text;

    [System.Serializable]
    class PlayerData
    {
        public string name="なし";
        public int money=0;
        public float hight=0;
        public int[] arrayNum = new int[10];
    }





    string PATH;
    string FILE_NAME;


    PlayerData playerData;
    
    private void Awake()
    {
        PATH = Application.persistentDataPath + "/";
        FILE_NAME = Application.companyName + "." + Application.productName + ".GamaData";

        playerData = new PlayerData();
        stringInput.GetComponent<InputField>().onEndEdit.AddListener((a) => SaveString());
        floatInput.GetComponent<InputField>().onEndEdit.AddListener((a) => SaveFloat());
        intInput.GetComponent<InputField>().onEndEdit.AddListener((a) => SaveInt());
        saveButton.GetComponent<Button>().onClick.AddListener(() => SaveData.SavePlayerData(playerData, PATH + FILE_NAME, AES_IV, AES_KEY));
        loadButton.GetComponent<Button>().onClick.AddListener(() => SaveData.LoadPlayerData(ref playerData, PATH + FILE_NAME, AES_IV, AES_KEY));

        test2();

    }

    void Update()
    {
        text.text = "名前は" + playerData.name + "\nお金は" + playerData.money + "\n高さは" + playerData.hight;
    }


    void SaveString()
    {
        InputField inputField = stringInput.GetComponent<InputField>();
        if(inputField.text != null)
        {
            playerData.name = inputField.text;
        }

    }

    void SaveInt()
    {
        InputField inputField = intInput.GetComponent<InputField>();
        int num;
        if (int.TryParse(inputField.text,out num))
        {
            playerData.money = num;
        }

    }

    void SaveFloat()
    {
        InputField inputField = floatInput.GetComponent<InputField>();
        float num;
        if (float.TryParse(inputField.text, out num))
        {
            playerData.hight= num;
        }

    }



    void test()
    {
        try
        {
            a = new PlayerData();
            Debug.Log(JsonUtility.ToJson(a));
            a = JsonUtility.FromJson<PlayerData>("{ \"name\":\"最強\",\"money\":19292,\"hight\":999.666}");
        }
        catch
        {
            Debug.Log("huseinaSaveData");

        }
        Debug.Log(a.name + a.money + a.hight);
    }

    void test2()
    {
        string iv = "pppppppppppppppp";
        string key = "cccccccccccccccc";
        try
        {
            a = new PlayerData();
            PlayerData b = new PlayerData();

            a.name = "配列君";
            a.money = 6666;
            a.hight = 161.2f;
            for (int i = 0; i < 10; i++)
            {
                a.arrayNum[i] = i + 1;
            }

            SaveData.SavePlayerData<PlayerData>(a, PATH + "TestSaveData", iv,key);

            SaveData.LoadPlayerData<PlayerData>(ref b, PATH + "TestSaveData", iv,key);

            for (int i = 0; i < 10; i++)
            {
                Debug.Log(b.arrayNum[i]);
            }


        }
        catch
        {
            Debug.Log("huseinaSaveData");

        }
        Debug.Log(a.name + a.money + a.hight);
    }


}
