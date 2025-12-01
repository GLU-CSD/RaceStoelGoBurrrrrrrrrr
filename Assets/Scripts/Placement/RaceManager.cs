using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class RaceManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlacementText;
    [SerializeField] private CarProgress player;
    [SerializeField] private CarProgress[] aiCars;

    [SerializeField] private int playerPosition;

    private void Update()
    {
        List<CarProgress> allCars = new List<CarProgress>();
        allCars.Add(player);
        allCars.AddRange(aiCars);

        allCars = allCars.OrderByDescending(c => c.totalProgress).ToList();

        playerPosition = allCars.IndexOf(player) + 1;

        PlacementText.text = playerPosition.ToString();
    }
}
