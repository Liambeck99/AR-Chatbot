using IBM.Watson.NaturalLanguageUnderstanding.V1;
using IBM.Watson.NaturalLanguageUnderstanding.V1.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Cloud.SDK;
using System.IO;

public class NaturalLanguageUnderstanding 
{
    private string iamApikey = "XeATNGzUjO_yE_x2Owo3LGEgcKx1wfPMhNKGHnavnHOC";
    private string serviceUrl = "https://api.us-south.natural-language-understanding.watson.cloud.ibm.com/instances/26adb4e6-65ba-4a76-ad91-9140e8d38edf";
    private string versionDate = "2021-02-22";

    private NaturalLanguageUnderstandingService service;
    private string nluText;

    public AnalysisResults analyzeResponse = null;

    public NaturalLanguageUnderstanding()
    {
        
    }

    public IEnumerator NLURun(string text)
    {
        nluText = text;

        if (string.IsNullOrEmpty(iamApikey))
        {
            throw new IBMException("Please add IAM ApiKey to the Iam Apikey field in the inspector.");
        }

        IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

        while (!authenticator.CanAuthenticate())
        {
            yield return null;
        }


        service = new NaturalLanguageUnderstandingService(versionDate, authenticator);
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            service.SetServiceUrl(serviceUrl);
        }

        Features features = new Features()
        {
            Keywords = new KeywordsOptions()
            {
                Limit = 20
            }
        };

        analyzeResponse = null;

        service.Analyze(
            callback: (DetailedResponse<AnalysisResults> response, IBMError error) =>
            {
                Log.Debug("NaturalLanguageUnderstandingServiceV1", "Analyze result: {0}", response.Response);

                Debug.Log("User NLU Message: " + response.Response);
                analyzeResponse = response.Result;

                //save the response in a JSON
                NLUSaveToJSON(response.Response);

            },
            features: features,
            text: nluText
        );

        while (analyzeResponse == null)
            yield return null;

    }

    public void NLUSaveToJSON(string data)
    {    
        Debug.Log("Saving NLU JSON");
        WriteToFile("NLUResponse.json", data);   
    }

    private void WriteToFile(string fileName, string json)
    {
        string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using(StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }

        fileStream.Dispose();
    }

    public string GetFilePath(string fileName)
    {
        Debug.Log("NLU filepath is: " + (Application.persistentDataPath + "/" + fileName));
        return Application.persistentDataPath + "/" + fileName;
    }

}
