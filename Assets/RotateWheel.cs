using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWheel : MonoBehaviour {
	public GameObject image1;
	public GameObject image2;
	public GameObject image3;
	public GameObject image4;
	public GameObject image5;
	public GameObject image6;
	public GameObject image7;
	public GameObject image8;
	GameObject[] images = new GameObject[8];

	public float showInterver = 3.0f;
	float nextSlideStartTime;

	float angleNow = 0;
	float imageAngleInterver = 45;

	int imageIndexNow = 0;
	int sequenceSlideImage = 1;	// 0:To Back / 1: Rotate / 2:To Front / 3:Wait 3seconds

	float toFrontTargetX = 0;
	float toFrontTargetZ = 0;
	float toFrontUnitX = 0;
	float toFrontUnitZ = 0;
	int moveFrameIndex = 0;

	float frontBackRate = 21.0f / 8.0f;
	int moveFrameCount = 10;

	// Use this for initialization
	void Start () {
		angleNow = transform.localRotation.y;

		images [0] = image1;
		images [1] = image2;
		images [2] = image3;
		images [3] = image4;
		images [4] = image5;
		images [5] = image6;
		images [6] = image7;
		images [7] = image8;

		imageAngleInterver = 360.0f / images.Length;
	}

	void SlideImage_WaitSeconds () {
		if (Time.time > nextSlideStartTime) {
			int frameCount = 10;

			GameObject image = images [imageIndexNow];
			toFrontTargetX = image.transform.position.x / frontBackRate;
			toFrontTargetZ = image.transform.position.z / frontBackRate;
			toFrontUnitX = (toFrontTargetX - image.transform.position.x) / frameCount;
			toFrontUnitZ = (toFrontTargetZ - image.transform.position.z) / frameCount;
			moveFrameIndex = 0;
			sequenceSlideImage = 0;
		}
	}

	// Update is called once per frame
	void Update () {
		switch (sequenceSlideImage) {
		case 0:
			{
				bool bSequenceCompleted = SlideImage_ToFront ();
				if (bSequenceCompleted) {
					imageIndexNow++;
					imageIndexNow %= images.Length;
				}
				break;
			}
		case 1:
			SlideImage_Rotate ();
			break;
		case 2:
			{
				bool bSequenceCompleted = SlideImage_ToFront ();
				if (bSequenceCompleted) {
					nextSlideStartTime = Time.time + showInterver;
				}
				break;
			}
		case 3:
			// 압력 감지
			if (Input.GetButtonDown ("Fire1")) {    // "Fire1" 은 기본 입력 명칭
				nextSlideStartTime = Time.time;
			}
			SlideImage_WaitSeconds ();
			break;
		}
	}

	bool SlideImage_ToFront() {
		bool bSequenceCompleted = false;
		GameObject image = images [imageIndexNow];
		Vector3 pos = image.transform.position;
		pos.x += toFrontUnitX;
		pos.z += toFrontUnitZ;
		moveFrameIndex++;

		bool bArriveToTarget = checkArriveToTarget(pos.x, pos.z);
		if (bArriveToTarget) {
			pos.x = toFrontTargetX;
			pos.z = toFrontTargetZ;

			sequenceSlideImage++;
			bSequenceCompleted = true;
		}

		image.transform.position = pos;
		return bSequenceCompleted;
	}

	bool checkArriveToTarget(float x, float z) {
		if (moveFrameIndex >= moveFrameCount) {
			moveFrameIndex = 0;
			return true;
		}
		return false;
	}

	void RotateOneFrame() {
		angleNow += 1.0f;
		if (angleNow > 360.0f)
			angleNow -= 360.0f;
		transform.localRotation = Quaternion.Euler (0, angleNow, 0);
	}

	float getImageAngle(int imageIndex) {
		float imageAngle = imageIndex * imageAngleInterver;
		if (imageIndex == 0 && angleNow > 300) {
			imageAngle = 360.0f;
		}

		return imageAngle;
	}

	void SlideImage_Rotate() {
		float imageAngle = getImageAngle( imageIndexNow );
		if (imageAngle > 360)
			imageAngle -= 360;
		
		if( angleNow < imageAngle ) {
			print ("SlideImage_Rotate()-" + angleNow + " / " + imageAngle);
			RotateOneFrame ();
		} else {
			print("SlideImage_Rotate()-" + angleNow + " / " + imageAngle);
			if (angleNow >= 360)
				angleNow -= 360;

			GameObject image = images [imageIndexNow];
			toFrontTargetX = image.transform.position.x * frontBackRate;
			toFrontTargetZ = image.transform.position.z * frontBackRate;
			toFrontUnitX = (toFrontTargetX - image.transform.position.x) / moveFrameCount;
			toFrontUnitZ = (toFrontTargetZ - image.transform.position.z) / moveFrameCount;
			moveFrameIndex = 0;

			sequenceSlideImage++;
		}
	}

}
