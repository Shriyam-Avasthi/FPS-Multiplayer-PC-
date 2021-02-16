using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] GameObject cameraHolder;
    
    [SerializeField] Item[] items;
    [SerializeField] float mouseSensitivity , sprintSpeed, walkSpeed , jumpForce , smoothTime;
    public bool grounded;
    Vector3 smoothMoveVelocity;
    float verticalLookRotation;
    Vector3 moveAmount;
    Rigidbody rb;
    PhotonView PV;
    int itemIndex;
    int previousItemIndex = -1;

    private Animator animator;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>(); // PV.InstantiationData[0] will give the ViewId Object
    }

    private void Start() {
        if(PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }

    private void Update() {
        if(!PV.IsMine)
        {
            return;
        }
        LookAround();
        Move();
        Jump();

        if(transform.position.y < -10f)
        {
            playerManager.Die();
        }

        for(int i = 0 ; i < items.Length; i++)
        {
            if(Input.GetKeyDown((i+1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if(itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
            
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if(itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else 
            {
                EquipItem(itemIndex - 1);
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }
    }

    void EquipItem(int _index)
    {
        if(_index == previousItemIndex)
        {
            return;
        }
        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);

        if(previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if(PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("Item Index" , itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!PV.IsMine && targetPlayer == PV.Owner ){
            {
                EquipItem((int)changedProps["Item Index"]);
            }
        }
    }
  
    private void LookAround()
    {
        transform.Rotate(transform.up *Input.GetAxis("Mouse X") * mouseSensitivity );

        verticalLookRotation += Input.GetAxisRaw("Mouse Y")*mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation , -90f , 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }         

    private void Move()
    {
        animator.SetFloat("MoveHorizontal", Input.GetAxis("Horizontal"));
        animator.SetFloat("MoveVertical" , Input.GetAxis("Vertical"));
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f , Input.GetAxisRaw("Vertical")).normalized;
        
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift)? sprintSpeed : walkSpeed),ref smoothMoveVelocity , smoothTime);
    }

    private void Jump()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(transform.up*jumpForce);
        }
    }
    public void SetGroundState(bool _grounded)
    {
        grounded = _grounded;
    }

    private void FixedUpdate() 
    {
        if(!PV.IsMine) 
            return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount)*Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All , damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage) 
    {
        if(!PV.IsMine)
            return;
        
        currentHealth -= damage;

        if ( currentHealth <= 0 )
        {
            Die();
        }
    }

    void Die() 
    {
        playerManager.Die();
    }
}
