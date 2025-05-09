using System.Collections;
using UnityEngine;

namespace CompanySystem
{
    public class DynamicTableManager : MonoBehaviour
    {
        public RectTransform tableContainer;
        public float maskHeight;
        private float containerHeigth;
        public float templateHeight;
        private float upperBound;
        private float lowerBound;
        public float lowerBoundTolerance;

        private int count;
        // Start is called before the first frame update
        void OnEnable()
        {
            count = 0;
            upperBound = tableContainer.anchoredPosition.y;
            StartCoroutine(SetLowerBound());
        }

        private void OnDisable()
        {
            tableContainer.anchoredPosition = new Vector2(0f, upperBound);
        }

        // Update is called once per frame
        void Update()
        {
            if (tableContainer.anchoredPosition.y < upperBound || containerHeigth <= maskHeight)
                tableContainer.anchoredPosition = new Vector2(0f, upperBound);
            if (tableContainer.anchoredPosition.y > lowerBound && containerHeigth > maskHeight)
                tableContainer.anchoredPosition = new Vector2(0f, lowerBound);
        }

        private IEnumerator SetLowerBound()
        {
            // Wait for 0.5 seconds
            yield return new WaitForSeconds(0.5f);

            CountRecord();
            containerHeigth = templateHeight * count;
            lowerBound = containerHeigth - maskHeight + upperBound + lowerBoundTolerance;
        }

        private void CountRecord()
        {
            foreach (Transform child in tableContainer)
            {
                if (child.gameObject.name == "Record Template(Clone)")
                {
                    count++;
                }
            }
        }
    }
}