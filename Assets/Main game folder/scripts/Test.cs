using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject boss;
    float shootingTimer = 4f;

    int[] array = { 3, 2, 2, 3 };
    // Start is called before the first frame update
    void Start()
    {
        RemoveElement(array, 3);
        for(int i =0;i< array.Length; i++)
        {
            Debug.Log(array[i]);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

    }

    public int RemoveElement(int[] nums, int val)
    {
        int temp;
        int countOfEqualNumber = 0;
        for (int i = 0; i < nums.Length; i++)
        {
            int j = 1;
            if (nums[i] == val)
            {
                countOfEqualNumber += 1;
                while (nums[nums.Length - j] == val)
                {
                    if (nums.Length - j == i) break;
                    j += 1;
                }
                temp = nums[i];
                nums[i] = nums[nums.Length - j];
                nums[nums.Length - j] = temp;
            }
        }
        return nums.Length - countOfEqualNumber;
    }
}
