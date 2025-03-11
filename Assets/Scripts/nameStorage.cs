using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nameStorage : MonoBehaviour
{
    private string name = "";

    public void setName(string newName) {
        name = newName;
    }

    public string getName() {
        return name;
    }

}
