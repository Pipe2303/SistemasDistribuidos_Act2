using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class HTTPManager : MonoBehaviour
{
    private string fakeApiLink = "https://my-json-server.typicode.com/pipe2303/SistemasDistribuidos_Act2";
    private string rickMortyLink = "https://rickandmortyapi.com/api";

    public RawImage[] cardsImages = new RawImage[5];
    public TextMeshProUGUI[] cardsNames = new TextMeshProUGUI[5];
    public TextMeshProUGUI[] cardsSpecies = new TextMeshProUGUI[5];
    
    public void SendRequest(int userID)
    {
        StartCoroutine(GetUserInfo(userID));
    }
    IEnumerator DownloadImage(string url, int indexDeck)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            cardsImages[indexDeck].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
    IEnumerator GetUserInfo (int userID)
    {
        UnityWebRequest request = UnityWebRequest.Get(fakeApiLink + "/users/" + userID);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if(request.responseCode == 200)
            {
                int imagesCount = 0;
                UserInfo user = JsonUtility.FromJson<UserInfo>(request.downloadHandler.text);
                foreach(int cardID in user.deck)
                {
                    StartCoroutine(GetCharacter(cardID, imagesCount));
                    imagesCount++;
                }
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }
    IEnumerator GetCharacter(int cardId, int indexDeck)
    {
        UnityWebRequest request = UnityWebRequest.Get(rickMortyLink + "/character/" + cardId);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Code server: " + request.responseCode);

            if (request.responseCode == 200)
            {
                Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);
                cardsNames[indexDeck].text = character.name;
                cardsSpecies[indexDeck].text = character.species;
                StartCoroutine(DownloadImage(character.image, indexDeck));              
            }
        }
    }
}
[System.Serializable]
public class UserInfo
{
    public int[] deck;
    public int id;
    public string name;
}
[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}
