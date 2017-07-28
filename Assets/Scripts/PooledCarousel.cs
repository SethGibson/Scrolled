using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CL.ProjectIris.UI {
    public class PooledCarousel : MonoBehaviour
    {
        public int                  DisplayCount;
        public List<ScrolledData>   ItemList;
        public ScrolledPool         Pool;

        public RectTransform        ContentArea;
        private int     nextData,
                        prevData;
        private float   scrollStep,
                        boundMin,
                        boundMax;

        private List<GameObject>    displayedItems = new List<GameObject>();

        public void Start()
        {
            scrollStep = Pool.pooledPrefab.GetComponent<RectTransform>().sizeDelta.x;
            if(ContentArea.sizeDelta.x<scrollStep*DisplayCount||ContentArea.sizeDelta.x>scrollStep*DisplayCount) {
                var current = ContentArea.sizeDelta;
                ContentArea.sizeDelta = new Vector2(scrollStep*DisplayCount, current.y);
            }

            boundMin = 0;
            boundMax = ContentArea.rect.width;
            nextData = DisplayCount;
            prevData = ItemList.Count-1;

            startUp();
        }

        public void ScrollLeft()
        {
            updateButtons(-1);
            nextData+=1;
            prevData+=1;
            if(nextData>=ItemList.Count)
                nextData = 0;
            if(prevData>=ItemList.Count)
                prevData = 0;        
        }

        public void ScrollRight()
        {
            updateButtons(1);
            nextData-=1;
            prevData-=1;
            if(nextData<0)
                nextData = ItemList.Count-1;
            if(prevData<0)
                prevData = ItemList.Count-1;
        }

        private void startUp()
        {
            Pool.FillPool(DisplayCount);

            for(int i=0;i<DisplayCount;++i) {
                var go = setupNewItem(ItemList[i], i*scrollStep);
                displayedItems.Add(go);
            }
        }

        private void updateButtons(int scrollDir)
        {
            int id = scrollDir<0?nextData:prevData;
            float offset = scrollDir<0?boundMax:(boundMin-scrollStep);
            var newGo = setupNewItem(ItemList[id], offset);
            
            if(scrollDir<0)
                displayedItems.Add(newGo);
            else
                displayedItems.Insert(0, newGo);

            for(int i=0;i<displayedItems.Count;++i) {
                var go = displayedItems[i];
                var item = go.GetComponent<ScrolledItem>();
                var dest = getNextPosition(go, scrollDir);
                item.StartScrolling(dest);
            }
        }

        private GameObject setupNewItem(ScrolledData datum, float offset)
        {
            var newGo = Pool.Spawn();
            var newRt = newGo.GetComponent<RectTransform>();
            var newItem = newGo.GetComponent<ScrolledItem>();
            newItem.Setup(datum, Pool);

            newGo.transform.SetParent(ContentArea);
            newRt.localScale = Vector3.one;
            newRt.localRotation = Quaternion.identity;
            newRt.anchoredPosition3D = new Vector3(offset, 0, 0);

            return newGo;
        }

        private float getNextPosition(GameObject toCheck, int scrollFactor)
        {
            var item = toCheck.GetComponent<ScrolledItem>();
            var rt = toCheck.GetComponent<RectTransform>();
            var pos = rt.anchoredPosition3D;
            var nextOffset = pos.x+(scrollStep*scrollFactor);
        
            item.isAlive = !(nextOffset<boundMin||nextOffset>boundMax-scrollStep);
            return nextOffset;

        }

        //private bool nextPositionSet(GameObject toCheck, int scrollFactor)
        //{
        //    bool retval = false;
        //    var item = toCheck.GetComponent<ScrolledItem>();
        //    var rt = toCheck.GetComponent<RectTransform>();
        //    var pos = rt.anchoredPosition3D;
        //    var nextOffset = pos.x+(scrollStep*scrollFactor);
        //
        //    if(nextOffset<boundMin||nextOffset>boundMax) {
        //        item.isAlive = false;   //Flip the killswitch
        //        retval = true;
        //    }
        //    item.StartScrolling(nextOffset);
        //    return retval;
        //}

        public void PopItem(GameObject which)
        {
            displayedItems.Remove(which);
        }

    }
}