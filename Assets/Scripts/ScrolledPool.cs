using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CL.ProjectIris.UI {
    public class ScrolledPool : MonoBehaviour
    {
        public GameObject       pooledPrefab;
        public PooledCarousel   carousel;
        private Stack<GameObject>    pooledObjects = new Stack<GameObject>();

	    public void Start()
	    {
	    }
	
	    public void Update()
	    {
		
	    }

        public void FillPool(int count)
        {
            var all = count+2;
            for(int i=0;i<all;++i) {
                var go = Instantiate(pooledPrefab);

                pooledObjects.Push(go);
                go.transform.SetParent(gameObject.transform);
                go.SetActive(false);
            }
        }

        public GameObject Spawn()
        {
            ScrolledItem item = null;
            if(pooledObjects.Count>0) {
                var go = pooledObjects.Pop();
                go.SetActive(true);
                item = go.GetComponent<ScrolledItem>();
            }
            else
                item = Instantiate(pooledPrefab).GetComponent<ScrolledItem>();
            item.Parent = this;
            return item.gameObject;
        }

        public void Recycle(GameObject recycled)
        {
            var item = recycled.GetComponent<ScrolledItem>();
            if(item!=null&&item.Parent==this) {
                recycled.transform.SetParent(gameObject.transform, false);
                pooledObjects.Push(recycled);
                //carousel.PopItem(recycled);
                recycled.SetActive(false);
            }
            else 
                Destroy(recycled);
        }
    }
}