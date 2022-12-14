using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cinemachine;
public class PlayerController : MonoBehaviour
{
    [SerializeField] float Speed;
    [SerializeField] float JumpForce;
    [SerializeField]private float DetectionRange;
    [SerializeField] LayerMask layer;
    [SerializeField]private Transform detectTransform;
    [SerializeField]private Transform holdTransform;
    [SerializeField]private Transform dropTransform;
    [SerializeField]private float itemCount;
    [SerializeField]private float ItemDistanceBetween;
    [SerializeField]private float dropCount;
    [SerializeField]private float dropDistanceBetween;
    [SerializeField] List<GameObject> CollectedItems;
    [SerializeField]float DropSecond;
    [SerializeField]float DropRate;
    private bool _IsDead=false;
    private bool _isGround= true;
    private float Horizontal;
    public GameObject RestartLevel;
    float NextDropTime;
    Rigidbody rb;
    Collider[] colliders;
    SplineFollower _splineFollower;
    Animator anim;
    [SerializeField]List <Transform> droppos;
    Vector3 distance;
    int dropCountt = 0;
    Vector3 sellDistance = new Vector3(1, 0, 0);
    private CinemachineVirtualCamera _cinemachineCamera;
    private CinemachineTransposer _transposer;
    
    // Getting Components
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        _splineFollower = GetComponentInParent<SplineFollower>();
        _cinemachineCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        _transposer = _cinemachineCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    // Moving and Jump
    void Update()
    {
        //Checking Death Condition
        if (_IsDead)
        {
            dropItems();
        }
        //Moving Jumping
        if (Input.GetKeyDown(KeyCode.Space) && _isGround && _IsDead == false)
        {
            anim.SetTrigger("JumpT");
            rb.AddForce(Vector3.up * JumpForce,ForceMode.Impulse);
            _isGround=false;
        }
        Horizontal = Input.GetAxis("Horizontal");         
        transform.localPosition += new Vector3(Horizontal, 0, 0) *Time.deltaTime * Speed;

        //Collecting objects in the lane
        colliders = Physics.OverlapSphere(detectTransform.position, DetectionRange, layer);
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Collectable"))
            {
                hit.tag = "Collected";
                hit.transform.parent = holdTransform;

                 var seq = DOTween.Sequence();

                 seq.Append(hit.transform.DOLocalJump(new Vector3(0, itemCount * ItemDistanceBetween), 2, 1, 0.2f))
                     .Join(hit.transform.DOScale(1f, 0.2f));
                 seq.AppendCallback(() =>
                 {
                     hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
                 });
                 itemCount++;
                 CollectedItems.Add(hit.gameObject);
            }
        }
    }

                //Dropping items to base // Finish Line
         public void OnTriggerStay(Collider other)
         {
            if (other.transform.CompareTag("Dropzone"))
            {
                //Rotation on Death
                 transform.DOLocalRotate(new Vector3(0,-180,0),0.3f);    
                _transposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
                _transposer.m_FollowOffset = new Vector3(10f,7f,1.5f);

                //Drop
                if (Time.time >= NextDropTime)
                {
                    if (CollectedItems.Count > 0)
                    {
                     GameObject go = CollectedItems[CollectedItems.Count - 1];
                     go.transform.parent = null;
                     var Seq = DOTween.Sequence();
                     Seq.Append(go.transform.DOJump((droppos[dropCountt].transform.position + sellDistance), 2, 1, 0.3f))
                    .Join(go.transform.DOScale(1.2f, 0.1f))
                    .Insert(0.1f, go.transform.DOScale(1, 0.2f))
                    .AppendCallback(() => { go.transform.rotation = Quaternion.Euler(0, 0, 0); });
                    dropCountt ++;

                    if (dropCountt == 3)
                    {
                        dropCountt = 0;
                        sellDistance.x +=1;
                    }
                     CollectedItems.Remove(go);
                     dropCount++;
                     NextDropTime = Time.time + DropSecond / DropRate;
                     }
                }
                anim.SetTrigger("Win");
                _splineFollower.followSpeed = 0f;
                Speed = 0f;
                RestartLevel.SetActive(true);
        }
         }

        // Checking enemy
    public void OnTriggerEnter(Collider other) 
    {
        if (other.transform.CompareTag("Enemy"))
        {
            _splineFollower.followSpeed = 0f;
            anim.SetTrigger("Death");
            Speed = 0f;
            RestartLevel.SetActive(true);  
            _IsDead = true;
        }         
    }
        // Checking ground
     private void OnCollisionEnter(Collision other) 
     {
         if(other.transform.CompareTag("Ground"))
        {
            anim.SetBool("GroundT",true);
            _isGround=true;
        }
    }
        // Restarting
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

        //Death Drop things
    void dropItems ()
    {
        foreach (GameObject drops in CollectedItems)
        {
            distance = new Vector3 (Random.Range(-1, 3), 0.5f, Random.Range(-1, 3));
            drops.transform.parent = null;
            var seq = DOTween.Sequence();
            drops.AddComponent<Rigidbody>();
            seq.Append(drops.transform.DOLocalJump((gameObject.transform.position + distance), 1.3f, 1, 0.5f)).Join(drops.transform.DOScale(1f, 0.2f));
             CollectedItems.Remove(drops.gameObject);
        } 
    }
}
