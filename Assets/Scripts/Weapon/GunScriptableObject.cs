using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
{
    //public ImpactType ImpactType;
    public GunType Type;
    public string Name;
    public GameObject ModelPrefab;
    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;

    public ShootConfigurationScriptableObject ShootConfig;
    public TrailConfigurationSriptableObject TrailConfig;
    public Vector3 GunTip;
    public GameObject CameraPos;

    private MonoBehaviour ActiveMonoBehaviour;
    private GameObject Model;
    private float LastShootTime;
    private ObjectPool<TrailRenderer> TrailPool;

    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour){
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        LastShootTime = 0;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        Model = Instantiate(ModelPrefab);
        Model.transform.SetParent(Parent, false);
        Model.transform.localPosition = SpawnPoint;
        Model.transform.localRotation = Quaternion.Euler(SpawnRotation);
    }

    public void Shoot(){
        if (Time.time > ShootConfig.FireRate + LastShootTime){
            LastShootTime = Time.time;

            Vector3 shootOrigin = CameraPos.transform.position;
            Vector3 shootDirection = CameraPos.transform.forward 
                + new Vector3(
                    Random.Range(-ShootConfig.Spread.x, ShootConfig.Spread.x),
                    Random.Range(-ShootConfig.Spread.y, ShootConfig.Spread.y),
                    Random.Range(-ShootConfig.Spread.z, ShootConfig.Spread.z)
                );
            shootDirection.Normalize();

            if (Physics.Raycast(
                    shootOrigin,
                    shootDirection,
                    out RaycastHit hit,
                    float.MaxValue,
                    ShootConfig.HitMask
            )){
                ActiveMonoBehaviour.StartCoroutine(PlayTrail(Model.transform.position + GunTip, hit.point, hit));
                if(hit.collider.gameObject.tag == ("Enemy"))
                {
                    if(hit.collider.gameObject.TryGetComponent<RedFox>(out RedFox redFox))
                    {
                        redFox.TakeDamage(10);
                    }
                    if (hit.collider.gameObject.TryGetComponent<Thumper>(out Thumper thumper))
                    {
                        thumper.TakeDamage(10);
                    }
                    if (hit.collider.gameObject.TryGetComponent<TasmanianDevil>(out TasmanianDevil tasmanianDevil))
                    {
                        tasmanianDevil.TakeDamage(10);
                    }
                    if (hit.collider.gameObject.TryGetComponent<Tiger>(out Tiger tiger))
                    {
                        tiger.TakeDamage(10);
                    }
                }

            } else {
                ActiveMonoBehaviour.StartCoroutine(PlayTrail(
                    Model.transform.position + GunTip,
                    Model.transform.position + GunTip + (shootDirection * TrailConfig.MissDistance),
                    new RaycastHit()
                ));
            }
        }
    }

    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit){
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = StartPoint;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;
        while(remainingDistance > 0){
            instance.transform.position = Vector3.Lerp(StartPoint, EndPoint, Mathf.Clamp01(1 -(remainingDistance / distance)));
            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        if (Hit.collider != null){
            //SurfaceManager.Instance.HandleImpact(Hit.transform.gameObject, EndPoint, Hit.normal, ImpactType, 0);
        }

        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
    }

    private TrailRenderer CreateTrail(){

        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }
}
