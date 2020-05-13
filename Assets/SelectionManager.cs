using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// USE SPACE TO SELECT PLANE
public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material defaultMaterial;


    private Transform _selection;

    // private static List<GameObject> selected = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Deselect
        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            var agentScript = _selection.GetComponent<Agent>();
            // try
            // {
            //     selectionRenderer.materials[1] = defaultMaterial;
            //     _selection = null;
            //     // Debug.Log("22");
            // }
            // catch (System.Exception)
            // {
                
            //     // throw;
            // }
            if (agentScript != null)
            {
                bool isSelected = agentScript.isSelected;
                if (! isSelected)
                {
                    selectionRenderer.material = defaultMaterial;
                    // if !isSeleced?
                    _selection = null;
                }
            }

            
        }
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Select
        if (Physics.Raycast(ray, out hit))
        {
            var selection = hit.transform;
            var selectionRenderer = selection.GetComponent<Renderer>();
            var agentScript = selection.GetComponent<Agent>();
            if (selectionRenderer != null)
            {
                // try
                // {
                //     selectionRenderer.materials[1] = highlightMaterial;
                //     Debug.Log("33");
                // }
                // catch (System.Exception)
                // {
                    
                //     // throw;
                // }
                if (agentScript != null)
                {
                    if (!agentScript.isSelected)
                    {
                        selectionRenderer.material = highlightMaterial;
                    }
                    
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (agentScript.isSelected)
                        {
                            // selected.Add(selection.GetComponent<GameObject>());
                            // A bool for isSelected?
                            agentScript.isSelected = false;
                            selectionRenderer.material = defaultMaterial;
                        }
                        else
                        {
                            // REMOVE
                            // selected.Remove(selection.GetComponent<GameObject>());
                            // A bool for isSelected?
                            agentScript.isSelected = true;
                            selectionRenderer.material = selectedMaterial;
                        }

                    }
                }


            }
            
            _selection = selection;
        }
    }
}
