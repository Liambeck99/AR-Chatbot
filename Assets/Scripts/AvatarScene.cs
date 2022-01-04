using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AvatarScene : BaseSessionScene
{
    private void Start()
    {
        ConfigureInputs();
        ConfigureConversation();
    }

    private void Update()
    {

    }

    public override void OnKeyboardSubmit(string message)
    {

    }
}