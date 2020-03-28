using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleLock : MonoBehaviour
{
    public List<KeySquare> Squares;
    public bool Active = false;
    public float RotationSpeed = 1.0f;
    public KeySquare SolutionKey;
    List<string> charTemplate = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    List<string> numTemplate = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

    public enum KeyColour
    {
        red, blue, green, yellow, orange, purple, black, white
    }
    List<KeyColour> colTemplate = new List<KeyColour>() { KeyColour.red, KeyColour.blue, KeyColour.green, KeyColour.yellow, KeyColour.orange, KeyColour.purple, KeyColour.black, KeyColour.white };

    private Dictionary<KeyColour, Sprite> keyBoxImages = new Dictionary<KeyColour, Sprite>();
    private Quaternion origin;
    private Vector3 previousPosition = Vector3.zero;
    private Vector3 changeInPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.rotation;
        getKeyBoxImages();
        randomizeKeys();
        chooseSolutionKey();
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            if (Input.GetMouseButton(0))
            {
                changeInPosition = Input.mousePosition - previousPosition;
                
                transform.Rotate(transform.forward, -Vector3.Dot(changeInPosition, transform.right));
            }
            previousPosition = Input.mousePosition;
        }
    }

    private float angleBetweenPoints(Vector2 v2Position1, Vector2 v2Position2)
    {
        Vector2 v2FromLine = v2Position2 - v2Position1;
        Vector2 v2ToLine = new Vector2(1, 0);

        float fltAngle = Vector2.Angle(v2FromLine, v2ToLine);

        // If rotation is more than 180
        Vector3 v3Cross = Vector3.Cross(v2FromLine, v2ToLine);
        if (v3Cross.z > 0)
        {
            fltAngle = 360f - fltAngle;
        }

        return fltAngle;
    }

    public void resetAngles()
    {
        transform.rotation = origin;
    }

    private void getKeyBoxImages()
    {
        List<Sprite> allImages = Resources.LoadAll<Sprite>("Keys").ToList();

        foreach (Sprite image in allImages)
        {
            foreach (KeyColour colour in KeyColour.GetValues(typeof(KeyColour)))
            {
                if (image.name.Contains(colour.ToString()))
                {
                    keyBoxImages.Add(colour,image);
                }
            }
        }
    }

    public void randomizeKeys()
    {
        List<string> remainingChars = new List<string>(charTemplate.ToArray());
        List<string> remainingNums = new List<string>(numTemplate.ToArray());
        List<KeyColour> remainingCols = new List<KeyColour>(colTemplate.ToArray());
        int temp = 0;
        string temp2;

        foreach(KeySquare key in Squares)
        {
            temp = Random.Range(0, remainingCols.Count);
            temp2 = "";
            key.KeyImage.sprite = keyBoxImages[remainingCols[temp]];
            remainingCols.RemoveAt(temp);
            temp = Random.Range(0, remainingChars.Count);
            temp2 = remainingChars[temp];
            remainingChars.RemoveAt(temp);
            temp = Random.Range(0, remainingNums.Count);
            temp2 += remainingNums[temp];
            remainingNums.RemoveAt(temp);
            key.CodeText.text = temp2;
        }
    }

    public void chooseSolutionKey()
    {
        SolutionKey = Squares[Random.Range(0, Squares.Count)];
    }

    public void getInvalids(out string invalids1, out string invalids2)
    {
        invalids1 = "";
        invalids2 = "";

        List<string> invalidList1 = new List<string>();
        List<string> invalidList2 = new List<string>();
        List<string> remainingChars = new List<string>(charTemplate.ToArray());
        List<string> remainingNums = new List<string>(numTemplate.ToArray());
        List<KeyColour> remainingCols = new List<KeyColour>(colTemplate.ToArray());

        KeyColour currentKeyColour;
        int differentiator, invalidsChooser;
        int currentIndex;

        foreach (KeySquare key in Squares)
        {
            if(key == SolutionKey)
            {
                remainingChars.Remove(key.CodeText.text[0].ToString());
                remainingNums.Remove(key.CodeText.text[1].ToString());

                currentKeyColour = remainingCols.Single(c => key.KeyImage.sprite.ToString().Contains(c.ToString()));
                remainingCols.Remove(currentKeyColour);
                
            }
            else
            {
                differentiator = Random.Range(0, 3);
                invalidsChooser = Random.Range(0, 2);
                switch (differentiator)
                {
                    case 0:
                        remainingChars.Remove(key.CodeText.text[0].ToString());
                        if(invalidsChooser == 0)
                        {
                            invalidList1.Add(key.CodeText.text[0].ToString() + System.Environment.NewLine);
                        }
                        else
                        {
                            invalidList2.Add(key.CodeText.text[0].ToString() + System.Environment.NewLine);
                        }
                        break;
                    case 1:
                        remainingNums.Remove(key.CodeText.text[1].ToString());
                        if (invalidsChooser == 0)
                        {
                            invalidList1.Add(key.CodeText.text[1].ToString() + System.Environment.NewLine);
                        }
                        else
                        {
                            invalidList2.Add(key.CodeText.text[1].ToString() + System.Environment.NewLine);
                        }
                        break;
                    case 2:
                        currentKeyColour = remainingCols.Single(c => key.KeyImage.sprite.ToString().Contains(c.ToString()));
                        remainingCols.Remove(currentKeyColour);
                        if (invalidsChooser == 0)
                        {
                            invalidList1.Add(currentKeyColour.ToString() + System.Environment.NewLine);
                        }
                        else
                        {
                            invalidList2.Add(currentKeyColour.ToString() + System.Environment.NewLine);
                        }
                        break;
                }
            }
        }

        differentiator = Random.Range(0, 3);

        switch (differentiator)
        {
            case 0:
                currentIndex = Random.Range(0, remainingChars.Count);
                invalidList1.Add(remainingChars[currentIndex] + System.Environment.NewLine);
                remainingChars.RemoveAt(currentIndex);
                break;
            case 1:
                currentIndex = Random.Range(0, remainingNums.Count);
                invalidList1.Add(remainingNums[currentIndex] + System.Environment.NewLine);
                remainingNums.RemoveAt(currentIndex);
                break;
            case 2:
                currentIndex = Random.Range(0, remainingCols.Count);
                invalidList2.Add(remainingCols[currentIndex].ToString() + System.Environment.NewLine);
                remainingCols.RemoveAt(currentIndex);
                break;
        }

        differentiator = Random.Range(0, 3);

        switch (differentiator)
        {
            case 0:
                currentIndex = Random.Range(0, remainingChars.Count);
                invalidList2.Add(remainingChars[currentIndex] + System.Environment.NewLine);
                remainingChars.RemoveAt(currentIndex);
                break;
            case 1:
                currentIndex = Random.Range(0, remainingNums.Count);
                invalidList2.Add(remainingNums[currentIndex] + System.Environment.NewLine);
                remainingNums.RemoveAt(currentIndex);
                break;
            case 2:
                currentIndex = Random.Range(0, remainingCols.Count);
                invalidList2.Add(remainingCols[currentIndex].ToString() + System.Environment.NewLine);
                remainingCols.RemoveAt(currentIndex);
                break;
        }

        invalidList1 = invalidList1.OrderBy(a => System.Guid.NewGuid()).ToList();
        invalidList2 = invalidList2.OrderBy(a => System.Guid.NewGuid()).ToList();
        foreach(string invalid in invalidList1)
        {
            invalids1 += invalid;
        }
        foreach(string invalid in invalidList2)
        {
            invalids2 += invalid;
        }
    }
}
