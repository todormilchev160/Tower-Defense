using UnityEngine;

public class RallyButton : MonoBehaviour
{
    public void PressRallyButton()
    {
        // Find all Barracks currently in the scene
        Barracks[] allBarracks =
            FindObjectsByType<Barracks>(
                FindObjectsSortMode.None
            );


        if (allBarracks.Length == 0)
        {
            Debug.LogWarning(
                "No Barracks found!"
            );

            return;
        }


        Barracks closestBarracks = null;

        float closestDistance =
            Mathf.Infinity;


        // Find Barracks closest to THIS button
        foreach (Barracks barracks in allBarracks)
        {
            float distance =
                Vector3.Distance(
                    transform.position,
                    barracks.transform.position
                );


            if (distance < closestDistance)
            {
                closestDistance =
                    distance;

                closestBarracks =
                    barracks;
            }
        }


        if (closestBarracks == null)
            return;


        Debug.Log(
            "Rally button selected: " +
            closestBarracks.name
        );


        // Tell the global RallyPointManager
        if (TroopRallyPoint.Instance != null)
        {
            TroopRallyPoint.Instance
                .StartRallyPointSelection(
                    closestBarracks
                );
        }
        else
        {
            Debug.LogWarning(
                "No TroopRallyPoint manager found!"
            );
        }
    }
}