using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rob02Ctrl : MonoBehaviour
{
    public float speed = 1.0f;
    public float battleSpeed = 2f;
    public Transform bulletSpawnPointLeft;
    public Transform bulletSpawnPointRight;
    public GameObject bulletPrefab;
    public float bulletSpeed = 5;


    Animator anim;
    CharacterController controller;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        anim.SetFloat("speedMultiplier", speed);

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("shoot", false);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetAxis("Horizontal") > 0.1f)
            {
                anim.SetBool("turnLeft", true);
                anim.SetBool("turnRight", false);
            }
            if (Input.GetAxis("Horizontal") < -0.1f)
            {
                anim.SetBool("turnRight", true);
                anim.SetBool("turnLeft", false);
            }
        }
        else
        {
            anim.SetBool("turnLeft", false);
            anim.SetBool("turnRight", false);

            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
            {
                anim.SetFloat("Turns", Input.GetAxis("Horizontal"));
            }
            else
            {
                anim.SetFloat("Turns", 0);
            }

            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
            {
                anim.SetFloat("Speed", Input.GetAxis("Vertical"));
            }
            else
            {
                anim.SetFloat("Speed", 0);
            }

            if (Input.GetKeyDown("e"))
            {
                anim.SetBool("Packed", !(anim.GetBool("Packed")));
                if (anim.GetBool("battle"))
                {
                    anim.SetBool("battle", false);
                }

            }

            if (Input.GetKeyDown("1"))
            {
                anim.SetBool("battle",false);
                anim.SetFloat("speedMultiplier", speed);
            }

            if (Input.GetKeyDown("2"))
            {
                anim.SetBool("battle", true);
                anim.SetFloat("speedMultiplier",battleSpeed);
            }

            if ((Input.GetKeyDown("r"))&&(anim.GetBool("battle")))
            {
                anim.SetBool("shoot", true);
                var bulletL = Instantiate(bulletPrefab, bulletSpawnPointLeft.position, bulletSpawnPointLeft.rotation);
                bulletL.GetComponent<Rigidbody>().velocity = bulletSpawnPointLeft.forward * bulletSpeed;
                var bulletR = Instantiate(bulletPrefab, bulletSpawnPointRight.position, bulletSpawnPointRight.rotation);
                bulletR.GetComponent<Rigidbody>().velocity = bulletSpawnPointRight.forward * bulletSpeed;
            }

            if ((Input.GetKeyDown("p"))) //death
            {
                anim.SetBool("die", true);
            } 
            if ((Input.GetKeyDown("u"))) //look around
            {
                anim.SetBool("look1", true);
            } else { anim.SetBool("look1", false); }
            if ((Input.GetKeyDown("y"))) //look at side
            {
                anim.SetBool("look2", true);
            } else { anim.SetBool("look2", false); }
            if ((Input.GetKeyDown("o"))) //take damage 1
            {
                anim.SetBool("hitLeft", true);
            } else { anim.SetBool("hitLeft", false); }
            if ((Input.GetKeyDown("i"))) //take damage 2
            {
                anim.SetBool("hitRight", true);
            } else { anim.SetBool("hitRight", false); }
        }
    }
}

