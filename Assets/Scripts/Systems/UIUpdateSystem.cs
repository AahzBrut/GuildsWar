using System.Collections.Generic;
using Components;
using MonoBeh;
using Unity.Collections;
using Unity.Entities;
using Utils;

namespace Systems
{
    [UpdateAfter(typeof(TurnSystem))]
    [UpdateBefore(typeof(DealSpawnSystem)), UpdateBefore(typeof(MerchantUpdateSystem))]
    public class UIUpdateSystem : SystemBase
    {
        private int _updatedYear = -2;
        
        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());

            if (calendar.Turn != 0 || _updatedYear == calendar.Year) return;
            _updatedYear = calendar.Year;

            var merchantQuery = GetEntityQuery(ComponentType.ReadOnly<Merchant>());
            var merchants = merchantQuery.ToComponentDataArray<Merchant>(Allocator.TempJob);

            var merchantInfoList = new Dictionary<int, MerchantInfo>(merchants.Length);

            foreach (var merchant in merchants)
            {
                merchantInfoList.Add(merchant.Id, new MerchantInfo{Id = merchant.Id, AIType = merchant.Type.ToString(), Money = merchant.Money});
            }

            ScoreBoard.SetMerchantInfo(merchantInfoList);
            merchants.Dispose();
        }
    }
}