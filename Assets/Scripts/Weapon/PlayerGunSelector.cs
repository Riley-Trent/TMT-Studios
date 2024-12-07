using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField] private GunType Gun;
    [SerializeField] private Transform GunParent;
    [SerializeField] private Transform GunTip;
    [SerializeField] private GameObject CameraView;
    [SerializeField] private List<GunScriptableObject> Guns;
    //[SerializeField] private PlayerIk InverseKinematics;

    [Space]
    [Header("Runtime Filled")]
    public GunScriptableObject ActiveGun;

    private void Start(){
        GunScriptableObject gun = Guns.Find(gun => gun.Type == Gun);

        if(gun == null){
            Debug.LogError($"No GunScriptableObject found for GunType:  {gun}");
            return;
        }

        ActiveGun = gun;
        gun.Spawn(GunParent, this);
        
        gun.CameraPos = CameraView;
        gun.GunTip = GunTip;
        
    }
}
