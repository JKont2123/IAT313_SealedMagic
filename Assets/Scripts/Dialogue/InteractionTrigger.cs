using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public GameObject interactionCue;
    bool canInteract;

    [SerializeField] private TextAsset inkJson;


    // Start is called before the first frame update
    void Start()
    {
        interactionCue.SetActive(false);
        canInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            interactionCue.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJson);
            }
        }
        else
        {
            interactionCue.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {           
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canInteract = false;
        }
    }
}
