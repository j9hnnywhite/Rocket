using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Rocket : MonoBehaviour
{
    [SerializeField] Text energyText;
    [SerializeField] int energyTotal = 2000;
    [SerializeField] int energyApply = 150;

    [SerializeField] float rotSpeed = 180f;
    [SerializeField] float flySpeed = 7f;

    [SerializeField] AudioClip flySound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip finishSound;

    [SerializeField] ParticleSystem flyParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem finishParticles;

    bool collisionOFF = false;   

    Light lightIntensity;
    Rigidbody rigidBody;
    AudioSource audioSourse;
    enum State {Playing, Dead, NextLevel};
    State state = State.Playing;

    void Start()
    {
        energyText.text = energyTotal.ToString();
        state = State.Playing;
        lightIntensity = GetComponent<Light>();
        rigidBody = GetComponent<Rigidbody>();
        audioSourse = GetComponent<AudioSource>();
    }

    
    void Update()
    {
        if (state == State.Playing)
        { 
        Launch();
        Rotation();
        }
        if (Debug.isDebugBuild) { DebugKeys(); }  // run only in develop build
    }
    void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }

        else if (Input.GetKey(KeyCode.C))
        {
            collisionOFF = !collisionOFF;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state == State.Dead || state == State.NextLevel || collisionOFF)
        {
            return;
        }

      switch(collision.gameObject.tag)
        {
            case "Friendly":
                print("okkkkk!");
                break;
            case "Battery":
                PlusEnergy(1000, collision.gameObject);
                break;
            case "Finish":
                Finish();
                break;
            default:
                Lose();
                break;
        }
    }
    void PlusEnergy(int energyToAdd, GameObject batteryObj)
    {
        batteryObj.GetComponent<SphereCollider>().enabled = false;
        energyTotal += energyToAdd;
        energyText.text = energyTotal.ToString();
        Destroy(batteryObj);
    }

    void Lose()
    {
        state = State.Dead;
        Invoke("LoadFirstLevel", 2f);
        audioSourse.Stop();
        audioSourse.PlayOneShot(deathSound);
        deathParticles.Play();
        print("boom");
    }

    void Finish() 
    {
        state = State.NextLevel;
        audioSourse.Stop();
        audioSourse.PlayOneShot(finishSound);
        finishParticles.Play();
        Invoke("LoadNextLevel", 2f);
    }

    void LoadNextLevel () // Finish
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentLevelIndex + 1;

        if (nextLevelIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextLevelIndex = 0;
        }

        SceneManager.LoadScene(nextLevelIndex);
    }

    void LoadFirstLevel() // Lose
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevelIndex);
    }

    void Launch()
    {
        if (Input.GetKey(KeyCode.Space) && energyTotal > 0)
        {
            energyTotal -= Mathf.RoundToInt(energyApply * Time.deltaTime);
            energyText.text = energyTotal.ToString();
            rigidBody.AddRelativeForce(Vector3.up * flySpeed * Time.deltaTime);
            if (audioSourse.isPlaying == false)
                audioSourse.PlayOneShot(flySound);
                flyParticles.Play();
                lightIntensity.intensity = 1.3f;
        }
        else
        {
            lightIntensity.intensity = 0.1f;
            audioSourse.Pause();
            flyParticles.Stop();
        }
        
    }
    void Rotation()
    {
        float rotationSpeed = rotSpeed * Time.deltaTime;

        rigidBody.freezeRotation = true;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward*rotationSpeed); 
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward*rotationSpeed);
        }
        rigidBody.freezeRotation = false;
    }
}
