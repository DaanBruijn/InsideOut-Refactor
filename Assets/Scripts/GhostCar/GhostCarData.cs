using UnityEngine;
using System.Collections.Generic;

// - Data script for the Ghost car
// - Daniel Bruijn

[System.Serializable]
public class GhostCarData
{
    // - Variables
    [SerializeField] List<GhostCarDataListItem> ghostCarRecorderList = new List<GhostCarDataListItem>();

    public void AddDataItem(GhostCarDataListItem ghostCarDataListItem)
    {
        ghostCarRecorderList.Add(ghostCarDataListItem);
    }

    public List<GhostCarDataListItem> GetDataList()
    {
        return ghostCarRecorderList;
    }
}
