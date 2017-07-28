using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CL.ProjectIris.UI {
    public class ScrolledItem : MonoBehaviour
    {
        [SerializeField]private Image           background;
        [SerializeField]private Text            label;

        public bool isAlive;
        public ScrolledPool    Parent;

        private Coroutine scroller;
        private RectTransform rt;

        private float scrollFrames = 15f;

        public void Setup(ScrolledData data, ScrolledPool parent)
        {
            background.color = data.Tint;
            label.text = data.Name;
            Parent = parent;
            rt = (RectTransform)transform;
            isAlive = true;
        }

        public void StartScrolling(float whereTo)
        {
            scroller = StartCoroutine(scroll(whereTo));
        }

        private IEnumerator scroll(float destination)
        {
            var pos0 = rt.anchoredPosition3D;
            var pos1 = pos0;
            float counter = 0f;
            
            while(counter<=scrollFrames) {
                pos1.x=Mathf.Lerp(pos0.x, destination, counter/scrollFrames);
                rt.anchoredPosition3D = pos1;
                ++counter;
                yield return new WaitForEndOfFrame();
            }

            if(!isAlive)
                Parent.Recycle(gameObject);
        }
    }

    [Serializable]
    public class ScrolledData
    {
        public string Name;
        public Color  Tint;
    }
}