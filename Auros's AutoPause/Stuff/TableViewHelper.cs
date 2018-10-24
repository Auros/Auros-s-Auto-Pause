﻿using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberUI
{
    class TableViewHelper : MonoBehaviour
    {
        TableView table;
        RectTransform viewport;

        RectTransform _scrollRectTransform
        {
            get { return table.GetPrivateField<RectTransform>("_scrollRectTransform"); }
            set { table.SetPrivateField("_scrollRectTransform", value); }
        }

        int _numberOfRows
        {
            get { return table.GetPrivateField<int>("_numberOfRows"); }
            set { table.SetPrivateField("_numberOfRows", value); }
        }

        float _rowHeight
        {
            get { return table.GetPrivateField<float>("_rowHeight"); }
            set { table.SetPrivateField("_rowHeight", value); }
        }

        float _targetVerticalNormalizedPosition
        {
            get { return table.GetPrivateField<float>("_targetVerticalNormalizedPosition"); }
            set { table.SetPrivateField("_targetVerticalNormalizedPosition", value); }
        }

        void Awake()
        {
            table = GetComponent<TableView>();
            viewport = GetComponentsInChildren<RectTransform>().First(x => x.name == "Viewport");
        }

        public void PageScrollUp()
        {
            float scrollStep = GetScrollStep();
            _targetVerticalNormalizedPosition = Mathf.RoundToInt(_targetVerticalNormalizedPosition / scrollStep + Mathf.Max(1f, GetNumberOfVisibleRows() - 1f)) * scrollStep;
            if (_targetVerticalNormalizedPosition > 1f)
            {
                _targetVerticalNormalizedPosition = 1f;
            }
            table.RefreshScrollButtons();
            _scrollRectTransform.sizeDelta = new Vector2(-20f, -10f);
        }

        public void PageScrollDown()
        {
            float scrollStep = GetScrollStep();
            _targetVerticalNormalizedPosition = Mathf.RoundToInt(_targetVerticalNormalizedPosition / scrollStep - Mathf.Max(1f, GetNumberOfVisibleRows() - 1f)) * scrollStep;
            if (_targetVerticalNormalizedPosition < 0f)
            {
                _targetVerticalNormalizedPosition = 0f;
            }
            table.RefreshScrollButtons();
            _scrollRectTransform.sizeDelta = new Vector2(-20f, -10f);
        }

        private float GetNumberOfVisibleRows()
        {
            return 6.0f;
        }

        public virtual float GetScrollStep()
        {
            float height = viewport.rect.height;
            float num = _numberOfRows * _rowHeight - height;
            int num2 = Mathf.CeilToInt(num / _rowHeight);
            return 1f / num2;
        }
    }
}
