using System;
using UnityEngine;
using TMPro;

namespace BetonMultiplayer
{
    public class Player
    {
        public string name;
        public uint connectionId;
        public GameObject body;

        public Player(String name)
        {
            this.name = name;
        }

        public Player(String name, uint connectionId) 
        {
            this.name = name;
            this.connectionId = connectionId;
        }

        public void Init()
        {
            body = UnityEngine.Object.Instantiate(BetonMultiplayerMod.ghostManager.ghostPrefab).transform.gameObject;
            body.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = name;
            body.GetComponent<Light>().color = Color.red;
            body.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        public void Move(Vector3 position, Quaternion rotation)
        {
            body.transform.SetPositionAndRotation(position, rotation);
        }
    }
}
