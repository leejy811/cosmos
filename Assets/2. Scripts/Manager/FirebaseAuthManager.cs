using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth firebaseAuth;
    private FirebaseUser firebaseUser;

    void Start()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;
    }

}
