﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    private Transform _selection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
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

            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selection = hit.transform;
            var selectionRenderer = selection.GetComponent<Renderer>();
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
                selectionRenderer.material = highlightMaterial;
            }
            
            _selection = selection;
        }
    }
}
