using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class PlayerManager : MonoBehaviour
{
   private PhotonView PV;
   GameObject controller;
   
   private void Awake() {
       PV = GetComponent<PhotonView>();
   }

   private void Start() {
       if(PV.IsMine)
       {
           CreateController();
       }
   }

   void CreateController()
   {
       Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();
       controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs" , "PlayerController Variant 1") , spawnPoint.position , spawnPoint.rotation , 0 , new object[] {PV.ViewID} );
   }

   public void Die()
   {
       PhotonNetwork.Destroy(controller);
       CreateController();
   }
}
