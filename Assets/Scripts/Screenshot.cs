using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ScreenCapture : MonoBehaviour
{
    private const string API_KEY = "e682918fcbf644bb86565f02a2ab7adb";
    private const string VISION_API_URL = "https://aoai-techvswild-swedencentral-hackathon-001.openai.azure.com/openai/deployments/gpt4-002/chat/completions?api-version=2023-12-01-preview";
    [SerializeField] private GameObject windowScreen;
    [SerializeField] private GameObject newObject;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text result;
    [SerializeField] private float despawnPanelDelay = 5f;

    private float timer = 0f;
    private bool captureStarted = false;
    private string imgBase64 = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5f && !captureStarted)
        {
            captureStarted = true;
            CaptureAndApplyTexture();
        }
    }

    void CaptureAndApplyTexture()
    {
        if (windowScreen != null && windowScreen.GetComponent<MeshRenderer>() != null && windowScreen.GetComponent<MeshRenderer>().material != null && windowScreen.GetComponent<MeshRenderer>().material.mainTexture != null)
        {
            // Capture la texture de l'objet WindowScreen
            RenderTexture renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            Graphics.Blit(windowScreen.GetComponent<MeshRenderer>().material.mainTexture, renderTexture);

            // Cr�er une nouvelle Texture2D pour stocker la texture captur�e
            Texture2D capturedTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);

            // S�lectionner le rendu actif
            RenderTexture.active = renderTexture;

            // Capturer la texture
            capturedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            capturedTexture.Apply();

            // Convertir la texture en tableau de bytes (format JPG)
            byte[] bytes = capturedTexture.EncodeToJPG();

            imgBase64 = Convert.ToBase64String(bytes);

            // Lib�rer la m�moire
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);

            AnalyzeImage(imgBase64);
        }
        else
        {
            Debug.LogError("Le GameObject n'a pas de texture ou le mat�riel n'est pas d�fini.");
        }
    }

    public async void AnalyzeImage(string img)
    {
        Debug.Log(img);
        string jsonRequest = @"
{
    ""messages"": [ 
        {
            ""role"": ""system"", 
            ""content"": ""Je suis un syst�me d'analyse d'image. Veuillez fournir une image 3D � analyser pour d�tecter les risques potentiels ou pas.""
        },
        {
            ""role"": ""user"", 
            ""content"": [
                {
                    ""type"": ""text"",
                    ""text"": ""Veuillez identifier les risques principaux dans cette image en une courte phrase, en �tant concis. Ne prend pas en compte qu'il s'agit d'un environnement virtuel et fait de ton mieux pour quand m�me l'analyser. Si vous ne parvenez pas � les identifier, veuillez expliquer pourquoi :""
                },
                {
                    ""type"": ""image_url"",
                    ""image_url"": {
                        ""url"": ""https://media.discordapp.net/attachments/1092450846303866881/1221417349643632650/image.png?ex=6612808c&is=66000b8c&hm=614d064edb7440240d9a7acb500f740262d3b4fb2db929d0711cdef27f9ca617&=&format=webp&quality=lossless&width=1440&height=622""
                    }
                } 
           ] 
        }
    ],
    ""max_tokens"": 100, 
    ""stream"": false 
}";


        Debug.Log(jsonRequest);

        using (var httpClient = new HttpClient())
        {
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Add("api-key", API_KEY);
            HttpResponseMessage response = await httpClient.PostAsync(VISION_API_URL, content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonUtility.FromJson<RootObject>(responseContent);
                string messageContent = jsonObject.choices[0].message.content;
                float timer = 0f;
                panel.SetActive(true);
                result.text = messageContent;
                despawnPanelDelay -=Time.deltaTime;
                if(despawnPanelDelay <= 0.0f)
                {
                    panel.SetActive(false);
                    despawnPanelDelay = 5f;
                }
                Debug.Log("Response: " + messageContent);
                // Process the response here
            }
            else
            {
                Debug.LogError("Error response: " + response.StatusCode);
            }
        }
    }

    [System.Serializable]
    public class ContentFilterResults
    {
        public bool filtered;
        public string severity;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class Choice
    {
        public string finish_reason;
        public int index;
        public Message message;
        public ContentFilterResults content_filter_results;
    }

    [System.Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

    [System.Serializable]
    public class RootObject
    {
        public string id;
        public string _object;
        public int created;
        public string model;
        public List<object> prompt_filter_results;
        public List<Choice> choices;
        public Usage usage;
    }
}
