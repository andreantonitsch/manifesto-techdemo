using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGeneration : MonoBehaviour
{

    public Rect Domain;

    public GameObject BlockPrefab;
    [Header("Block Attributes")]
    [Range(0.0f, 1.0f)]
    public float CutOffsetMax;

    [Range(0.0f, 1.0f)]
    public float CutOffsetMin;

    [Range(0.0f, 1.0f)]
    public float CutVertPercentage;

    public float MinRectSide;

    public int MaxCuts;
    public int RandomCuts;

    public RectSplitter.SplitBalanceMode CutMode;

    public float road_size_aspas = 0.95f;


    [Header("House Attributes")]
    public List<GameObject> HousePrefabs;
    public float HouseWidth;
    public float MainRoadWidth;
    public float InnerRoadWidth;
    public float HouseOccupationPercent;
    

    [Header("Current Attributes")]
    public List<Rect> CurrentBlocks;

    public List<Rect> GenerateBlocks()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        var split_params = new RectSplitter.SplitParams
        {
            CutOffsetMax = CutOffsetMax,
            CutOffsetMin = CutOffsetMin,
            CutVertPercentage = CutVertPercentage,
            MaxCuts = MaxCuts,
            MinRectSide = MinRectSide,
            RandomCuts = RandomCuts,
            CutMode = CutMode
        };

        CurrentBlocks = new List<Rect>();

        CurrentBlocks = RectSplitter.Split(Domain, split_params);
        return CurrentBlocks;
    }


    public void FillBlock(Rect block) 
    {
        bool vert = block.width > block.height;

        float inner_length = vert ?  block.width  : block.height;
        inner_length -= MainRoadWidth;

        int lines = Mathf.FloorToInt(inner_length / (HouseWidth + InnerRoadWidth));

        float cell_size = HouseWidth + InnerRoadWidth;
        Vector2 start_anchor = block.position + new Vector2((MainRoadWidth/2 ), (MainRoadWidth/2)); 
        for (int i = 0; i < lines; i++)
        {
            float anchor_offset = (i * cell_size);
            float house_centering = cell_size / 2;
            Vector2 anchor_offset_vector;
            int houses;
            Vector2 house_offset;
            if (vert)
            {
                anchor_offset_vector = new Vector2(anchor_offset, house_centering);
                houses = Mathf.FloorToInt((block.height - MainRoadWidth) / cell_size);
                house_offset = new Vector2(0, cell_size);
            }
            else
            {
                anchor_offset_vector = new Vector2(house_centering, anchor_offset);
                houses = Mathf.FloorToInt((block.width - MainRoadWidth) / cell_size); 
                house_offset = new Vector2(cell_size, 0);
            }

            Vector2 anchor = start_anchor + anchor_offset_vector;

            for (int j = 0; j < houses; j++)
            {

                Vector3 p = new Vector3(anchor.x + HouseWidth/2, 0, anchor.y + HouseWidth/2) + j * new Vector3(house_offset.x, 0, house_offset.y);
                Vector3 s = new Vector3(HouseWidth, HouseWidth, HouseWidth);

                if (Random.value < HouseOccupationPercent)
                {

                    var h = Instantiate(HousePrefabs[Random.Range(0, HousePrefabs.Count)], p, Quaternion.identity, transform);

                    h.transform.localScale = s;
                }

            }

        }

    }

}
