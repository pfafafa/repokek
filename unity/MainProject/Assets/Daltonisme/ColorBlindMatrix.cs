﻿using UnityEngine;

/*
 * This enumeration had to be starting at 0 
 * until Enum.GetNames(typeof(ColorBlindMode)).Length
 */
public enum ColorBlindMode : int {
    NoColorBlind  = 0,
    Protanopia    = 1,
    Protanomaly   = 2,
    Deuteranopia  = 3,
    Deuteranomaly = 4,
    Tritanopia    = 5,
    Tritanomaly   = 6,
    Achromatopsia = 7,
    Achromatomaly = 8
}


/*
 * Singleton Class
 * Used to compute Blindness matrices thanks to @GetColorBlindnessMat()
 */
public class ColorBlindMatrix {

	private static Matrix4x4[] rgbMatrix = 
		new Matrix4x4[System.Enum.GetNames(typeof(ColorBlindMode)).Length];

	/*
	 * Static constructor
	 * Initiate all colorblind matrices
	 */
	static ColorBlindMatrix() {
		// Matrices from www.alanzucconi.com
		
		rgbMatrix [(int) ColorBlindMode.NoColorBlind] = Matrix4x4.identity;

		rgbMatrix [(int) ColorBlindMode.Protanopia] = MakeMatrix4x4 (
			.56667f, .43333f, 0f,
			.55833f, .44167f, 0f,
			0f, .24167f, .75833f);
		
		rgbMatrix [(int) ColorBlindMode.Protanomaly] = MakeMatrix4x4 (
			.81667f, .18333f, 0f,
			.33333f, .66667f, 0f,
			0f, .125f, .875f);

		rgbMatrix [(int) ColorBlindMode.Deuteranopia] = MakeMatrix4x4 (
			.625f, .375f, 0f,
			.70f, .30f, 0f,
			0f, .30f, .70f);

		rgbMatrix [(int) ColorBlindMode.Deuteranomaly] = MakeMatrix4x4 (
			.80f, .20f, 0f,
			.25833f, .74167f, 0,
			0f, .14167f, .85833f);

		rgbMatrix [(int) ColorBlindMode.Tritanopia] = MakeMatrix4x4 (
			.95f, .05f, 0f,
			0f, .43333f, .56667f,
			0f, .475f, .525f);

		rgbMatrix [(int) ColorBlindMode.Tritanomaly] = MakeMatrix4x4 (
			.96667f, .03333f, 0f,
			0f, .73333f, .26667f,
			0f, .18333f, .81667f);

		rgbMatrix [(int) ColorBlindMode.Achromatopsia] = MakeMatrix4x4 (
			.299f, .587f, .114f,
			.299f, .587f, .114f,
			.299f, .587f, .114f);

		rgbMatrix[(int) ColorBlindMode.Achromatomaly] = MakeMatrix4x4 (
			.618f, .32f, .062f,
			.163f, .775f, .062f,
			.163f, .320f, .516f);
	}

	private static Matrix4x4 MakeMatrix4x4(float Rr, float Rg, float Rb, float Gr, float Gg, float Gb, float Br, float Bg, float Bb) {
		Matrix4x4 mat = new Matrix4x4 ();
		mat.SetRow (0, new Vector4 (Rr, Rg, Rb, 0f));
		mat.SetRow (1, new Vector4 (Gr, Gg, Gb, 0f));
		mat.SetRow (2, new Vector4 (Br, Bg, Bb, 0f));
		mat.SetRow (3, new Vector4 (0f, 0f, 0f, 1f));
		return mat;
	}
		
	/*
	 * Basic function to manipulate Matrix4x4 with our specific format
	 */
	private static Matrix4x4 Add(Matrix4x4 m1, Matrix4x4 m2) {
		Matrix4x4 res = new Matrix4x4();
		for (int i = 0; i < 15; i++)
				res [i] = m1 [i] + m2 [i];
		res [15] = 1f;
		return res;
	}

	private static Matrix4x4 Sub(Matrix4x4 m1, Matrix4x4 m2) {
		Matrix4x4 res = new Matrix4x4();
		for (int i = 0; i < 15; i++)
				res [i] = m1 [i] - m2 [i];
		res [15] = 1f;
		return res;
	}

	private static Matrix4x4 Scale(float alpha, Matrix4x4 m) {
		Matrix4x4 res = new Matrix4x4();
		for (int i = 0; i < 15; i++)
			res [i] = alpha * m [i];
		res [15] = 1f;
		return res;
	}



	// Compute correction matrices
	private static Matrix4x4 ColorBlindCorrection(Matrix4x4 colorblindMat) {
		Matrix4x4 correct = MakeMatrix4x4 (
			           		0f, 0f, 0f,
			                .7f, 1f, 0f,
			                .7f, 0f, 1f);
		return Add(Sub(correct, correct * colorblindMat), Matrix4x4.identity);
	}

	// Compute a shade matrix
	private static Matrix4x4 Shade(Matrix4x4 m, float alpha) {
		return Add(Scale(alpha, m), Scale(1f - alpha, Matrix4x4.identity));
	}



	// Singleton pattern
	private ColorBlindMatrix() {}

	// The only getter
	public static Matrix4x4 GetColorBlindnessMat(ColorBlindMode m, bool correction, float alpha) {
		if (correction)
			return Shade(ColorBlindCorrection (rgbMatrix [(int) m]), alpha);
		else
			return Shade(rgbMatrix [(int) m], alpha);
	}
}