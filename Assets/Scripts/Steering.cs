using UnityEngine;

public class Steering
{
    public float H { get; private set; }
    public float V { get; private set; }
    public bool Cruising { get; private set; } 

    public void Start()
    {
        H = 0f;
        V = 0f;
        Cruising = false;
    }

    public void UpdateValues()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Cruising = !Cruising;
        }

        if (Cruising)
        {
            V = 0.1f; // gets to max speed at a gradual pace
        }
        else
        {
            V = Input.GetAxis("Vertical");
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (H > -1.0)
            {
                H -= 0.05f;
            }
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (H < 1.0)
            {
                H += 0.05f;
            }
        }
        else
        {
            H = Input.GetAxis("Horizontal");
        }
    }
}
