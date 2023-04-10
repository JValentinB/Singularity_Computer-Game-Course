using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] public int mode;
    public List<GameObject> impactEffects;

    private ParticleSystem ps;
    private ParticleSystem.MainModule _ps;
    private ParticleSystemRenderer psr;
    private Vector3 dir;
    private int dmg;
    private bool foundhit = false;
    private Vector3 stop_pos;
    private bool destroyed = false;
    private bool alreadyDestroyed = false;
    public bool closeToTreeBoss;
    [Header("Index 0 and 2 can stay empty, they won't be loaded")]
    [SerializeField] private List<Material> modeMaterials;
    [SerializeField]
    private List<string> ignoreCollisionWithTag = new List<string>(){
        "Player",
        "FOV",
        "Bonfire",
        "Projectile",
        "TreeBoss",
        "IgnoreCollision"
    };

    private ObjectSounds objectSounds;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        _ps = ps.main;
        psr = GetComponent<ParticleSystemRenderer>();
        if (mode == 2) findcollision();

        objectSounds = GetComponent<ObjectSounds>();
        ChangeColor();
    }

    void Update()
    {
        Move();
        AttractProjectiles();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (mode == 2)
            return;

        var obj = col.gameObject;
        Shifter shifter = null;
        if (obj.GetComponent<m_Projectile>() && mode == 1)
        {   
            mProjCollision(obj);
            destroyed = true;
        }
        else if (mode == 4 && obj.tag != "Player" && !col.isTrigger)
        {
            controlShot();
            destroyed = true;
        }
        else if (obj.GetComponent<Damageable>() && obj.tag != "Player" && !col.isTrigger)
        {
            obj.GetComponent<Damageable>().ApplyDamage(dmg);
            destroyed = true;
        }
        else if (obj.GetComponent<AcornBranchTarget>())
        {
            obj.GetComponent<Rigidbody>().AddForce(dir * 1500f);
            destroyed = true;
        }
        else if (obj.GetComponent<TreeBossHitzone>())
        {
            obj.GetComponent<TreeBossHitzone>().gettingHit(dmg);
            destroyed = true;
        }
        else if (obj.GetComponent<Shifter>() && mode == 1)
        {
            obj.GetComponent<Shifter>().ToggleShifter();
            destroyed = true;
        }
        else if (!col.isTrigger && isChildOfShifter(out shifter, obj.transform, 2) && mode == 1)
        {
            shifter.ToggleShifter();
            destroyed = true;
        }
        else if (!col.isTrigger)
        {
            destroyed = true;
        }

        if (destroyed && !alreadyDestroyed && (mode == 1 || mode == 3))
        {
            alreadyDestroyed = true;

            GameObject impact = Instantiate(impactEffects[mode], transform.position, transform.rotation);
            objectSounds.Play("Impact");

            ps.Stop();
            Destroy(impact, 2f);
            Destroy(gameObject, 1f);
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (mode != 2) return;
        CheckAttractedObjects(col.gameObject);
        var obj = col.gameObject;
        Vector3 projectile_pos = transform.position;
        if (obj.tag != "Player" && obj.GetComponent<Damageable>())
        {
            var obj_rb = obj.GetComponent<Rigidbody>();
            Vector3 obj_pos = obj.GetComponent<Transform>().position;

            obj_rb.AddForce((projectile_pos - obj_pos) * 81f, ForceMode.Acceleration);
        }
    }

    public void setProjectileConfig(Vector3 dir, float speed, int mode)
    {
        this.dir = Vector3.Normalize(dir);
        this.projectileSpeed = speed;
        this.mode = mode;
        if (mode == 2)
        {
            GetComponent<SphereCollider>().radius = 30f;
        }
        else
        {
            GetComponent<SphereCollider>().radius = 1.25f;
        }
    }

    private void AttractProjectiles()
    {
        if (mode != 2) return;

        var projectiles = GameObject.FindGameObjectsWithTag("m_Projectile");
        foreach (var projectile in projectiles)
        {
            if(!projectile.GetComponent<m_Projectile>()) break;
            projectile.GetComponent<m_Projectile>().setDir(transform.position);
            // projectile.GetComponent<m_Projectile>().waitBeforeAttack = false;
        }
        var bombFruits = GameObject.FindGameObjectsWithTag("BombFruit");
        foreach (var fruit in bombFruits)
        {
            if(!fruit.GetComponent<BombFruit>()) break;
            fruit.GetComponent<BombFruit>().setDir(transform.position);
        }
    }

    private void CheckAttractedObjects(GameObject obj)
    {
        if (mode != 2) return;

        var treeBoss = GameObject.FindWithTag("TreeBoss");
        if (obj.GetComponent<m_Projectile>())
        {
            StartCoroutine(AttractToCenter(obj));
        }
        else if (obj.GetComponent<BombFruit>())
        {
            StartCoroutine(AttractToCenter(obj));
        }
    }

    private IEnumerator AttractToCenter(GameObject obj)
    {
        yield return new WaitForSeconds(0.3f);
        if (!obj) yield break;

        var treeBoss = GameObject.FindWithTag("TreeBoss");
        int projDmg = 0;
        if (closeToTreeBoss)
        {
            if (obj.GetComponent<BombFruit>()) projDmg = obj.GetComponent<BombFruit>().damage;
            else projDmg = obj.GetComponent<m_Projectile>().damage;

            treeBoss.GetComponent<TreeBoss>().ApplyDamage(projDmg);
            Debug.Log("Hit!");
        }
        if (obj.GetComponent<BombFruit>()) obj.GetComponent<BombFruit>().OnDeath();
        else obj.GetComponent<m_Projectile>().OnDeath();

    }

    private void ChangeColor()
    {
        if (mode == 0 || mode == 2) return;
        psr.material = modeMaterials[mode];
        if (mode == 3) dmg = 20;
        else dmg = 0;
    }

    private void mProjCollision(GameObject obj)
    {
        var m_Proj = obj.GetComponent<m_Projectile>();
        if (!m_Proj.freeze)
        {
            m_Proj.freeze = true;
            GameObject.FindWithTag("Player").GetComponent<Player>().setDirectionShot = true;
        }
        else
        {
            m_Proj.OnDeath();
        }
    }

    private void controlShot()
    {
        var m_Proj = GameObject.FindGameObjectsWithTag("m_Projectile");
        foreach (var mProjObject in m_Proj)
        {
            if (mProjObject.GetComponent<m_Projectile>().freeze)
            {
                mProjObject.GetComponent<m_Projectile>().setDir(transform.position);
            }
        }
    }

    private void Move()
    {
        if ((mode == 2 && foundhit && (stop_pos - transform.position).magnitude < 1) || destroyed) return;
        transform.Translate(dir * projectileSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    //sets stop-position if raycast hit
    private void findcollision()
    {
        if (mode != 2) return;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;
        LayerMask hitLayer = LayerMask.NameToLayer("Default");
        int layerMask = (1 << hitLayer);
        if (Physics.Raycast(ray, out hit, 60, layerMask))
        {
            stop_pos = hit.point;
            //Debug.Log(stop_pos);
            foundhit = true;

        }
    }

    bool noTriggerCollider(GameObject obj)
    {
        var colliders = obj.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            if (collider.isTrigger)
            {
                return false;
            }
        }
        return true;
    }

    bool isChildOfShifter(out Shifter shifter, Transform obj, int depth = 0)
    {
        if (obj.parent == null || depth <= 0)
        {
            shifter = null;
            return false;
        }
        else if (obj.parent.GetComponent<Shifter>())
        {
            shifter = obj.parent.GetComponent<Shifter>();
            return true;
        }
        else
        {
            shifter = null;
            return isChildOfShifter(out shifter, obj.parent, depth - 1);
        }
    }
}
