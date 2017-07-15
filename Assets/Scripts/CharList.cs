using UnityEngine;
using System.Collections;

public class CharList : MonoBehaviour{

    public GameObject charRyu;
    public GameObject charChun;

    public GameObject GetCharAtIndex(int index)
    {
        switch(index)
        {
            case 0:
                return charRyu;
            case 1:
                return charChun;
        }

        return charRyu;
    }
}
