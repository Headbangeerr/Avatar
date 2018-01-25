using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarToggle : MonoBehaviour {

	
    public void OnValueChanged(bool isOn)
    {
        if (isOn)
        {            
            string[] names = gameObject.name.Split('-');
            string part = names[0];
            string num = names[1];
            Debug.Log(part+":"+num);
            AvatarSystem.GetInstance().ChangeClothes(part, num);
        }
    }
}
