using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

namespace MonoBeh
{
    public class ScoreBoard : MonoBehaviour
    {
        private Transform _content;
        private Transform _template;
        private static IDictionary<int, MerchantInfo> _merchantInfoList;

        private void Awake()
        {
            _template = transform.Find("Viewport").Find("Content").Find("EntryTemplate");
            _content = transform.Find("Viewport").Find("Content");
            _template.gameObject.SetActive(false);

            const float entryHeight = 18;
            for (var i = 0; i < 70; i++)
            {
                var entryTransform = Instantiate(_template, _content);
                var entryRectTransform = entryTransform.GetComponent<RectTransform>();
                entryRectTransform.anchoredPosition = new Vector2(0, -10 - entryHeight * i);
                var idCell = entryTransform.Find("Id");
                idCell.GetComponent<TextMeshProUGUI>().text = (i+1).ToString();
                entryTransform.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (_merchantInfoList == null) return;

            for (var i = 0; i < _content.childCount; i++)
            {
                var entry  =_content.GetChild(i);
                if (!entry.gameObject.activeSelf) continue;

                var id = int.Parse(entry.Find("Id").GetComponent<TextMeshProUGUI>().text);
                entry.Find("AIType").GetComponent<TextMeshProUGUI>().text = _merchantInfoList[id].AIType;
                entry.Find("Money").GetComponent<TextMeshProUGUI>().text = _merchantInfoList[id].Money.ToString();
            }
        }

        public static void SetMerchantInfo(IDictionary<int, MerchantInfo> merchantInfoList)
        {
            _merchantInfoList = merchantInfoList;
        }
    }
}