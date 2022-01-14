using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    LineRenderer lineRend;
    Vector2 grapplePoint;
    public LayerMask whatIsGrapplable;
    public Transform shootPoint;
    public float hookLength;
    SpringJoint2D joint;

    bool hasHolder;
    Transform holderTransform;
    PlayerController holderController;
    Rigidbody2D holderRb;

    bool isGrappling;

    public float distanceMultiplier;
    public float dampingRatio;
    public float frequency;

    public float pullSensitivity;

    public float flipSensitivity;

    Item item;

    bool didItHit;

    AudioSource ads;
    public AudioClip grappleSound;

    // Start is called before the first frame update
    void Start()
    {
        didItHit = false;
        hasHolder = false;
        isGrappling = false;

        lineRend = GetComponent<LineRenderer>();
        item = GetComponent<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        if (item.givenToPlayer && !hasHolder)
        {
            hasHolder = true;

            holderTransform = item.holder.GetComponent<Transform>();
            holderController = item.holder.GetComponent<PlayerController>();
            holderRb = item.holder.GetComponent<Rigidbody2D>();
            ads = item.holder.GetComponent<AudioSource>();

            holderController.useItem.AddListener(Grapple);
            holderController.stopUsingItem.AddListener(StopGrapple);
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void Grapple()
    {
        StartCoroutine(StartGrapple());
    }

    IEnumerator StartGrapple()
    {
        RaycastHit2D hit = Physics2D.Raycast(shootPoint.position, transform.right, hookLength, whatIsGrapplable);
        if (hit.collider != null)
        {
            ads.PlayOneShot(grappleSound);

            joint = holderTransform.gameObject.AddComponent<SpringJoint2D>();

            if (hit.collider.gameObject.CompareTag("Player") && hit.collider.gameObject != item.holder)
            {
                Rigidbody2D hitPlayerRb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                joint.connectedBody = hitPlayerRb;
                grapplePoint = hit.collider.gameObject.transform.position;
            }
            else
            {
                grapplePoint = hit.point;
                joint.connectedAnchor = grapplePoint;
            }

            didItHit = true;
            item.aimable = false;

            holderController.hasForce = false;
            holderController.forceLowJump = true;

            joint.enableCollision = true;
            joint.autoConfigureConnectedAnchor = false;
            joint.autoConfigureDistance = false;

            float distanceFromPoint = Vector2.Distance(holderTransform.position, grapplePoint);

            joint.distance = distanceFromPoint * distanceMultiplier;
            joint.dampingRatio = dampingRatio;
            joint.frequency = frequency;

            isGrappling = true;

            while (isGrappling)
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    grapplePoint = hit.collider.gameObject.transform.position;

                    Vector3 dir = new Vector2(hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                else
                {
                    Vector3 dir = grapplePoint - new Vector2(transform.position.x, transform.position.y);
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }

                if (distanceFromPoint > Vector2.Distance(holderTransform.position, grapplePoint) + pullSensitivity)
                {
                    distanceFromPoint = Vector2.Distance(holderTransform.position, grapplePoint);
                    joint.distance = distanceFromPoint * distanceMultiplier;

                    yield return null;
                }

                yield return null;
            }

            yield return null;
        }

        yield return null;
    }

    void StopGrapple()
    {
        item.aimable = true;

        lineRend.positionCount = 0;
        Destroy(joint);

        isGrappling = false;

        holderController.hasForce = true;
        holderController.ForceLowJump();

        if (holderController.movementDirection == 1 && holderRb.velocity.x < holderController.movementSpeed / flipSensitivity * -1 || holderController.movementDirection == -1 && holderRb.velocity.x > holderController.movementSpeed / flipSensitivity)
        {
            holderController.Flip();
        }

        if (didItHit)
        {
            didItHit = false;
            item.Use();
        }
    }

    void DrawRope()
    {
        if (!joint)
        {
            return;
        }

        lineRend.positionCount = 2;
        lineRend.SetPosition(0, shootPoint.position);
        lineRend.SetPosition(1, grapplePoint);
    }
}
