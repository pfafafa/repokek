﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBlindCamera : LoadScene {

	public Text title;
	public Slider slider;


	void Start () {

		// set title
		if (SavedValue.correction) {
			title.text = "Correction ";
		} else { 
			title.text = "Simulation ";
		}
		title.text += System.Enum.GetName (typeof(ColorBlindMode), SavedValue.colorBlindMode);

		// set slider
		slider.value = SavedValue.alpha;
	}

	/*
	 * Save slider value and apply on the camera
	 */
	public void OnSliderChange() {
		SavedValue.alpha = slider.value;
		// apply alpha on the camera

		CameraAndroidDaltonisme.SetFilter(SavedValue.colorBlindMode, SavedValue.correction, SavedValue.alpha);
	}
}
