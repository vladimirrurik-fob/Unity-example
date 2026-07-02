using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Data
{
   [Serializable]
   public class PlayerProgress
   {
      public WorldData WorldData;
      public PlayerData PlayerData;
      public EnemyData EnemyData;
      public EconomyData EconomyData;

      public PlayerProgress()
      {
         WorldData = new WorldData();
         PlayerData = new PlayerData();
         EnemyData = new EnemyData();
         EconomyData = new EconomyData();
      }
   }

   [Serializable]
   public class EnemyData
   {
      private List<Vector3> _lastEnemyPositions;
   }

   [Serializable]
   public class PlayerData
   {
      public int CurrentHealth;
      public int CurrentLevel;
      public Vector3 Position;
   }

   [Serializable]
   public class WorldData
   {
      public WorldData()
      {
         Debug.Log("Create WorldData");
      }
   }
}