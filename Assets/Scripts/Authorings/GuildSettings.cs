using System;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;
using Utils;

namespace Authorings
{
    [Serializable]
    internal struct MerchantSetting
    {
        public MerchantTypes type;
        public int number;
    }


    [DisallowMultipleComponent]
    public class GuildSettings : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private GameObject merchantPrefab;

        [SerializeField] private List<MerchantSetting> guildSettings;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            using var store = new BlobAssetStore();
            var merchantEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(merchantPrefab,
                GameObjectConversionSettings.FromWorld(dstManager.World, store));

            var counter = 1;
            foreach (var setting in guildSettings)
            {
                for (var i = 0; i < setting.number; i++)
                {
                    //Debug.Log($"Instantiate Merchant: {setting.type}");
                    dstManager.AddComponentData(merchantEntity, new Merchant {Id = counter++, Type = setting.type});
                    dstManager.Instantiate(merchantEntity);
                }
            }
        }
    }
}