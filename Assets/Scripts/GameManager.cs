using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  

public class GameManager : MonoBehaviour
{
    public List<Sprite> firstImagePool;  
    public List<Sprite> secondImagePool;  
    public GameObject imagePrefab;        
    public Transform firstSpawnParent;    
    public Transform secondSpawnParent;   
    public float firstSetSpacing = 150f;  
    public float secondSetSpacing = 200f; 
    public Canvas mainCanvas;             

    private List<int> selectedIndices = new List<int>(); 
    private List<GameObject> spawnedImages = new List<GameObject>(); 
    private GameObject highlightedImage; 
    private int wrongSelectionCount = 0;

    void Start()
    {        
        SpawnRandomImages(firstImagePool, 3, firstSpawnParent, firstSetSpacing, selectedIndices);
        HighlightRandomImage();        
        SpawnRandomImagesFromSelectedThree();
    }

    void Update()
    {      
        if (Input.GetMouseButtonDown(0))
        {
            DetectAndCompareImage();
        }
    }

    void SpawnRandomImages(List<Sprite> imagePool, int count, Transform parent, float spacing, List<int> storeIndices)
    {
        float totalWidth = (count - 1) * spacing;
        float startX = -totalWidth / 2; 
        HashSet<int> selected = new HashSet<int>();  
        for (int i = 0; i < count; i++)
        {
            int randomIndex;            
            do
            {
                randomIndex = Random.Range(0, imagePool.Count);
            } while (selected.Contains(randomIndex));
            selected.Add(randomIndex);
            storeIndices.Add(randomIndex);  
            GameObject newImage = Instantiate(imagePrefab, parent);
            newImage.GetComponent<Image>().sprite = imagePool[randomIndex];            
            spawnedImages.Add(newImage);
            RectTransform imageRect = newImage.GetComponent<RectTransform>();
            imageRect.anchoredPosition = new Vector2(startX + i * spacing, 0);
        }
    }

    
    void HighlightRandomImage()
    {
        if (spawnedImages.Count > 0)
        {
            int randomIndex = Random.Range(0, spawnedImages.Count);
            highlightedImage = spawnedImages[randomIndex];    
            Outline outline = highlightedImage.AddComponent<Outline>();           
            outline.effectColor = Color.black;  
            outline.effectDistance = new Vector2(10, 10);  
        }
    }

    void SpawnRandomImagesFromSelectedThree()
    {
        if (selectedIndices.Count == 0) return;
        float totalWidth = (7 - 1) * secondSetSpacing;
        float startX = -totalWidth / 2;  
        for (int i = 0; i < 7; i++)
        {
            int randomIndex = selectedIndices[Random.Range(0, selectedIndices.Count)];   
            GameObject newImage = Instantiate(imagePrefab, secondSpawnParent);
            newImage.GetComponent<Image>().sprite = secondImagePool[randomIndex];  
            RectTransform imageRect = newImage.GetComponent<RectTransform>();
            imageRect.anchoredPosition = new Vector2(startX + i * secondSetSpacing, 0);
        }
    }

    void DetectAndCompareImage()
    {        
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;   
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster raycaster = mainCanvas.GetComponent<GraphicRaycaster>();
        raycaster.Raycast(pointerData, results);
        if (results.Count > 0)
        {
            RaycastResult result = results[0];  
            GameObject clickedImage = result.gameObject;  
            Image clickedImageComponent = clickedImage.GetComponent<Image>();
            Image highlightedImageComponent = highlightedImage.GetComponent<Image>();
            if (clickedImageComponent.sprite.name == highlightedImageComponent.sprite.name)
            {
                Destroy(clickedImage);
                Debug.Log("Correct image clicked. Destroyed!");   
                wrongSelectionCount = 0;
            }
            else
            {
                Debug.Log("Wrong image clicked.");  
                wrongSelectionCount++;
                if (wrongSelectionCount > 1)
                {
                    Debug.Log("More than 2 wrong images selected. Respawning new set of images.");
                    RespawnNewSetOfImages();     
                    wrongSelectionCount = 0;
                }
            }
        }
    }


    void SpawnNewRandomImageAtPosition(Vector3 position)
    {
        int randomIndex = Random.Range(0, secondImagePool.Count);   
        GameObject newImage = Instantiate(imagePrefab, secondSpawnParent);
        newImage.GetComponent<Image>().sprite = secondImagePool[randomIndex];
        RectTransform newImageRect = newImage.GetComponent<RectTransform>();
        newImageRect.position = position;
        Debug.Log("New image spawned at position of destroyed image.");
    }

    private Vector2[] fixedPositions = new Vector2[]
    {
    new Vector2(-600, 0),   
    new Vector2(-400, 0),   
    new Vector2(-200, 0),   
    new Vector2(0, 0),      
    new Vector2(200, 0),    
    new Vector2(400, 0),    
    new Vector2(600, 0)     
    };

    void RespawnNewSetOfImages()
    {       
        foreach (Transform child in secondSpawnParent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < 7; i++)
        {
            int randomIndex = Random.Range(0, secondImagePool.Count);
            GameObject newImage = Instantiate(imagePrefab, secondSpawnParent);
            newImage.GetComponent<Image>().sprite = secondImagePool[randomIndex];
            RectTransform newImageRect = newImage.GetComponent<RectTransform>();
            newImageRect.anchoredPosition = fixedPositions[i];
        }
        Debug.Log("New set of 7 images spawned.");
    }
}
